using CurrencyLauncher;
using System.Xml.Linq;

Console.WriteLine("Initializing Restful Handler...");
new Restful();

Console.WriteLine("Fetching current exchange rates...");
var rates = await Restful.Instance.GetRates();

Console.WriteLine($"Writing {rates.Count()} exchange rates into database...");

XElement root = new XElement("Currencies");

//XElement root = new XElement("Currencies",
//	from keyValue in rates
//	select new XElement(keyValue.Key, keyValue.Value)
//);
await Task.Run(() =>
{
	int progress = 0;
	int total = rates.Count;
	foreach (var rate in rates)
	{
		//Get prefix
		string currSymbol;
		int pattern = -1;
		if (!Utils.TryGetCurrencySymbol(rate.Key, out currSymbol, out pattern))
		{
			currSymbol = "";
		}

		progress++;
		Console.WriteLine(progress + "/" + total + ": Adding exchange rate for " + rate.Key);
		XElement currencyRoot = new XElement("Currency");
		currencyRoot.Add(new XElement("Name", rate.Key));
		switch (pattern)
		{
			case 0:
				currencyRoot.Add(new XElement("Prefix", currSymbol));
				currencyRoot.Add(new XElement("Postfix", ""));
				break;
			case 1:
				currencyRoot.Add(new XElement("Prefix", ""));
				currencyRoot.Add(new XElement("Postfix", currSymbol));
				break;
			case 2:
				currencyRoot.Add(new XElement("Prefix", currSymbol + " "));
				currencyRoot.Add(new XElement("Postfix", ""));
				break;
			case 3:
				currencyRoot.Add(new XElement("Prefix", ""));
				currencyRoot.Add(new XElement("Postfix", " " + currSymbol));
				break;
			default:
				currencyRoot.Add(new XElement("Prefix", currSymbol));
				currencyRoot.Add(new XElement("Postfix", ""));
				break;
		}
		
		currencyRoot.Add(new XElement("Rate", rate.Value));
		root.Add(currencyRoot);
	}
});


Console.WriteLine("Overwriting Currencies.xml...");
File.WriteAllText("Currencies.xml", root.ToString());

Console.WriteLine("All done! Your exchange rates are up to date =)");
Console.WriteLine("Press any key to close this window, we're done here!");
Console.ReadKey();

