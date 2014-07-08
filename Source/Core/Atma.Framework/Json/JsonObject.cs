using System;
using System.Collections.Generic;
using System.Text;

namespace Atma.Json
{
    public class JsonObject : JsonValue, IJsonObject, IEnumerable<KeyValuePair<string, JsonValue>>
    {
        private Dictionary<string, JsonValue> items = new Dictionary<string, JsonValue>();

        public JsonObject()
        {

        }

        public JsonObject(string[] names, object[] data)
        {
            if (names.Length != data.Length)
                throw new ArgumentOutOfRangeException("names|data");

            for (var i = 0; i < names.Length; i++)
                this.Add(names[i], data[i]);
        }

        public int Count { get { return items.Count; } }

        public override void Write(StringBuilder sb)
        {
            sb.Append('{');
            var first = true;
            foreach (var i in items)
            {
                if (!first)
                    sb.Append(',');

                sb.AppendFormat("\"{0}\"", i.Key);
                sb.Append(':');
                i.Value.Write(sb);

                first = false;
            }
            sb.Append('}');
        }

        public void Add(string field, object value)
        {
            if (value != null && value is JsonValue)
            {
                if (items.ContainsKey(field))
                    items[field] = (JsonValue)value;
                else
                    items.Add(field, (JsonValue)value);
            }
            else
            {
                if (items.ContainsKey(field))
                    items[field] = JsonValue.GetValue(value);
                else
                    items.Add(field, JsonValue.GetValue(value));
            }
        }

        public void Clear()
        {
            items.Clear();
        }

        public T get<T>(string name)
            where T: JsonValue
        {
            var value = this[name];
            if (value.Type == JsonTypes.Null)
                return null;

            return (T)value;
        }       

        public JsonValue this[string key]
        {
            get
            {
                if (!items.ContainsKey(key))
                    return new JsonNull();

                return items[key];
            }
            set
            {
                if (!items.ContainsKey(key))
                    items.Add(key, value is JsonValue ? value :  new JsonNull());
                else
                    items[key] = value is JsonValue ? value : new JsonNull();
            }
        }

        public override JsonTypes Type { get { return JsonTypes.Object; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }

        public IEnumerable<string> Keys
        {
            get
            {
                foreach (var key in items.Keys)
                    yield return key;
            }
        }

        public IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

}
