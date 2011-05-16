using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace DataViz
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AjaxMethodAttribute : Attribute
    {
    }

    public class AjaxPage : Page
    {
        public AjaxPage()
        {
        }

        string InvokeMethod(PostRequest pr)
        {
            var type = this.GetType();
            var mi = type.GetMethod(pr.MethodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (mi == null || mi.GetCustomAttributes(typeof(AjaxMethodAttribute), true).Length == 0)
                return "{\"error\": \"Method not found.\"}";

            try
            {
                var pis = mi.GetParameters();
                var args = new List<object>();
                if (pr.Arguments.Count != pis.Length) 
                    return "{\"error\": \"Parameter count mismatch.\"}";
                
                for (int i = 0; i < pis.Length; i++)
                {
                    var pi = pis[i];
                    var arg = pr.Arguments[i];

                    var converter = TypeDescriptor.GetConverter(pi.ParameterType);
                    if (converter != null)
                        args.Add(converter.ConvertFromString(arg));
                    else
                        args.Add(arg);
                }
                object ret = mi.Invoke(this, args.ToArray());
                return string.Format("{{ \"result\": {0} }}", Utils.ConvertToJson(ret));
            }
            catch (Exception e)
            {
                var ret = new Dictionary<string, object>();
                ret["error"] = e.Message;
                ret["stackTrace"] = e.StackTrace;
                return Utils.ConvertToJson(ret);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.Clear();
            if (Request.RequestType.ToLowerInvariant() != "post")
                return;

            string postData = Utils.GetPostData(Request);
            var pr = ParsePostData(postData);
            if (pr.MethodName != null)
            {
                Response.Output.WriteLine(InvokeMethod(pr));
            }
            else
            {
                Response.Output.WriteLine("{\"error\": \"Method not specified.\"}");
            }
            base.OnLoad(e);
        }

        PostRequest ParsePostData(string postData)
        {
            int pos = 0;
            if (!ReadDelimiter(postData, '{', ref pos))
                return null;

            PostRequest request = new PostRequest();
            while (pos < postData.Length)
            {
                string key = ReadString(postData, ref pos);
                if (!ReadDelimiter(postData, ':', ref pos))
                    break;

                object value = ReadValue(postData, ref pos);

                if (key == "method")
                    request.MethodName = value as string;
                else if (key == "arguments")
                    request.Arguments.AddRange(value as List<string>);
                ReadDelimiter(postData, ',', ref pos);

                if (pos < postData.Length && postData[pos] == '}')
                    break;
            }

            return request;
        }

        object ReadValue(string source, ref int pos)
        {
            SkipWhitespace(source, ref pos);
            char c = source[pos];
            if (c == '[')
                return ReadArray(source, ref pos);
            else if (c == '"' || c == '\'')
                return ReadString(source, ref pos);
            else if (char.IsDigit(c) || c == '-' || c == '+' || c == '.')
                return ReadNumber(source, ref pos);
            else
                return null;
        }

        List<string> ReadArray(string source, ref int pos)
        {
            SkipWhitespace(source, ref pos);
            if (source[pos] != '[') { return null; }

            List<string> ret = new List<string>();
            pos++;
            while (pos < source.Length)
            {
                SkipWhitespace(source, ref pos);
                char c = source[pos];
                if (c == ']')
                {
                    pos++;
                    break;
                } 
                else if (char.IsDigit(c) || c == '-' || c == '.' || c == '+')
                {
                    ret.Add(ReadNumber(source, ref pos));
                }
                else if (c == '"' || c == '\'')
                {
                    ret.Add(ReadString(source, ref pos));
                }
                else
                {
                    bool delim = ReadDelimiter(source, ',', ref pos);
                    if (!delim && source[pos] != ']')
                        break;
                }
            }

            return ret;
        }

        string ReadNumber(string source, ref int pos)
        {
            SkipWhitespace(source, ref pos);

            var sb = new StringBuilder();
            while (pos < source.Length)
            {
                char c = source[pos];
                if (char.IsDigit(c) || c == '-' || c == '.' || c == '+')
                    sb.Append(c);
                else
                    break;
                
                pos++;
            }

            return sb.ToString();
        }

        string ReadString(string source, ref int pos)
        {
            SkipWhitespace(source, ref pos);

            char start = source[pos];
            if (start != '"' && start != '\'') { return null; }

            pos++;
            var sb = new StringBuilder();
            while (pos < source.Length)
            {
                char c = source[pos];
                if (c == '\\')
                {
                    pos++;
                    if (pos < source.Length)
                    {
                        sb.Append(source[pos]);
                    }
                }
                else if (c == start)
                {
                    pos++;
                    break;
                }
                else
                {
                    pos++;
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        bool ReadDelimiter(string source, char delim, ref int pos)
        {
            SkipWhitespace(source, ref pos);
            if (source[pos] == delim)
            {
                pos++;
                return true;
            }

            return false;
        }

        void SkipWhitespace(string source, ref int pos)
        {
            while (pos < source.Length)
            {
                char c = source[pos];
                if (!char.IsWhiteSpace(c))
                    return;
                pos++;
            }
        }
    }

    public class PostRequest
    {
        List<string> arguments = new List<string>();
        public string MethodName { get; set; }
        public List<string> Arguments
        {
            get { return arguments; }
        }
    }
}