using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using shelltwitlib.Helpers;

namespace shelltwitlib.API.Options
{
	public class SearchOptions : TwitterOptions
	{
		public enum ResultTypeOptions
		{
			Mixed,
			Recent,
			Popular
		}

		public string Query { get; set; }
		public int Count { get; set; } = 100;
		public bool IncludeEntities { get; set; } = false;
		public ResultTypeOptions ResultType { get; set; } = ResultTypeOptions.Mixed;

		Dictionary<string, string> m_parameters = null;
		public override Dictionary<string, string> GetParameters()
		{
			if (m_parameters == null)
			{
				m_parameters = new Dictionary<string, string>();
				m_parameters.Add("q", Query);
				if (Count != 100 && Count > 0)
					m_parameters.Add("count", Count.ToString());
				m_parameters.Add("include_entities", IncludeEntities.ToString().ToLower());
				if (ResultType != ResultTypeOptions.Recent)
					m_parameters.Add("result_type", ResultType.ToString().ToLower());
			}
			return m_parameters;
		}
	}
}
