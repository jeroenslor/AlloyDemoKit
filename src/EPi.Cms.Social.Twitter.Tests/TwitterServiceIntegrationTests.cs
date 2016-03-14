using System.Linq;
using Xunit;

namespace EPi.Cms.Social.Twitter.Tests
{
    public class TwitterServiceIntegrationTests
    {
        [Fact]
        public async void GetUserTimeLineStatuses_From_JeroenSlor_Should_Not_Throw_Exception()
        {
            var twitterService = new TwitterService(null);
            var statuses =
                await
                    twitterService.GetUserTimeLineStatusesAsync("a6tKq5Fj6dCLcn5kfOaYq8p8r",
                        "SWSrMphHurGtY2hFraL1jc38949eBIkaGjTocpg4oFcKcM65O6", "jeroenslor",100);

            Assert.NotEqual(0, statuses.Length);

            var statusWithMedia = statuses.Single(x=>x.Id == 667765221918949376);
            var text = statusWithMedia.Text(removeEntities: true);
        }
    }
}
