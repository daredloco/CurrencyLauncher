using CurrencyLauncher;
using System.Diagnostics;
using System.Xml.Linq;

Settings settings = new();

if (args.Length > 0)
{
	foreach (var arg in args)
	{
		if(arg == "-debug")
		{
			settings.DebugMode = true;
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
	Console.WriteLine("[3] Set Steam location");
	Console.WriteLine("[4] Exit");
	var key = Console.ReadKey(true).Key;


	if (key == ConsoleKey.D1)
	{
		answer = 1;
	}
	else if (key == ConsoleKey.D2)
	{
		answer = 2;
	}else if(key == ConsoleKey.D3)
	{
		Console.Write("Enter path to Steam: ");
		var location = Console.ReadLine();
		if (location == null || location == "") { 
		} else if (Directory.Exists(location))
		{
			string path = Path.Combine(location, "steam.exe");
			if (File.Exists(path))
			{
				settings.SteamLocation = path;
				Console.WriteLine("Steam path updated!");
				Console.WriteLine("Trying to find Software INC path...");
				var sincPath = Utils.GetSINCPath();
				if(sincPath == "")
				{
					Console.WriteLine("Couldnt find Software INC path... Autostart is deactivated!");
					Console.WriteLine("Take a look at debug_log.txt to see what went wrong. You can activate it with the -debug parameter");
				}
				else
				{
					Console.WriteLine("Found Software INC path!");
				}
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
			}
			else
			{
				Console.WriteLine("Couldn't find Steam at '" + path + "'!");
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
			}
		}
		else
		{
			Console.Write("Path isn't a file!");
			Console.ReadKey();
		}
		answer = null;
	}else if(key == ConsoleKey.D4)
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
Console.WriteLine("Update settings...");
settings.Save();

Console.WriteLine("Fetching current exchange rates...");
var rates = await Restful.Instance.GetRates(answer == 2);

Console.WriteLine($"Writing {rates.Count()} exchange rates into database...");

XElement root = new XElement("Currencies");

XElement kennethRoot = new XElement("Currency");
kennethRoot.Add(new XElement("Name", "KENNETH"));
kennethRoot.Add(new XElement("Prefix", ""));
kennethRoot.Add(new XElement("Postfix", " KENNETH"));
kennethRoot.Add(new XElement("Rate", "0.01"));
root.Add(kennethRoot);
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


Console.WriteLine("Creating Currencies.xml...");
File.WriteAllText("Currencies.xml", root.ToString());
Console.WriteLine("All done! Your exchange rates are up to date =)");

if(settings.SteamLocation is null)
{
	Console.WriteLine("Press any key to launch the game, we're done here!");
	Console.ReadKey();
}
else
{
	if(settings.SINCLocation is null)
	{
		Console.WriteLine("Can't autostart Software INC because the location is not set...");
		Console.ReadKey();
		return;
	}
	Console.WriteLine("Trying to copy Currencies.xml to Software Inc folder...");
	if (!Utils.CopyCurrencies())
	{
		Console.WriteLine("Couldn't copy Currencies.xml to Software Inc folder, you'll need to manually copy it!");
		Console.ReadKey();
		return;
	}
	Console.WriteLine("Currencies.xml copied!");
	Console.WriteLine("Trying to launch game with Steam...");
	Process.Start(settings.SteamLocation, "steam://rungameid/" + Settings.GetId());
	Console.WriteLine("Have fun! You can close this window now...");
	Console.ReadKey();
}

