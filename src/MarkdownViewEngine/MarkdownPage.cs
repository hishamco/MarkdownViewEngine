using CommonMark;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownViewEngine
{
    public class MarkdownPage : IMarkdownPage
    {
        private readonly IFileProvider _contentRootFileProvider;
        private readonly MarkdownViewEngineOptions _options;

        public MarkdownPage(IFileProvider contentRootFileProvider,
            MarkdownViewEngineOptions options)
        {
            _contentRootFileProvider = contentRootFileProvider;
            _options = options;
        }

        public IHtmlContent BodyContent { get; set; }

        public string Layout { get; set; }

        public string Path { get; set; }

        public string Title { get; set; }

        public ViewContext ViewContext { get; set; }

        public async Task ExecuteAsync()
        {
            var fileInfo = _contentRootFileProvider.GetFileInfo(Path);
            string content;
            string markdown = string.Empty;
            using (var readStream = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(readStream))
            {
                content = await reader.ReadToEndAsync();
            }
            if (content.StartsWith(MarkdownDirectives.Page, StringComparison.OrdinalIgnoreCase))
            {
                var newLineIndex = content.IndexOf(Environment.NewLine, MarkdownDirectives.Page.Length);
                var pageProperties = content.Substring(MarkdownDirectives.Page.Length, newLineIndex - MarkdownDirectives.Page.Length).Trim();
                var pageDirective = new MarkdownPageDirective();
                pageDirective.Process(pageProperties);
                Title = pageDirective.Title;
                Layout = Layout ?? pageDirective.Layout;
                markdown = content.Substring(content.IndexOf(Environment.NewLine));
            }
            else if (content.StartsWith(MarkdownDirectives.Layout, StringComparison.OrdinalIgnoreCase))
            {
                var layoutProperties = content.Substring(MarkdownDirectives.Layout.Length).Trim();
                var layoutDirective = new MarkdownLayoutDirective();
                layoutDirective.Process(layoutProperties);
                Layout = Layout ?? layoutDirective.Name;
            }
            else
            {
                markdown = content;
            }

            var html = CommonMarkConverter.Convert(markdown, _options.MarkdownSettings);
            BodyContent = new HtmlString(html);
        }
    }
}
