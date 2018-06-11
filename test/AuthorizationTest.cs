using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class AuthorizationTest : BaseTests
	{
		[TestMethod]
		public void GetWebAccessToken()
		{
			try
			{
				string token = OAuthAuthenticator.GetOAuthToken().Result;

				Assert.IsTrue(token.Length == 27, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		//[TestMethod] Not automatic... it needs the PIN and oAuthToken
		public void GetPINAuthToken()
		{
			try
			{
				string pin = "3616991";
				string oAuthToken = "Kn5i6AAAAAAAARPWAAABXHm2bfw";
				string accessTokens = OAuthAuthenticator.GetPINToken(oAuthToken, pin).Result;

				AuthenticatedUser user = new AuthenticatedUser();
				user.SerializeTokens(accessTokens);

			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
