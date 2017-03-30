using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nustache.Core;

namespace My.AspNetCore.Mvc.Markdown
{
    public class MarkdownView : IView
    {
        private readonly IMarkdownViewEngine _viewEngine;
        private readonly IMarkdownPage _viewStartPage;

        public MarkdownView(
            IMarkdownViewEngine viewEngine,
            IMarkdownPage viewStartPage,
            IMarkdownPage markdownPage)
        {
            _viewEngine = viewEngine ?? 
                throw new ArgumentNullException(nameof(viewEngine));
            _viewStartPage = viewStartPage;
            MarkdownPage = markdownPage ?? 
                throw new ArgumentNullException(nameof(markdownPage));
        }

        public string Path => MarkdownPage.Path;

        public IMarkdownPage MarkdownPage { get; }

        public async Task RenderAsync(ViewContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var bodyWriter = await RenderPageAsync(MarkdownPage, context, invokeViewStart: true);
            await RenderLayoutAsync(context, bodyWriter);
        }

        private async Task<TextWriter> RenderPageAsync(
            IMarkdownPage page,
            ViewContext context,
            bool invokeViewStart)
        {
            if (invokeViewStart)
            {
                await RenderViewStartsAsync(context);
            }

            await RenderPageCoreAsync(page, context);

            return page.ViewContext.Writer;
        }

        private Task RenderPageCoreAsync(IMarkdownPage page, ViewContext context)
        {
            page.ViewContext = context;
            return page.ExecuteAsync();
        }

        private async Task RenderViewStartsAsync(ViewContext context)
        {
            if (_viewStartPage != null)
            {
                string layout = null;
                await RenderPageCoreAsync(_viewStartPage, context);
                layout = _viewEngine.GetAbsolutePath(_viewStartPage.Path, _viewStartPage.Layout);

                if (layout != null)
                {
                    MarkdownPage.Layout = layout;
                }
            }
        }

        private async Task RenderLayoutAsync(ViewContext context, TextWriter writer)
        {
            var pageContent = MarkdownPage.BodyContent.ToString();
            MarkdownPage.Layout = MarkdownPage.Layout ?? _viewStartPage.Layout;

            if (MarkdownPage.Layout != null)
            {
                const string BodyToken = "{{body}}";
                const string TitleTokenName = "title";
                var layoutPage = GetLayoutPage(context, MarkdownPage.Path, MarkdownPage.Layout);
                writer = await RenderPageAsync(layoutPage, context, invokeViewStart: true);

                var layoutContent = layoutPage.BodyContent.ToString();
                if (!layoutContent.Contains(BodyToken))
                {
                    throw new InvalidOperationException($"The {BodyToken} is missing in {layoutPage.Path}.");
                }
                layoutContent = layoutContent.Replace(BodyToken, pageContent);

                var data = new ExpandoObject();
                ((IDictionary<string, object>)data)[TitleTokenName] = MarkdownPage.Title;
                if (context.ViewData.Model != null)
                {
                    foreach (var prop in context.ViewData.Model.GetType().GetProperties())
                    {
                        ((IDictionary<string, object>)data)[prop.Name] = prop.GetValue(context.ViewData.Model);
                    }
                }
                else
                {
                    if (MarkdownPage.Model != null)
                    {
                        foreach (var prop in (IDictionary<string, object>)MarkdownPage.Model)
                        {
                            ((IDictionary<string, object>)data)[prop.Key] = prop.Value;
                        }
                    }
                }
                layoutContent = Render.StringToString(layoutContent, data);
                await writer.WriteAsync(layoutContent);
            }
            else
            {
                await writer.WriteAsync(pageContent);
            }
        }

        private IMarkdownPage GetLayoutPage(ViewContext context, string executingFilePath, string layoutPath)
        {
            var layoutPageResult = _viewEngine.GetView(executingFilePath, layoutPath, isMainPage: true);
            var originalLocations = layoutPageResult.SearchedLocations;
            if (layoutPageResult.View == null)
            {
                layoutPageResult = _viewEngine.FindView(context, layoutPath, isMainPage: true);
            }

            if (layoutPageResult.View == null)
            {
                var locations = string.Empty;
                if (originalLocations.Any())
                {
                    locations = Environment.NewLine + string.Join(Environment.NewLine, originalLocations);
                }

                if (layoutPageResult.SearchedLocations.Any())
                {
                    locations +=
                        Environment.NewLine + string.Join(Environment.NewLine, layoutPageResult.SearchedLocations);
                }

                throw new InvalidOperationException($"The layout view '{layoutPath}' could not be located. The following locations were searched:{locations}");
            }

            var layoutPage = ((MarkdownView) layoutPageResult.View).MarkdownPage;
            return layoutPage;
        }
    }
}