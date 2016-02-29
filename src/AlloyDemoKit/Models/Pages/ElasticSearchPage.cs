using System.ComponentModel.DataAnnotations;
using AlloyDemoKit.Models.Blocks;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace AlloyDemoKit.Models.Pages
{
    /// <summary>
    /// Used the elasticsearch kickstart implementation
    /// </summary>
    [SiteContentType(
        GUID = "4AA68C3F-C6C8-4B32-A4B7-E5C84DB4A758",
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl]
    public class ElasticSearchPage : SitePageData, IHasRelatedContent
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 310)]
        [CultureSpecific]
        [AllowedTypes(new[] { typeof(IContentData) }, new[] { typeof(JumbotronBlock) })]
        public virtual ContentArea RelatedContentArea { get; set; }
    }
}
