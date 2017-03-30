using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace My.AspNetCore.Mvc.Markdown
{
    public interface IMarkdownViewEngine : IViewEngine
    {
        string GetAbsolutePath(string executingFilePath, string pagePath);
    }
}
