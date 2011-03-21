using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AnalyticsApi
{
    public class AnalyticsService
    {
        private readonly IAnalyticsDataProvider _dataProvider;

        protected string _username;
        protected string _password;
        protected int? _profile;
        protected string _token;
        private int? _account;

        public AnalyticsService(IAnalyticsDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Check if we have a token or not
        /// </summary>
        public bool IsLogedIn
        {
            get { return _token != string.Empty; }
        }

        /// <summary>
        /// Set username for current context
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AnalyticsService Username(string username)
        {
            _username = username;
            return this;
        }

        /// <summary>
        /// Set password for current context
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public AnalyticsService Password(string password)
        {
            _password = password;
            return this;
        }

        /// <summary>
        /// Set profile id for current context
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AnalyticsService Profile(int id)
        {
            _profile = id;
            return this;
        }

        /// <summary>
        /// Set account id for current context
        /// </summary>
        /// <param name="accountId"></param>
        public AnalyticsService Account(int accountId)
        {
            _account = accountId;
            return this;
        }

        /// <summary>
        /// Get current username
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            return _username;
        }

        /// <summary>
        /// Get current password
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            return _password;
        }

        /// <summary>
        /// Get current profile id
        /// </summary>
        /// <returns></returns>
        public int? GetProfile()
        {
            return _profile;
        }

        /// <summary>
        /// Get current token
        /// </summary>
        /// <returns></returns>
        public string GetToken()
        {
            return _token;
        }

        /// <summary>
        /// Login to service
        /// </summary>
        public AnalyticsService Logon()
        {
            // Do not login if we have a token
            if (string.IsNullOrEmpty(_token) == false) return this;

            _token = _dataProvider.RequestLoginToken(_username, _password);
            return this;
        }

        /// <summary>
        /// Get all accounts for a specific user
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AnalyticsAccountRequest> GetAccounts()
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No token found. Did you forget to login?");

            var data = _dataProvider.SendRequest(_token, "https://www.google.com/analytics/feeds/accounts/default");
            var parser = new AnalyticsXmlParser();
            return parser.Parse(data, new AnalyticsAccountApiMap());
        }

        /// <summary>
        /// Get dashboard summary for a given profile
        /// </summary>
        /// <returns></returns>
        public DashboardRequest GetDashboard(string start, string end)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No token found. Did you forget to login?");

            if(_profile == null)
                throw new Exception("No profile set. Use .Profile() to set.");

            var data = _dataProvider.SendRequest(_token, "https://www.google.com/analytics/feeds/data?ids=ga%3A" + _profile.Value + "&metrics=ga%3Avisits,ga%3Apageviews,ga%3Abounces,ga%3Aentrances,ga%3AtimeOnSite,ga%3AnewVisits&start-date=" + start + "&end-date=" + end + "&max-results=50");

            var parser = new AnalyticsXmlParser();
            return parser.Parse(data, new DashboardApiMap()).First();
        }

        public IEnumerable<Goal> GetGoals()
        {
            var data = _dataProvider.SendRequest(_token, "https://www.google.com/analytics/feeds/datasources/ga/accounts/~all/webproperties/~all/profiles/~all/goals");
            Debug.WriteLine(data);

            var parser = new AnalyticsGoalParser();
            var goals = parser.ParseGoals(data);

            return goals;
        }

        public IEnumerable<Goal> GetGoalCompletions(IEnumerable<Goal> goals, string start, string end)
        {
            var metrics = "ga%3Avisits,";
            foreach(var goal in goals)
            {
                metrics += "ga%3Agoal" + goal.Number + "Completions,";
            }

            metrics = metrics.TrimEnd(',');

            var data = _dataProvider.SendRequest(_token, "https://www.google.com/analytics/feeds/data?ids=ga%3A" + _profile.Value + "&metrics=" + metrics + "&start-date=" + start + "&end-date=" + end + "&max-results=50");
            
            var parser = new AnalyticsGoalParser();
            var visits = parser.ParseVisits(data);

            var completions = parser.ParseCompletions(data);

            foreach(var completion in completions)
            {
                var goal = goals.FirstOrDefault(x => x.Number == completion.Key);
                goal.Completions = completion.Value;
                goal.ConversionRate = Math.Round((completion.Value / (double)visits) * 100, 2);

                yield return goal;
            }
        }

        public void GetTopTrafficSources(string start, string end)
        {
            if (string.IsNullOrEmpty(_token))
                throw new Exception("No token found. Did you forget to login?");

            if (_profile == null)
                throw new Exception("No profile set. Use .Profile() to set.");

            var url = "https://www.google.com/analytics/feeds/data?ids=ga%3A" + _profile.Value +
                      "&metrics=ga%3Avisits&dimensions=ga%3Akeyword&start-date=" + start + "&end-date=" + end +
                      "&max-results=50" +
                      "&filters=ga%3Akeyword!=(not set)0&sort=-ga%3Avisits";

            var data = _dataProvider.SendRequest(_token, url);
            Debug.Write(data);
        }

        public Goal GetGoalCompletions(Goal goal, string end)
        {
            var metrics = string.Format("ga%3Avisits,ga%3Agoal{0}Completions", goal.Number);
            var data = _dataProvider.SendRequest(_token, "https://www.google.com/analytics/feeds/data?ids=ga%3A" + _profile.Value + "&metrics=" + metrics + "&start-date=" + goal.Updated.ToShortDateString() + "&end-date=" + end + "&max-results=50");

            var parser = new AnalyticsGoalParser();

            var visits = parser.ParseVisits(data);
            var completions = parser.ParseCompletions(data);

            foreach(var completion in completions)
            {
                goal.Completions = completion.Value;
                goal.ConversionRate = Math.Round((completion.Value / (double)visits) * 100, 2);
            }

            return goal;
        }
    }

    public class Goal
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public bool Active { get; set; }
        public int Completions { get; set; }
        public double ConversionRate { get; set; }
        public decimal Value { get; set; }
        public DateTime Updated { get; set; }
    }
}