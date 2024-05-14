﻿using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using Autodesk.DataManagement.Client.Framework.Vault.Results;
using cs_vault_bridge_console.Command;
using cs_vault_bridge_console.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.EnterpriseServices;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using VDF = Autodesk.DataManagement.Client.Framework;
namespace cs_vault_bridge_console.Service
{
	internal class TestService : BaseService
	{
		public TestService(string serverName, string vaultName, string userName, string password)
			: base(userName, password, serverName, vaultName) { }

		public void GetAllAssociationPropertyDefinitionInfos() {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			AssocPropDefInfo[] assocPropDefInfos = connection.WebServiceManager.PropertyService.GetAllAssociationPropertyDefinitionInfos(AssocPropTyp.ChangeOrderItem);
			if (assocPropDefInfos != null){
				foreach (AssocPropDefInfo assocPropDefInfo in assocPropDefInfos){
					Console.WriteLine(assocPropDefInfo.PropDef);
					foreach (EntClassCtntSrcPropCfg entClassCtntSrcPropCfg in assocPropDefInfo.EntClassCtntSrcPropCfgArray){
						Console.WriteLine($"{entClassCtntSrcPropCfg.EntClassId}");
					}
					foreach (var value in assocPropDefInfo.ListValArray){
						Console.WriteLine(value);
					}
				}
			}
			base.Logout(connection);
		}
		// Read all items
		//	TODO : Move it to ItemService.cs
		public List<Item> GetItemMasters() {
			List<Item> items = new List<Item>();
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			string bookmark = null;
			SrchStatus status = null;
			while (status == null || status.TotalHits > items.Count){
				items.AddRange(connection.WebServiceManager.ItemService.FindItemRevisionsBySearchConditions(null, null, true, ref bookmark, out status));
			}
			int index = 1;
			foreach (Item item in items) {
				Console.WriteLine($"{index++} | {item.ItemNum} | {item.MasterId} | {item.RevNum} | {item.Id} ");
			}
			base.Logout(connection);
			return items;
		}
		public Item FindItemByName(Parameter parameter)
		{
			string name = parameter.parameters["name"];
			//TODO : Find better searching algorithm later
			List<Item> items = GetItemMasters();
			foreach (Item item in items)
			{
				if (item.ItemNum == name)
				{
					return item;
				}
			}
			return null;
		}
		public void PrintBomOfItem(Item item) {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			//Read all BOMs of each items
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(new long[] { item.Id }, true);
			foreach (ItemAssoc itemAssoc in itemAssocs)
			{
				Item[] temp = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { itemAssoc.ParItemID });
				Item[] child = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { itemAssoc.CldItemID });
				Console.WriteLine($"Parent : {itemAssoc.ParItemMasterID} id : {itemAssoc.Id} | Parent Item name : {temp[0].ItemNum} | {itemAssoc.CldItemID} | {child[0].ItemNum}");
			}
			base.Logout(connection);
		}
		public CustomItem[] GetBOMList(Item item) {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			//Tree<CustomItem> itemTree = new Tree<CustomItem>();
			Updater<CustomItem> updater = new Updater<CustomItem>("http://localhost:8080");
			//Read all BOMs of each items
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(new long[] { item.Id }, true);

			foreach (ItemAssoc itemAssoc in itemAssocs)
			{
				Item[] parentItem = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { itemAssoc.ParItemID });
				Item[] child = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { itemAssoc.CldItemID });
				Console.WriteLine($"Parent ID : {itemAssoc.ParItemID}  | Quantity {itemAssoc.Quant} | Parent Item name : {parentItem[0].ItemNum} | Child Id : {itemAssoc.CldItemID} | Child Item Name : {child[0].ItemNum}");
			}

			base.Logout(connection);
			return null;
		}
		public IEnumerable<ItemAssoc> GetItemAssocByIdFromBOM(long targetId, ItemAssoc[] itemAssocs) { 
			IEnumerable<ItemAssoc> toRet =
				from itemAssoc in itemAssocs
				where itemAssoc.ParItemID == targetId
				select itemAssoc;

			foreach (ItemAssoc itemAssoc in toRet) {
				Console.WriteLine(itemAssoc.ParItemID);
			}
			return toRet;
		}

		public IEnumerable<long> GetChildrenIdsByParentIdFromBOM(long targetId, ItemAssoc[] itemAssocs) { 
			IEnumerable<long> toRet = 
				from itemAssoc in itemAssocs
				where itemAssoc.ParItemID ==targetId
				select itemAssoc.CldItemID;
			foreach (long id in toRet) { 
				Console.WriteLine($"{id}");
			}
			return toRet;
		}

		public IEnumerable<long> GetParentIdsFromBOM(ItemAssoc[] itemAssocs) {
			IEnumerable<long> toRet=
				from itemAssoc in itemAssocs
				select itemAssoc.ParItemID;


			foreach (long id in toRet.Distinct()) { 
				Console.WriteLine($"Parents' id : {id}");
			}
			return toRet.Distinct();
		}

		public IEnumerable<long> GetChildrenIdsFromBOM(ItemAssoc[] itemAssocs) {
			IEnumerable<long> toRet =
				from itemAssoc in itemAssocs
				select itemAssoc.CldItemID;
			return toRet;
		}
