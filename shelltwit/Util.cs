using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace shelltwit
{
	internal static class Util
	{
		public static string ToFullString(this string[] args)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string s in args)
				sb.AppendFormat("{0} ",s);
			
			return sb.ToString().Remove(sb.Length - 1);
		}
	}
}
