using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CurrencyLauncher
{
	internal class Restful
	{
		readonly HttpClient client;
		internal static Restful Instance;

		internal Restful()
		{
			Instance = this;
			client = new HttpClient();
			client.DefaultRequestHeaders.Add("Accept", "application/json");
		}

		public async Task<Dictionary<string, float>> GetRates(bool crypto = false)
		{
			string ApiRoot = "https://api.exchangerate.host/latest?base=USD";

			if (crypto)
			{
				ApiRoot += "&source=crypto";
			}
			try
			{
				Uri url = new(ApiRoot);
				HttpRequestMessage message = new(HttpMethod.Get, url);
				var response = await client.SendAsync(message);

				if (response.StatusCode != System.Net.HttpStatusCode.OK)
				{
					throw new Exception("Invalid status! => " + response.StatusCode.ToString());
				}
				var content = await response.Content.ReadAsStringAsync();
				JObject? jObject = JsonConvert.DeserializeObject<JObject>(content);
				
				bool? success = jObject.GetValue("success").Value<bool>();
				JToken rates = jObject.GetValue("rates");
				var dict = rates.ToObject<Dictionary<string, float>>();
				return dict;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
