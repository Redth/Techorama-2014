using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigTed;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TekConf.Client;

namespace TekConfApp
{
	partial class ConferenceListViewController : UITableViewController
	{
		public ConferenceListViewController (IntPtr handle) : base (handle)
		{
		}

		ConferencesSource source;
		Conference selectedConference;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			source = new ConferencesSource {
				ConferenceSelected = c => {
					selectedConference = c;
					PerformSegue("SelectConference", this);
				}
			};

			TableView.Source = source;

			RefreshControl = new UIRefreshControl ();
			RefreshControl.ValueChanged += async delegate {
				await Reload();

				RefreshControl.EndRefreshing();
			};
		}

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			((ConferenceTabBarController)segue.DestinationViewController).Conference = selectedConference;
		}

		public override async void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			await Reload ();
		}

		async Task Reload()
		{
			BTProgressHUD.Show("Loading Conferences"); 

			await source.Reload ();

			TableView.ReloadData ();

			BTProgressHUD.Dismiss ();
		}
	}

	class ConferencesSource : UITableViewSource
	{
		List<Conference> conferences = new List<Conference> ();

		public Action<Conference> ConferenceSelected { get; set; }

		public async Task Reload ()
		{
			var client = new TekConfClient (); //"http://localhost:8000/");

			var results = await client.GetConferencesAsync ();

			conferences.Clear ();
			conferences.AddRange (results);
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var c = conferences [indexPath.Row];

			var cell = tableView.DequeueReusableCell ("CONFERENCE")
			           ?? new UITableViewCell (UITableViewCellStyle.Subtitle, "CONFERENCE");

			cell.TextLabel.Text = c.Name;
			cell.DetailTextLabel.Text = c.Start.ToString ("ddd, MMM d");

			return cell;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return conferences.Count;
		}
			
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var c = conferences [indexPath.Row];

			tableView.DeselectRow (indexPath, true);

			ConferenceSelected (c);
		}
	}
}
