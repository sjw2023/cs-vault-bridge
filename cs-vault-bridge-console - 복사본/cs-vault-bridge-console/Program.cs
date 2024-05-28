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
	//	TODO : Need to implement Initializing module to load all the foundation data of VAULT DB

	internal class Program
	{
		private static Parser parser;
		private FolderService folderService;

		static void Main(string[] args)
		{
			parser = new Parser(args);
			parser.Execute();
		}
	
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

	}
}
