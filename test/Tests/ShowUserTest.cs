using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class ShowUserTest : BaseTests
	{
		[TestMethod]
		public async Task GetSebaGomez()
		{
			await InternalGetUser("sebagomez");
		}

		[TestMethod]
		public async Task GetSebaTestAPI()
		{
			await InternalGetUser("sebatestapi");
		}

		private async Task InternalGetUser(string screenname)
		{
			try
			{
				UserShowOptions options = new UserShowOptions { ScreenName = screenname, User = m_user };
				User user = await Sebagomez.ShelltwitLib.API.Tweets.UserData.GetUser(options);
				Assert.IsTrue(user.screen_name == screenname, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
