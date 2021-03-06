﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using AvenueClothing.Project.Catalog.ViewModels;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.Services;
using UCommerce.Api;
using UCommerce.Runtime;
using UCommerce.Search;
using UCommerce.Search.Facets;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class FacetsController : BaseController
    {
        private readonly ICatalogContext _catalogContext;
        private readonly SearchLibraryInternal _searchLibraryInternal;

        public FacetsController(ICatalogContext catalogContext, SearchLibraryInternal searchLibraryInternal)
        {
            _catalogContext = catalogContext;
            _searchLibraryInternal = searchLibraryInternal;
        }

        public ActionResult Rendering([ModelBinder(typeof(FacetModelBinder))]IList<Facet> facetsForQuerying)
        {

            var category = _catalogContext.CurrentCategory;
            var viewModel = new FacetsRenderingViewModel();

            IList<Facet> facets = _searchLibraryInternal.GetFacetsFor(category, facetsForQuerying);
            if (facets.Any(x => x.FacetValues.Any(y => y.Hits > 0)))
            {
                viewModel.Facets = MapFacets(facets);
            }

            return View(viewModel);
        }

        private List<FacetsRenderingViewModel.Facet> MapFacets(IList<Facet> facetsInCategory)
        {
            var facets = new List<FacetsRenderingViewModel.Facet>();

            foreach (var facet in facetsInCategory)
            {
                var facetViewModel = new FacetsRenderingViewModel.Facet
                {
                    Name = facet.Name,
                    DisplayName = facet.DisplayName
                };

                if (!facet.FacetValues.Any())
                {
                    continue;
                }

                foreach (var value in facet.FacetValues)
                {
                    if (value.Hits > 0)
                    {
                        var facetVal = new FacetsRenderingViewModel.FacetValue(value.Value, value.Hits);
                        facetViewModel.FacetValues.Add(facetVal);
                    }
                }

                facets.Add(facetViewModel);
            }

            return facets;
        }
    }
}
