using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class UserTimelineOptions : TimelineOptions
	{
		public long UserId { get; set; }
		public string ScreenName { get; set; }
		public long MaxId { get; set; }
		public bool ExcludeReplies { get; set; }
		public bool IncludeRTs { get; set; } = true;

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = base.GetParameters();
			if (UserId != 0)
				parameters.Add("user_id", UserId.ToString());
			if (!string.IsNullOrEmpty(ScreenName))
				parameters.Add("screen_name", ScreenName.Trim());
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
