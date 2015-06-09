using System.Web.Mvc;

namespace Bookland.Helpers.Abstract
{
    public interface IMvcHelpers
    {
        string RenderViewToString(ControllerContext controllerContext, string viewName, object model);
    }
}
