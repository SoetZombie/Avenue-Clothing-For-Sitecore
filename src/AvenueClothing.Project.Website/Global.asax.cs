﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Mvc;
using System.Web.Optimization;
using AvenueClothing.Project.Transaction.Services;
using AvenueClothing.Project.Transaction.Services.Impl;
using AvenueClothing.Project.Website.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UCommerce.Catalog;
using UCommerce.Content;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.GetProduct;
using UCommerce.Runtime;
using UCommerce.Search;
using UCommerce.Transactions;
using ObjectFactory = UCommerce.Infrastructure.ObjectFactory;

namespace AvenueClothing.Project.Website
{
    public class Global : HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureUCommerceServices(services);
            ConfigureControllerServices(services);
			ConfigureAcceleratorServices(services);
            
            var resolver = new ServiceProviderDependencyResolver(services.BuildServiceProvider());
            DependencyResolver.SetResolver(resolver);

            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

	    private void ConfigureAcceleratorServices(ServiceCollection services)
	    {
			services.AddTransient<IMiniBasketService, MiniBasketService>();
	    }

	    public void ConfigureUCommerceServices(IServiceCollection services)
        {
            services.AddTransient(p => ObjectFactory.Instance.Resolve<TransactionLibraryInternal>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<CatalogLibraryInternal>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<SearchLibraryInternal>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<ICatalogContext>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IOrderContext>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IPipeline<IPipelineArgs<GetProductRequest, GetProductResponse>>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IImageService>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<Product>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<Category>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IRepository<ProductReviewStatus>>());
            services.AddTransient(p => ObjectFactory.Instance.Resolve<IPipeline<ProductReview>>());
            services.AddTransient(p => Country.All());
        }

        public void ConfigureControllerServices(IServiceCollection services)
        {
            var controllerTypesToRegister = GetControllerTypesToRegister();
            foreach (var type in controllerTypesToRegister)
            {
                services.AddTransient(type);
            }
        }

        public static Type[] GetControllerTypesToRegister(params Assembly[] assemblies)
        {
            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = BuildManager.GetReferencedAssemblies().OfType<Assembly>().ToArray();
            }

            return (
                from assembly in assemblies
                where !assembly.IsDynamic
                from type in GetExportedTypes(assembly)
                where typeof(IController).IsAssignableFrom(type)
                where !type.IsAbstract
                where !type.IsGenericTypeDefinition
                where type.Name.EndsWith("Controller", StringComparison.Ordinal)
                select type)
                .ToArray();
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetExportedTypes();
            }
            catch (NotSupportedException)
            {
                // A type load exception would typically happen on an Anonymously Hosted DynamicMethods 
                // Assembly and it would be safe to skip this exception.
                return Type.EmptyTypes;
            }
            catch (FileNotFoundException)
            {
                return Type.EmptyTypes;
            }
            catch (FileLoadException)
            {
                return Type.EmptyTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Return the types that could be loaded. Types can contain null values.
                return ex.Types.Where(type => type != null);
            }
            catch (Exception ex)
            {
                // Throw a more descriptive message containing the name of the assembly.
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                    "Unable to load types from assembly {0}. {1}", assembly.FullName, ex.Message), ex);
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}