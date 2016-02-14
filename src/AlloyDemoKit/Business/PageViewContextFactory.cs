using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Models.ViewModels;
using EPiServer;
using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPi.Cms.SiteSettings;

namespace AlloyDemoKit.Business
{
    public class PageViewContextFactory
    {
        private readonly IContentLoader _contentLoader;
        private readonly UrlResolver _urlResolver;
        private readonly ISiteSettingsRepository _siteSettingsRepository;

        public PageViewContextFactory(IContentLoader contentLoader, UrlResolver urlResolver, ISiteSettingsRepository siteSettingsRepository)
        {
            _contentLoader = contentLoader;
            _urlResolver = urlResolver;
            _siteSettingsRepository = siteSettingsRepository;
        }

        public virtual LayoutModel CreateLayoutModel(ContentReference currentContentLink, RequestContext requestContext)
        {
            var settings = _siteSettingsRepository.Get();

            return new LayoutModel
                {
                    Logotype = settings.SiteLogotype,
                    LogotypeLinkUrl = new MvcHtmlString(_urlResolver.GetUrl(SiteDefinition.Current.StartPage)),
                    ProductPages = settings.ProductPageLinks,
                    CompanyInformationPages = settings.CompanyInformationPageLinks,
                    NewsPages = settings.NewsPageLinks,
                    CustomerZonePages = settings.CustomerZonePageLinks,
                    LoggedIn = requestContext.HttpContext.User.Identity.IsAuthenticated,
                    LoginUrl = new MvcHtmlString(GetLoginUrl(currentContentLink)),
                    SearchPageRouteValues = requestContext.GetPageRoute(settings.SearchPageLink)
                };
        }

        private string GetLoginUrl(ContentReference returnToContentLink)
        {
            return string.Format(
                "{0}?ReturnUrl={1}", 
                FormsAuthentication.LoginUrl,
                _urlResolver.GetUrl(returnToContentLink));
        }

        public virtual IContent GetSection(ContentReference contentLink)
        {
            var currentContent = _contentLoader.Get<IContent>(contentLink);
            if (currentContent.ParentLink != null && currentContent.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
            {
                return currentContent;
            }

            return _contentLoader.GetAncestors(contentLink)
                .OfType<PageData>()
                .SkipWhile(x => x.ParentLink == null || !x.ParentLink.CompareToIgnoreWorkID(SiteDefinition.Current.StartPage))
                .FirstOrDefault();
        }
    }
}
