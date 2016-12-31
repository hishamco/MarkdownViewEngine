using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace MarkdownViewEngine
{
    public class MarkdownMvcViewOptionsSetup : IConfigureOptions<MvcViewOptions>
    {
        private readonly IMarkdownViewEngine _markdownViewEngine;

        public MarkdownMvcViewOptionsSetup(IMarkdownViewEngine markdownViewEngine)
        {
            _markdownViewEngine = markdownViewEngine ?? 
                throw new ArgumentNullException(nameof(markdownViewEngine));
        }

        public void Configure(MvcViewOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.ViewEngines.Add(_markdownViewEngine);
        }
    }
}
