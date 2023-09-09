using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyLauncher
{
	internal class Settings
	{
		public static Settings? Instance;

		private readonly string location = "launcher.cfg";

		public string SteamLocation = "";
		public string SINCLocation = "";
		public bool DebugMode = true;

		public static string? GetSteamLocation()
		{
			if(Instance == null) { return null; } return Instance.SteamLocation;
		}

		public static string? GetSINCLocation()
		{
			if(Instance == null) { return null; } return Instance.SINCLocation;
		}

		public static string GetId()
		{
			return "362620";
		}

		internal Settings()
		{
			Instance = this;
			Load();
		}

		private void Load()
		{
			if (!File.Exists(location))
			{
				return;
			}
			string[] fLines = File.ReadAllLines(location);
			SteamLocation = fLines[0];
			SINCLocation = fLines[1];
			
		}

		public void Save()
		{
			File.WriteAllLines(location, new string[]{
				SteamLocation,
				SINCLocation,
			});
		}
	}
}
