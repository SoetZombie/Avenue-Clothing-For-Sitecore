﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;
using AvenueClothing.Project.Catalog.ViewModels;
using Sitecore.Mvc.Presentation;
using Sitecore.Web.UI.WebControls;

namespace AvenueClothing.Project.Catalog.Controllers
{
    public class ItemAddedToBasketController : BaseController
    {
        public ActionResult Rendering()
        {
            var item = new ItemAddedToBasketViewModel();
          
            var dataSourceId = RenderingContext.CurrentOrNull.Rendering.DataSource;
            var dataSource = Sitecore.Context.Database.GetItem(dataSourceId);
            item.Message = new HtmlString(FieldRenderer.Render(dataSource, "Message For Banner"));
            return View(item);
        }
    }
}