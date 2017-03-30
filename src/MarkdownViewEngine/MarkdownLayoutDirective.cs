namespace My.AspNetCore.Mvc.Markdown
{
    public class MarkdownLayoutDirective : MarkdownDirective
    {
        public string Name { get; set; }

        public override void Process(string properties)
        {
            base.Process(properties);

            if (TryGetProperty(Properties, nameof(Name), out object name))
            {
                Name = name.ToString();
            }
        }
    }
}
