﻿namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
	public class OrderLinePreviewViewModel
	{
		public string Total { get; set; }

		public int Quantity { get; set; }

		public int OrderLineId { get; set; }

		public string Sku { get; set; }

		public string VariantSku { get; set; }

		public string ProductName { get; set; }

		public string Tax { get; set; }

		public decimal? Discount { get; set; }

		public string ProductUrl { get; set; }

		public string Price { get; set; }

		public string PriceWithDiscount { get; set; }
	}
}