using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class UserTimelineOptions : TwitterOptions
	{
		public long UserId { get; set; }
		public string ScreenName { get; set; }
		public int Count { get; set; }
		public long SinceId { get; set; }
		public long MaxId { get; set; }
		public bool ExcludeReplies { get; set; }
		public bool IncludeRTs { get; set; } = true;

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (UserId != 0)
				parameters.Add("user_id", UserId.ToString());
			if (!string.IsNullOrEmpty(ScreenName))
				parameters.Add("screen_name", ScreenName);
			if (SinceId != 0)
				parameters.Add("since_id", SinceId.ToString());
			if (Count != 0)
				parameters.Add("count", Count.ToString());
			if (MaxId != 0)
				parameters.Add("max_id", MaxId.ToString());
			if (ExcludeReplies)
				parameters.Add("exclude_replies", ExcludeReplies.ToString());
			if (!IncludeRTs)
				parameters.Add("include_rts", IncludeRTs.ToString());

			return parameters;
		}
	}
}
