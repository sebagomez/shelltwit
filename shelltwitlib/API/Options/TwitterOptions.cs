using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shelltwitlib.Helpers;

namespace shelltwitlib.API.Options
{
	public abstract class TwitterOptions
	{
		public AuthenticatedUser User { get; set; }

		public abstract Dictionary<string, string> GetParameters();

		public string GetUrlParameters()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var item in GetParameters())
				builder.Append($"&{item.Key}={item.Value}");

			return builder.ToString().Substring(1);
		}
	}
}
