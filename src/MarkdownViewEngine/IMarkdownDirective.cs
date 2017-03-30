namespace My.AspNetCore.Mvc.Markdown
{
    public interface IMarkdownDirective
    {
        void Process(string properties);
    }
}
