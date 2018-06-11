using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class LikesOptions : TwitterOptions
	{
		public long UserId { get; set; }
		public string ScreenName { get; set; }
		public int Count { get; set; }
		public long SinceId { get; set; }
		public long MaxId { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (UserId != 0)
				parameters.Add("user_id", UserId.ToString());
			if (!string.IsNullOrEmpty(ScreenName))
				parameters.Add("screen_name", ScreenName.Trim());
			if (SinceId != 0)
				parameters.Add("since_id", SinceId.ToString());
			if (Count != 0)
				parameters.Add("count", Count.ToString());
			if (MaxId != 0)
				parameters.Add("max_id", MaxId.ToString());

			parameters.Add("tweet_mode", "extended");

			return parameters;
		}
	}
}
