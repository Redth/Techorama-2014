using System;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using MonoTouch.Dialog;

namespace TekConfApp
{
	partial class SessionsViewController : DialogViewController
	{
		public SessionsViewController (IntPtr handle) : base (handle)
		{
			var c = ((ConferenceTabBarController)TabBarController).Conference;

			Root = new RootElement ("Sessions");

			var groups = from s in c.Sessions
			             group s by s.Start.ToString ("MMM d @ h:mm tt") into g
			             select g;

			foreach (var g in groups) {
				var section = new Section (g.Key);

				foreach (var s in g) {
					section.Add (new StyledStringElement (s.Title));
				}

				Root.Add (section);
			}


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TableView.ContentInset = new UIEdgeInsets (64, 0, 50, 0);
		}
	}
}
