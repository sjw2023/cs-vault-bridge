using Autodesk.Connectivity.WebServices;
using Autodesk.Connectivity.WebServicesTools;
using Autodesk.DataManagement.Client.Framework.Currency;
using Autodesk.DataManagement.Client.Framework.Internal.ExtensionMethods;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using VDF = Autodesk.DataManagement.Client.Framework;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using cs_vault_bridge_console.Service;

namespace cs_vault_bridge_console
{
	internal class Program
	{
		private static Parser parser;
		private FolderService folderService;

		static void Main(string[] args)
		{
			parser = new Parser(args);
			parser.Execute();
		}
		//private void ServerConfiguration() {
		//	ServerCfg serverCfg = m_connection.WebServiceManager.AdminService.GetServerConfiguration();
		//	if (serverCfg != null)
		//	{
		//		foreach (var ele in serverCfg.EntClassCfgArray)
		//		{
		//			Console.WriteLine($"{ele.Id}");
		//		}
		//	}
		//	Console.ReadLine();
		//}
		//private void TestAddProperty() {
		//	ReadAllItems();
		//	//FindItemById(37582);
		//	AddItemProperty(37582);
		//}
	
	
		//private void AddLifeCycle( WebServiceManager serviceManager, Item item ) {
		//	//Set Life Cycle Definitions
		//	LfCycDef[] lfCycDefs = serviceManager.LifeCycleService.GetAllLifeCycleDefinitions();
		//	foreach (LfCycDef lf in lfCycDefs)
		//	{
		//		Console.WriteLine($"{lf.SysName} | {lf.DispName} |{lf.Id}");
		//		foreach (LfCycState state in lf.StateArray)
		//		{
		//			Console.WriteLine(state.Name);
		//			Console.WriteLine(state.DispName);
		//		}
		//		if (lf.DispName == "항목 릴리즈 프로세스")
		//		{
		//			item.LfCycStateId = 2;
		//			item.LfCyc.LfCycDefId = lf.Id;
		//		}

		//	}
		//}

		

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
	}
}
