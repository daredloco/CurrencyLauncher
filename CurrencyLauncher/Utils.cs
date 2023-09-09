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
		internal static bool DebugMode = false;

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
	}
}
