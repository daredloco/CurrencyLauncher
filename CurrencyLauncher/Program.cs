using CurrencyLauncher;
using System.Xml.Linq;
if (args.Length > 0)
{
	foreach (var arg in args)
	{
		if(arg == "--debug")
		{
			Utils.DebugMode = true;
		}
	}
}
Console.ForegroundColor = ConsoleColor.Green;

int? answer = null;
while(answer == null)
{
	Console.WriteLine("Currency Updater for Software INC");
	Console.WriteLine("---------------------------------");
	Console.WriteLine();
	Console.WriteLine("[1] Update forex currencies");
	Console.WriteLine("[2] Update crypto currencies");
	Console.WriteLine("[3] Exit");
	var key = Console.ReadKey().Key;


	if (key == ConsoleKey.D1)
	{
		answer = 1;
	}else if(key == ConsoleKey.D2)
	{
		answer = 2;
	}else if(key == ConsoleKey.D3)
	{
		Environment.Exit(0);
	}
	else
	{
		answer = null;
	}
	Console.Clear();
}


Console.WriteLine("Initializing Restful Handler...");
new Restful();

Console.WriteLine("Fetching current exchange rates...");
var rates = await Restful.Instance.GetRates(answer == 2);

Console.WriteLine($"Writing {rates.Count()} exchange rates into database...");

XElement root = new XElement("Currencies");

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
				currencyRoot.Add(new XElement("Prefix", currSymbol != "" ? currSymbol : rate.Key));
				currencyRoot.Add(new XElement("Postfix", ""));
				break;
			case 1:
				currencyRoot.Add(new XElement("Prefix", ""));
				currencyRoot.Add(new XElement("Postfix", currSymbol != "" ? currSymbol : rate.Key));
				break;
			case 2:
				currencyRoot.Add(new XElement("Prefix", currSymbol != "" ? currSymbol + " " : rate.Key));
				currencyRoot.Add(new XElement("Postfix", ""));
				break;
			case 3:
				currencyRoot.Add(new XElement("Prefix", ""));
				currencyRoot.Add(new XElement("Postfix", currSymbol != "" ? " " + currSymbol : rate.Key));
				break;
			default:
				currencyRoot.Add(new XElement("Prefix", currSymbol != "" ? currSymbol + " " : rate.Key + " "));
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

