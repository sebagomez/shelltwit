
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class LikesTest : BaseTests
	{
		[TestMethod]
		public async Task GetUserLikes()
		{
			try
			{
				LikesOptions options = new LikesOptions { ScreenName = "sebagomez", User = m_user };
				List<Sebagomez.ShelltwitLib.Entities.Status> ss = await Likes.GetUserLikes(options);
				Assert.IsTrue(ss.Count > 0, "Ningún tweet!");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
