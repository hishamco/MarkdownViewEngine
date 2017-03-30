using MarkdownViewEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarkdownViewEngine
{
    public abstract class MarkdownDirective : IMarkdownDirective
    {
        protected dynamic Properties { get; private set; }

        public virtual void Process(string properties)
        {
            Properties = new ExpandoObject();
            var regex = new Regex("([^=\\s]+)\\s*=\\s*\"([^\"]*)\"", RegexOptions.Compiled);
            foreach (Match match in regex.Matches(properties))
            {
                var name = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                ((IDictionary<String, Object>)Properties)[name] = value;
            }
        }

        protected static bool TryGetProperty(dynamic obj, string name, out object value)
        {
            value = ((IDictionary<string, object>)obj)
                .Where(o => o.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
                .Select(o => o.Value)
                .SingleOrDefault();

            return value != null;
        }
    }
}
