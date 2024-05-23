using Autodesk.Connectivity.WebServices;
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

		public Parser(string[] args) 
		{ 
			if (args != null){
				if (args.Length < 2){
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
		public void Execute() {
			if (entity.Name == "folder") {
				FolderService folderService = new FolderService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				folderService.TestGetFolderStructure();
			}
			if (entity.Name == "item") {
				cs_vault_bridge_console.Service.ItemService itemService = new cs_vault_bridge_console.Service.ItemService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInfo mi = itemService.GetType().GetTypeInfo().GetMethod(method.MethodName);
				if (method.MethodName == "getItems"){
					itemService.ReadAllItems();
				}
				if (method.MethodName == "GetFileAssociationsByMasterItemNum") { 
					itemService.GetFileAssociationsByMasterItemNum(parameter);
				}
				if (method.MethodName == "") { 
				}
				foreach (var parameterInfo in mi.GetParameters()) {

				}
			}
			if(entity.Name == "property"){
				cs_vault_bridge_console.Service.PropertyService propertyService = new cs_vault_bridge_console.Service.PropertyService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				if (method.MethodName == "getPropertiesByEntityClass"){
					propertyService.GetCategoriesByEntityClassId(parameter);
				}
			}
			if (entity.Name == "VaultItemService") { 
				//	TODO :  Find the way to make Reflection more generic
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
				try
				{
					MethodInfo mi = connection.WebServiceManager.ItemService.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
					var instance = connection.WebServiceManager.ItemService;
					ParameterInfo[] info = mi.GetParameters();
					Object[] parameters = new Object[info.Length];
					int parametersIndex = 0;
					//	TODO : when the parameter is array.
					foreach (ParameterInfo parameterInfo in mi.GetParameters()) {
						if (parameterInfo.ParameterType.GetTypeInfo().IsArray)
						{
							int listIndex = 0;
							IList<JToken> jsonResults = parameter.ParameterObject[parameterInfo.Name].ToList();
							//parameters[parametersIndex]
							Object obj = Array.CreateInstance(parameterInfo.ParameterType, jsonResults.Count);
							parameters[parametersIndex] = obj;
							foreach (var value in jsonResults.Values()) {
								obj[listIndex] = value;								
								parameters[parametersIndex] = value;
								Console.WriteLine($"Object has this : {obj[listIndex++]} |The parameter value is : {value}");
							}
							parametersIndex++;
						}
						else
						{
							var jsonResult = parameter.ParameterObject[parameterInfo.Name];
							parameters[parametersIndex] = Activator.CreateInstance(parameterInfo.ParameterType);
							parameters[parametersIndex++] = jsonResult;
						}
					}
					//	TODO : Make it more generic form
					Type returnType = mi.ReturnType;
					Object returnObject;
					if (returnType.IsArray) {
						returnObject = Array.CreateInstance(returnType, 100);
					}
					returnObject = mi.Invoke(instance, parameters);
					//returnObject = (int)Convert.ChangeType(mi.Invoke(instance, parameters), mi.ReturnType);
					Console.WriteLine(returnObject);

					var updateObject = Activator.CreateInstance(typeof(Updater<>).MakeGenericType(returnType));
					var prop = updateObject.GetType().GetProperty("baseUrl");
					prop.SetValue(updateObject, "http://localhost:8080/");
					//updateObject.GenericPost("post-item", ins);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString(), "Error");
					return;
				}
				Console.ReadLine();
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
	}


	enum CommandName 
	{
		Direct,
		Root,
		Category
	}
}