using System;
using System.Collections.Generic;
using System.Linq;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.Shelltwit
{
	internal static class Util
	{
		#region Print

		static void PrintTwits(List<Status> twits)
		{
			if (twits == null)
				Console.WriteLine("No twits :(");
			else
				twits.ForEach(twit => PrintTwit(twit));
		}

		static void PrintTwit(Status twit)
		{
			Console.WriteLine($"{twit.user.name} (@{twit.user.screen_name}): {twit.text}");
		}

		static void PrintTwits(SearchResult results)
		{
			if (results.statuses.Length == 0)
				Console.WriteLine("Sorry, no tweets found :(");
			else
				PrintTwits(results.statuses.ToList<Status>());
		}

		#endregion

		public static void ClearCredentials()
		{
			AuthenticatedUser.ClearCredentials();
			Console.WriteLine("User credentials cleared!");
		}

		public static void UserTimeLine(AuthenticatedUser user)
		{
			PrintTwits(Timeline.GetTimeline(new TimelineOptions { User = user }).GetAwaiter().GetResult());
		}

		public static void UserMentions(AuthenticatedUser user)
		{
			PrintTwits(Mentions.GetMentions(new MentionsOptions { User = user }).GetAwaiter().GetResult());
		}

		public static void UserSearch(AuthenticatedUser user, string[] args)
		{
			SearchOptions options = new SearchOptions { Query = string.Join(" ", args.Skip(1)), User = user };
			PrintTwits(Search.SearchTweets(options).GetAwaiter().GetResult());
		}

		public static void UserLikes(AuthenticatedUser user)
		{
			PrintTwits(Likes.GetUserLikes(new LikesOptions { User = user }).GetAwaiter().GetResult());
		}

		public static void UserTwits(AuthenticatedUser user, string[] args)
		{
			if (args.Length != 2)
				throw new ArgumentNullException("screenname", "The user' screen name must be provided");
			UserTimelineOptions usrOptions = new UserTimelineOptions { ScreenName = args[1], User = user };
			PrintTwits(UserTimeline.GetUserTimeline(usrOptions).GetAwaiter().GetResult());
		}

		public static void StreamingTrack(AuthenticatedUser user, string[] args)
		{
			if (args.Length == 1)
				throw new ArgumentNullException("streaming", "The track must be provided");
			string track = string.Join(" ", args.Skip(1));
			StreamingFilterOptions streamingOptions = new StreamingFilterOptions { Track = track, User = user };
			Console.WriteLine($"Starting live streaming for '{track}', press ctrl+c to quit");
			StreamingFilter filter = new StreamingFilter();
			foreach (Status s in filter.GetStreamingStatus(streamingOptions))
				PrintTwit(s);
		}

		public static void StreamingTimeLine(AuthenticatedUser user)
		{
			StreamingUser streaing = new StreamingUser();
			foreach (Status status in streaing.GetStreamingStatus(new StreamingUserOptions { User = user }))
				PrintTwit(status);
		}
	}
}
