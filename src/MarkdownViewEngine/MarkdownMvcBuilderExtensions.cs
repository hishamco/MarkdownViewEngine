using My.AspNetCore.Mvc.Markdown;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MarkdownMvcBuilderExtensions
    {
        public static IMvcBuilder AddMarkdownViewEngine(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddOptions();
            builder.Services.AddTransient<IConfigureOptions<MarkdownViewEngineOptions>, MarkdownViewEngineOptionsSetup>();
            builder.Services.AddTransient<IConfigureOptions<MvcViewOptions>, MarkdownMvcViewOptionsSetup>();
            builder.Services.AddSingleton<IMarkdownViewEngine, MarkdownViewEngine>();

            return builder;
        }

        public static IMvcBuilder AddMarkdownViewEngine(
            this IMvcBuilder builder,
            Action<MarkdownViewEngineOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.Services.AddOptions();
            builder.Services.AddTransient<IConfigureOptions<MarkdownViewEngineOptions>, MarkdownViewEngineOptionsSetup>();
            builder.Services.Configure(setupAction);
            builder.Services.AddTransient<IConfigureOptions<MvcViewOptions>, MarkdownMvcViewOptionsSetup>();
            builder.Services.AddSingleton<IMarkdownViewEngine, MarkdownViewEngine>();

            return builder;
        }
    }
}
