using cs_vault_bridge_console.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace cs_vault_bridge_console.Service
{
	internal class MethodService : BaseService
	{
		private MethodInfo[] m_methodInfos;
		private MethodInfo m_methodInfo;
		private string m_host;
		private string m_endpoint;
		private List<CsMethodInfo> m_csMethodInfos = new List<CsMethodInfo>();

		public MethodService(string host, string endpoint)
		{
			m_endpoint = endpoint;
			m_host = host;
		}
		public void MethodInvokation(object instance, string methodName, JObject parameter)
		{
			m_methodInfo = instance.GetType().GetTypeInfo().GetMethod(methodName);
			try
			{
				ParameterInfo[] info = m_methodInfo.GetParameters();
				Object[] parameters = new Object[info.Length];
				int parametersIndex = 0;
				foreach (ParameterInfo parameterInfo in m_methodInfo.GetParameters())
				{
					if (parameterInfo.ParameterType.GetTypeInfo().IsArray)
					{
						IList<JToken> jsonResults = parameter[parameterInfo.Name].ToList();
						Array array = Array.CreateInstance(parameterInfo.ParameterType.GetElementType(), m_methodInfo.GetParameters().Length);
						int listIndex = 0;
						parameters[parametersIndex] = array;
						foreach (var value in jsonResults.Values())
						{
							parameters[parametersIndex].GetType().InvokeMember("SetValue", System.Reflection.BindingFlags.InvokeMethod, null, parameters[parametersIndex], new object[] { Convert.ChangeType(value, parameterInfo.ParameterType.GetElementType()), listIndex++ });
						}
						parametersIndex++;
					}
					else
					{
						var jsonResult = parameter[parameterInfo.Name];
						parameters[parametersIndex] = Activator.CreateInstance(parameterInfo.ParameterType);
						foreach (var value in jsonResult.Values())
						{
							parameters[parametersIndex++] = value;
						}
					}
				}
				Object returnObject = m_methodInfo.Invoke(instance, parameters);
				if (returnObject == null)
				{
					throw new Exception("Item cannot be found");
				}
				var updateObject = Activator.CreateInstance(typeof(Updater<>).MakeGenericType(m_methodInfo.ReturnType), m_host);
				updateObject.GetType().InvokeMember("GenericPost", System.Reflection.BindingFlags.InvokeMethod, null, updateObject, new object[] { m_endpoint, returnObject });
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString(), "Error");
				return;
			}

		}
		public void GetMethodInfos(object instance, string methodName, JObject parameter)
		{
			m_methodInfos = instance.GetType().GetTypeInfo().GetMethods();
			foreach (var mi in m_methodInfos)
			{
				CsMethodInfo temp = new CsMethodInfo();
				temp.methodName = mi.Name;
				temp.returnType = mi.ReturnType.Name;
				temp.parameterTypes = new string[mi.GetParameters().Length];
				int index = 0;
				foreach (var pi in mi.GetParameters())
				{
					temp.parameterTypes[index++] = pi.ParameterType.Name;
				}
				m_csMethodInfos.Add(temp);
				Console.WriteLine($"{temp.returnType} {temp.methodName}");
			}

			Updater<CsMethodInfo[]> updater = new Updater<CsMethodInfo[]>(m_host);
			updater.GenericPost(m_endpoint, m_csMethodInfos.ToArray());
		}
	}
}
