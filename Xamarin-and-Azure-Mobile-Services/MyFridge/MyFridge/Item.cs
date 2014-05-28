using System;

namespace MyFridge
{
	public class Item
	{
		public Item ()
		{
		}

		public string Id { get;set; }
		public string Name { get;set; }
		public DateTime Expires { get;set; }
	}
}

