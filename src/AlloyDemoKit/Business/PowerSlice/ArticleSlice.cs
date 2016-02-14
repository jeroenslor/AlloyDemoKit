using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Web;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Models.Pages;
using PowerSlice;
using System.Collections.Generic;
using EPi.Cms.SiteSettings;

namespace EPiServer.Templates.Alloy.Business.PowerSlice
{
    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class ArticleSlice : ContentSliceBase<ArticlePage>
    {
        public override string Name
        {
            get { return "Articles"; }
        }
        public override IEnumerable<CreateOption> CreateOptions
        {
            get
            {
                var contentType = ContentTypeRepository.Load(typeof(ArticlePage));
                yield return new CreateOption(contentType.LocalizedName, ServiceLocator.Current.GetInstance<ISiteSettingsRepository>().Get().NewsPageLink, contentType.ID);
            }
        }
    }
}