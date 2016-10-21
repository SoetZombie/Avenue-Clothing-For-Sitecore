﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
    public class ProductReviewViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
    }
}