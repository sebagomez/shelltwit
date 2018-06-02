using System;
using System.Collections.Generic;
using System.Linq;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.Shelltwit
{
	internal class Option
	{
		public string Short { get; set; }
		public string Long { get; set; }
		public string Description { get; set; }
		public bool IsDefault { get; set; }

		public Action<AuthenticatedUser, string[]> Action { get; private set; }

		public static Option GetOption(string[] arguments)
		{
			if (arguments.Length == 0)
				return GetAll(arguments).FirstOrDefault((o) => o.IsDefault);

			if (!arguments[0].StartsWith("-"))
				return Update();

			Option option = GetAll(arguments).FirstOrDefault((o) =>
		   {
			   if (arguments[0].StartsWith("--"))
				   return o.Long == arguments[0].Substring(2);
			   else if (arguments[0].StartsWith("-"))
				   return o.Short == arguments[0].Substring(1);

			   return false;

		   });

			return option;
		}

		public static Option Clear()
		{
			Option o = new Option()
			{
				Short = "c",
				Long = "clear",
				Description = "clears user stored credentials",
				Action = (u, s) => Util.ClearCredentials()
			};

			return o;
		}

		public static Option Timeline()
		{
			Option o = new Option()
			{
				Short = "t",
				Long = "timeline",
				Description = "show user's timeline",
				IsDefault = true,
				Action = (u, s) => Util.UserTimeLine(u)
			};

			return o;
		}

		public static Option Query()
		{
			Option o = new Option()
			{
				Short = "q",
				Long = "query",
				Description = "query twits containing words",
				Action = (u, s) => Util.UserSearch(u, s)
			};

			return o;
		}

		public static Option Mentions()
		{
			Option o = new Option()
			{
				Short = "m",
				Long = "mentions",
				Description = "show user's mentions",
				Action = (u, s) => Util.UserMentions(u)
			};

			return o;
		}

		public static Option User()
		{
			Option o = new Option()
			{
				Short = "u",
				Long = "user",
				Description = "show another user's timeline",
				Action = (u, s) => Util.UserTwits(u, s)
			};

			return o;
		}

		public static Option Track()
		{
			Option o = new Option()
			{
				Short = "k",
				Long = "track",
				Description = "live status with a specific track",
				Action = (u, s) => Util.StreamingTrack(u, s)
			};

			return o;
		}

		public static Option StreamedTimeline()
		{
			Option o = new Option()
			{
				Short = "s",
				Long = "streamed",
				Description = "streamed user timeline",
				Action = (u, s) => Util.StreamingTimeLine(u)
			};

			return o;
		}

		public static Option Likes()
		{
			Option o = new Option()
			{
				Short = "l",
				Long = "likes",
				Description = "user's likes (fka favorites)",
				Action = (u, s) => Util.UserLikes(u)
			};

			return o;
		}

		public static Option Help()
		{
			Option o = new Option()
			{
				Short = "h",
				Long = "help",
				Description = "show this help",
				Action = (u, s) => Util.ShowUsage()
			};

			return o;
		}

		public static Option Update()
		{
			Option o = new Option()
			{
				Action = (u, s) => Util.UpdateStatus(u, s)
			};

			return o;
		}

		public static IEnumerable<Option> GetAll(string[] args)
		{
			yield return Clear();
			yield return Timeline();
			yield return Query();
			yield return Mentions();
			yield return User();
			yield return Track();
			yield return StreamedTimeline();
			yield return Likes();
			yield return Help();
		}
	}
}
