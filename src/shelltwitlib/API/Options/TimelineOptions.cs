using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class TimelineOptions : TwitterOptions
	{
		public int SinceId { get; set; }
		public int Count { get; set; } = 50;

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (Count != 0)
				parameters.Add("count", Count.ToString());
			if (SinceId != 0)
				parameters.Add("since_id", SinceId.ToString());

			parameters.Add("tweet_mode", "extended");

			return parameters;
		}
	}
}