/// <summary>
/// Generating item tree 
/// </summary>
/// <param name="parameter"></param>
		//	TODO :  Put this method into Tree class;
		public void CreateItemTreeFromBOM(Parameter parameter)
		{
			Item item = FindItemByName(parameter); 
			VDF.Vault.Currency.Connections.Connection connection; 
			base.LogIn(out connection);
			//	TODO : set Root Item with first item who has children
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(new long[] { item.Id }, false);

			Node<CustomItem> root = new Node<CustomItem>(new CustomItem(), item.Id);
			foreach (ItemAssoc BOM in itemAssocs) { 
				int level = 1;
				root.AddChild(DFSTreeGeneratorFromBOM(new Node<CustomItem>(new CustomItem(), BOM.CldItemID), connection, level));
			}
			Updater<Node<CustomItem>> updater = new Updater<Node<CustomItem>>("http://localhost:8080");
			updater.GenericPost("/post-item-tree", root);
			PrintItemTree(root, 1);
			base.Logout(connection);
		}
		//	TODO :  Put this method into Tree class;
		private void PrintItemTree(Node<CustomItem> root, int level) {
			if (root.Children == null) {
				return;
			}
			foreach (Node<CustomItem> node in root.Children) {
				for (int i = 0; i < level; i++) {
					Console.Write("--");
				}
				Console.WriteLine($"{level} {node.nodeID} | {node.Value.Title}");
				PrintItemTree(node, level+1);		
			}
		}
		//	TODO :  Put this method into Tree class;
		//	TODO :  Join several results of query with LINQ
		private Node<CustomItem> DFSTreeGeneratorFromBOM(Node<CustomItem> parent, Connection connection, int level)
		{ 
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds( new long[] { parent.nodeID }, false );
			if (itemAssocs == null){
				return parent;
			}
			// while looping put into chilren list and move to next recursive call
			foreach ( ItemAssoc child in itemAssocs ) {
				//Getting property definitions of ITEM entity
				PropDef[] propDefs = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId( "ITEM" );
				List<long> tempPropDefIds = new List<long>();
				foreach ( PropDef prop in propDefs) {
					tempPropDefIds.Add(prop.Id);
				}
				//Getting custom property values
				PropInst[] propertyInstance = connection.WebServiceManager.PropertyService.GetProperties( "ITEM", new long[] { child.CldItemID }, tempPropDefIds.ToArray());
				// Debug code to see what's inside of PropInst can be removed later
				CustomItem childToAdd = new CustomItem();
				if (propertyInstance != null) { 
					foreach (PropInst instance in propertyInstance) {
						PropertyDefinition tempDef = connection.PropertyManager.GetPropertyDefinitionById(instance.PropDefId);
						//	TODO : Think about better way rather than hard coding this
						if (tempDef.DisplayName.Equals("이름")){
							childToAdd.Title = (string)instance.Val;
						}
						if (tempDef.DisplayName.Equals("비고")){
							childToAdd.Note = (string)instance.Val;
						}
					}
				}
				//	TODO : Put Unit type, Item Category.
				//	Add child to parent.
				Item[] temp = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { child.CldItemID });
				parent.AddChild( DFSTreeGeneratorFromBOM(new Node<CustomItem>( childToAdd , child.CldItemID ), connection, level++ ));
			}
			return parent;
		}

		//	TODO : Move to PropertyService.cs
		//	Create Items property definition according to Vault information


		//	TODO : Move to PropertyService.cs after complete
		//	Get Property definitions value.
		public void PrintPropertyValues() { 
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			// Getting ITEMs property definitions and its values
			PropertyDefinitionDictionary dictionary =  connection.PropertyManager.GetPropertyDefinitions( VDF.Vault.Currency.Entities.EntityClassIds.Items, null, VDF.Vault.Currency.Properties.PropertyDefinitionFilter.IncludeUserDefined);
			foreach ( PropertyDefinition element in dictionary.Values) {
				Console.WriteLine($"프로퍼티 : {element}");
				if (element.ValueList != null) { 
					foreach (var value in element.ValueList) {
						Console.WriteLine($"{value.Display}");
					}
				}
			}
			base.Logout(connection);
		}
		
		// Get duplicate search Configuration
		public void GetDuplicateSerachConfiguration() {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			DuplicateSearchConfiguration duplicateSearchConfiguration = connection.WebServiceManager.AnalyticsService.GetDuplicateSearchConfiguration();
			foreach (var kvp in duplicateSearchConfiguration.DuplicateSearchFolderConfigurationArray)
			{ 
				Console.WriteLine(kvp.FolderId);
			}

			base.Logout(connection);
		}
		public void GetEnablementConfiguration() { 
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			EnablementConfig enablement = connection.WebServiceManager.ItemService.GetEnablementConfiguration();
			Console.WriteLine(enablement);

			Console.ReadLine();
			base.Logout(connection);
		}

		// 현재 서버에서 사용중인 프로덕트 정보 읽어 오기
		public void GetSupportedProducts() { 
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			Product[] products = connection.WebServiceManager.InformationService.GetSupportedProducts();
			foreach (Product product in products)
			{
				Console.WriteLine($"{product.ProductName} | {product.DisplayName} | {product.ProductVersion}");
			}
			Console.ReadLine();
			base.Logout(connection);
		}

		// 현재 서버에서 설정되어있는 모든 CustomEntity 값을 읽어 온다.
		public void GetAllCustomEntityDefinitions(Parameter parameter) {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			CustEntDef[] custEntDefs = connection.WebServiceManager.CustomEntityService.GetAllCustomEntityDefinitions();

			foreach (CustEntDef custEntDef in custEntDefs) { 
				Console.WriteLine($"| {custEntDef.Id} | {custEntDef.Name} | {custEntDef.DispName} ");
			}

			Console.ReadLine();
			base.Logout(connection);
		}

		//현재 서버에 설정되어있는 Behavior 값들과 Entity 값들을 읽어 온다.
		public void GetServerConfigurationTest(Parameter parameter) { 
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			ServerCfg serverCfg = connection.WebServiceManager.AdminService.GetServerConfiguration();
			foreach (string behavior in serverCfg.BhvArray) { 
				Console.WriteLine($"Behaviors : {behavior}");
			}
			foreach (EntClassCfg entClassCfg in serverCfg.EntClassCfgArray) { 
				foreach (string behavior in entClassCfg.BhvArray)
					Console.WriteLine($"Behavior of entity : {behavior}");
				Console.WriteLine( $"Entity class display name : {entClassCfg.DispName}");
            }
			Console.ReadLine();
			base.Logout(connection);
		}

		//전달 받은 카테고리에 해당하는 프로퍼티들을 반환
		public void TestPropertyPrint(Method method, Parameter parameter) {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			Cat[] categories;
			string type = parameter.parameters["type"];
			//Read categories and create new item.
			categories = connection.WebServiceManager.CategoryService.GetCategoriesByEntityClassId( type, true );
			long catId = -1;
			foreach (Cat category in categories)
			{
				Console.WriteLine($"{category.SysName} | {category.Name} | {category.Id}");
				if (category.Name == type)
					catId = category.Id;
			}

			//Get properties
			PropDef[] propDefs = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId( type );
			foreach ( PropDef prop in propDefs) {
				Console.WriteLine($"{prop.DispName} | {prop.SysName} | {prop.Typ} ");
			}

			//Console.WriteLine($"ID : {catId}");
			//Console.ReadLine();
			Console.ReadLine();
			base.Logout(connection);
			//return catId;
		}
		// 입력 받은 항목 ID
		public void TestBOM(Method method, Parameter parameter)
		{
			//로그인
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			//마스터 항목들 읽어 오기
			List<Item> masterItems = new List<Item>();
			List<long> itemIds = new List<long>();
			SrchStatus status = null;
			string bookmark = null;
			while (status == null || status.TotalHits > masterItems.Count)
			{
				masterItems.AddRange(connection.WebServiceManager.ItemService.FindItemRevisionsBySearchConditions(null, null, true, ref bookmark, out status));
			}
			foreach (Item item in masterItems)
			{
				itemIds.Add(item.Id);
				Console.WriteLine($"{item.ItemNum}");
			}

			//Read all BOMs of each items
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(itemIds.ToArray(), true);
			foreach (ItemAssoc itemAssoc in itemAssocs)
			{
				Item[] temp = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { itemAssoc.ParItemID });
				Console.WriteLine($"Parent : {itemAssoc.ParItemID} id : {itemAssoc.Id} | Item name : {temp[0].ItemNum}");	
			}

			//Pring parameters
			foreach (KeyValuePair<string, string> element in parameter.parameters)
			{ 
				Console.WriteLine(element.Key);
				Console.WriteLine(element.Value);
			}
			Console.ReadLine();
			base.Logout(connection);
		}
		public void ReadItemStructure() { 
			
		}
	}
}
