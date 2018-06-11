using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class TimelineOptions : TwitterOptions
	{
		public int SinceId { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (SinceId != 0)
				parameters.Add("since_id", SinceId.ToString());

			parameters.Add("tweet_mode", "extended");

			return parameters;
		}
	}
}
