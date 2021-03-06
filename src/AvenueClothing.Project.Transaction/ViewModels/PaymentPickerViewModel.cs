﻿using System.Collections.Generic;
using System.Web.Mvc;

namespace AvenueClothing.Project.Transaction.ViewModels
{
	public class PaymentPickerViewModel
	{
		public PaymentPickerViewModel()
		{
			AvailablePaymentMethods = new List<SelectListItem>();
		}

		public IList<SelectListItem> AvailablePaymentMethods { get; set; }
		public int SelectedPaymentMethodId { get; set; }
		public string ShippingCountry { get; set; }
	}
}