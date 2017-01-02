using CommonMark;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownViewEngine
{
    public class MarkdownPage : IMarkdownPage
    {
        private readonly IFileProvider _contentRootFileProvider;

        public MarkdownPage(IFileProvider contentRootFileProvider)
        {
            _contentRootFileProvider = contentRootFileProvider;
        }

        public IHtmlContent BodyContent { get; set; }

        public string Layout { get; set; }

        public string Path { get; set; }

        public ViewContext ViewContext { get; set; }

        public async Task ExecuteAsync()
        {
            var fileInfo = _contentRootFileProvider.GetFileInfo(Path);
            using (var readStream = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(readStream))
            {
                var layoutLine = await reader.ReadLineAsync();
                var markdown = await reader.ReadToEndAsync();
                if (layoutLine.StartsWith(MarkdownDirectives.Layout))
                {
                    Layout = new String(layoutLine.Skip(MarkdownDirectives.Layout.Length + 1).ToArray());
                    markdown.Remove(0, layoutLine.Length - 1);
                }
                else
                {
                    markdown = String.Concat(layoutLine, markdown);
                }

                var html = CommonMarkConverter.Convert(markdown);
                BodyContent = new HtmlString(html);
            }
        }
    }
}
