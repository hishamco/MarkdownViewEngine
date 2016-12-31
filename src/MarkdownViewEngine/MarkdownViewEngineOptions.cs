using System.Collections.Generic;

namespace MarkdownViewEngine
{
    public class MarkdownViewEngineOptions
    {
        public IList<string> ViewLocationFormats { get; } = new List<string>();
    }
}