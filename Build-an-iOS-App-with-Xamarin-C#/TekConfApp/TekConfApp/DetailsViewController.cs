using System;
using System.Threading.Tasks;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TekConfApp
{
	partial class DetailsViewController : UIViewController
	{
		public DetailsViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var c = ((ConferenceTabBarController)this.TabBarController).Conference;

			title.Text = c.Name;
			description.Text = c.Description;

			Task.Factory.StartNew (() => {
				using (var data = NSData.FromUrl (new NSUrl (c.ImageUrl))) {

					var img = UIImage.LoadFromData (data);

					BeginInvokeOnMainThread(() => {
						image.Alpha = 0f;
						image.Image = img;

						UIView.Animate(1.0, () => {
							image.Alpha = 1f;
						});
					});
				}
			});
		}
	}
}
