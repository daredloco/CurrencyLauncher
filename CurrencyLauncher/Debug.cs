using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyLauncher
{
	internal static class Debug
	{
		internal static readonly string debugFile = "debug_log.txt";

		internal static void Log(string message)
		{
			message = DateTime.Now.ToString() + ": " + message;
			if (Settings.Instance is not null && Settings.Instance.DebugMode) { File.AppendAllText("debug.txt", message + Environment.NewLine); }
		}
	}
}
