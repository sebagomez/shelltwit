using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class StreamingAPITests : BaseTests
	{
		void Execute(StreamingOptions options)
		{
			try
			{
				StreamingEndpoint streamingFilter = new StreamingEndpoint();
				int count = 0;
				foreach (Status status in streamingFilter.GetStreamingStatus(options))
				{
					System.Diagnostics.Debug.Write(status);
					count++;

					if (count == 5)
						break;
				}

			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public void GetStreamingTimeline()
		{
			StreamingOptions options = new StreamingOptions { Track = "trump", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithSpace()
		{
			StreamingOptions options = new StreamingOptions { Track = "twitter com", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithComa()
		{
			StreamingOptions options = new StreamingOptions { Track = "twitter,facebook", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithHashtag()
		{
			StreamingOptions options = new StreamingOptions { Track = "#Trump", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingUserTimeline()
		{
			//sebatestapi 108356361
			StreamingOptions options = new StreamingOptions { User = m_user, Follow = "sebatestapi" };

			try
			{
				StreamingEndpoint streamingService = new StreamingEndpoint();

				int count = 0;
				foreach (Status status in streamingService.GetStreamingStatus(options))
				{
					System.Diagnostics.Debug.Write(status);
					count++;

					if (count == 1)
						break;
				}
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
