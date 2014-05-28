
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

namespace MyFridge
{
	[Activity (Label = "ItemActivity")]			
	public class ItemActivity : Activity
	{
		EditText textName;
		DatePicker dateExpiry;
		Button buttonSave;

		string itemId = string.Empty;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Item);

			textName = FindViewById<EditText> (Resource.Id.editTextName);
			dateExpiry = FindViewById<DatePicker> (Resource.Id.datePickerExpiry);
			buttonSave = FindViewById<Button> (Resource.Id.buttonSave);

			dateExpiry.DateTime = DateTime.Now.AddDays (7);

			if (Intent.HasExtra("ITEM_ID"))
				itemId = Intent.GetStringExtra ("ITEM_ID");
			if (Intent.HasExtra ("ITEM_NAME"))
				textName.Text = Intent.GetStringExtra ("ITEM_NAME");
			if (Intent.HasExtra ("ITEM_EXPIRY")) {
				var str = Intent.GetStringExtra ("ITEM_EXPIRY");
				dateExpiry.DateTime = DateTime.Parse (str);
			}

			buttonSave.Click += async delegate {

				var item = new Item {
					Id = itemId,
					Name = textName.Text,
					Expires = dateExpiry.DateTime
				};

				if (string.IsNullOrEmpty(itemId))
					await AzureMobile.MobileService.GetTable<Item>().InsertAsync(item);
				else
					await AzureMobile.MobileService.GetTable<Item>().UpdateAsync(item);

				Finish();
			};
		}
	}
}

