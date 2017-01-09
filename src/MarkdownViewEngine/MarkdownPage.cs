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

        public string Title { get; set; }

        public ViewContext ViewContext { get; set; }

        public async Task ExecuteAsync()
        {
            var fileInfo = _contentRootFileProvider.GetFileInfo(Path);
            using (var readStream = fileInfo.CreateReadStream())
            using (var reader = new StreamReader(readStream))
            {
                var directiveLine = await reader.ReadLineAsync();
                var markdown = await reader.ReadToEndAsync();

                if (directiveLine.StartsWith(MarkdownDirectives.Page, StringComparison.OrdinalIgnoreCase))
                {
                    var parts = directiveLine.Split(' ').Skip(1);
                    foreach (var part in parts)
                    {
                        var seperatorIndex = part.IndexOf("=");
                        var name = part.Substring(0, seperatorIndex);
                        var value = part.Substring(seperatorIndex + 1).Trim('"');
                        if (name == "layout")
                        {
                            Layout = value;
                        }
                        else if(name == "title")
                        {
                            Title = value;
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                    }
                    if (!String.IsNullOrEmpty(markdown))
                    {
                        markdown.Remove(0, directiveLine.Length - 1);
                    }
                }
                else if (directiveLine.StartsWith(MarkdownDirectives.Layout, StringComparison.OrdinalIgnoreCase))
                {
                    Layout = directiveLine.Substring(MarkdownDirectives.Layout.Length + 1);
                }
                else
                {
                    markdown = String.Concat(directiveLine, markdown);
                }

                var html = CommonMarkConverter.Convert(markdown);
                BodyContent = new HtmlString(html);
            }
        }
    }
}
