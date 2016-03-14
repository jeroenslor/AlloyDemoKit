using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace EPi.Cms.Social.Twitter
{
    [ContentType(DisplayName = "TwitterUserTimelineBlock", GUID = "a2dec251-9275-4b41-b7db-55ff321fd50e", Description = "")]
    public class TwitterUserTimelineBlock : BlockData
    {
        [Required]
        [Display(
            Name = "Heading",
            Description = "The heading of the block",
            GroupName = SystemTabNames.Content,
            Order = 0)]
        public virtual string Heading { get; set; }

        [Required]
        [Display(
            Name = "Screen name",
            Description = "The screen name of the user",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string ScreenName { get; set; }

        [Required]
        [Display(
            Name = "Consumer key",
            Description = "The consumer key of the twitter application",
            GroupName = SystemTabNames.Content,
            Order = 10)]
        public virtual string ConsumerKey { get; set; }

        [Required]
        [Display(
            Name = "Consumer secret",
            Description = "The consumer secret of the twitter application",
            GroupName = SystemTabNames.Content,
            Order = 20)]
        public virtual string ConsumerSecret { get; set; }

        [Required]
        [Display(
            Name = "Number of tweets",
            Description = "The number of tweets to show",
            GroupName = SystemTabNames.Content,
            Order = 30)]
        public virtual int NumberOfTweets { get; set; }
    }
}