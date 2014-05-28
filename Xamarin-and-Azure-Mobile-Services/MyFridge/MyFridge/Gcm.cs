
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gcm;
using WindowsAzure.Messaging;

namespace MyFridge
{
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE },
		Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
		Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
		Categories = new string[] { "@PACKAGE_NAME@" })]
	public class SampleGcmBroadcastReceiver : GcmBroadcastReceiverBase<SampleGcmService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project Number
		public static string[] SENDER_IDS = { "YOUR-PROJECT-NUMBER" };
	}

	[Service] //Must use the service tag
	public class SampleGcmService : GcmServiceBase
	{
		static NotificationHub hub;

		public static void Initialize(Context context)
		{
			// Call this from our main activity
			var cs = "YOUR-SHARED-LISTEN-CONNECTION-STRING";
			var hubName = "YOUR-HUB-NAME";

			hub = new NotificationHub (hubName, cs, context);
		}

		public static void Register(Context Context)
		{
			// Makes this easier to call from our Activity
			GcmClient.Register (Context, SampleGcmBroadcastReceiver.SENDER_IDS);
		}

		public SampleGcmService() : base(SampleGcmBroadcastReceiver.SENDER_IDS)
		{
		}

		protected override void OnRegistered (Context context, string registrationId)
		{
			//Receive registration Id for sending GCM Push Notifications to

			Console.WriteLine("Registered: " + registrationId);

			if (hub != null) {
				hub.UnregisterAll (registrationId);

				hub.RegisterTemplate (registrationId, "message", "{ \"data\": { \"message\": \"$(message)\" } }",
					AzureMobile.MobileService.CurrentUser.UserId);				
			}
		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			if (hub != null)
				hub.UnregisterAll (registrationId);
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			Console.WriteLine ("Received Notification");

			//Push Notification arrived - print out the keys/values
			if (intent != null || intent.Extras != null) {

				var keyset = intent.Extras.KeySet ();

				foreach (var key in intent.Extras.KeySet())
					Console.WriteLine ("Key: {0}, Value: {1}", key, intent.Extras.GetString(key));

				var msg = intent.GetStringExtra ("message");

				var n = new Notification.Builder (context);
				n.SetSmallIcon (Android.Resource.Drawable.IcDialogAlert);
				n.SetContentTitle ("InFridge Expiration Notice");
				n.SetTicker (msg);
				n.SetContentText (msg);

				var nm = NotificationManager.FromContext (context);
				nm.Notify (0, n.Build ());
			}
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			//Some recoverable error happened
			return true;
		}

		protected override void OnError (Context context, string errorId)
		{
			//Some more serious error happened
		}
	}
}

