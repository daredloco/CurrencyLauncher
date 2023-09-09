using CurrencyLauncher.Models;
using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyLauncher
{
	internal class Utils
	{
		public static bool TryGetCurrencySymbol(string ISOCurrencySymbol, out string symbol, out int pattern)
		{
			int p = -1;
			symbol = CultureInfo
				.GetCultures(CultureTypes.AllCultures)
				.Where(c => !c.IsNeutralCulture)
				.Select(culture => {
					try
					{
						p = culture.NumberFormat.CurrencyPositivePattern;
						return new RegionInfo(culture.Name);
					}
					catch
					{
						return null;
					}
				})
				.Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol)
				.Select(ri => ri.CurrencySymbol)
				.FirstOrDefault();

			pattern = p;
			return symbol != null;
		}
	
		public static string GetSINCPath()
		{
			string? location = Settings.GetSINCLocation();
			if(location is not null)
			{
				//Check if location is valid
				if(File.Exists(Path.Combine(location, "Software Inc.exe")))
				{
					return location;
				}
			}

			//Load libraryfolders.vdf as some people don't use the default steam folder 
			string? steamLocation = Settings.GetSteamLocation().Replace("steam.exe","");
			if(steamLocation is null)
			{
				Debug.Log("Steam location is not set!");
				return "";
			}

			//Convert vdf data in readable data
			string librarysettingsPath = Path.Combine(steamLocation, "steamapps", "libraryfolders.vdf");
			if (!File.Exists(librarysettingsPath))
			{
				Debug.Log("Steams libraryfolders.vdf does not exist at \"" + librarysettingsPath + "\"...");
				return "";
			}
			var deserializedSettings = VdfConvert.Deserialize(File.ReadAllText(librarysettingsPath));
			var folders = deserializedSettings.Value.ToJson().ToObject<Dictionary<string,LibraryFolder>>();

			//Search folders for SINC folder
			foreach(LibraryFolder folder in folders.Values)
			{
				var sincLocation = folder.GetSincLocation();
				if(sincLocation is not null)
				{
					Settings.Instance.SINCLocation = sincLocation;
					return sincLocation;
				}
				
			}
			throw new Exception("Ups!");
		}

		public static bool CopyCurrencies()
		{
			string? sincloc = Settings.GetSINCLocation();
			if (sincloc == null)
			{
				Debug.Log("Couldnt copy currencies because no Software INC location is set. Please add the location first!");
				return false;
			}
			string modloc = Path.Combine(sincloc, "Mods");
			Directory.CreateDirectory(modloc);
			File.Copy("Currencies.xml", Path.Combine(modloc, "Currencies.xml"), true);
			return true;
		}
	}
}
