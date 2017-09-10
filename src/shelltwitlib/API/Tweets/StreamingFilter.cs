using System;
using System.Net;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class StreamingFilter : StreamingBase
	{
		const string STATUS_FILTER = "https://stream.twitter.com/1.1/statuses/filter.json";

		protected override string GetStreamingUrl()
		{
			return STATUS_FILTER;
		}

		protected override void EncodeOptions(StreamingOptions options)
		{
			StreamingFilterOptions sfo = options as StreamingFilterOptions;
			if (sfo == null)
				throw new InvalidCastException("Option must be StreamingFilterOptions");

			base.EncodeOptions(sfo);
			sfo.Locations = string.IsNullOrEmpty(sfo.Locations) ? sfo.Locations : Util.EncodeString(WebUtility.HtmlDecode(sfo.Locations));
		}

	}

}
