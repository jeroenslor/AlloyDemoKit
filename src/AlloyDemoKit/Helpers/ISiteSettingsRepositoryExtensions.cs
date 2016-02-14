using AlloyDemoKit.Models;
using EPi.Cms.SiteSettings;
using EPiServer.Web;
using System.Collections.Generic;

namespace AlloyDemoKit.Helpers
{
    public static class ISiteSettingsRepositoryExtensions
    {
        public static IAlloySiteSettings Get(this ISiteSettingsRepository repository, SiteDefinition site = null)
        {
            return repository.Get<IAlloySiteSettings>(site);
        }

        public static IEnumerable<IAlloySiteSettings> GetAll(this ISiteSettingsRepository repository)
        {
            return repository.GetAll<IAlloySiteSettings>();
        }
    }
}