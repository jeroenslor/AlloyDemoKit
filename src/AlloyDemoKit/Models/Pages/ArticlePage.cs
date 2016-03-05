using System;
using System.Globalization;
using EPi.Cms.Search.Elasticsearch.Indexing;
using EPi.Cms.Search.Elasticsearch.Indexing.TypeMap;
using Nest;

namespace AlloyDemoKit.Models.Pages
{
    /// <summary>
    /// Used primarily for publishing news articles on the website
    /// </summary>
    [SiteContentType(
        GroupName = Global.GroupNames.News,
        GUID = "AEECADF2-3E89-4117-ADEB-F8D43565D2F4")]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-article.png")]
    public class ArticlePage : StandardPage, IIndexablePageData, IIndexableTypeMapper
    {
        public IPageDataIndexModel CreateIndexModel()
        {
            var indexModel = new ArticlePageIndexModel();
            this.SetIndexablePageDataProperties(indexModel);

            indexModel.Name = Name;
            indexModel.MetaDescription = MetaDescription;
            indexModel.MainBody = MainBody?.StripHtml();

            return indexModel;
        }

        public bool ShouldIndex() => true;

        public TypeName TypeName => TypeName.From<ArticlePageIndexModel>();

        public ITypeMapping CreateTypeMapping(CultureInfo cultureInfo)
        {
            return new TypeMappingDescriptor<ArticlePageIndexModel>().AutoMap();
        }
    }

    [ElasticsearchType(Name = "articles")]
    public class ArticlePageIndexModel : IPageDataIndexModel
    {
        public Guid Id { get; set; }
        public string ContentReference { get; set; }
        public Guid? SiteDefinitionId { get; set; }
        public string Name { get; set; }
        public string MetaDescription { get; set; }
        public string MainBody { get; set; }
    }
}
