using CommonMark;
using System.Collections.Generic;

namespace MarkdownViewEngine
{
    public class MarkdownViewEngineOptions
    {
        public IList<string> ViewLocationFormats { get; } = new List<string>();

        public CommonMarkSettings MarkdownSettings { get; } = CommonMarkSettings.Default.Clone();
    }
}