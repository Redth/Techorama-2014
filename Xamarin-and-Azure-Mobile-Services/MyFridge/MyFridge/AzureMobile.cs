using System;
using Microsoft.WindowsAzure.MobileServices;
using System.IO;

namespace MyFridge
{
	public class AzureMobile
	{
		public AzureMobile ()
		{
		}

		public static MobileServiceClient MobileService;

		static AzureMobile()
		{
			MobileService = new MobileServiceClient(
				"YOUR-MOBILE-SERVICE-URL",
				"YOUR-MOBILE-SERVICE-KEY"
			);

			CurrentPlatform.Init();
		}

		public static void SaveLogin()
		{
			var token = MobileService.CurrentUser.MobileServiceAuthenticationToken;
			var user = MobileService.CurrentUser.UserId;

			var path = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "cache.txt");

			File.WriteAllText (path, user + "|" + token);
		}

		public static bool LoadLogin()
		{
			var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "cache.txt");

			if (File.Exists (path)) {
				var data = File.ReadAllText (path);

				if (!string.IsNullOrEmpty (data) && data.Contains ("|")) {
					var parts = data.Split(new [] { '|' }, 2);

					if (parts != null && parts.Length == 2) {
						MobileService.CurrentUser = new MobileServiceUser(parts[0]);
						MobileService.CurrentUser.MobileServiceAuthenticationToken = parts[1];
						return true;
					}
				}
			}

			return false;
		}
	}
}

