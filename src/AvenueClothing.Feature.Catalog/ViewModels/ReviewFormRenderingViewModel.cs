﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AvenueClothing.Feature.Catalog.ViewModels
{
    public class ReviewFormRenderingViewModel
    {
        public Guid ProductGuid { get; set; }
        public Guid CategoryGuid { get; set; }
    }
}