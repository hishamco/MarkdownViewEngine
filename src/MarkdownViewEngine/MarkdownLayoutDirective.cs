namespace MarkdownViewEngine
{
    public class MarkdownLayoutDirective : MarkdownDirective
    {
        public string Name { get; set; }

        public override void Process(string properties)
        {
            base.Process(properties);

            if (HasProperty(Properties, nameof(Name)))
            {
                Name = Properties.Name;
            }
        }
    }
}
