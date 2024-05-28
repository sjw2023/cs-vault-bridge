using Autodesk.Connectivity.WebServices;
using Autodesk.DataManagement.Client.Framework.Vault.Currency.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VDF = Autodesk.DataManagement.Client.Framework;

namespace cs_vault_bridge_console.Service
{
	internal class BaseService
	{
		protected string userName;
		protected string password;
		protected string serverName;
		protected string vaultName;

		public void LogIn(out Connection connection)
		{
			VDF.Vault.Results.LogInResult results = VDF.Vault.Library.ConnectionManager.LogIn(this.serverName, this.vaultName, this.userName, this.password
							//"192.168.10.250", "DTcenter", "DTcenter", "1234"
							//"192.168.10.250", "DTcenter", "joowon.suh@woosungautocon.com", "R-6qEbT#*nrJLZp"
							, VDF.Vault.Currency.Connections.AuthenticationFlags.Standard, null
							) ;
			if (!results.Success)
			{
				foreach (var key in results.ErrorMessages.Keys)
				{
					Console.WriteLine(results.ErrorMessages[key]);
				}
			}
			connection = results.Connection;
		}
		public void Logout(Connection connection) {
			VDF.Vault.Library.ConnectionManager.LogOut(connection);
		}

		public BaseService() { }
		public BaseService(string userName, string password, string serverName, string vaultName)
		{
			this.userName = userName;
			this.password = password;
			this.serverName = serverName;
			this.vaultName = vaultName;
		}
	}
}
