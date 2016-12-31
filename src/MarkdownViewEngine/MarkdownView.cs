using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
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

            await RenderPageAsync(MarkdownPage, context);
        }

        private async Task RenderPageAsync(IMarkdownPage page, ViewContext context)
        {
            page.ViewContext = context;
            await page.ExecuteAsync();
            await page.ViewContext.Writer.WriteAsync(page.BodyContent.ToString());
        }
    }
}