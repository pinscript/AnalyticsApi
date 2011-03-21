using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace AnalyticsApi
{
    internal class AnalyticsXmlParser
    {
        public IEnumerable<TReturn> Parse<TReturn>(string xml, ApiMap<TReturn> map)
            where TReturn : new()
        {
            var result = new List<TReturn>();
            var doc = XDocument.Parse(xml);
            var ns = doc.Root.GetDefaultNamespace();
            var nodes = doc.Root.Descendants(ns + "entry"); // Default path is entry

            foreach (var node in nodes)
            {
                var item = new TReturn();
                var type = item.GetType();

                foreach (var prop in map.ParseMap)
                {
                    var key = prop.Expr;
                    var propertyName = (key.Body as MemberExpression).Member.Name;

                    var property = type.GetProperty(propertyName);

                    var value = GetXmlValue(doc, prop.Level, prop.Path, prop.AttributeName);
                    
                    property.SetValue(item, value, null);
                }

                result.Add(item);
            }

            return result;
        }

        public string GetXmlValue(XDocument doc, ElementLevel level, string path, string attributeName)
        {
            // Get namespaces
            var ns = doc.Root.GetDefaultNamespace();
            var prefix = doc.Root.GetNamespaceOfPrefix("dxp");
            var gaPrefix = doc.Root.GetNamespaceOfPrefix("ga");

            if (level == ElementLevel.FeedLevel)
            {
                return GetXmlTopValue(doc, path);
            }

            // Elementlevel is entry level
            var nodes = doc.Root.Descendants(ns + "entry");

            var value = "";

            foreach (var node in nodes)
            {
                // Try finding properties first
                var xmlNode = node.Elements(prefix + "property")
                    .Where(x => x.Attribute("name").Value.ToLower() == path.ToLower());

                // Then metrics
                if (xmlNode.Any() == false)
                    xmlNode = node.Elements(prefix + "metric")
                        .Where(x => x.Attribute("name").Value.ToLower() == path.ToLower());

                // Then dimensions
                if (xmlNode.Any() == false)
                    xmlNode = node.Elements(prefix + "dimension")
                        .Where(x => x.Attribute("name").Value.ToLower() == path.ToLower());

                if (xmlNode.Any() == false)
                    return string.Empty;
                
                value = xmlNode.First().Attribute(attributeName).Value;
            }

            return value;
        }

        private string GetXmlTopValue(XDocument doc,string path)
        {
            if (doc.Root == null) return string.Empty;

            var prefix = doc.Root.GetNamespaceOfPrefix("dxp");

            var node = doc.Descendants(prefix + path);
            return node.Any() == false ? string.Empty : doc.Descendants(prefix + path).First().Value;
        }
    }
}