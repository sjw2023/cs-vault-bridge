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
		private MethodService m_methodService;

		public Parser(string[] args)
		{
			if (args != null)
			{
				if (args.Length < 2)
				{
					entity = new Target(args[0]);
				}
				else
				{
					entity = new Target(args[0]);
					method = new Method(args[1]);
					host = args[2];
					endPoint = args[3];
					parameter = new Parameter(args.Skip(4).ToArray());
					this.m_methodService = new MethodService(host, endPoint);
				}
			}
			else
			{
				//TODO : Throw exception for null case.
			}
		}
		public void Execute()
		{
			if (entity.Name == "folder")
			{
				instance = new cs_vault_bridge_console.Service.FolderService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				this.m_methodService.MethodInvokation(instance, method.MethodName, parameter.ParameterObject);
			}
			if (entity.Name == "item")
			{
				instance = new cs_vault_bridge_console.Service.ItemService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				this.m_methodService.MethodInvokation(instance, method.MethodName, parameter.ParameterObject);
			}
			if (entity.Name == "property")
			{
				instance = new cs_vault_bridge_console.Service.PropertyService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				this.m_methodService.MethodInvokation(instance, method.MethodName, parameter.ParameterObject);
			}
			if (entity.Name == "test")
			{
				instance = new cs_vault_bridge_console.Service.TestService("192.168.10.250", "DTcenter", "DTcenter", "1234");
				this.m_methodService.MethodInvokation(instance, method.MethodName, parameter.ParameterObject);
			}
			if (entity.Name.StartsWith("Vault"))
			{
				// Without out keyword compiler will say "use of uninitialized variable warning"
				Login(out connection);
				switch (entity.Name)
				{
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
				if (m_methodService != null && instance != null && method != null && parameter != null)
				{
					m_methodService.MethodInvokation(instance, method.MethodName, parameter.ParameterObject);
				}
				else
				{
					// Handle the error
					Console.WriteLine("Error");
				}
				Logout(connection);
			}
			if (entity.Name == "method")
			{
				VDF.Vault.Currency.Connections.Connection connection;
				Login(out connection);
				switch (method.MethodName)
				{
					case "ItemService":
						instance = connection.WebServiceManager.ItemService;
						break;
					case "PropertyService":
						instance = connection.WebServiceManager.PropertyService;
						break;
					case "CategoryService":
						instance = connection.WebServiceManager.CategoryService;
						break;
					case "FileStore":
						instance = connection.WebServiceManager.FilestoreService;
						break;
					case "DocumentService":
						instance = connection.WebServiceManager.DocumentService;
						break;
					case "LifeCycleService":
						instance = connection.WebServiceManager.LifeCycleService;
						break;
					case "NumberingService":
						instance = connection.WebServiceManager.NumberingService;
						break;
				}
				m_methodService.GetMethodInfos(instance, method.MethodName, parameter.ParameterObject);
				Logout(connection);
			}
		}
		public void Login(out Connection connection)
		{
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
	}
}