using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace MyFridge
{
	[Activity (Label = "MyFridge", MainLauncher = true)]
	public class MainActivity : Activity
	{
		ItemAdapter adapter;

		protected override async void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			adapter = new ItemAdapter (this);

			var listView = FindViewById<ListView> (Resource.Id.listView);
			listView.Adapter = adapter;

			listView.ItemClick += (sender, e) => {
				var item = adapter[e.Position];
				var intent = new Intent(this, typeof(ItemActivity));
				intent.PutExtra("ITEM_ID", item.Id);
				intent.PutExtra("ITEM_NAME", item.Name);
				intent.PutExtra("ITEM_EXPIRY", item.Expires.ToString());

				StartActivity(intent);
			};

			listView.ItemLongClick += async (sender, e) => {
				var item = adapter[e.Position];

				await AzureMobile.MobileService.GetTable<Item>().DeleteAsync(item);

				await adapter.Reload();
			};

			FindViewById<Button> (Resource.Id.buttonAdd).Click += delegate {
				StartActivity(typeof(ItemActivity));
			};

			if (!AzureMobile.LoadLogin ()) {
				await AzureMobile.MobileService.LoginAsync (this, MobileServiceAuthenticationProvider.Twitter);

				AzureMobile.SaveLogin ();

				await adapter.Reload ();
			}

			SampleGcmService.Initialize (this);
			SampleGcmService.Register (this);
		}

		protected override async void OnResume ()
		{
			base.OnResume ();

			if (AzureMobile.MobileService.CurrentUser != null)
				await adapter.Reload();
		}
	}

	public class ItemAdapter : BaseAdapter<Item>
	{
		public ItemAdapter (Context context) : base()
		{
			Context = context;
			Items = new List<Item> ();
		}

		public Context Context { get; private set; }
		public List<Item> Items { get; set; }

		// Overrides
		public override int Count { get { return Items.Count; } }
		public override Item this [int index] { get { return Items [index]; } }
		public override long GetItemId (int position) { return position; }

		public async Task Reload() 
		{
			var results = await AzureMobile.MobileService.GetTable<Item> ().ToListAsync ();

			Items.Clear ();
			Items.AddRange (results);

			// Tell UI our data has changed
			NotifyDataSetChanged ();
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = Items [position];

			var view = convertView ?? View.Inflate (Context, Android.Resource.Layout.SimpleListItem2, null);

			view.FindViewById<TextView> (Android.Resource.Id.Text1).Text = item.Name;
			view.FindViewById<TextView> (Android.Resource.Id.Text2).Text = item.Expires.ToString ();

			return view;
		}
	}
}


