using System;
using System.Threading.Tasks;

namespace EPi.Cms.Social.Twitter
{
    public interface ITwitterService
    {
        Task<TwitterStatus[]> GetUserTimeLineStatusesAsync(string consumerKey, string consumerSecret, string screenName, int count, bool excludeReplies = true, bool includeRetweets = false, bool cache = false, TimeSpan? cacheExpiration = null);
    }
}