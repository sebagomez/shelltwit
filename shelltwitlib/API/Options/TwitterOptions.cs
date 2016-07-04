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

		public abstract string GetUrlParameters();
	}
}
