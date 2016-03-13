using AlloyDemoKit.Models.Pages;
using System.Web.Mvc;
using AlloyDemoKit.Models.ViewModels;
using EPi.Cms.Search.Elasticsearch.Indexing;
using EPiServer.Find.Helpers;
using EPiServer.Globalization;
using Nest;
using SiteDefinition = EPiServer.Web.SiteDefinition;

namespace AlloyDemoKit.Controllers
{
    public class ElasticSearchPageController :  PageControllerBase<ElasticSearchPage>
    {
        private const int MaxResults = 1000;
        private readonly IElasticClient _searchClient;

        public ElasticSearchPageController(
            IElasticClient searchClient)
        {
            _searchClient = searchClient;
        }

        [ValidateInput(false)]
        public ViewResult Index(ElasticSearchPage currentPage, string q)
        {
            var model = new ElasticSearchContentModel(currentPage);

            var searchResponse = _searchClient
                .Search<IPageDataIndexModel>(s =>
                    s.Index($"site_{ContentLanguage.PreferredCulture.Name}")
                    .Type(Types.Type(typeof(ArticlePageIndexModel), typeof(NewsPageIndexModel)))
                        .Query(qy =>
                            qy.Bool(b =>
                            {
                                if (!string.IsNullOrWhiteSpace(model.Query))
                                {
                                    b.Should(sh =>
                                        sh.Match(m => m.Field("name")),
                                        sh => sh.Match(m => m.Field("meta_description")),
                                        sh => sh.Match(m => m.Field("main_body")));
                                }

                                b.Filter(f =>
                                    f.Term("site_definition_id", SiteDefinition.Current.Id));
                                return b;
                            }
                                )
                        )
                        .From((model.PagingPage - 1) * model.CurrentPage.PageSize)
                        .Size(model.CurrentPage.PageSize)                    
                );

            model.Hits = searchResponse.Hits;
            model.NumberOfHits = searchResponse.Total;

            /*if (!string.IsNullOrWhiteSpace(model.Query))
            {
                var query =
                    _searchClient.UnifiedSearchFor(model.Query, _searchClient.Settings.Languages.GetSupportedLanguage(ContentLanguage.PreferredCulture) ??
                                                  Language.None)
                                .UsingSynonyms()
                    //Include a facet whose value we can use to show the total number of hits
                    //regardless of section. The filter here is irrelevant but should match *everything*.
                                .TermsFacetFor(x => x.SearchSection)
                                .FilterFacet("AllSections", x => x.SearchSection.Exists())
                    //Fetch the specific paging page.
                                .Skip((model.PagingPage - 1) * model.CurrentPage.PageSize)
                                .Take(model.CurrentPage.PageSize)
                    //Range facet for date
                    //.RangeFacetFor(x => x.SearchUpdateDate, model.PublishedDateRange.ToArray())
                    //Allow editors (from the Find/Optimizations view) to push specific hits to the top 
                    //for certain search phrases.
                                .ApplyBestBets();

                // obey DNT
                var doNotTrackHeader = System.Web.HttpContext.Current.Request.Headers.Get("DNT");
                // Should Not track when value equals 1
                if (doNotTrackHeader == null || doNotTrackHeader.Equals("0"))
                {
                    query = query.Track();
                }

                //If a section filter exists (in the query string) we apply
                //a filter to only show hits from a given section.
                if (!string.IsNullOrWhiteSpace(model.SectionFilter))
                {
                    query = query.FilterHits(x => x.SearchSection.Match(model.SectionFilter));
                }

                //We can (optionally) supply a hit specification as argument to the GetResult
                //method to control what each hit should contain. Here we create a 
                //hit specification based on values entered by an editor on the search page.
                var hitSpec = new HitSpecification
                {
                    HighlightTitle = model.CurrentPage.HighlightTitles,
                    HighlightExcerpt = model.CurrentPage.HighlightExcerpts
                };

                //Execute the query and populate the Result property which the markup (aspx)
                //will render.
                model.Hits = query.GetResult(hitSpec);
            }*/
           
            return View(model);
        }

     

       
    }
}
