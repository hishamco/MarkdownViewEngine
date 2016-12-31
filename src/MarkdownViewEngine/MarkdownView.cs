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
            if (viewEngine == null)
            {
                throw new ArgumentNullException(nameof(viewEngine));
            }

            if (markdownPage == null)
            {
                throw new ArgumentNullException(nameof(markdownPage));
            }

            _viewEngine = viewEngine;
            MarkdownPage = markdownPage;
        }

        public string Path
        {
            get { return MarkdownPage.Path; }
        }

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