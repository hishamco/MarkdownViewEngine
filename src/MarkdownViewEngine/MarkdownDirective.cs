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
            var parts = Regex.Split(properties,
                "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)",
                RegexOptions.Compiled);
            foreach (var part in parts)
            {
                var seperatorIndex = part.IndexOf("=");
                var name = ToProper(part.Substring(0, seperatorIndex));
                var value = part.Substring(seperatorIndex + 1).Trim('"');
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

        private string ToProper(string value) =>
            String.Concat(Char.ToUpper(value[0]), value.Substring(1));
    }
}
