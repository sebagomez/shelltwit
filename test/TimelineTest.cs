using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class TimelineTest : BaseTests
	{
		[TestMethod]
		public async Task GetTimeline()
		{
			try
			{
				List<Sebagomez.ShelltwitLib.Entities.Status> ss = await Timeline.GetTimeline(new TimelineOptions { User = m_user });
				Assert.IsTrue(ss.Count > 0, "Ningún tweet!");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task GetUserTimeline()
		{
			try
			{
				UserTimelineOptions options = new UserTimelineOptions { ScreenName = "sebagomez", ExcludeReplies = true, IncludeRTs = false, User = m_user };
				List<Sebagomez.ShelltwitLib.Entities.Status> ss = await UserTimeline.GetUserTimeline(options);
				Assert.IsTrue(ss.Count > 0, "Ningún tweet!");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task GetMaxUserTimeline()
		{
			try
			{
				int count = 200;
				UserTimelineOptions options = new UserTimelineOptions { ScreenName = "sebagomez", Count = count, User = m_user };
				List<Sebagomez.ShelltwitLib.Entities.Status> ss = await UserTimeline.GetUserTimeline(options);
				Assert.IsTrue(ss.Count > 0, "Ningún tweet!");
				Assert.IsTrue(ss.Count <= count, $"No trajo {count}?");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
