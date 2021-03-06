﻿using System;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using UCommerce;
using UCommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class PaymentPickerController : BaseController
    {
	    private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public PaymentPickerController(TransactionLibraryInternal transactionLibraryInternal)
	    {
	        _transactionLibraryInternal = transactionLibraryInternal;
	    }

		public ActionResult Rendering()
		{
			var paymentPickerViewModel = new PaymentPickerViewModel();

			var basket = _transactionLibraryInternal.GetBasket(false).PurchaseOrder;
			var shippingCountry = basket.GetShippingAddress(Constants.DefaultShipmentAddressName).Country;

			paymentPickerViewModel.ShippingCountry = shippingCountry.Name;
			
			var availablePaymentMethods = _transactionLibraryInternal.GetPaymentMethods(shippingCountry);

			var existingPayment = basket.Payments.FirstOrDefault();
			paymentPickerViewModel.SelectedPaymentMethodId = existingPayment != null
				? existingPayment.PaymentMethod.PaymentMethodId
				: -1;

			foreach (var availablePaymentMethod in availablePaymentMethods)
			{
				var option = new SelectListItem();
				decimal feePercent = availablePaymentMethod.FeePercent;
				var fee = availablePaymentMethod.GetFeeForCurrency(basket.BillingCurrency);
				var formattedFee = new Money((fee == null ? 0 : fee.Fee), basket.BillingCurrency);

				option.Text = String.Format(" {0} ({1} + {2}%)", availablePaymentMethod.Name, formattedFee,
					feePercent.ToString("0.00"));
				option.Value = availablePaymentMethod.PaymentMethodId.ToString();
				option.Selected = availablePaymentMethod.PaymentMethodId == paymentPickerViewModel.SelectedPaymentMethodId;

				paymentPickerViewModel.AvailablePaymentMethods.Add(option);
			}

			return View(paymentPickerViewModel);
		}

		[HttpPost]
		public ActionResult CreatePayment(PaymentPickerViewModel createPaymentViewModel)
		{
			_transactionLibraryInternal.CreatePayment(createPaymentViewModel.SelectedPaymentMethodId, -1m, false, true);
			_transactionLibraryInternal.ExecuteBasketPipeline();

			return Redirect("/preview");
		}
	}
}