using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataViz
{
    public class JsObject
    {
        Dictionary<string, object> map = new Dictionary<string, object>();
        public JsObject()
        {
        }

        public object this[string key]
        {
            get
            {
                object val = null;
                map.TryGetValue(key, out val);
                return val;
            }

            set
            {
                map[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return map.Count;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return map.Keys;
            }
        }
    }
}