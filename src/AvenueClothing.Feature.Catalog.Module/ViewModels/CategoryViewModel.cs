﻿using System.Collections.Generic;

namespace AvenueClothing.Feature.Catalog.Module.ViewModels
{
	public class CategoryViewModel
	{
		public CategoryViewModel()
		{
			Categories = new List<CategoryViewModel>();
			Products = new List<ProductViewModel>();
		}
		public string Url { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IList<CategoryViewModel> Categories { get; set; }

		public IList<ProductViewModel> Products { get; set; } 
	}
}