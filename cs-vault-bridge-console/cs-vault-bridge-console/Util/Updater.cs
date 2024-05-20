using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace cs_vault_bridge_console
{
	internal class Updater<T>
	{
		public string baseUrl { get { return this.baseUrl; } set { this.baseUrl = value; } }
		static readonly HttpClient client = new HttpClient();
		LinkedList<T> list = new LinkedList<T>();
		public Updater(string url){
			baseUrl = url;
		}
		// HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.

		public async void UpdateTestMethod() {
			try
			{
				HttpResponseMessage response = await client.GetAsync("http://localhost:8080/test");
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();
				// Above three lines can be replaced with new helper method below
				// string responseBody = await client.GetStringAsync(uri);

				Console.WriteLine(responseBody);
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
		public async void GenericPost( string uri, T toPost )
		{
			try
			{
				string json = JsonSerializer.Serialize(toPost);
				//Console.WriteLine(json);
				StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync( this.baseUrl + uri, httpContent);
				response.EnsureSuccessStatusCode();
				
				string responseBody = await response.Content.ReadAsStringAsync();
				// Above three lines can be replaced with new helper method below
				// string responseBody = await client.GetStringAsync(uri);

				Console.WriteLine(responseBody);
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
	}
}
