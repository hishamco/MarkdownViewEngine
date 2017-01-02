using CommonMark;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownViewEngine
{
    public class MarkdownPage : IMarkdownPage
    {
        private const string LayoutDirective = "@Layout";

        public IHtmlContent BodyContent { get; set; }

        public string Layout { get; set; }

        public string Path { get; set; }

        public ViewContext ViewContext { get; set; }

        public async Task ExecuteAsync()
        {
            using (var reader = new StreamReader(File.OpenRead(Path)))
            {
                var layoutLine = await reader.ReadLineAsync();
                var markdown = await reader.ReadToEndAsync();
                if (layoutLine.StartsWith(LayoutDirective))
                {
                    Layout = new String(layoutLine.Skip(LayoutDirective.Length + 1).ToArray());
                    markdown.Remove(0, layoutLine.Length - 1);
                }

                var html = CommonMarkConverter.Convert(markdown);
                BodyContent = new HtmlString(html);
            }
        }
    }
}
