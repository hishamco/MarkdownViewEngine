using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace MarkdownViewEngine
{
    public interface IMarkdownViewEngine : IViewEngine
    {
        string GetAbsolutePath(string executingFilePath, string pagePath);
    }
}
