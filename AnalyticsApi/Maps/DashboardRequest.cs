using System;
using System.Diagnostics;
using System.Globalization;

namespace AnalyticsApi
{
    public class DashboardRequest
    {
        public string Visits { get; set; }
        public string PageViews { get; set; }
        public double PagesPerVisit
        {
            get
            {
                return Math.Round(double.Parse(PageViews, NumberStyles.Any) / double.Parse(Visits, NumberStyles.Any), 2);
            }
        }

        /* Calculate bouncerate */
        public string Bounces { get; set; }
        public string Entrances { get; set; }
        public double Bouncrate
        {
            get
            {
                return Math.Round((double.Parse(Bounces, NumberStyles.Any) / double.Parse(Entrances, NumberStyles.Any)) * 100, 2);
            }
        }

        /* Calculate average time on site */
        public string TimeOnSite { get; set; }
        public double AverageTime
        {
            get
            {
                var timePerVisit = float.Parse(TimeOnSite.Replace(".", ",")) / float.Parse(Visits.Replace(".", ","), NumberStyles.Any);

                var minutes = Math.Floor(timePerVisit / 60);
                var seconds = Math.Round(timePerVisit % 60);
                return double.Parse(minutes + "," + seconds);
            }
        }

        /* Calculate number of new visits */
        public double NewVisitsPercent
        {
            get
            {
                return Math.Round((double.Parse(NewVisits, NumberStyles.Any) / double.Parse(Visits, NumberStyles.Any) * 100), 2);
            }
        }
        public string NewVisits { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    internal class DashboardApiMap : ApiMap<DashboardRequest>
    {
        public DashboardApiMap()
        {
            Map(x => x.Visits, "ga:visits");
            Map(x => x.PageViews, "ga:pageviews");
            Map(x => x.Bounces, "ga:bounces");
            Map(x => x.Entrances, "ga:entrances");
            Map(x => x.TimeOnSite, "ga:timeonsite");
            Map(x => x.NewVisits, "ga:newvisits");

            Map(x => x.StartDate, "startDate", ElementLevel.FeedLevel);
            Map(x => x.EndDate, "endDate", ElementLevel.FeedLevel);
        }
    }
}