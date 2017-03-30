using System.Collections.Generic;

namespace My.AspNetCore.Mvc.Markdown
{
    public class MarkdownViewEngineOptions
    {
        public IList<string> ViewLocationFormats { get; } = new List<string>();
    }
}