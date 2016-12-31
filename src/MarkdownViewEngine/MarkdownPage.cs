using CommonMark;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownViewEngine
{
    public class MarkdownPage : IMarkdownPage
    {
        public IHtmlContent BodyContent { get; set; }

        public string Path { get; set; }

        public ViewContext ViewContext { get; set; }

        public async Task ExecuteAsync()
        {
            using (var reader = new StreamReader(File.OpenRead(Path)))
            {
                var markdown = await reader.ReadToEndAsync();
                var html = CommonMarkConverter.Convert(markdown);

                BodyContent = new HtmlString(html);
            }
        }
    }
}
