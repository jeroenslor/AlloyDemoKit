using System;
using System.Configuration;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using AlloyDemoKit.Business.Rendering;
using AlloyDemoKit.Helpers;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using StructureMap;
using AlloyDemoKit.Business.Data;
using Elasticsearch.Net;
using EPi.Cms.Search.Elasticsearch.Indexing;
using Nest;

namespace AlloyDemoKit.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(ConfigureContainer);

            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            //Swap out the default ContentRenderer for our custom
            //container.For<IContentRenderer>().Use<ErrorHandlingContentRenderer>();
            container.For<ContentAreaRenderer>().Use<AlloyContentAreaRenderer>();

            //Implementations for custom interfaces can be registered here.
            container.For<IFileDataImporter>().Use<FileDataImporter>();

            //Elastic search client
            var node = new Uri(ConfigurationManager.ConnectionStrings["ElasticSearch"].ConnectionString);
            var connectionPool = new SniffingConnectionPool(new[] { node });
            var connectionSettings = new ConnectionSettings(connectionPool);
            connectionSettings.DisableDirectStreaming();

            container.For<IElasticClient>().Use(new ElasticClient(connectionSettings));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
