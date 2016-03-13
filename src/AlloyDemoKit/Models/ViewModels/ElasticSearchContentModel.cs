using System.Collections.Generic;
using System.Web;
using AlloyDemoKit.Models.Pages;
using EPiServer;
using EPiServer.Find.Api.Facets;
using System;
using EPi.Cms.Search.Elasticsearch.Indexing;
using Nest;

namespace AlloyDemoKit.Models.ViewModels
{
    public class ElasticSearchContentModel : PageViewModel<ElasticSearchPage>
    {
        public ElasticSearchContentModel(ElasticSearchPage currentPage) : base(currentPage)
        {
            
        }

        public IEnumerable<IHit<IPageDataIndexModel>> Hits { get; set; }

        public string PublicProxyPath { get; set; }

        public string GetSectionGroupUrl(string groupName)
        {
            string url = UriSupport.AddQueryString(HttpContext.Current.Request.RawUrl, "t", HttpContext.Current.Server.UrlEncode(groupName));
            url = UriSupport.AddQueryString(url, "p", "1");
            return url;
        }

        public long NumberOfHits { get; set; }

        public List<DateRange> PublishedDateRange
        {
            get
            {
                var dateRanges = new List<DateRange>()
                {
                    new DateRange { From = DateTime.Now.AddDays(-1), To = DateTime.Now },
                    new DateRange { From = DateTime.Now.AddDays(-7), To = DateTime.Now.AddDays(-1) },
                    new DateRange { From = DateTime.Now.AddDays(-30), To = DateTime.Now.AddDays(-7) },
                    new DateRange { From = DateTime.Now.AddDays(-365), To = DateTime.Now.AddDays(-365) },
                    new DateRange { From = DateTime.Now.AddYears(-2), To = DateTime.Now.AddDays(-365) },
                };
                return dateRanges;
            }
        }

        public string SectionFilter
        {
            get { return HttpContext.Current.Request.QueryString["t"] ?? string.Empty; }
        }

        //Retrieve the paging page from the query string parameter "p".
        //If no such parameter exists the user hasn't requested a specific
        //page so we default to the first (1).
        public int PagingPage
        {
            get
            {
                int pagingPage;
                if (!int.TryParse(HttpContext.Current.Request.QueryString["p"], out pagingPage))
                {
                    pagingPage = 1;
                }

                return pagingPage;
            }
        }

        //Calculate the number of paged result listings based on the
        //total number of hits and the PageSize.
        public long TotalPagingPages
        {
            get
            {
                if (CurrentPage.PageSize > 0)
                {
                    return 1 + (NumberOfHits - 1) / CurrentPage.PageSize;
                }

                return 0;
            }
        }

        //Create URL for a specific paging page.
        public string GetPagingUrl(int pageNumber)
        {
            return UriSupport.AddQueryString(HttpContext.Current.Request.RawUrl, "p", pageNumber.ToString());
        }

        public string Query
        {
            get { return (HttpContext.Current.Request.QueryString["q"] ?? string.Empty).Trim(); }
        }  
    }
}
