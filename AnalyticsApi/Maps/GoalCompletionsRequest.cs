namespace AnalyticsApi
{
    public class GoalCompletionsRequest
    {
        public string Completions { get; set; }
    }

    internal class GoalCompletionsRequestApiMap : ApiMap<GoalCompletionsRequest>
    {
        public GoalCompletionsRequestApiMap(int goalNumber)
        {
            Map(x => x.Completions, "ga:goal" + goalNumber + "Completions");
        }
    }
}