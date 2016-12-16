using System;
using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class StreammingFilterOptions : TwitterOptions
	{
		public string Track { get; set; }
		public string Follow { get; set; }
		public string Locations { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			//if (string.IsNullOrWhiteSpace(Track + Follow + Locations))
			//	throw new ArgumentNullException("At least track, follow or locations must be added");

			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(Track))
				parameters.Add("track", Track);
			if (!string.IsNullOrWhiteSpace(Follow))
				parameters.Add("follow", Follow);
			if (!string.IsNullOrWhiteSpace(Locations))
				parameters.Add("locations", Locations);

			return parameters;
		}
	}
}
