using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class BaseTests
	{
		internal const string MEDIA1_NAME = @"Noooooooo.jpg";
		internal const string MEDIA2_NAME = @"Snapshot.jpg";
		internal const string HUGE_MEDIA = @"nemonew.png";

		protected AuthenticatedUser m_user;

		protected AuthenticatedUser LoadTestUser(string userhandle)
		{
			using (FileStream file = File.Open(Path.Combine(AppContext.BaseDirectory, $"{userhandle}.usr"), FileMode.Open))
				return Util.Deserialize<AuthenticatedUser>(file);
		}

		public BaseTests()
		{
			m_user = LoadTestUser("sebatestapi");
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		// You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		[ClassInitialize()]
		public static void StatusInitialize(TestContext testContext)
		{
			Sebagomez.ShelltwitLib.API.Tweets.Update.SetMessageAction(s =>
			{
				Console.WriteLine(s);
			});
		}

		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		//[TestInitialize()]
		[TestCleanup()]
		public void MyTestInitialize()
		{
			Thread.Sleep(1 * 1000); // Wait a little bit before updting (twit quota might be exceeded
		}
	}
}
