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
		void Execute(StreamingFilterOptions options)
		{
			try
			{
				StreamingFilter streamingFilter = new StreamingFilter();
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
			StreamingFilterOptions options = new StreamingFilterOptions { Track = "irma", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithSpace()
		{
			StreamingFilterOptions options = new StreamingFilterOptions { Track = "twitter com", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithComa()
		{
			StreamingFilterOptions options = new StreamingFilterOptions { Track = "twitter,facebook", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingTimelineWithHashtag()
		{
			StreamingFilterOptions options = new StreamingFilterOptions { Track = "#Camila", User = m_user };
			Execute(options);
		}

		[TestMethod]
		public void GetStreamingUserTimeline()
		{
			AuthenticatedUser sebagomez = LoadTestUser("sebagomez");

			StreamingUserOptions options = new StreamingUserOptions { User = sebagomez };

			try
			{
				StreamingUser streamingService = new StreamingUser();

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
