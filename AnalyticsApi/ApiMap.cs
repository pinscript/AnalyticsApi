using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AnalyticsApi
{
    internal abstract class ApiMap<T>
    {
        public IList<ApiParseableProperty<T>> ParseMap = new List<ApiParseableProperty<T>>();
        
        public void Map(Expression<Func<T, string>> exp, string path, string attribute = "value")
        {
            Map(exp, path, ElementLevel.EntryLevel);
        }

        public void Map(Expression<Func<T, string>> exp, string path, ElementLevel level, string attribute = "value")
        {
            ParseMap.Add(new ApiParseableProperty<T>(exp, level, path, attribute));
        }
    }

    internal class ApiParseableProperty<T>
    {
        public Expression<Func<T, string>> Expr { get; set; }
        public ElementLevel Level { get; set; }
        public string AttributeName { get; set; }
        public string Path { get; set; }

        public ApiParseableProperty(Expression<Func<T, string>> expr, ElementLevel level, string path, string attribute)
        {
            Expr = expr;
            Path = path;
            Level = level;
            AttributeName = attribute;
        }

        public ApiParseableProperty(Expression<Func<T, string>> expr, string path, string attribute)
            : this(expr, ElementLevel.FeedLevel, path, attribute)
        {
        }
    }
}