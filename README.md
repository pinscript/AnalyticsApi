# What's AnalyticsApi?

AnalyticsApi is a library for communicating with Google Analytics. It has a nice fluent interface that's a breeze to work with.

## Sample usage

This will instantiate a new connection to Google Analytics and get the dashboard data between to given dates:

	var dashboard = new AnalyticsService(new AnalyticsDataProvider(HttpWrapper.Standard))
					.Username("username@gmail.com")
					.Password("password")
					.Logon()
                    .Profile(1312231)
					.GetDashboard("2011-01-01", "2011-01-30");
                    
	Console.WriteLine("PageViews: {0}", dashboard.PageViews);
	Console.WriteLine("Bouncrate: {0}", dashboard.Bouncrate);
	Console.WriteLine("Visits: {0}", dashboard.Visits);

and so on...