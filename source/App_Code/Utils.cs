using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Reflection;
using System.Collections;

namespace DataViz
{
    public static class Utils
    {
        public static string ConvertToJson(object value)
        {
            var str = value as string;
            if (str != null)
            {
                return "\"" + Utils.EscapeString(str) + "\"";
            }

            var jsobj = value as JsObject;
            if (jsobj != null)
            {
                var sb = new StringBuilder();
                sb.Append("{");
                foreach (var key in jsobj.Keys)
                {
                    sb.AppendFormat("\"{0}\":{1},", key, ConvertToJson(jsobj[key]));
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
                return sb.ToString();
            }

            var map = value as IDictionary;
            if (map != null)
            {
                var sb = new StringBuilder();
                sb.Append("{");
                foreach (DictionaryEntry kv in map)
                {
                    sb.AppendFormat("\"{0}\":{1},", kv.Key, ConvertToJson(kv.Value));
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
                sb.Append("}");
                return sb.ToString();
            }

            var enumerable = value as IEnumerable;
            if (enumerable != null)
            {
                var sb = new StringBuilder();
                sb.Append("[");
                foreach (var e in enumerable)
                {
                    sb.AppendFormat("{0},", ConvertToJson(e));
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
                sb.Append("]");
                return sb.ToString();
            }

            switch (value.GetType().FullName)
            {
                case "System.Int64":
                case "System.Int32":
                case "System.Int16":
                case "System.UInt64":
                case "System.UInt32":
                case "System.UInt16":
                case "System.Boolean":
                case "System.Char":
                    return value.ToString();

                default:
                    return value.ToString();
            }
        }

        public static string EscapeString(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '\\' || c == '"')
                {
                    sb.Append('\\');
                    sb.Append(c);
                }
                else if (c == '\r')
                {
                    sb.Append("\\r");
                }
                else if (c == '\n')
                {
                    sb.Append("\\n");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string GetPostData(HttpRequest request)
        {
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string RenderView(string path, Dictionary<string, object> properties)
        {
            Page pageHolder = new Page();
            UserControl viewControl = (UserControl)pageHolder.LoadControl(path);

            if (properties != null)
            {
                Type viewControlType = viewControl.GetType();
                foreach (var kv in properties)
                {
                    var prop = viewControlType.GetProperty(kv.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null)
                    {
                        prop.SetValue(viewControl, kv.Value, null);
                    }
                }
            }

            pageHolder.Controls.Add(viewControl);

            using (StringWriter output = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pageHolder, output, false);
                return output.ToString();
            }
        }
    }
}