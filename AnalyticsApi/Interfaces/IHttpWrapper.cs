using System.Collections.Generic;

namespace AnalyticsApi
{
    public interface IHttpWrapper
    {
        string Get(string url, Dictionary<string, string> dictionary);
        string Post(string url, Dictionary<string, string> parameters);
    }
}