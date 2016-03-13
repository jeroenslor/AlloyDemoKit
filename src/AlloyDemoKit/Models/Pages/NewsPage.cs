using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using AlloyDemoKit.Business;
using AlloyDemoKit.Models.Blocks;
using EPiServer.DataAnnotations;
using EPiServer.Core;
using AlloyDemoKit.Models.Pages.Models.Pages;
using EPi.Cms.Search.Elasticsearch.Indexing;
using EPi.Cms.Search.Elasticsearch.Indexing.TypeMap;
using Nest;

namespace AlloyDemoKit.Models.Pages
{
    /// <summary>
    /// Presents a news section including a list of the most recent articles on the site
    /// </summary>
    [SiteContentType(GUID = "638D8271-5CA3-4C72-BABC-3E8779233263")]
    [SiteImageUrl]
    [AvailableContentTypes(
        Availability.Specific,
        Exclude = new[] { typeof(ContainerPage), typeof(ProductPage), typeof(FindPage), typeof(SearchPage), typeof(LandingPage), typeof(ContentFolder), typeof(ContactPage), typeof(BlogItemPage), typeof(BlogStartPage), typeof(BlogListPage) },
        Include = new[] { typeof(ArticlePage), typeof(StandardPage), typeof(NewsPage) })] // Pages we can create under the news page...
    public class NewsPage : StandardPage, IIndexablePageData, IIndexableTypeMapper
    {
        [Display(
            GroupName = SystemTabNames.Content,
            Order = 305)]
        public virtual PageListBlock NewsList { get; set; }

        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            NewsList.Count = 20;
            NewsList.Heading = ServiceLocator.Current.GetInstance<LocalizationService>().GetString("/newspagetemplate/latestnews");
            NewsList.IncludeIntroduction = true;
            NewsList.IncludePublishDate = true;
            NewsList.Recursive = true;
            NewsList.PageTypeFilter = typeof(ArticlePage).GetPageType();
            NewsList.SortOrder = FilterSortOrder.PublishedDescending;
        }

        public IPageDataIndexModel CreateIndexModel()
        {
            var indexModel = new NewsPageIndexModel();
            this.SetIndexablePageDataProperties(indexModel);

            indexModel.Name = Name;
            indexModel.MetaDescription = MetaDescription;
            indexModel.MainBody = MainBody?.StripHtml();

            return indexModel;
        }

        public bool ShouldIndex() => true;

        public TypeName TypeName => TypeName.From<NewsPageIndexModel>();

        public ITypeMapping CreateTypeMapping(CultureInfo cultureInfo)
        {
            return new TypeMappingDescriptor<NewsPageIndexModel>().AutoMap();
        }
    }

    [ElasticsearchType(Name = "news")]
    public class NewsPageIndexModel : IPageDataIndexModel
    {
        public Guid Id { get; set; }
        [String(Name = "content_reference", Index = FieldIndexOption.NotAnalyzed)]
        public string ContentReference { get; set; }
        [String(Name = "site_definition_id", Index = FieldIndexOption.NotAnalyzed)]
        public Guid? SiteDefinitionId { get; set; }
        public string Name { get; set; }
        [String(Name = "meta_description")]
        public string MetaDescription { get; set; }
        [String(Name = "main_body")]
        public string MainBody { get; set; }
    }
}
