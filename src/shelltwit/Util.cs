﻿using System;
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

		public static void UpdateStatus(AuthenticatedUser user, string[] args)
		{
			if (args.Length == 1 && args[0].Length == 1)
			{
				Console.WriteLine($"Really? do you really wanna twit \"{string.Join(" ", args)}\"?{Environment.NewLine}[T]wit, or [N]o sorry, I messed up...");
				ConsoleKeyInfo input = Console.ReadKey();
				while (input.Key != ConsoleKey.T && input.Key != ConsoleKey.N)
				{
					Console.WriteLine();
					Console.WriteLine("[T]wit, or [N]o sorry, I messed up...");
					input = Console.ReadKey();
				}
				Console.WriteLine();

				if (input.Key == ConsoleKey.N)
				{
					Console.WriteLine("That's what I thought! ;)");
					return;
				}
			}

			UpdateOptions updOptions = new UpdateOptions { Status = string.Join(" ", args), User = user };
			string response = Update.UpdateStatus(updOptions).GetAwaiter().GetResult();

			if (response != "OK")
				Console.WriteLine($"Response was not OK: {response}");
		}

		public static void ShowUsage()
		{
			Console.WriteLine("Usage: twit [options] | <status> [<mediaPath>]");
			Console.WriteLine("");
			Console.WriteLine("Options:");
			foreach (Option option in Option.GetAll(new string[] { }))
				Console.WriteLine("\t-{0}|--{1}\t{2}{3}", option.Short, option.Long, option.Description, option.IsDefault ? " (Default)" : "");
			Console.WriteLine("");
			Console.WriteLine("status:\r\n\tstatus to update at twitter.com");
			Console.WriteLine("");
			Console.WriteLine("mediaPath:\r\n\tfull path, between brackets, to the media files (up to four) to upload.");
			Console.WriteLine("");
		}
	}
}
