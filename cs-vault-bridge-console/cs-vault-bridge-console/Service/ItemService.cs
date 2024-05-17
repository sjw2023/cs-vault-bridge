using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Properties;
using cs_vault_bridge_console.Command;
using cs_vault_bridge_console.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace cs_vault_bridge_console.Service
{
	internal class ItemService : BaseService
	{
		private Cat[] categories;
		private List<Item> items = new List<Item>();
		private Item m_selectedItem;
		private Dictionary<long, LfCycDef> m_lifeCycleMap;
		private Dictionary<string, Item> m_tableMap;
		private DataTable m_dataTable;
		public ItemService(string serverName, string vaultName, string userName, string password) : base(
				userName, password, serverName, vaultName)
		{ }

		private void FindItemById(long id)
		{
			//login
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			m_tableMap.Clear();
			m_dataTable.Clear();
			m_selectedItem = null;
			Autodesk.Connectivity.WebServices.ItemService itemSvc = connection.WebServiceManager.ItemService;

			//Read Items
			string bookmark = null;
			List<Item> items = new List<Item>();
			SrchStatus status = null;
			while (status == null || status.TotalHits > items.Count)
			{
				items.AddRange(itemSvc.FindItemRevisionsBySearchConditions(null, null, true, ref bookmark, out status));
			}

			//Select item by id
			if (items != null && items.Count > 0)
			{
				foreach (Item item in items)
				{
					LfCycDef lifeCycleDef = m_lifeCycleMap[item.LfCyc.LfCycDefId];
					LfCycState state = lifeCycleDef.StateArray.FirstOrDefault(lifecycleState => lifecycleState.Id == item.LfCycStateId);

					DataRow newRow = m_dataTable.NewRow();

					if (item.Id == id)
						Console.WriteLine($"Item ID{item.Id} | Item Number {item.ItemNum} | Revision NUmber {item.RevNum} | Title {item.Title} | Life Cycle State {state.Name}");


					ItemFileAssoc[] associations = itemSvc.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary);
					if (associations != null)
					{
						foreach (ItemFileAssoc assoc in associations)
						{
							newRow["Primary File Link"] = assoc.FileName;
							break;
						}
					}
					m_tableMap[item.ItemNum] = item;
					m_dataTable.Rows.Add(newRow);
				}
			}

			Console.ReadLine();
			base.Logout(connection);
		}

		public void ReadAllItems()
		{
			//login
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			items.Clear();
			DataSet dataSet = new DataSet("itemSet");
			m_dataTable = new DataTable("itemTable");
			m_dataTable.Columns.Add("Item Number");
			m_dataTable.Columns.Add("Revision Number");
			m_dataTable.Columns.Add("Title");
			m_dataTable.Columns.Add("Life Cycle State");
			m_dataTable.Columns.Add("Primary File Link");

			dataSet.Tables.Add(m_dataTable);
			//m_itemsGrid.SetDataBinding(dataSet, "ItemTable");
			m_tableMap = new Dictionary<string, Item>();
			m_selectedItem = null;


			// get all of the life cycle definitions
			LfCycDef[] definitions = connection.WebServiceManager.LifeCycleService.GetAllLifeCycleDefinitions();
			m_lifeCycleMap = new Dictionary<long, LfCycDef>();

			// put the life cycle definitions into a hashtable for easy lookup
			foreach (LfCycDef definition in definitions)
			{
				m_lifeCycleMap[definition.Id] = definition;
			}

			RefreshItemList(connection);
			base.Logout(connection);
		}

		public void RefreshItemList(Connection connection)
		{
			m_tableMap.Clear();
			m_dataTable.Clear();
			m_selectedItem = null;
			Updater<Item> update = new Updater<Item>("http://localhost:8080");

			Autodesk.Connectivity.WebServices.ItemService itemSvc = connection.WebServiceManager.ItemService;

			string bookmark = null;
			List<Item> items = new List<Item>();
			SrchStatus status = null;
			while (status == null || status.TotalHits > items.Count)
			{
				items.AddRange(itemSvc.FindItemRevisionsBySearchConditions(null, null, true, ref bookmark, out status));
			}


			if (items != null && items.Count > 0)
			{
				foreach (Item item in items)
				{
					LfCycDef lifeCycleDef = m_lifeCycleMap[item.LfCyc.LfCycDefId];
					LfCycState state = lifeCycleDef.StateArray.FirstOrDefault(lifecycleState => lifecycleState.Id == item.LfCycStateId);

					DataRow newRow = m_dataTable.NewRow();
					Console.WriteLine($"Item ID{item.Id} | Item Number {item.ItemNum} | Revision NUmber {item.RevNum} | Title {item.Title} | Life Cycle State {state.Name}");
					newRow["Item Number"] = item.ItemNum;
					newRow["Revision Number"] = item.RevNum;
					newRow["Title"] = item.Title;
					newRow["Life Cycle State"] = state.Name;

					ItemFileAssoc[] associations = itemSvc.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary);
					if (associations != null)
					{
						foreach (ItemFileAssoc assoc in associations)
						{
							Console.WriteLine($"Primary File Link : {assoc.FileName}");
							newRow["Primary File Link"] = assoc.FileName;
							break;
						}
					}
					m_tableMap[item.ItemNum] = item;
					m_dataTable.Rows.Add(newRow);
					//update.GenericPost("/post-item", item);

					//
					//items.Add(item);
				}
				Updater<Item[]> itemsUpdater = new Updater<Item[]>("http://localhost:8080");
				Item[] itemsArr = items.ToArray();
				itemsUpdater.GenericPost("/post-items", itemsArr);
			}

			Console.ReadLine();
		}
		//	TODO : Extract method into CategoryService class.
		//	카테고리 내용을 읽은뒤 인자로 받은 카테고리 이름에 해당하는 카테고리의 ID값을 반환 한다.
		//	Param : Category name
		//	Return : Category ID value
		private long GetCategoryIdByName(string categoryName, Connection connection)
		{
			categories = connection.WebServiceManager.CategoryService.GetCategoriesByEntityClassId(categoryName, true);
			long catId = -1;
			foreach (Cat category in categories)
			{
				Console.WriteLine($"{category.SysName} | {category.Name} | {category.Id}");
				if (category.Name == categoryName)
					catId = category.Id;
			}
			return catId;
		}

		public void AddItem(Parameter parameter)
		{
			//login
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			//TODO : Parameter의 Dictionary가 null일 경우 예외 처리

			//Create Item with item category
			long catId = GetCategoryIdByName(parameter.parameters["type"], connection);
			Item item = connection.WebServiceManager.ItemService.AddItemRevision(catId);

			// set item numbering scheme
			NumSchm numSchm1 = new NumSchm();
			NumSchm[] numSchms = connection.WebServiceManager.NumberingService.GetNumberingSchemes("ITEM", NumSchmType.All);
			foreach (NumSchm numSchm in numSchms)
			{
				Console.WriteLine(numSchm.Name);
				Console.WriteLine(numSchm.SchmID);
				//if (numSchm.Name == 8)
				//numSchm1 = numSchm;
			}
			item.NumSchmId = numSchm1.SchmID;
			string[] newItemNum;
			newItemNum = new string[] { "blabla" };
			StringArray[] fieldInputs = new StringArray[1];
			StringArray tempArr = new StringArray();
			tempArr.Items = newItemNum;
			fieldInputs[0] = tempArr;

			ProductRestric[] productRestrics;

			//Update Item number to Vault
			ItemNum[] itemNums = connection.WebServiceManager.ItemService.AddItemNumbers(new long[] { item.MasterId }, new long[] { item.NumSchmId }, fieldInputs, out productRestrics);

			//Set new item values
			item.ItemNum = itemNums[0].ItemNum1;
			item.Title = "11111";
			item.VerNum = 10;
			item.RevNum = "A";
			Console.WriteLine(numSchm1.SchmID);
			Console.WriteLine(item.NumSchmId);
			Console.WriteLine($"{item.ItemNum} | item id : {item.Id}");

			//Update Item values to Vault
			try
			{
				connection.WebServiceManager.ItemService.UpdateAndCommitItems(new Item[] { item });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Source);
				Console.WriteLine($"Failed to add item {ex.Message}");
				Console.WriteLine(ex.StackTrace);
				Console.WriteLine(ex.InnerException);
			}
			Console.ReadLine();
			base.Logout(connection);
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
			foreach (ItemAssoc BOM in itemAssocs)
			{
				int level = 1;
				root.AddChild(DFSTreeGeneratorFromBOM(new Node<CustomItem>(new CustomItem(), BOM.CldItemID), connection, level));
			}
			//	TODO : Put root item itself also
			Updater<Node<CustomItem>> updater = new Updater<Node<CustomItem>>("http://localhost:8080");
			updater.GenericPost("/post-item-tree", root);
			PrintItemTree(root, 1);
			base.Logout(connection);
		}

		//	TODO :  Put this method into Tree class;
		private void PrintItemTree(Node<CustomItem> root, int level)
		{
			if (root.Children == null)
			{
				return;
			}
			foreach (Node<CustomItem> node in root.Children)
			{
				for (int i = 0; i < level; i++)
				{
					Console.Write("--");
				}
				Console.WriteLine($"{level} {node.nodeID} | {node.Value.Title}");
				PrintItemTree(node, level + 1);
			}
		}

		//	TODO :  Put this method into Tree class;
		//	TODO :  Join several results of query with LINQ
		private Node<CustomItem> DFSTreeGeneratorFromBOM(Node<CustomItem> parent, Connection connection, int level)
		{
			ItemAssoc[] itemAssocs = connection.WebServiceManager.ItemService.GetItemBOMAssociationsByItemIds(new long[] { parent.nodeID }, false);
			if (itemAssocs == null)
			{
				return parent;
			}
			// while looping put into chilren list and move to next recursive call
			foreach (ItemAssoc child in itemAssocs)
			{
				//Getting property definitions of ITEM entity
				PropDef[] propDefs = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");
				List<long> tempPropDefIds = new List<long>();
				foreach (PropDef prop in propDefs)
				{
					tempPropDefIds.Add(prop.Id);
				}
				//Getting custom property values
				PropInst[] propertyInstance = connection.WebServiceManager.PropertyService.GetProperties("ITEM", new long[] { child.CldItemID }, tempPropDefIds.ToArray());
				// Debug code to see what's inside of PropInst can be removed later
				CustomItem childToAdd = new CustomItem();
				if (propertyInstance != null)
				{
					foreach (PropInst instance in propertyInstance)
					{
						PropertyDefinition tempDef = connection.PropertyManager.GetPropertyDefinitionById(instance.PropDefId);
						//	TODO : Think about better way rather than hard coding this
						if (tempDef.DisplayName.Equals("이름"))
						{
							childToAdd.Title = (string)instance.Val;
						}
						if (tempDef.DisplayName.Equals("비고"))
						{
							childToAdd.Note = (string)instance.Val;
						}
					}
				}
				//	TODO : Put Unit type, Item Category.
				//	Add child to parent.
				Item[] temp = connection.WebServiceManager.ItemService.GetItemsByIds(new long[] { child.CldItemID });
				parent.AddChild(DFSTreeGeneratorFromBOM(new Node<CustomItem>(childToAdd, child.CldItemID), connection, level++));
			}
			return parent;
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

		// Read all items
		//	TODO : Move it to ItemService.cs
		public List<Item> GetItemMasters()
		{
			List<Item> items = new List<Item>();
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);
			string bookmark = null;
			SrchStatus status = null;
			while (status == null || status.TotalHits > items.Count)
			{
				items.AddRange(connection.WebServiceManager.ItemService.FindItemRevisionsBySearchConditions(null, null, true, ref bookmark, out status));
			}
			int index = 1;
			foreach (Item item in items)
			{
				Console.WriteLine($"{index++} | {item.ItemNum} | {item.MasterId} | {item.RevNum} | {item.Id} ");
			}
			base.Logout(connection);
			return items;
		}

	}
}
