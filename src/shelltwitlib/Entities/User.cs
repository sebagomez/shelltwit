﻿namespace Sebagomez.ShelltwitLib.Entities
{
	public class User
	{
		public long id { get; set; }
		public string id_str { get; set; }
		public string name { get; set; }
		public string screen_name { get; set; }
		public string location { get; set; }
		public string description { get; set; }
		public string url { get; set; }
		public bool _protected { get; set; }
		public int followers_count { get; set; }
		public int friends_count { get; set; }
		public int listed_count { get; set; }
		public string created_at { get; set; }
		public int favourites_count { get; set; }
		public int? utc_offset { get; set; }
		public string time_zone { get; set; }
		public bool geo_enabled { get; set; }
		public bool verified { get; set; }
		public int statuses_count { get; set; }
		public string lang { get; set; }
		public bool contributors_enabled { get; set; }
		public bool is_translator { get; set; }
		public bool? is_translation_enabled { get; set; }
		public string profile_background_color { get; set; }
		public string profile_background_image_url { get; set; }
		public string profile_background_image_url_https { get; set; }
		public bool profile_background_tile { get; set; }
		public string profile_image_url { get; set; }
		public string profile_image_url_https { get; set; }
		public string profile_banner_url { get; set; }
		public string profile_link_color { get; set; }
		public string profile_sidebar_border_color { get; set; }
		public string profile_sidebar_fill_color { get; set; }
		public string profile_text_color { get; set; }
		public bool profile_use_background_image { get; set; }
		public bool? has_extended_profile { get; set; }
		public bool default_profile { get; set; }
		public bool default_profile_image { get; set; }
		public bool? following { get; set; }
		public bool? follow_request_sent { get; set; }
		public bool? notifications { get; set; }

		public override string ToString()
		{
			return $"{name} ({screen_name})";
		}
	}
}
