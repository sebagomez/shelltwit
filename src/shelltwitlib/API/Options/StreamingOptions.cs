using System;
using System.Collections.Generic;
using System.Text;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class StreamingOptions : TwitterOptions
	{
		public string Track { get; set; }
		public string Follow { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(Track))
				parameters.Add("track", Track);
			if (!string.IsNullOrWhiteSpace(Follow))
				parameters.Add("follow", Follow);

			return parameters;
		}
	}
}
