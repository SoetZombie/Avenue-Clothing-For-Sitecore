﻿using System.Collections.Specialized;
using AvenueClothing.Installer.Helpers;
using Sitecore.Install.Framework;
using UCommerce.Pipelines;

namespace AvenueClothing.Installer.Pipelines.Installation.Tasks
{
    public class CreateUCommerceCatalogTask : IPipelineTask<InstallationPipelineArgs>
    {
        public PipelineExecutionResult Execute(InstallationPipelineArgs subject)
        {
            var catalog = new Catalog("Avenue-Clothing.com", "Demo Catalog");
            catalog.Configure();

            return PipelineExecutionResult.Success;
        }
    }
}