using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TekConf.Client;

namespace TekConfApp
{
	partial class ConferenceTabBarController : UITabBarController
	{
		public ConferenceTabBarController (IntPtr handle) : base (handle)
		{
		}

		public Conference Conference {
			get;
			set;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (NavigationItem != null && Conference != null)
				NavigationItem.Title = Conference.Name;
		}
	}
}
