using System;
using System.Collections.Generic;
using System.Text;

namespace Atma.Json
{
    public class JsonArray : JsonValue, IJsonArray, IEnumerable<JsonValue>
    {
        private List<JsonValue> items = new List<JsonValue>();

        public JsonArray()
        {

        }

        public JsonArray(params JsonValue[] values)
        {
            if (values != null && values.Length > 0)
            {
                foreach (var v in values)
                    this.Add(v);
            }
        }

        public override void Write(StringBuilder sb)
        {
            sb.Append('[');
            var first = true;
            foreach (var i in items)
            {
                if (!first)
                    sb.Append(',');

                i.Write(sb);

                first = false;
            }
            sb.Append(']');
        }

        public int Count { get { return items.Count; } }

        public void Add(object value)
        {
            if (value is JsonValue)
                items.Add((JsonValue)value);
            else
                items.Add(JsonValue.GetValue(value));
            
            //items.Add(new JsonInstance(value));
        }

        public JsonValue this[int index]
        {
            get
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index");
                return items[index];
            }
            set
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException("index");
                items[index] = value;
            }
        }

        public override JsonTypes Type { get { return JsonTypes.Array; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }

        public IEnumerator<JsonValue> GetEnumerator()
        {
            foreach (var item in items)
                yield return item;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var item in items)
                yield return item;
        }

        //public static implicit operator Vector2(JsonArray ja)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 2)
        //        throw new InvalidCastException("expected an array with 2 elements to convert to vector2");

        //    var x = ja[0];
        //    var y = ja[1];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector2(((JsonNumber)x).Number, ((JsonNumber)y).Number);
        //}

        //public static implicit operator Vector3(JsonArray ja)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 3)
        //        throw new InvalidCastException("expected an array with 3 elements to convert to vector3");

        //    var x = ja[0];
        //    var y = ja[1];
        //    var z = ja[2];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector3(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number);
        //}

        //public static implicit operator Vector4(JsonArray ja)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 4)
        //        throw new InvalidCastException("expected an array with 4 elements to convert to vector4");

        //    var x = ja[0];
        //    var y = ja[1];
        //    var z = ja[2];
        //    var w = ja[3];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number || w.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector4(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number, ((JsonNumber)w).Number);
        //}

        //public static implicit operator JsonArray(Vector2 val)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 2)
        //        throw new InvalidCastException("expected an array with 2 elements to convert to vector2");

        //    var x = ja[0];
        //    var y = ja[1];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector2(((JsonNumber)x).Number, ((JsonNumber)y).Number);
        //}

        //public static implicit operator JsonArray(Vector3 val)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 3)
        //        throw new InvalidCastException("expected an array with 3 elements to convert to vector3");

        //    var x = ja[0];
        //    var y = ja[1];
        //    var z = ja[2];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector3(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number);
        //}

        //public static implicit operator JsonArray(Vector4 val)
        //{
        //    //if (val == null)
        //    //    throw new NullReferenceException("val");

        //    //if (val.Type == JsonTypes.Null)
        //    //    throw new InvalidCastException("can not convert a null value to an vector2");

        //    //if (val.Type != JsonTypes.Array)
        //    //    throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

        //    //var ja = (JsonArray)val;
        //    if (ja.Count != 4)
        //        throw new InvalidCastException("expected an array with 4 elements to convert to vector4");

        //    var x = ja[0];
        //    var y = ja[1];
        //    var z = ja[2];
        //    var w = ja[3];

        //    if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number || w.Type != JsonTypes.Number)
        //        throw new InvalidCastException("array elements have to be of type number to convert to vector");

        //    return new Vector4(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number, ((JsonNumber)w).Number);
        //}

    }

}
