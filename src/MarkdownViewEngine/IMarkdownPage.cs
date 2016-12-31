using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarkdownViewEngine
{
    public interface IMarkdownPage
    {
        ViewContext ViewContext { get; set; }

        IHtmlContent BodyContent { get; set; }

        string Path { get; set; }

        Task ExecuteAsync();
    }
}
