using System;
using System.Collections.Generic;
using System.Text;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class UserShowOptions: TwitterOptions
	{
		public string ScreenName { get; set; }
		public string UserId { get; set; }

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(ScreenName))
				parameters.Add("screen_name", ScreenName);
			if (!string.IsNullOrEmpty(UserId))
				parameters.Add("user_id", UserId);

			return parameters;
		}
	}
}
