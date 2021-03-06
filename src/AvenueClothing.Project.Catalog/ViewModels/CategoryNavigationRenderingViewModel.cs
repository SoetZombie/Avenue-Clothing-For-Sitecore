﻿using System;
using System.Collections.Generic;

namespace AvenueClothing.Project.Catalog.ViewModels
{
	public class CategoryNavigationRenderingViewModel
	{
		public CategoryNavigationRenderingViewModel()
		{
			Categories = new List<Category>();
		}
		public IList<Category> Categories { get; set; }

        public class Category
        {
            public Category()
            {
                ProductItemGuids = new List<Guid>();
                Categories = new List<Category>();
            }

            public IList<Guid> ProductItemGuids { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public IList<Category> Categories { get; set; }
        }
    }
}