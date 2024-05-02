using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Command
{
	internal class Method
	{
		public Method(string methodName) { 
			this.MethodName = methodName;
		}
		public string MethodName { get; set; }
	}
}
