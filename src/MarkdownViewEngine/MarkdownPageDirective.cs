namespace My.AspNetCore.Mvc.Markdown
{
    public class MarkdownPageDirective : MarkdownDirective
    {
        public string Title { get; set; }

        public string Layout { get; set; }

        public override void Process(string properties)
        {
            base.Process(properties);

            if (TryGetProperty(Properties, nameof(Title), out object title))
            {
                Title = title.ToString();
            }

            if (TryGetProperty(Properties, nameof(Layout), out object layout))
            {
                Layout = layout.ToString();
            }
        }
    }
}
