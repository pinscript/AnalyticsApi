using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace AnalyticsApi
{
    public class HttpWrapper : IHttpWrapper
    {
        public static HttpWrapper Standard
        {
            get { return new HttpWrapper(); }
        }

        public string Get(string url, Dictionary<string, string> headers)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers.Add(headers.ToNameValueCollection());

            Debug.WriteLine(url);

            using(var response = (HttpWebResponse)req.GetResponse())
            using(var responseStream = response.GetResponseStream())
            using(var sr = new StreamReader(responseStream))
            {
                return sr.ReadToEnd().Trim();   
            }
        }

        public string Post(string url, Dictionary<string, string> parameters)
        {
            if (url == null) throw new ArgumentNullException("url");
            if (parameters == null) throw new ArgumentNullException("parameters");

            var strParameters = GetParameters(parameters);

            var webRequest = WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";

            var bytes = Encoding.ASCII.GetBytes(strParameters);

            Stream stream = null;

            try
            {
                webRequest.ContentLength = bytes.Length;
                stream = webRequest.GetRequestStream();
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (WebException ex)
            {
                throw new WebException("Error", ex);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            try
            {
                var webResponse = webRequest.GetResponse();
                if (webResponse == null)
                    return null;

                var streamReader = new StreamReader(webResponse.GetResponseStream());
                return streamReader.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                if(ex.Message.Contains("403"))
                {
                    throw new InvalidCredentialsException("Invalid credentials");
                }

                throw new WebException("Post exception", ex);
            }
        }

        protected string GetParameters(Dictionary<string, string> values)
        {
            if (values == null) throw new ArgumentNullException("values");

            string parameters = string.Empty;

            bool first = true;

            foreach (var val in values)
            {
                if (!first)
                    parameters += "&";

                parameters += string.Concat(val.Key, "=", val.Value);

                first = false;
            }

            return parameters;
        }
    }
}