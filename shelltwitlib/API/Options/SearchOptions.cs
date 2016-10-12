using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class SearchOptions : TwitterOptions
	{
		public enum ResultTypeOptions
		{
			Mixed,
			Recent,
			Popular
		}

		public enum FeedType
		{
			All,
			Tweets,
			User,
			Images,
			Videos, 
			News
		}

		public string Query { get; set; }
		public int Count { get; set; } = 100;
		public bool IncludeEntities { get; set; } = false;
		public ResultTypeOptions ResultType { get; set; } = ResultTypeOptions.Mixed;
		public long SinceId { get; set; }
		public long MaxId { get; set; }
		public FeedType Feed { get; set; } =  FeedType.All;

		public override Dictionary<string, string> GetParameters()
		{
			Dictionary<string, string> parameters = new Dictionary<string, string>();
			parameters.Add("q", Query);
			if (Feed != FeedType.All)
				parameters.Add("f", Feed.ToString().ToLower());
			if (Count > 0)
				parameters.Add("count", Count.ToString());
			parameters.Add("include_entities", IncludeEntities.ToString().ToLower());
			if (ResultType != ResultTypeOptions.Recent)
				parameters.Add("result_type", ResultType.ToString().ToLower());
			if (SinceId != 0)
				parameters.Add("since_id", SinceId.ToString());
			if (MaxId != 0)
				parameters.Add("max_id", MaxId.ToString());

			return parameters;
		}
	}
}
