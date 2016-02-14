using AlloyDemoKit.Models.Blocks;
using EPi.Cms.SiteSettings;
using EPiServer.Core;
using EPiServer.SpecializedProperties;

namespace AlloyDemoKit.Models
{
    public interface IAlloySiteSettings : ISiteSettings
    {
        LinkItemCollection ProductPageLinks { get; set; }

        LinkItemCollection CompanyInformationPageLinks { get; set; }

        LinkItemCollection NewsPageLinks { get; set; }

        LinkItemCollection CustomerZonePageLinks { get; set; }

        PageReference GlobalNewsPageLink { get; set; }

        PageReference ContactsPageLink { get; set; }

        PageReference SearchPageLink { get; set; }

        PageReference NewsPageLink { get; set; }

        PageReference BlogPageLink { get; set; }

        ContentReference EmployeeContainerPageLink { get; set; }

        PageReference EmployeeLocationPageLink { get; set; }

        ContentReference EmployeeExpertiseLink { get; set; }

        SiteLogotypeBlock SiteLogotype { get; set; }

        CategoryList Category { get; set; }
    }
}
