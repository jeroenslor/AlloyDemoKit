using EPi.Cms.ContainerPages;

namespace AlloyDemoKit.Models.Pages
{
    [SiteContentType(
        GUID = "48E693F7-0C4E-4C3F-9A67-819BDA0EC89B",
        DisplayName = "Employee search page",
        Description = "Used to allow user search",
        GroupName = Global.GroupNames.Specialized)]
    [SiteImageUrl(Global.StaticGraphicsFolderPath + "page-type-thumbnail-contact.png")]
    public class EmployeeSearchPage : SitePageData, IContainerPage
    {
    }
}