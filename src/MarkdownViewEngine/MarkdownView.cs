using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownViewEngine
{
    public class MarkdownView : IView
    {
        private readonly IMarkdownViewEngine _viewEngine;

        public MarkdownView(
            IMarkdownViewEngine viewEngine,
            IMarkdownPage markdownPage)
        {
            _viewEngine = viewEngine ?? 
                throw new ArgumentNullException(nameof(viewEngine));
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

            var bodyWriter = await RenderPageAsync(MarkdownPage, context);
            await RenderLayoutAsync(context, bodyWriter);
        }

        private async Task<TextWriter> RenderPageAsync(IMarkdownPage page, ViewContext context)
        {
            page.ViewContext = context;
            await page.ExecuteAsync();
            return page.ViewContext.Writer;
        }

        private async Task RenderLayoutAsync(ViewContext context, TextWriter writer)
        {
            if (MarkdownPage.Layout != null)
            {
                var layoutPage = GetLayoutPage(context, MarkdownPage.Path, MarkdownPage.Layout);
                var pageContent = MarkdownPage.BodyContent.ToString();
                writer = await RenderPageAsync(layoutPage, context);
                using (var reader = new StreamReader(File.OpenRead(layoutPage.Path)))
                {
                    var layoutContent = await reader.ReadToEndAsync();
                    layoutContent = layoutContent.Replace("@Body", pageContent);
                    await writer.WriteAsync(layoutContent);
                }
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