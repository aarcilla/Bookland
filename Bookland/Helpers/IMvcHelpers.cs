using System.Web.Mvc;

namespace Bookland.Helpers
{
    public interface IMvcHelpers
    {
        string RenderViewToString(ControllerContext controllerContext, string viewName, object model);
    }
}
