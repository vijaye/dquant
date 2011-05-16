using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace DataViz
{
    public static class ODataUtils
    {
        public static List<Dictionary<string, object>> GetRows(string url, int start, int length)
        {
            var list = new List<Dictionary<string, object>>();
            try
            {
                using (WebClient wc = new WebClient())
                {
                    string content = wc.DownloadString(string.Format("{0}?$skip={1}&$top={2}", url, start, length));

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(content);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("d", doc.DocumentElement.NamespaceURI);

                    var entries = doc.SelectNodes("d:feed/d:entry", nsmgr);
                    foreach (XmlNode entry in entries) 
                    {
                        list.Add(ConvertToMap(entry));
                    }
                }
            }
            catch (Exception e)
            {
                list.Add(new Dictionary<string,object> { {"error", e.Message} });
            }

            return list;
        }

        static Dictionary<string, object> ConvertToMap(XmlNode entry)
        {
            var ret = new Dictionary<string, object>();
            foreach (XmlNode node in entry.ChildNodes)
            {
                ret[node.Name] = node.InnerXml;
            }

            return ret;
        }
    }
}