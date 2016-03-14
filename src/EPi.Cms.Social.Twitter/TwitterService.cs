using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EPiServer;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;

namespace EPi.Cms.Social.Twitter
{
    [ServiceConfiguration(typeof(ITwitterService))]
    public class TwitterService : ITwitterService
    {
        private readonly ISynchronizedObjectInstanceCache _cacheManager;
        private const string OAuthUrl = "https://api.twitter.com/oauth2/token";
        private const string TimeLineUrl = "https://api.twitter.com/1.1/statuses/user_timeline.json";

        public TwitterService(ISynchronizedObjectInstanceCache cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public async Task<TwitterStatus[]> GetUserTimeLineStatusesAsync(string consumerKey, string consumerSecret, string screenName, int count, bool excludeReplies = true, bool includeRetweets = false, bool cache = false, TimeSpan? cacheExpiration = null)
        {
            if (cache)
            {
                var cachedTimeLine = _cacheManager.Get("TwitterService_GetUserTimeLineStatuses") as TwitterStatus[];
                if (cachedTimeLine != null)
                    return cachedTimeLine;
            }

            // TODO store OAuthToken to settings or cache
            var oAuthTokenResponse = await GetOAuthToken(consumerKey, consumerSecret);

            var userTimeLine = await GetUserTimeLine(screenName, count, excludeReplies, includeRetweets, oAuthTokenResponse);

            if (cache)
                _cacheManager.Insert("TwitterService_GetUserTimeLineStatuses", userTimeLine,
                    new CacheEvictionPolicy(null, cacheExpiration ?? TimeSpan.FromHours(1), CacheTimeoutType.Absolute));

            return userTimeLine;
        }

        protected async Task<TwitterStatus[]> GetUserTimeLine(string screenName, int count, bool excludeReplies, bool includeRetweets, string oAuthTokenResponse)
        {
            var requestUrl = TimeLineUrl;

            if (screenName != null)
                requestUrl = UriSupport.AddQueryString(requestUrl, "screen_name", screenName);
            if (excludeReplies)
                requestUrl = UriSupport.AddQueryString(requestUrl, "exclude_replies", "1");
            if (includeRetweets)
                requestUrl = UriSupport.AddQueryString(requestUrl, "include_rts", "1");

            requestUrl = UriSupport.AddQueryString(requestUrl, "count", count.ToString());

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthTokenResponse);
                var response = await httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TwitterStatus[]>(jsonString);
            }
        }

        protected async Task<string> GetOAuthToken(string consumerKey, string consumerSecret)
        {
            TwitterOAuthTokenResponse twitterOAuthTokenResponse;
            using (
                var httpClient =
                    new HttpClient(new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    }))
            {
                var basicAuthHeaderValue =
                    Convert.ToBase64String(
                        Encoding.UTF8.GetBytes(
                            $"{Uri.EscapeDataString(consumerKey)}:{Uri.EscapeDataString(consumerSecret)}"));

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    basicAuthHeaderValue);
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                HttpContent postBody = new StringContent("grant_type=client_credentials");
                postBody.Headers.ContentType =
                    new MediaTypeHeaderValue("application/x-www-form-urlencoded") {CharSet = Encoding.UTF8.WebName};

                var response = await httpClient.PostAsync(OAuthUrl, postBody);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                twitterOAuthTokenResponse = JsonConvert.DeserializeObject<TwitterOAuthTokenResponse>(jsonString);
            }

            return twitterOAuthTokenResponse.AccessToken;
        }                

        public class TwitterOAuthTokenResponse
        {
            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }
    }

    public class TwitterStatus
    {
        public long Id { get; set; }
        public TwitterUser User { get; set; }
        public string Text { get; set; }
        public TwitterStatusEntity Entities { get; set; }
        [JsonProperty("retweeted_status")]
        public TwitterStatus RetweetedStatus { get; set; }        
    }

    public class TwitterUser
    {
        [JsonProperty("screen_name")]
        public string ScreenName { get; set; }
        public string Url { get; set; }
        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
    }

    public class TwitterStatusEntity
    {
        public TwitterStatusEntity()
        {
            Media = new TwitterStatusMedia[0];
        }

        public TwitterStatusMedia[] Media { get; set; }
    }

    public class TwitterStatusMedia
    {
        [JsonProperty("media_url")]
        public string MediaUrl { get; set; } // use this for the actual source

        public string Url { get; set; } // use this for matching in the status text and replacing
        public string Type { get; set; } // photo, video
    }

    public static class TwitterStatusExtensions
    {
        public static bool IsRetweet(this TwitterStatus twitterStatus)
        {
            return twitterStatus.RetweetedStatus != null;
        }

        public static string Text(this TwitterStatus twitterStatus, bool fallbackToRetweetStatus = true, bool removeEntities = false)
        {
            var text = twitterStatus.IsRetweet() && fallbackToRetweetStatus ? twitterStatus.RetweetedStatus.Text : twitterStatus.Text;

            if (!removeEntities)
                return text;

            foreach (var twitterStatusTextEntityMatch in twitterStatus.GetTextEntityMatches(fallbackToRetweetStatus))
            {
                text = text.Remove(twitterStatusTextEntityMatch.StatusTextMatch.Index,
                    twitterStatusTextEntityMatch.StatusTextMatch.Length);
            }

            return text;
        }

        public static IEnumerable<TwitterStatusTextEntityMatch> GetTextEntityMatches(this TwitterStatus twitterStatus, bool fallbackToRetweetStatus = true)
        {
            var text = twitterStatus.Text(fallbackToRetweetStatus);

            foreach (var twitterStatusMedia in twitterStatus.Entities.Media)
            {
                var match = Regex.Match(text, twitterStatusMedia.Url);
                if (match.Success)
                    yield return new TwitterStatusTextMediaEntityMatch(match, twitterStatusMedia);
            }

            //TODO map url's
        }
    }

    public abstract class TwitterStatusTextEntityMatch
    {
        protected TwitterStatusTextEntityMatch(Match statusTextMatch)
        {
            StatusTextMatch = statusTextMatch;
        }

        public Match StatusTextMatch { get; set; }
    }

    public class TwitterStatusTextMediaEntityMatch : TwitterStatusTextEntityMatch
    {
        public TwitterStatusTextMediaEntityMatch(Match statusTextMatch, TwitterStatusMedia media) : base(statusTextMatch)
        {
            Media = media;
        }

        public TwitterStatusMedia Media { get; set; }
    }

}
