using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using AlloyDemoKit.Models.Pages.Models.Pages;
using AlloyDemoKit.Models.Pages.Business.Initialization;
using EPiServer.DataAbstraction;
using System.Web.Routing;
using EPiServer.Web.Routing;

namespace AlloyDemoKit.Models.Pages.Business.Tags
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class BlogTagInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var partialRouter = new BlogPartialRouter();
            RouteTable.Routes.RegisterPartialRouter<BlogStartPage, Category>(partialRouter);
        }

        public void Uninitialize(InitializationEngine context)
        {
            // empty by design
        }
    }
}