namespace My.AspNetCore.Mvc.Markdown
{
    public class MarkdownModelDirective : MarkdownDirective
    {
        public dynamic Model { get; set; }

        public override void Process(string properties)
        {
            base.Process(properties);
            Model = Properties;
        }
    }
}
