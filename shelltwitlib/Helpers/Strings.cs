using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shelltwitlib.Helpers
{
	internal static class Strings
	{
		public static string FormatString(this string text, params object[] args)
		{
			return string.Format(text, args);
		}
	}
}
