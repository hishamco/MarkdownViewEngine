using MarkdownViewEngine.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                var name = match.Groups[1].Value.ToProper();
                var value = match.Groups[2].Value;
                ((IDictionary<String, Object>)Properties)[name] = value;
            }
        }

        protected static bool HasProperty(dynamic obj, string name)
        {
            var objType = obj.GetType();

            if (objType == typeof(ExpandoObject))
            {
                return ((IDictionary<string, object>)obj).ContainsKey(name);
            }

            return objType.GetProperty(name) != null;
        }
    }
}
