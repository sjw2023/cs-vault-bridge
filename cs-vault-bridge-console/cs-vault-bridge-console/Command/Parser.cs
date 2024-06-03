using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using cs_vault_bridge_console.Command;
using cs_vault_bridge_console.Service;
using cs_vault_bridge_console.Util;
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
		private object instance;
		private VDF.Vault.Currency.Connections.Connection connection;

		public Parser() { }
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
				instance = new cs_vault_bridge_console.Service.FolderService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInvokation(instance);
			}
			if (entity.Name == "item") {
				instance = new cs_vault_bridge_console.Service.ItemService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInvokation(instance);
			}
			if(entity.Name == "property"){
				instance = new cs_vault_bridge_console.Service.PropertyService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInvokation(instance);
			}
			if (entity.Name == "test") { 
				instance = new cs_vault_bridge_console.Service.TestService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInvokation(instance);
			}
			if (entity.Name == "test") { 
				instance = new TestService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				MethodInvokation(instance);
			}
			if (entity.Name.StartsWith("Vault")){
				// Without out keyword compiler will say "use of uninitialized variable warning"
				Login(out connection);
				switch (entity.Name) {
					case "VaultItemService":
						instance = connection.WebServiceManager.ItemService;
						break;
					case "VaultPropertyService":
						instance = connection.WebServiceManager.PropertyService;
						break;
					case "VaultCategoryService":
						instance = connection.WebServiceManager.CategoryService;
						break;
					case "VaultChangeOrder":		
						instance = connection.WebServiceManager.ChangeOrderService;
						break;
					case "VaultDocumentService":
						instance = connection.WebServiceManager.DocumentService;
						break;
					case "VaultFileStoreService":
						instance = connection.WebServiceManager.FilestoreService;
						break;
					case "VaultLifeCycleService":
						instance = connection.WebServiceManager.LifeCycleService;
						break;
					case "VaultNumberingService":
						instance = connection.WebServiceManager.NumberingService;
						break;
				}
				MethodInvokation(instance);
				Logout(connection);
			}
			if (entity.Name == "method") {
				VDF.Vault.Currency.Connections.Connection connection;
				Login(out connection);
				List<CsMethodInfo> csMethodInfos = new List<CsMethodInfo>();
				MethodInfo[] mis = null; 
				switch (method.MethodName) {
					case "ItemService":
						mis = connection.WebServiceManager.ItemService.GetType().GetTypeInfo().GetMethods();
						break;
					case "PropertyService":
						mis = connection.WebServiceManager.PropertyService.GetType().GetTypeInfo().GetMethods();
						break;
					case "CategoryService":
						mis = connection.WebServiceManager.CategoryService.GetType().GetTypeInfo().GetMethods();
						break;
					case "FileStore":  
						mis = connection.WebServiceManager.FilestoreService.GetType().GetTypeInfo().GetMethods();
						break;
					case "DocumentService":
						mis = connection.WebServiceManager.DocumentService.GetType().GetTypeInfo().GetMethods();
						break;
					case "LifeCycleService":
						mis = connection.WebServiceManager.LifeCycleService.GetType().GetTypeInfo().GetMethods();
						break;
					case "NumberingService":
						mis = connection.WebServiceManager.NumberingService.GetType().GetTypeInfo().GetMethods();
						break;
				}
				foreach ( var mi in mis ) {
					CsMethodInfo temp = new CsMethodInfo();
					temp.methodName = mi.Name;
					temp.returnType = mi.ReturnType.Name;
					temp.parameterTypes = new string[mi.GetParameters().Length];
					int index = 0;
					foreach (var pi in mi.GetParameters()) {
						temp.parameterTypes[index++] = pi.ParameterType.Name;
					}
					csMethodInfos.Add(temp);
					Console.WriteLine($"{temp.returnType} {temp.methodName}");
				}

				Updater<CsMethodInfo[]> updater = new Updater<CsMethodInfo[]>(host);
				updater.GenericPost(endPoint, csMethodInfos.ToArray());
				Logout(connection);
			}
		}
		public void Login(out Connection connection) {
			VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn("192.168.10.250", "DTcenter", "DTcenter", "1234"
						//"192.168.10.250", "DTcenter", "joowon.suh@woosungautocon.com", "R-6qEbT#*nrJLZp"
						, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null
						);
			if (!results.Success)
			{
				foreach (var key in results.ErrorMessages.Keys)
				{
					Console.WriteLine(results.ErrorMessages[key]);
				}
			}
			connection = results.Connection;
		}
		public void Logout(Connection connection)
		{
			VDF.Vault.Library.ConnectionManager.LogOut(connection);
		}
		public void MethodInvokation( object instance )  { 
			var mi = instance.GetType().GetTypeInfo().GetMethod(this.method.MethodName);
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
			updateObject.GetType().InvokeMember("GenericPost", System.Reflection.BindingFlags.InvokeMethod, null, updateObject, new object[] { endPoint, returnObject });
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.ToString(), "Error");
			return;
		}
		}
	}
}