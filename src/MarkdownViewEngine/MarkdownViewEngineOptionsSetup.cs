using Microsoft.Extensions.Options;

namespace MarkdownViewEngine
{
    public class MarkdownViewEngineOptionsSetup : ConfigureOptions<MarkdownViewEngineOptions>
    {
        public MarkdownViewEngineOptionsSetup()
            : base(options => ConfigureMarkdown(options))
        {

        }

        private static void ConfigureMarkdown(MarkdownViewEngineOptions markdownOptions)
        {
            markdownOptions.ViewLocationFormats.Add("Views/{1}/{0}" + MarkdownViewEngine.ViewExtension);
            markdownOptions.ViewLocationFormats.Add("Views/Shared/{0}" + MarkdownViewEngine.ViewExtension);
        }
    }
}
