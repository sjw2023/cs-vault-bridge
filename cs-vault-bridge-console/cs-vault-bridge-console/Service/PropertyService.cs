using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using cs_vault_bridge_console.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace cs_vault_bridge_console.Service
{
	internal class PropertyService : BaseService
	{
		public PropertyService(string serverName, string vaultName, string userName, string password) : base(
				userName, password, serverName, vaultName) { }
		public void GetCategoriesByEntityClassId(Parameter parameter) {
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			string type = parameter.parameters["type"];
			//Get properties
			PropDef[] propDefs = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId(type);
			foreach (PropDef prop in propDefs)
			{
				Console.WriteLine($"{prop.DispName} | {prop.SysName} | {prop.Typ} ");
			}
			Updater<PropDef[]> updater = new Updater<PropDef[]>("http://localhost:8080");
			updater.GenericPost("/properties", propDefs);

			base.Logout(connection);
		}
		private void AddItemProperty(long id)
		{
			VDF.Vault.Currency.Connections.Connection connection;
			base.LogIn(out connection);

			LfCycDef[] definitions = connection.WebServiceManager.LifeCycleService.GetAllLifeCycleDefinitions();
			Dictionary<long, LfCycDef> m_lifeCycleMap = new Dictionary<long, LfCycDef>();

			// put the life cycle definitions into a hashtable for easy lookup
			foreach (LfCycDef definition in definitions)
			{
				m_lifeCycleMap[definition.Id] = definition;
			}

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

					//DataRow newRow = m_dataTable.NewRow();


					if (item.Id == id) {
						Console.WriteLine($"Item ID{item.Id} | Item Number {item.ItemNum} | Revision NUmber {item.RevNum} | Title {item.Title} | Life Cycle State {state.Name}");
						Item[] toAdd = itemSvc.EditItems(new long[] { item.RevId });

						PropDef[] propDefs = connection.WebServiceManager.PropertyService.GetPropertyDefinitionsByEntityClassId("ITEM");
						PropInstParam param = new PropInstParam();
						List<PropInstParam> propertyValues = new List<PropInstParam>();
						Item[] propItems = new Item[propDefs.Length];

						foreach (PropDef propDef in propDefs)
						{
							Console.WriteLine($"{propDef.DispName} | {propDef.Id} | ${propDef.SysName}");
							if (propDef.DispName == "품명")
							{
								param.PropDefId = propDef.Id;
								param.Val = "품명 추가";
								//propInstParams[0] = param;
								propertyValues.Add(param);
								Console.WriteLine($"{propertyValues[0].Val}");
								propItems = itemSvc.UpdateItemProperties(new long[] { item.RevId },
									new PropInstParamArray[] {
									new PropInstParamArray() {
										Items= propertyValues.ToArray()
										}
									});
								itemSvc.UpdateAndCommitItems(toAdd);
							}
						}
					}



					ItemFileAssoc[] associations = itemSvc.GetItemFileAssociationsByItemIds(new long[] { item.Id }, ItemFileLnkTypOpt.Primary);
					if (associations != null)
					{
						foreach (ItemFileAssoc assoc in associations)
						{
							//newRow["Primary File Link"] = assoc.FileName;
							break;
						}
					}
					//m_tableMap[item.ItemNum] = item;
					//m_dataTable.Rows.Add(newRow);
				}
			}

			Console.ReadLine();
		}
	}
}
