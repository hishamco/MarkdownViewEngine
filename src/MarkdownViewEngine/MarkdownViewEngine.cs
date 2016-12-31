using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Options;

namespace MarkdownViewEngine
{
    public class MarkdownViewEngine : IMarkdownViewEngine
    {
        public static readonly string ViewExtension = ".md";

        private const string ControllerKey = "controller";

        private readonly MarkdownViewEngineOptions _options;

        public MarkdownViewEngine(
            IOptions<MarkdownViewEngineOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            if (_options.ViewLocationFormats.Count == 0)
            {
                throw new ArgumentException(nameof(optionsAccessor));
            }
        }

        public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentException(nameof(viewName));
            }

            if (IsApplicationRelativePath(viewName) || IsRelativePath(viewName))
            {
                return ViewEngineResult.NotFound(viewName, Enumerable.Empty<string>());
            }

            return LocatePageFromViewLocations(context, viewName, isMainPage);
        }

        public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
        {
            if (string.IsNullOrEmpty(viewPath))
            {
                throw new ArgumentException(nameof(viewPath));
            }

            if (!(IsApplicationRelativePath(viewPath) || IsRelativePath(viewPath)))
            {
                return ViewEngineResult.NotFound(viewPath, Enumerable.Empty<string>());
            }

            return LocatePageFromPath(executingFilePath, viewPath, isMainPage);
        }

        public string GetAbsolutePath(string executingFilePath, string pagePath)
        {
            if (string.IsNullOrEmpty(pagePath))
            {
                return pagePath;
            }

            if (IsApplicationRelativePath(pagePath))
            {
                return pagePath;
            }

            if (!IsRelativePath(pagePath))
            {
                return pagePath;
            }

            if (string.IsNullOrEmpty(executingFilePath))
            {
                return Path.AltDirectorySeparatorChar + pagePath;
            }

            var index = executingFilePath.LastIndexOf(Path.AltDirectorySeparatorChar);
            return executingFilePath.Substring(0, index + 1) + pagePath;
        }

        private ViewEngineResult LocatePageFromViewLocations(
            ActionContext actionContext,
            string viewName,
            bool isMainPage)
        {
            var controllerName = GetNormalizedRouteValue(actionContext, ControllerKey);
            var searchedLocations = new List<string>();

            foreach (var location in _options.ViewLocationFormats)
            {
                var view = string.Format(location, viewName, controllerName);
                if (File.Exists(view))
                {
                    var page = new MarkdownPage { Path = view };

                    return ViewEngineResult.Found(viewName, new MarkdownView(this, page));
                }

                searchedLocations.Add(view);
            }
            return ViewEngineResult.NotFound(viewName, searchedLocations);
        }

        private ViewEngineResult LocatePageFromPath(string executingFilePath, string pagePath, bool isMainPage)
        {
            var applicationRelativePath = GetAbsolutePath(executingFilePath, pagePath);

            if (!(IsApplicationRelativePath(pagePath) || IsRelativePath(pagePath)))
            {
                return ViewEngineResult.NotFound(applicationRelativePath, Enumerable.Empty<string>());
            }

            var page = new MarkdownPage { Path = applicationRelativePath };

            return ViewEngineResult.Found(applicationRelativePath, new MarkdownView(this, page));
        }

        private static string GetNormalizedRouteValue(ActionContext context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!context.RouteData.Values.TryGetValue(key, out object routeValue))
            {
                return null;
            }

            var actionDescriptor = context.ActionDescriptor;
            string normalizedValue = null;
            if (actionDescriptor.RouteValues.TryGetValue(key, out string value) &&
                !string.IsNullOrEmpty(value))
            {
                normalizedValue = value;
            }

            var stringRouteValue = routeValue?.ToString();
            if (string.Equals(normalizedValue, stringRouteValue, StringComparison.OrdinalIgnoreCase))
            {
                return normalizedValue;
            }

            return stringRouteValue;
        }

        private static bool IsApplicationRelativePath(string name) =>
            name[0] == '~' || name[0] == Path.AltDirectorySeparatorChar;

        private static bool IsRelativePath(string name) =>
            name.EndsWith(ViewExtension, StringComparison.OrdinalIgnoreCase);
    }
}
