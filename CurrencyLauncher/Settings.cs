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
		public bool DebugMode = false;

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
			DebugMode = bool.Parse(fLines[1]);
		}

		public void Save()
		{
			File.WriteAllLines(location, new string[]{
				SteamLocation,
				DebugMode.ToString()
			});
		}
	}
}
