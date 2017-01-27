namespace MarkdownViewEngine
{
    public class MarkdownPageDirective : MarkdownDirective
    {
        public string Title { get; set; }

        public string Layout { get; set; }

        public override void Process(string properties)
        {
            base.Process(properties);

            if (HasProperty(Properties, nameof(Title)))
            {
                Title = Properties.Title;
            }

            if (HasProperty(Properties, nameof(Layout)))
            {
                Layout = Properties.Layout;
            }
        }
    }
}
