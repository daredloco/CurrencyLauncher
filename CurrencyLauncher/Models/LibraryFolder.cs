using Gameloop.Vdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyLauncher.Models
{
	public class LibraryFolder
	{
		public string? Path { get; set; }
		public Dictionary<string, string>? Apps { get; set; }

		public string? GetSincLocation()
		{
			string manifestDir = System.IO.Path.Combine(Path, "steamapps", "appmanifest_" + Settings.GetId() + ".acf");
			if (!File.Exists(manifestDir))
			{
				//Manifest file was not found
				Debug.Log("Couldn't find the manifest file at \"" + manifestDir + "\"!");
				return null;
			}

			string[] manifestContent = File.ReadAllLines(manifestDir);
			string rightLine = manifestContent.Where(x => x.Contains("installdir")).First();
			rightLine = rightLine.Replace("\"installdir\"", "");
			rightLine = rightLine.Replace("\"", "");
			rightLine = rightLine.Trim();

			string SincPath = System.IO.Path.Combine(Path, "steamapps", "common", rightLine);
			return SincPath;
		}

	}
}
