using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace AnalyticsApi
{
    internal class AnalyticsGoalParser
    {
        public IEnumerable<Goal> ParseGoals(string xml)
        {
            var doc = XDocument.Parse(xml);
            var ns = doc.Root.GetDefaultNamespace();
            var gaPrefix = doc.Root.GetNamespaceOfPrefix("ga");

            var nodes = doc.Root.Descendants(ns + "entry");
            var goals = new List<Goal>();

            if (nodes.Any())
            {
                foreach (var node in nodes)
                {
                    var goal = node.Descendants(gaPrefix + "goal");
                    if(goal.Any())
                    {
                        var number = int.Parse(goal.First().Attribute("number").Value);
                        var name = goal.First().Attribute("name").Value;
                        var active = bool.Parse(goal.First().Attribute("active").Value);
                        var value = decimal.Parse(goal.First().Attribute("value").Value.Replace(".", ","));
                        var updated = DateTime.Parse(node.Element(ns + "updated").Value);

                        goals.Add(new Goal {Number = number, Name = name, Active = active, Value = value, Updated = updated});
                    }
                }
            }

            return goals;
        }

        public Dictionary<int, int> ParseCompletions(string xml)
        {
            var dictionary = new Dictionary<int, int>();

            var doc = XDocument.Parse(xml);
            var ns = doc.Root.GetDefaultNamespace();
            var prefix = doc.Root.GetNamespaceOfPrefix("dxp");
            
            var nodes = doc.Root.Descendants(ns + "entry");
            
            if(nodes.Any())
            {
                var node = nodes.First();
                var metrics = node.Descendants(prefix + "metric");

                if(metrics.Any())
                {
                    foreach(var metric in metrics)
                    {
                        var number = metric.Attribute("name").Value.Replace("ga:goal", string.Empty).Replace("Completions", string.Empty);

                        if (number == "ga:visits")
                            continue;

                        var completions = metric.Attribute("value").Value;
                        
                        try
                        {
                            dictionary.Add(int.Parse(number), int.Parse(completions));    
                        }
                        catch(ArgumentException ex)
                        {
                            // Swallow
                        }
                    }
                }
            }

            return dictionary;
        }

        public int ParseVisits(string xml)
        {
            //<dxp:metric confidenceInterval='0.0' name='ga:visits' type='integer' value='3733'/>
            var visits = 0;

            var doc = XDocument.Parse(xml);
            var ns = doc.Root.GetDefaultNamespace();
            var prefix = doc.Root.GetNamespaceOfPrefix("dxp");

            var nodes = doc.Root.Descendants(ns + "entry");

            if (nodes.Any())
            {
                var node = nodes.First();
                var metrics = node.Descendants(prefix + "metric");
                foreach(var metric in metrics)
                {
                    if(metric.Attribute("name").Value == "ga:visits")
                    {
                        var value = metric.Attribute("value").Value;
                        visits = int.Parse(value);
                    }
                }
            }

            return visits;
        }
    }
}