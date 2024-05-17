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
				//Item item = testService.FindItemByName(parameter);
				//testService.PrintBomOfItem(item);
				//testService.GetBOMList(item);
				//testService.CreateItemTreeFromBOM(parameter);
				//testService.PrintPropertyValues();
				testService.GetFileAssociationsByMasterItemNum(parameter);
				Console.WriteLine("Call done");
				Console.ReadLine();
			}
		}
		public Parser() { 



		}

	}
	//		public void DirectCall(string serviceName, string method, string[] args)
		//{
		//	try
		//	{
		//		// Start at the root Folder.
		//		VDF.Vault.Currency.Entities.Folder root = m_connection.FolderManager.RootFolder;
		//		// Call a function which prints all files in a Folder and sub-Folders
		//		MethodInfo mi = m_connection.WebServiceManager.DocumentService.GetType().GetTypeInfo().GetMethod(method);
		//		File[] childFiles = (File[])mi.Invoke(m_connection.WebServiceManager.DocumentService, new object[] { root.Id, false });
		//		///// Send to spring boot
		//		Updater<File[]> updater1 = new Updater<File[]>("http://localhost:8080/");
		//		updater1.GenericPost("post-files", childFiles);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.ToString(), "Error");
		//		return;
		//	}
		//}
		//	}

	enum CommandName 
	{
		Direct,
		Root,
		Category
	}
}