﻿using System.Collections.Generic;

namespace AvenueClothing.Feature.Transaction.Module.ViewModels
{
    public class AddToBasketViewModel
    {
        public int Quantity { get; set; }

        public string ProductSku { get; set; }

        public string VariantSku { get; set; }
    }
}