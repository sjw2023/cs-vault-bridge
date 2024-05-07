using Autodesk.Connectivity.WebServices;
using cs_vault_bridge_console.Command;
using cs_vault_bridge_console.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace cs_vault_bridge_console
{
	internal class Parser
	{
		private Target entity;
		private Method method;
		private Parameter parameter;

		public Parser(string[] args) { 
			if (args != null)
			{
				if (args.Length < 2)
				{
					entity = new Target(args[0]);
				}
				else {
					entity = new Target(args[0]);
					method = new Method(args[1]);
					parameter = new Parameter(args.Skip(2).ToArray());
				}
			}
			else { 
				//TODO : Throw exception for null case.
			}
		}
		public void Execute(  ) {
			if (entity.Name == "folder") {
				FolderService folderService = new FolderService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				folderService.TestGetFolderStructure();
			}
			if (entity.Name == "item") {
				if (method.MethodName == "getItems")
				{
					cs_vault_bridge_console.Service.ItemService itemService = new cs_vault_bridge_console.Service.ItemService("192.168.10.250", "DTcenter", "DTcenter", "1234");
					itemService.ReadAllItems();
				}
			}
			if(entity.Name == "property")
			{
				if (method.MethodName == "getPropertiesByEntityClass")
				{
					cs_vault_bridge_console.Service.PropertyService propertyService = new cs_vault_bridge_console.Service.PropertyService("192.168.10.250", "DTcenter", "DTcenter", "1234");
					propertyService.GetCategoriesByEntityClassId(parameter);
				}
			}
			if (entity.Name == "test") { 
				TestService testService = new TestService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				//testService.TestBOM(method, parameter);
				//testService.TestPropertyPrint(method, parameter);
				//testService.GetServerConfigurationTest(parameter);
				//testService.GetAllCustomEntityDefinitions(parameter);
				//testService.GetSupportedProducts();
				//testService.GetDuplicateSerachConfiguration();
				//testService.GetAllAssociationPropertyDefinitionInfos();
				//testService.GetItemMasters();
				Item item = testService.FindItemByName(parameter);
				//testService.PrintBomOfItem(item);
				testService.GetBOMList(item);
				Console.WriteLine("Call done");
				Console.ReadLine();
			}
		}
		public Parser() { 



		}

	}
	enum CommandName 
	{
		Direct,
		Root,
		Category
	}
}