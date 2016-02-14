using EPiServer.ServiceLocation;
using EPiServer.Shell.ContentQuery;
using EPiServer.Web;
using AlloyDemoKit.Helpers;
using AlloyDemoKit.Models.Pages.Models.Pages;
using PowerSlice;
using System.Collections.Generic;
using AlloyDemoKit.Models.Pages;
using EPi.Cms.SiteSettings;

namespace EPiServer.Templates.Alloy.Business.PowerSlice
{
    [ServiceConfiguration(typeof(IContentQuery)), ServiceConfiguration(typeof(IContentSlice))]
    public class BlogSlice : ContentSliceBase<BlogItemPage>
    {
        public override string Name
        {
            get { return "Blogs"; }
        }
        public override IEnumerable<CreateOption> CreateOptions
        {
            get
            {
               
                var contentType = ContentTypeRepository.Load(typeof(BlogItemPage));
                yield return new CreateOption(contentType.LocalizedName, ServiceLocator.Current.GetInstance<ISiteSettingsRepository>().Get().BlogPageLink, contentType.ID);
            }
        }
    }
}