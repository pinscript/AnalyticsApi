namespace AnalyticsApi
{
    public class AnalyticsAccountRequest
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string ProfileId { get; set; }
    }

    internal class AnalyticsAccountApiMap : ApiMap<AnalyticsAccountRequest>
    {
        public AnalyticsAccountApiMap()
        {
            Map(x => x.AccountId, "ga:accountId");
            Map(x => x.AccountName, "ga:accountName");
            Map(x => x.ProfileId, "ga:profileId");
        }
    }
}