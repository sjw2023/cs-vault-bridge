using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Command
{
	internal class Parameter
	{
		public Parameter(string[] parameters) {
			this.parameters = new Dictionary<string, string>();
			foreach (var parameter in parameters)
			{
				string converted = parameter.Replace('_', ' ');
				string[] splited = converted.Split('=');
				this.parameters.Add(splited[0], splited[1]);
			};
		}
		public Dictionary<string, string> parameters;
	}
}
