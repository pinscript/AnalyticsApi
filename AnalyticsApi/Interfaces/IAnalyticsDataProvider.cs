using System.Collections.Generic;

namespace AnalyticsApi
{
    public interface IAnalyticsDataProvider
    {
        /// <summary>
        /// Try fetching login token from api
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string RequestLoginToken(string username, string password);

        /// <summary>
        /// Request specified resource from api
        /// </summary>
        /// <param name="token">Authorization token</param>
        /// <param name="resource"></param>
        /// <returns></returns>
        string SendRequest(string token, string resource);
    }
}