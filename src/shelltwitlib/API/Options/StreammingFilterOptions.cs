using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class StreamingFilterOptions : StreamingOptions
	{
		public string Locations { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (!string.IsNullOrWhiteSpace(Locations))
				parameters.Add("locations", Locations);

			foreach (KeyValuePair<string, string> p in base.GetParameters())
				parameters[p.Key] = p.Value;

			return parameters;
		}
	}
}
