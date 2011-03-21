using System.Collections.Generic;

namespace AnalyticsApi
{
    public class AnalyticsDataProvider : IAnalyticsDataProvider
    {
        private readonly string _applicationName;
        private readonly IHttpWrapper _httpWrapper;

        public AnalyticsDataProvider(string applicationName, IHttpWrapper httpWrapper)
        {
            _applicationName = applicationName;
            _httpWrapper = httpWrapper;
        }

        public string RequestLoginToken(string username, string password)
        {
            var parameters = new Dictionary<string, string>
                                 {
                                     {"Email", username},
                                     {"Passwd", password},
                                     {"accountType", "GOOGLE"},
                                     {"service", "analytics"},
                                     {"source", _applicationName}
                                 };

            var response = _httpWrapper.Post("https://www.google.com/accounts/ClientLogin", parameters);

            var token = response.Remove(0, response.IndexOf("Auth=")).Replace("Auth=", string.Empty);
            return token;
        }

        public string SendRequest(string token, string resource)
        {
            return _httpWrapper.Get(resource, new Dictionary<string, string> { { "Authorization", "GoogleLogin auth=" + token } });
        }
    }
}