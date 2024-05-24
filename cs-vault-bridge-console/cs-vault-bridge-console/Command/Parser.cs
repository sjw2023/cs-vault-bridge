using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using cs_vault_bridge_console.Command;
using cs_vault_bridge_console.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ItemService = cs_vault_bridge_console.Service.ItemService;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace cs_vault_bridge_console
{
	internal class Parser
	{
		private Target entity;
		private Method method;
		private Parameter parameter;
		private string host;
		private string endPoint;

		public Parser(string[] args) 
		{ 
			if (args != null){
				if (args.Length < 2){
					entity = new Target(args[0]);
				}
				else {
					entity = new Target(args[0]);
					method = new Method(args[1]);
					host = args[2];
					endPoint = args[3];
					parameter = new Parameter(args.Skip(4).ToArray());

				}
			}
			else { 
				//TODO : Throw exception for null case.
			}
		}
		public void Execute() {

			if (entity.Name == "folder") {
				FolderService folderService = new FolderService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				folderService.TestGetFolderStructure();
			}
			if (entity.Name == "item") {
				cs_vault_bridge_console.Service.ItemService itemService = new cs_vault_bridge_console.Service.ItemService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInfo mi = itemService.GetType().GetTypeInfo().GetMethod(method.MethodName);
				//if (method.MethodName == "getItems"){
				//	itemService.ReadAllItems();
				//}
				//if (method.MethodName == "GetFileAssociationsByMasterItemNum") { 
				//	itemService.GetFileAssociationsByMasterItemNum(parameter);
				//}
				//if (method.MethodName == "GetItemMasters") {
				//	itemService.GetItemMasters();
				//}
				MethodInvokation(mi, itemService);
			}
			if(entity.Name == "property"){
				cs_vault_bridge_console.Service.PropertyService propertyService = new cs_vault_bridge_console.Service.PropertyService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				if (method.MethodName == "getPropertiesByEntityClass"){
					propertyService.GetCategoriesByEntityClassId(parameter);
				}
			}
			if (entity.Name.StartsWith("Vault")){
				VDF.Vault.Currency.Connections.Connection connection; 
				VDF.Vault.Results.LogInResult loginResults = VDF.Vault.Library.ConnectionManager.LogIn(
							"192.168.10.250", "DTcenter", "DTcenter", "1234"
							//"192.168.10.250", "DTcenter", "joowon.suh@woosungautocon.com", "R-6qEbT#*nrJLZp"
							, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null
							) ;
				if (!loginResults.Success)
				{
					foreach (var key in loginResults.ErrorMessages.Keys)
					{
						Console.WriteLine(loginResults.ErrorMessages[key]);
					}
				}
				connection = loginResults.Connection;
				MethodInfo mi = null;
				Object instance = null;
				switch (entity.Name) {
					case "VaultItemService":
						mi = connection.WebServiceManager.ItemService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.ItemService;
						break;
					case "VaultPropertyService":
						mi = connection.WebServiceManager.PropertyService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.PropertyService;
						break;
					case "VaultCategoryService":
						mi = connection.WebServiceManager.CategoryService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.CategoryService;
						break;
					case "VaultChangeOrder":		
						mi = connection.WebServiceManager.ChangeOrderService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.ChangeOrderService;
						break;
					case "VaultDocumentService":
						mi = connection.WebServiceManager.DocumentService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.DocumentService;
						break;
					case "VaultFileStoreService":
						mi = connection.WebServiceManager.FilestoreService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.FilestoreService;
						break;
					case "VaultLifeCycleService":
						mi = connection.WebServiceManager.LifeCycleService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.LifeCycleService;
						break;
					case "VaultNumberingService":
						mi = connection.WebServiceManager.NumberingService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
						instance = connection.WebServiceManager.NumberingService;
						break;
				}
				MethodInvokation(mi, instance);
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
				Console.WriteLine("Call done");
				Console.ReadLine();
			}
		}
		public Parser() { 



		}
		public void MethodInvokation(MethodInfo mi, object instance) { 
			
			try
			{
				ParameterInfo[] info = mi.GetParameters();
				Object[] parameters = new Object[info.Length];
				int parametersIndex = 0;
				foreach (ParameterInfo parameterInfo in mi.GetParameters()) {
				if (parameterInfo.ParameterType.GetTypeInfo().IsArray)
				{
					IList<JToken> jsonResults = parameter.ParameterObject[parameterInfo.Name].ToList();
					Array array = Array.CreateInstance( parameterInfo.ParameterType.GetElementType(), mi.GetParameters().Length );
					int listIndex  = 0;
					parameters[parametersIndex] = array;
					foreach (var value in jsonResults.Values())
					{
						parameters[parametersIndex].GetType().InvokeMember("SetValue", System.Reflection.BindingFlags.InvokeMethod, null, parameters[parametersIndex], new object[] { Convert.ChangeType(value, parameterInfo.ParameterType.GetElementType()), listIndex++ });
					}
					parametersIndex++;
				}
				else
				{
					var jsonResult = parameter.ParameterObject[parameterInfo.Name];
					parameters[parametersIndex] = Activator.CreateInstance(parameterInfo.ParameterType);
					foreach (var value in jsonResult.Values()) { 
						parameters[parametersIndex++] = value;
					}
				}
			}
			Object returnObject = mi.Invoke(instance, parameters);
			if (returnObject == null) {
				throw new Exception("Item cannot be found");
			}
			Console.WriteLine(mi.ReturnType);
			var updateObject = Activator.CreateInstance(typeof(Updater<>).MakeGenericType(mi.ReturnType), host );
			//	TODO : return URL, have to be generic its fixed.
			updateObject.GetType().InvokeMember("GenericPost", System.Reflection.BindingFlags.InvokeMethod, null, updateObject, new object[] { endPoint, returnObject });
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString(), "Error");
			return;
		}
		Console.ReadLine();
		}
	}


	enum CommandName 
	{
		Direct,
		Root,
		Category
	}
}