﻿using System.Web.Mvc;
using AvenueClothing.Foundation.MvcExtensions;

namespace AvenueClothing.Project.Transaction.Controllers
{
	public class ConfirmationController : BaseController
	{
		public ActionResult Rendering()
		{
			return View();
		}
	}
}