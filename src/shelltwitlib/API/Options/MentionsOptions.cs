using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class MentionsOptions : TwitterOptions
	{
		public string Since { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(Since))
				parameters.Add("since_id", Since);

			parameters.Add("tweet_mode", "extended");

			return parameters;
		}
	}
}
