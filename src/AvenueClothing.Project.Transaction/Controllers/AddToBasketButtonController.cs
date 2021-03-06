﻿using System;
using System.Web.Mvc;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Transaction.ViewModels;
using UCommerce.Runtime;
using UCommerce.Transactions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class AddToBasketButtonController : BaseController
    {
        private readonly TransactionLibraryInternal _transactionLibraryInternal;
        private readonly ICatalogContext _catalogContext;
	    private readonly IMiniBasketService _miniBasketService;

	    public AddToBasketButtonController(TransactionLibraryInternal transactionLibraryInternal, ICatalogContext catalogContext, IMiniBasketService miniBasketService)
        {
            _transactionLibraryInternal = transactionLibraryInternal;
            _catalogContext = catalogContext;
		    _miniBasketService = miniBasketService;
        }

        [HttpGet]
        public ActionResult Rendering()
        {
            var product = _catalogContext.CurrentProduct;

            var viewModel = new AddToBasketButtonRenderingViewModel
            {
                AddToBasketUrl = Url.Action("AddToBasket"),
                BasketUrl = "/cart",
                ConfirmationMessageTimeoutInMillisecs = (int)TimeSpan.FromSeconds(5).TotalMilliseconds,
                ConfirmationMessageClientId = "js-add-to-basket-button-confirmation-message-" + Guid.NewGuid(),
                ProductSku = product.Sku,
                IsProductFamily = product.ProductDefinition.IsProductFamily()
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AddToBasket(AddToBasketButtonAddToBasketViewModel viewModel)
        {
            _transactionLibraryInternal.AddToBasket(viewModel.Quantity, viewModel.ProductSku, viewModel.VariantSku);

	        return Json(_miniBasketService.Refresh(), JsonRequestBehavior.AllowGet);
        }
    }
}