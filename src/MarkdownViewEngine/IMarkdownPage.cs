using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace My.AspNetCore.Mvc.Markdown
{
    public interface IMarkdownPage
    {
        ViewContext ViewContext { get; set; }

        string Layout { get; set; }

        IHtmlContent BodyContent { get; set; }

        dynamic Model { get; set; }

        string Path { get; set; }

        string Title { get; set; }

        Task ExecuteAsync();
    }
}
