using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class StatusTest : BaseTests
	{
		[TestMethod]
		public async Task StatusBasico()
		{
			string status = string.Format("Hello World:{0}", DateTime.Now);
			//string status = "Maybe he'll finally find his keys. #peterfalk";
			try
			{
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusBasicoRepetido()
		{
			DateTime now = DateTime.Now;
			string status = string.Format("Hello World:{0}", now);
			UpdateOptions options = new UpdateOptions { Status = status, User = m_user };
			string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(options);
			Assert.AreEqual(response, "OK");

			Thread.Sleep(1500);
			try
			{
				response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(options);
				Assert.AreEqual("{\"errors\":[{\"code\":187,\"message\":\"Status is a duplicate.\"}]}",response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusConUrl()
		{
			string status = string.Format("Hello World @ http://twitter.com :{0}", DateTime.Now);
			try
			{
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusConImagen()
		{
			try
			{
				string mediaPath = Path.Combine(AppContext.BaseDirectory, MEDIA1_NAME);

				string status = $"Este viene con imagen [{mediaPath}]: {DateTime.Now}";
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusConImagenGigante()
		{
			try
			{
				string mediaPath = Path.Combine(AppContext.BaseDirectory, HUGE_MEDIA);

				string status = string.Format(@"Este viene con imagen GRANDE [{0}]: {1}", mediaPath, DateTime.Now);
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual("{\"errors\":[{\"code\":324,\"message\":\"Image file size must be <= 3145728 bytes\"}]}", response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusConDosImagenes()
		{
			try
			{
				string mediaPath1 = Path.Combine(AppContext.BaseDirectory, MEDIA1_NAME);
				string mediaPath2 = Path.Combine(AppContext.BaseDirectory, MEDIA2_NAME);

				string status = string.Format(@"Este viene con 2 imagenes [{0}] [{1}]: {2}", mediaPath1, mediaPath2, DateTime.Now);
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		//[TestMethod]
		//public void StatusConDosImagenesSinBrackets()
		//{
		//	try
		//	{
		//		string mediaPath1 = Path.Combine(BASE_PATH, MEDIA1_NAME);
		//		string mediaPath2 = Path.Combine(BASE_PATH, MEDIA2_NAME);

		//		string status = string.Format(@"Este viene con 2 imagenes {0} {1}: {2}", mediaPath1, mediaPath2, DateTime.Now);
		//		string response = Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(status);
		//		Assert.AreEqual(response, "OK");
		//	}
		//	catch (Exception ex)
		//	{
		//		Assert.Fail(Util.ExceptionMessage(ex));
		//	}
		//}

		[TestMethod]
		public async Task StatusConImagenyCaracteresEspeciales()
		{
			try
			{
				string mediaPath = Path.Combine(AppContext.BaseDirectory, MEDIA1_NAME);

				string status = $"á é í ó ú ñ ! # @ [{mediaPath}]: {DateTime.Now}";
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task CaracteresEspeciales()
		{
			string status = string.Format("Los tildes son á é í ó ú y la ñ. Las myúsculas son Á É Í Ó Ú Ñ: {0}", DateTime.Now);
			try
			{
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task AConTilde()
		{
			string status = string.Format("á:{0}", DateTime.Now);
			try
			{
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task Exclamacion()
		{
			string status = string.Format("!:{0}", DateTime.Now);
			try
			{
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusProblematico()
		{
			string status = string.Format("des-i5 y la reconchaqueteparió!!!:{0}", DateTime.Now);
			try
			{
				string response = await Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task Menciones()
		{
			try
			{
				List<Sebagomez.ShelltwitLib.Entities.Status> result = await Mentions.GetMentions(new MentionsOptions { User = m_user });
				Assert.IsTrue(result.Count > 0, "Ningún tweet!");

			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task StatusEncodeado()
		{
			try
			{
				string status = string.Format("Excepción JAVA -Can't execute dynamic call:{0}", DateTime.Now);
				status = WebUtility.HtmlEncode(status);
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task AreYouSure()
		{
			try
			{
				string status = string.Format("3 millones de tsunaminólogos:{0}", DateTime.Now);
				string response = await Sebagomez.ShelltwitLib.API.Tweets.Update.UpdateStatus(new UpdateOptions { Status = status, User = m_user });
				Assert.AreEqual(response, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
