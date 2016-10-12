﻿using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class VariantPickerViewModel
    {
        public List<Variant> Variants { get; set; }
        public string ProductSku { get; set; }

        public class Variant
        {
            public Variant()
            {
                VaraintItems = new List<VaraintValue>();
            }

            public string Name { get; set; }
            public string DisplayName { get; set; }
            public List<VaraintValue> VaraintItems { get; set; }

            public class VaraintValue
            {
                public string Name { get; set; }
                public string DisplayName { get; set; }
            }
        }
    }
}