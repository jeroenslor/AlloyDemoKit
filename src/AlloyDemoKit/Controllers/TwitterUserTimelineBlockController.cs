using System.Threading.Tasks;
using System.Web.Mvc;
using EPi.Cms.Social.Twitter;
using EPiServer.Web.Mvc;

namespace AlloyDemoKit.Controllers
{
    public class TwitterUserTimelineBlockController : BlockController<TwitterUserTimelineBlock>
    {
        private readonly ITwitterService _twitterService;

        public TwitterUserTimelineBlockController(ITwitterService twitterService)
        {
            _twitterService = twitterService;
        }

        public new async Task<ActionResult> Index(TwitterUserTimelineBlock currentBlock)
        {
            var statuses =
                await
                    _twitterService.GetUserTimeLineStatusesAsync(currentBlock.ConsumerKey, currentBlock.ConsumerSecret,
                        currentBlock.ScreenName, currentBlock.NumberOfTweets);

            return
                PartialView(new TwitterUserTimeLineViewModel()
                {
                    Heading = currentBlock.Heading,
                    TwitterStatuses = statuses
                });
        }
    }

    public class TwitterUserTimeLineViewModel
    {
        public string Heading { get; set; }
        public TwitterStatus[] TwitterStatuses { get; set; }
    }
}
