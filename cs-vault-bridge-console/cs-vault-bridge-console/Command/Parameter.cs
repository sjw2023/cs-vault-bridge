using Newtonsoft.Json.Linq;
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
			//	TODO : add using JSON
			//this.parameters = new Dictionary<string, string>();
			//foreach (var parameter in parameters)
			//{
			//	string converted = parameter.Replace('_', ' ');
			//	string[] splited = converted.Split('=');
			//	this.parameters.Add(splited[0], splited[1]);
			//};
			foreach(var parameter in parameters) {
				JObject jobject = JObject.Parse(parameter);
				foreach (long id in jobject["ids"].Children().ToList()) { 
					Console.WriteLine(id);
				}
				Console.WriteLine(jobject["type"]);
			}
		}
		public Dictionary<string, string> parameters;
	}
}
