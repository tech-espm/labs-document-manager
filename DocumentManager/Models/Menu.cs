using DocumentManager.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Drawing;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using DocumentManager.Localization;

namespace DocumentManager.Models {
	public class Menu {
		private static readonly Dictionary<int, Dictionary<int, List<Menu>>> MenusByProfileIdAndLanguageId = new Dictionary<int, Dictionary<int, List<Menu>>>(16);
		private static readonly CommonReadRareWriteLock Lock = new CommonReadRareWriteLock();

		public readonly string Link, IconClass, Text, ExtraAttributes;
		public readonly List<Menu> SubMenus;
		public readonly bool Spacer;

		private static bool HasFeature(int[] features, int featureCount, Feature requestedFeature) {
			int r = (int)requestedFeature, s = 0, e = featureCount - 1, a;
			while (s <= e) {
				a = (s + e) >> 1;
				int f = features[a];
				if (r == f)
					return true;
				if (r < f)
					e = a - 1;
				else
					s = a + 1;
			}
			return false;
		}

		private static List<Menu> GenerateMenu(int profileId, int languageId) {
			int oldLanguage = Str.CurrentLanguage;
			if (oldLanguage != languageId)
				Str.SetCurrentLanguage(languageId);

			try {
				List<Menu> menus = new List<Menu>(), sub;
				int featureCount = 0;
				int[] features = new int[(int)Feature.Max];

				if (profileId != Profile.ADMIN_ID) {
					using (MySqlConnection conn = Sql.OpenConnection()) {
						using (MySqlCommand cmd = new MySqlCommand($"SELECT feature_id FROM profile_feature WHERE profile_id = @profile_id ORDER BY feature_id ASC", conn)) {
							cmd.Parameters.AddWithValue("@profile_id", profileId);
							using (MySqlDataReader reader = cmd.ExecuteReader()) {
								while (reader.Read() && featureCount < (int)Feature.Max)
									features[featureCount++] = reader.GetInt32(0);
							}
						}
					}
				}

				bool create, list;

				sub = null;

				#region Units
				create = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.CourseCreate));
				list = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.CourseList));
				if (create || list) {
					sub = new List<Menu>();
					if (create)
						sub.Add(new Menu("/Unity/Create", "fa fa-plus fa-fw", Str.Create));
					if (list)
						sub.Add(new Menu("/Unity/Manage", "fa fa-tasks fa-fw", Str.Manage));
					menus.Add(new Menu("#", "fa fa-university fa-fw", Str.Units, sub));
				}
				#endregion

				#region Courses
				create = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.CourseCreate));
				list = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.CourseList));
				if (create || list) {
					sub = new List<Menu>();
					if (create)
						sub.Add(new Menu("/Course/Create", "fa fa-plus fa-fw", Str.Create));
					if (list)
						sub.Add(new Menu("/Course/Manage", "fa fa-tasks fa-fw", Str.Manage));
					menus.Add(new Menu("#", "fa fa-graduation-cap fa-fw", Str.Courses, sub));
				}
				#endregion

				#region Partition Types
				create = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.PartitionTypeCreate));
				list = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.PartitionTypeList));
				if (create || list) {
					sub = new List<Menu>();
					if (create)
						sub.Add(new Menu("/PartitionType/Create", "fa fa-plus fa-fw", Str.Create));
					if (list)
						sub.Add(new Menu("/PartitionType/Manage", "fa fa-tasks fa-fw", Str.Manage));
					menus.Add(new Menu("#", "fa fa-tags fa-fw", Str.PartitionTypes, sub));
				}
				#endregion

				#region Document Types
				create = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.DocumentTypeCreate));
				list = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.DocumentTypeList));
				if (create || list) {
					sub = new List<Menu>();
					if (create)
						sub.Add(new Menu("/DocumentType/Create", "fa fa-plus fa-fw", Str.Create));
					if (list)
						sub.Add(new Menu("/DocumentType/Manage", "fa fa-tasks fa-fw", Str.Manage));
					menus.Add(new Menu("#", "fa fa-tag fa-fw", Str.DocumentTypes, sub));
				}
				#endregion

				if (sub != null) {
					menus.Add(new Menu());
					sub = null;
				}

				#region Documents
				create = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.DocumentCreate));
				list = (profileId == Profile.ADMIN_ID || HasFeature(features, featureCount, Feature.DocumentList));
				if (create || list) {
					sub = new List<Menu>();
					if (create)
						sub.Add(new Menu("/Document/Create", "fa fa-plus fa-fw", Str.Create));
					if (list)
						sub.Add(new Menu("/Document/Manage", "fa fa-tasks fa-fw", Str.Manage));
					menus.Add(new Menu("#", "fa fa-file-text-o fa-fw", Str.Documents, sub));
				}
				#endregion

				if (sub != null) {
					menus.Add(new Menu());
					sub = null;
				}

				// There are no features for profiles and users because only
				// administrators can work with them!
				if (profileId == Profile.ADMIN_ID) {
					menus.Add(new Menu("#", "fa fa-users fa-fw", Str.Profiles, new List<Menu>() {
						new Menu("/Profile/Create", "fa fa-plus fa-fw", Str.Create),
						new Menu("/Profile/Manage", "fa fa-tasks fa-fw", Str.Manage)
					}));
					menus.Add(new Menu("#", "fa fa-user fa-fw", Str.Users, new List<Menu>() {
						new Menu("/User/Create", "fa fa-plus fa-fw", Str.Create),
						new Menu("/User/Manage", "fa fa-tasks fa-fw", Str.Manage)
					}));

					menus.Add(new Menu());
				}

				return menus;
			} finally {
				if (oldLanguage != languageId)
					Str.SetCurrentLanguage(oldLanguage);
			}
		}

		public static void PurgeCachedMenusByProfileId(int profileId) {
			try {
				Lock.EnterWriteLock();
				MenusByProfileIdAndLanguageId.Remove(profileId);
			} finally {
				Lock.ExitWriteLock();
			}
		}

		public static List<Menu> GetCachedMenusByProfileIdAndLanguageId(int profileId, int languageId) {
			Dictionary<int, List<Menu>> menusByLanguageId;
			List<Menu> list = null;
			try {
				Lock.EnterReadLock();
				MenusByProfileIdAndLanguageId.TryGetValue(profileId, out menusByLanguageId);
				if (menusByLanguageId != null)
					menusByLanguageId.TryGetValue(languageId, out list);
			} finally {
				Lock.ExitReadLock();
			}
			if (list == null) {
				try {
					Lock.EnterWriteLock();
					if (!MenusByProfileIdAndLanguageId.TryGetValue(profileId, out menusByLanguageId)) {
						menusByLanguageId = new Dictionary<int, List<Menu>>(16);
						MenusByProfileIdAndLanguageId[profileId] = menusByLanguageId;
					}
					if (!menusByLanguageId.TryGetValue(languageId, out list))
						menusByLanguageId[languageId] = (list = GenerateMenu(profileId, languageId));
				} finally {
					Lock.ExitWriteLock();
				}
			}
			return list;
		}

		private Menu() {
			Spacer = true;
		}

		private Menu(string link, string iconClass, string text, List<Menu> subMenus = null) {
			Link = link;
			IconClass = iconClass;
			Text = text;
			SubMenus = subMenus;
			ExtraAttributes = "";
		}

		private Menu(string link, string iconClass, string text, string extraAttributes) {
			Link = link;
			IconClass = iconClass;
			Text = text;
			SubMenus = null;
			ExtraAttributes = extraAttributes;
		}

		public override string ToString() {
			return Text;
		}
	}
}
