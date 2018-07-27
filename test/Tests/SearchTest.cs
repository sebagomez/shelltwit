using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class SearchTest : BaseTests
	{
		[TestMethod]
		public async Task SearchWord()
		{
			try
			{
				SearchOptions options = new SearchOptions { Query = "genexus", User = m_user };
				SearchResult result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.search_metadata.count >= result.statuses.Length, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task SearchManyWords()
		{
			try
			{
				SearchOptions options = new SearchOptions { Query = "genexus uruguay", User = m_user };
				SearchResult result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.search_metadata.count >= result.statuses.Length, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task SearchHashtag()
		{
			try
			{
				SearchOptions options = new SearchOptions { Query = "#Trump", User = m_user };
				SearchResult result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.search_metadata.count >= result.statuses.Length, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task SearchCount()
		{
			try
			{
				int count = 3;
				SearchOptions options = new SearchOptions { Query = "uruguay", Count = count, User = m_user };
				SearchResult result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.search_metadata.count >= result.statuses.Length, "OK");
				Assert.IsTrue(result.statuses.Length == count, "OK");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public async Task SearchSince()
		{
			try
			{
				SearchOptions options = new SearchOptions { Query = "uruguay", User = m_user };
				SearchResult result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.search_metadata.count >= result.statuses.Length, "OK");
				options = new SearchOptions { Query = "uruguay", SinceId = result.search_metadata.max_id };
				result = await Sebagomez.ShelltwitLib.API.Tweets.Search.SearchTweets(options);
				Assert.IsTrue(result.statuses.Length >= 0, "OK");

			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
