using System.Web.Mvc;

namespace Bookland.Helpers
{
    public static class LinkHelpers
    {
        /// <summary>
        /// Returns an image element wrapped by an anchor element that contains the virtual path of the specified action.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="imageLocation">URL specifying where the image is located.</param>
        /// <param name="imageHeight">Optionally specify the height of the image.</param>
        /// <param name="classes">Specify any CSS classes to attach to the image tag.</param>
        /// <param name="action">The name of the action.</param>
        /// <param name="controller">The name of the controller.</param>
        /// <param name="routeValues">
        ///     An object that contains the parameters for a route. The parameters are retrieved
        ///     through reflection by examining the properties of the object. The object
        ///     is typically created by using object initializer syntax.
        /// </param>
        /// <returns>An anchor element with the an image element enclosed within.</returns>
        public static MvcHtmlString ImageActionLink(this HtmlHelper htmlHelper, string imageLocation, int? imageHeight, string[] classes, string action, string controller, object routeValues = null)
        {
            // Generate the link URL based on the action method, controller and route values parameters
            UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            string linkURL = urlHelper.Action(action, controller, routeValues).ToString();

            // Create the image element
            TagBuilder imageTag = new TagBuilder("img");
            imageTag.MergeAttribute("src", imageLocation);
            imageTag.MergeAttribute("border", "0");
            imageTag.MergeAttribute("alt", controller + ": " + action);
            if (imageHeight != null)
            {
                imageTag.MergeAttribute("height", imageHeight.ToString());
            }

            if (classes != null)
            {
                foreach (string @class in classes)
                {
                    imageTag.AddCssClass(@class);
                }
            }

            // Create the anchor element and place the image tag within
            TagBuilder linkTag = new TagBuilder("a");
            linkTag.MergeAttribute("href", linkURL);
            linkTag.InnerHtml = imageTag.ToString();

            return MvcHtmlString.Create(linkTag.ToString());
        }
    }
}