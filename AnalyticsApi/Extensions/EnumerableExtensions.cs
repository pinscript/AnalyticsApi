using System.Collections.Generic;
using System.Collections.Specialized;

namespace AnalyticsApi
{
    internal static class EnumerableExtensions
    {
        public static NameValueCollection ToNameValueCollection<T>(this Dictionary<T, T> dictionary) where T : class
        {
            var collection = new NameValueCollection();

            foreach (var item in dictionary)
            {
                collection.Add(item.Key.ToString(), item.Value.ToString());
            }

            return collection;
        }
    }
}
