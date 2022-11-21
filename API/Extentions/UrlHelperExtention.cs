using Microsoft.AspNetCore.Mvc;

namespace Common.Extentions
{
    public static class UrlHelperExtention
    {
        public static string? ControllerAction<T>(this IUrlHelper urlHelper, string action, object? args)
            where T : ControllerBase
        {
            Type controllerType = typeof(T);

            if (controllerType.GetMethod(action) == null)
                return null;

            string controllerName = controllerType.Name.Replace("Controller", string.Empty);
            return urlHelper.Action(action, controllerName, args);
        }
    }
}
