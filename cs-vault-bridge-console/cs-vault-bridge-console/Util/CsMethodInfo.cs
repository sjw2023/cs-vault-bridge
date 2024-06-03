using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Util
{
	internal class CsMethodInfo
	{
		public string returnType {  get; set; }
		public string methodName {  get; set; }
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string[] parameterTypes { get; set; }

		public CsMethodInfo() {
			returnType = null;
			methodName = null;
			parameterTypes = null;
		}
	}
}
