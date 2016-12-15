﻿using System.Collections.Generic;
using System.Collections.Specialized;
using AvenueClothing.Installer.Postinstallation.Steps;
using Sitecore.Install.Framework;

namespace AvenueClothing.Installer.Postinstallation
{
    public class PostInstallation : IPostStep
    {
        private readonly IList<IPostStep> _composite;

        public PostInstallation()
        {
			_composite = new List<IPostStep>();

            //_composite.Add(new CreateUCommerceSettings());
            //_composite.Add(new CreateUCommerceCatalog());
            //_composite.Add(new UpdateStandardValuesForDefinitions());
            //_composite.Add(new RunScratchIndexer());
            //_composite.Add(new SynchronizeSitecoreItems());
            //_composite.Add(new ClearSitecoreCache());
            //_composite.Add(new PublishMasterDatabase());
            //_composite.Add(new MoveSitecoreConfigurationFiles());
        }

        public void Run(ITaskOutput output, NameValueCollection metaData)
        {
            foreach (var postStep in _composite)
            {
                postStep.Run(output, metaData);
            }
        }
    }
}
