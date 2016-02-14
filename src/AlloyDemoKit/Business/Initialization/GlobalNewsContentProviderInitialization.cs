using System;
using System.Configuration;
using System.Linq;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Business.ContentProviders;
using AlloyDemoKit.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPi.Cms.SiteSettings;

namespace AlloyDemoKit.Business.Initialization
{
    /// <summary>
    /// Registers a content provider used to clone global news from a global container to site(s) which specify a local page where global news should be accessible
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class GlobalNewsContentProviderInitialization : IInitializableModule
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        private bool _initialized;

        public void Initialize(InitializationEngine context)
        {
            if (_initialized || ContentReference.IsNullOrEmpty(GlobalNewsContainer) || context.HostType != HostType.WebApplication)
            {
                return;
            }

            var providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var siteDefinitionRepository = ServiceLocator.Current.GetInstance<SiteDefinitionRepository>();
            var siteSettingsRepository = ServiceLocator.Current.GetInstance<ISiteSettingsRepository>();

            // Attach content provider to each site's global news container
            foreach (var site in siteDefinitionRepository.List())
            {
                var siteSettings = siteSettingsRepository.Get(site);
                if (ContentReference.IsNullOrEmpty(siteSettings.GlobalNewsPageLink))
                    continue;

                try
                {
                    Logger.Debug("Attaching global news content provider to page {0} [{1}], global news will be retrieved from page {2} [{3}]",
                        contentLoader.Get<PageData>(siteSettings.GlobalNewsPageLink).Name,
                        siteSettings.GlobalNewsPageLink.ID, 
                        contentLoader.Get<PageData>(GlobalNewsContainer).PageName, 
                        GlobalNewsContainer.ID);

                    var provider = new ClonedContentProvider(GlobalNewsContainer.ToPageReference(), siteSettings.GlobalNewsPageLink, siteSettings.Category);

                    providerManager.ProviderMap.AddProvider(provider);
                }
                catch (Exception ex)
                {
                    Logger.Error("Unable to create global news content provider for site with name {0}: {1}", site.Name, ex.Message);
                }
            }

            _initialized = true;
        }

        /// <summary>
        /// The global news container, ie the content we want to clone
        /// </summary>
        /// <remarks>Returns ContentReference.Empty if no 'GlobalNewsContainerID' app settings exist</remarks>
        private ContentReference GlobalNewsContainer
        {
            get
            {
                const string appSettingName = "GlobalNewsContainerID";

                var pageLinkIdString = ConfigurationManager.AppSettings[appSettingName];

                if (string.IsNullOrWhiteSpace(pageLinkIdString))
                {
                    return ContentReference.EmptyReference;
                }

                int pageLinkId;

                if (!int.TryParse(pageLinkIdString, out pageLinkId) || pageLinkId == 0)
                {
                    Logger.Error("The '{0}' app setting was not set to a valid page ID, expected a positive integer", appSettingName);

                    return ContentReference.EmptyReference;
                }

                return new ContentReference(pageLinkId);
            }
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}
