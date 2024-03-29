﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Colorify;
using Colorify.UI;
using Sebagomez.Shelltwit.Security;
using Sebagomez.TwitterLib.API.Options;
using Sebagomez.TwitterLib.API.Tweets;
using Sebagomez.TwitterLib.Entities;
using Sebagomez.TwitterLib.Helpers;

namespace Sebagomez.Shelltwit.Misc
{
	internal static class PrintActions
	{
		#region Print

		//Taken from: https://github.com/deinsoftware/colorify
		static Format s_colorify = null;

		public static Format ColorifyInstance
		{
			get
			{
				if (s_colorify == null)
				{
					if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
						s_colorify = new Format(new ThemeLight());
					else
						s_colorify = new Format(new ThemeDark());
				}
				return s_colorify;
			}
		}

		static void PrintTwits(List<Status> twits)
		{
			if (twits == null)
				PrintError("No twits :(");
			else
				twits.ForEach(twit => PrintTwit(twit));
		}

		static void PrintTwit(Status twit)
		{
			ColorifyInstance.Write($"{twit.user.name}", Colors.txtInfo);
			if (twit.retweeted_status != null && twit.retweeted_status.user != null && !string.IsNullOrEmpty(twit.retweeted_status.user.screen_name))
				ColorifyInstance.Write($" (RT {twit.retweeted_status.user.screen_name})", Colors.txtSuccess);
			ColorifyInstance.WriteLine($": {twit.ResolvedText}", Colors.txtDefault);
		}

		static void PrintDMs(List<Event> dms)
		{
			dms.ForEach(dm => PrintDM(dm));
		}

		static void PrintDM(Event dm)
		{
			ColorifyInstance.Write($"{dm.message_create.target.recipient_id}", Colors.txtInfo);
			ColorifyInstance.WriteLine($": {dm.message_create.message_data.text}", Colors.txtDefault);
		}

		static void PrintTwits(SearchResult results)
		{
			if (results.statuses.Length == 0)
				PrintError("Sorry, no twits found :(");
			else
				PrintTwits(results.statuses.ToList<Status>());
		}

		public static void PrintWarning(string message) => ColorifyInstance.WriteLine(message, Colors.txtWarning);

		public static void PrintError(string message) => ColorifyInstance.WriteLine(message, Colors.txtDanger);

		public static void PrintInfo(string message) => ColorifyInstance.WriteLine(message, Colors.txtMuted);

		public static void Print(string message) => ColorifyInstance.WriteLine(message);

		#endregion

		public static void ClearCredentials()
		{
			CredentialsManager.ClearCredentials();
			PrintInfo("User credentials cleared!");
		}

		public static async Task UserTimeLine(AuthenticatedUser user, string[] args)
		{
			TimelineOptions options = new TimelineOptions { User = user };
			if (args.Length == 2)
			{
				try
				{
					options.Count = int.Parse(args[1]);
				}
				catch { }
			}

			PrintTwits(await Timeline.GetTimeline(options));
		}

		public static async Task UserMentions(AuthenticatedUser user)
		{
			PrintTwits(await Mentions.GetMentions(new MentionsOptions { User = user }));
		}

		public static async Task UserSearch(AuthenticatedUser user, string[] args)
		{
			SearchOptions options = new SearchOptions { Query = string.Join(" ", args.Skip(1)), User = user };
			PrintTwits(await Search.SearchTweets(options));
		}

		public static async Task UserLikes(AuthenticatedUser user)
		{
			PrintTwits(await Likes.GetUserLikes(new LikesOptions { User = user }));
		}

		public static async Task UserTwits(AuthenticatedUser user, string[] args)
		{
			if (args.Length != 2)
				throw new ArgumentNullException("screenname", "The user' screen name must be provided");
			UserTimelineOptions usrOptions = new UserTimelineOptions { ScreenName = args[1], User = user };
			PrintTwits(await UserTimeline.GetUserTimeline(usrOptions));
		}

		public static void StreamingTrack(AuthenticatedUser user, string[] args)
		{
			if (args.Length == 1)
				throw new ArgumentNullException("streaming", "The track must be provided");

			string track = string.Join(" ", args.Skip(1));
			PrintInfo($"Starting live streaming for '{track}', press Ctrl+C to quit");
			foreach (Status s in new StreamingEndpoint().GetStreamingStatus(new StreamingOptions { Track = track, User = user }))
				PrintTwit(s);
		}

		public static void StreamingTimeLine(AuthenticatedUser user, string[] args)
		{
			if (args.Length != 2)
				throw new ArgumentNullException("screenname", "The user id must be provided");

			string follow = args[1];
			PrintInfo($"Starting live streaming for user '{follow}', press Ctrl+C to quit");
			foreach (Status status in new StreamingEndpoint().GetStreamingStatus(new StreamingOptions { User = user, Follow = follow }))
				PrintTwit(status);
		}

		public static async Task UpdateStatus(AuthenticatedUser user, string[] args)
		{
			if (args.Length == 1 && args[0].Length == 1)
			{
				PrintWarning($"Really? do you really wanna twit \"{string.Join(" ", args)}\"?{System.Environment.NewLine}[T]wit, or [N]o sorry, I messed up...");
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
			string response = await Update.UpdateStatus(updOptions);

			if (response != "OK")
				PrintError($"Response was not OK: {response}");
		}

		public static async Task DirectMessage(AuthenticatedUser user, string[] args)
		{
			if (args.Length < 3)
				throw new ArgumentNullException("screenname", "The user' screen name (handle) must be provided");

			Console.WriteLine($"Validating handle '{args[1]}'");
			UserTimelineOptions usrOptions = new UserTimelineOptions { ScreenName = args[1], User = user };
			List<Status> result = await UserTimeline.GetUserTimeline(usrOptions);

			if (result.Count == 0)
				throw new ArgumentNullException("handle", $"The handle '{args[1]}' does not seem like a valid user.");

			long userId = result[0].user.id;

			DMOptions options = new DMOptions
			{
				User = user,
				RecipientId = userId.ToString(),
				Text = string.Join(" ", args, 2, args.Length - 2)
			};

			Console.WriteLine("Sending dm...");
			string response = await DirectMessages.SendDM(options);

			if (response != "OK")
				PrintError($"Response was not OK: {response}");
		}

		public static async Task ListDMs(AuthenticatedUser user, string[] args)
		{
			DMListOptions options = new DMListOptions { User = user };
			EventList result = await DirectMessages.GetDMList(options);

			PrintDMs(result.events);
		}

		public static void ShowUsage()
		{
			Console.WriteLine("Usage: twit [options] | <status> [<mediaPath>]");
			Console.WriteLine("");
			Console.WriteLine("Options:");
			foreach (Option option in Option.GetAll())
				Console.WriteLine("\t-{0}|--{1} {2}\t{3}", option.Short, option.Long, string.IsNullOrEmpty(option.Argument) ? "\t" : option.Argument, option.Description);
			Console.WriteLine("");
			Console.WriteLine("status:\r\n\tstatus to update at twitter.com");
			Console.WriteLine("");
			Console.WriteLine("mediaPath:\r\n\tfull path, between brackets, to the media files (up to four) to upload.");
			Console.WriteLine("");
		}
	}
}
