using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Curiosity.Tools.Web
{
    public static class HtmlHelperExtensions
    {
        private static readonly char[] Delimeters = {' ', ',', ';'};

        public static string PageClass(this IHtmlHelper htmlHelper)
        {
            string currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            return currentAction;
        }
        
        public static string IsSelected(
            this IHtmlHelper html,
            string? controller = null,
            string? actions = null,
            string? cssClass = null,
            string? notAction = null)
        {
            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = (string)html.ViewContext.RouteData.Values["action"];
            string currentController = (string)html.ViewContext.RouteData.Values["controller"];

            if (String.IsNullOrEmpty(controller))
                controller = currentController;

            if (String.IsNullOrEmpty(actions))
                actions = currentAction;

            var actionsToFilter = actions.Split(Delimeters, StringSplitOptions.RemoveEmptyEntries);

            return html.ViewContext.HttpContext.IsControllerActions(actions, controller) 
                   && (notAction == null || !actionsToFilter.Any(x => notAction.Equals(x, StringComparison.OrdinalIgnoreCase)))
                ? cssClass 
                : String.Empty;
        }
    }
}
