using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace Atma.Json
{
    public abstract class JsonValue : IJsonWriter
    {
        public abstract JsonTypes Type { get; }

        public abstract void Write(StringBuilder sb);

        public static JsonValue GetValue(object value)
        {
            if (value == null)
                return new JsonNull();
            else if (value is JsonValue)
                return (JsonValue)value;
            else if (value is string)
                return new JsonString((string)value);
            else if (value is bool)
            {
                if ((bool)value)
                    return new JsonBool(true);
                else
                    return new JsonBool(false);
            }
            else if (value is DateTime)
                return new JsonDate((DateTime)value);
            else
            {
                var data = value.ToString();
                var state = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    var ch = data[i];
                    switch (state)
                    {
                        case 0:
                            if (char.IsDigit(ch))
                            {
                                state = 1;
                                continue;
                            }
                            else if (i != data.Length - 1 && (ch == '-' || ch == '+') && char.IsDigit(data[i + 1]))
                            {
                                state = 1;
                                i++;
                                continue;
                            }

                            state = -1;
                            break;
                        case 1:
                            if (char.IsDigit(ch))
                                continue;
                            else if (i != data.Length - 1 && ch == '.' && char.IsDigit(data[i + 1]))
                            {
                                state = 2;
                                i++;
                                continue;
                            }

                            state = -1;
                            break;
                        case 2:
                            if (char.IsDigit(ch))
                                continue;
                            else if (i != data.Length - 1 && (ch == 'e' || ch == 'E'))
                            {
                                state = 3;
                                continue;
                            }
                            state = -1;
                            break;
                        case 3:
                            if (char.IsDigit(ch))
                            {
                                state = 4;
                                continue;
                            }
                            else if (i != data.Length - 1 && (ch == '-' || ch == '+') && char.IsDigit(data[i + 1]))
                            {
                                state = 4;
                                i++;
                                continue;
                            }
                            state = -1;
                            break;
                        case 4:
                            if (char.IsDigit(ch))
                                continue;
                            state = -1;
                            break;
                    }

                    if (state == -1)
                        break;
                }
                
                if (state == -1)
                    return new JsonString(data);
                else
                    return new JsonNumber(System.Convert.ToSingle(data));
            }

            throw new Exception("type " + value.GetType().Name + " is not supported in json");
        }

        //public static explicit operator JsonArray(JsonValue val)
        //{
        //    if (val == null)
        //        throw new NullReferenceException("val");

        //    if (val.Type == JsonTypes.Null)
        //        throw new InvalidCastException("can not convert a null value to an int");

        //    if (val.Type != JsonTypes.Array)
        //        throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "array"));

        //    return (JsonArray)val;
        //}

        //public static implicit operator JsonObject(JsonValue val)
        //{
        //    if (val == null)
        //        throw new NullReferenceException("val");

        //    if (val.Type == JsonTypes.Null)
        //        throw new InvalidCastException("can not convert a null value to an int");

        //    if (val.Type != JsonTypes.Object)
        //        throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "object"));

        //    return (JsonArray)val;
        //}

        #region Int Conversion
        public static implicit operator JsonValue(int val)
        {
            return new JsonNumber(val);
        }

        public static implicit operator int(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an int");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "int"));

            return (int)((JsonNumber)val).Number;
        }
        #endregion

        public static implicit operator JsonValue(string val)
        {
            return new JsonString(val);
        }

        public static implicit operator JsonValue(DateTime val)
        {
            return new JsonDate(val);
        }

        public static implicit operator float(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an float");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "float"));

            return (float)((JsonNumber)val).Number;
        }

        public static implicit operator JsonValue(float val)
        {
            return new JsonNumber(val);
        }

        //public static implicit operator JsonValue(double val)
        //{
        //    return new JsonNumber(val);
        //}

        public static implicit operator JsonValue(long val)
        {
            return new JsonNumber(val);
        }

        public static implicit operator JsonValue(bool val)
        {
            if (val)
                return new JsonBool(true);
            else
                return new JsonBool(false);
        }

        //public static implicit operator JsonValue(ulong val)
        //{
        //    return new JsonNumber(val);
        //}

        public static implicit operator string(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.String)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "string"));

            return ((JsonString)val).Data;
        }

        #region ValueTypes

        public static explicit operator bool(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an bool");

            if (val.Type == JsonTypes.Bool)
                return ((JsonBool)val).Value;

            throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "bool"));
        }

        public static explicit operator byte(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an byte");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "byte"));

            return Convert.ToByte(((JsonNumber)val).Number);
        }

        public static explicit operator short(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an short");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "short"));

            return Convert.ToInt16(((JsonNumber)val).Number);
        }

        public static explicit operator long(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an long");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "long"));

            return Convert.ToInt64(((JsonNumber)val).Number);
        }

        public static explicit operator sbyte(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an sbyte");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "sbyte"));

            return Convert.ToSByte(((JsonNumber)val).Number);
        }

        public static explicit operator ushort(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an ushort");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "ushort"));

            return Convert.ToUInt16(((JsonNumber)val).Number);
        }

        public static explicit operator uint(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an uint");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "uint"));

            return Convert.ToUInt32(((JsonNumber)val).Number);
        }

        public static explicit operator ulong(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an ulong");

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "ulong"));

            return Convert.ToUInt64(((JsonNumber)val).Number);
        }

        //public static explicit operator double(JsonValue val)
        //{
        //    if (val == null)
        //        throw new NullReferenceException("val");

        //    if (val.Type == JsonTypes.Null)
        //        throw new InvalidCastException("can not convert a null value to an double");

        //    if (val.Type != JsonTypes.Number)
        //        throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "double"));

        //    return Convert.ToDouble(((JsonNumber)val).Number);
        //}

        public static explicit operator Vector2(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an vector2");

            if (val.Type != JsonTypes.Array)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

            var ja = (JsonArray)val;
            if (ja.Count != 2)
                throw new InvalidCastException("expected an array with 2 elements to convert to vector2");

            var x = ja[0];
            var y = ja[1];

            if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number)
                throw new InvalidCastException("array elements have to be of type number to convert to vector");

            return new Vector2(((JsonNumber)x).Number, ((JsonNumber)y).Number);
        }

        public static explicit operator Vector3(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an vector2");

            if (val.Type != JsonTypes.Array)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

            var ja = (JsonArray)val;
            if (ja.Count != 3)
                throw new InvalidCastException("expected an array with 3 elements to convert to vector3");

            var x = ja[0];
            var y = ja[1];
            var z = ja[2];

            if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number)
                throw new InvalidCastException("array elements have to be of type number to convert to vector");

            return new Vector3(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number);
        }

        public static explicit operator Vector4(JsonValue val)
        {
            if (val == null)
                throw new NullReferenceException("val");

            if (val.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an vector2");

            if (val.Type != JsonTypes.Array)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}, expected json array", val.Type.ToString().ToLower(), "vector2"));

            var ja = (JsonArray)val;
            if (ja.Count != 4)
                throw new InvalidCastException("expected an array with 4 elements to convert to vector4");

            var x = ja[0];
            var y = ja[1];
            var z = ja[2];
            var w = ja[3];

            if (x.Type != JsonTypes.Number || y.Type != JsonTypes.Number || z.Type != JsonTypes.Number || w.Type != JsonTypes.Number)
                throw new InvalidCastException("array elements have to be of type number to convert to vector");

            return new Vector4(((JsonNumber)x).Number, ((JsonNumber)y).Number, ((JsonNumber)z).Number, ((JsonNumber)w).Number);
        }
        #endregion

        #region Nullable ValueTypes
        public static explicit operator bool?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type == JsonTypes.Bool)
                return ((JsonBool)val).Value;

            throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "bool?"));
        }

        public static explicit operator byte?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "byte?"));

            return Convert.ToByte(((JsonNumber)val).Number);
        }

        public static explicit operator short?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "short?"));

            return Convert.ToInt16(((JsonNumber)val).Number);
        }

        public static explicit operator int?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "int?"));

            return Convert.ToInt32(((JsonNumber)val).Number);
        }

        public static explicit operator long?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "long?"));

            return Convert.ToInt64(((JsonNumber)val).Number);
        }

        public static explicit operator sbyte?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "sbyte?"));

            return Convert.ToSByte(((JsonNumber)val).Number);
        }

        public static explicit operator ushort?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "ushort?"));

            return Convert.ToUInt16(((JsonNumber)val).Number);
        }

        public static explicit operator uint?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "uint?"));

            return Convert.ToUInt32(((JsonNumber)val).Number);
        }

        public static explicit operator ulong?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "ulong?"));

            return Convert.ToUInt64(((JsonNumber)val).Number);
        }

        public static explicit operator float?(JsonValue val)
        {
            if (val == null || val.Type == JsonTypes.Null)
                return null;

            if (val.Type != JsonTypes.Number)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "float?"));

            return Convert.ToSingle(((JsonNumber)val).Number);
        }

        //public static explicit operator double?(JsonValue val)
        //{
        //    if (val == null || val.Type == JsonTypes.Null)
        //        return null;

        //    if (val.Type != JsonTypes.Number)
        //        throw new InvalidCastException(string.Format("can not convert type {0} to {1}", val.Type.ToString().ToLower(), "double?"));

        //    return Convert.ToDouble(((JsonNumber)val).Number);
        //}
        #endregion

        public JsonObject ToObject()
        {
            if (this.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an int");

            if (this.Type != JsonTypes.Object)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", this.Type.ToString().ToLower(), "object"));

            return (JsonObject)this;
        }

        public JsonArray ToArray()
        {
            if (this.Type == JsonTypes.Null)
                throw new InvalidCastException("can not convert a null value to an int");

            if (this.Type != JsonTypes.Array)
                throw new InvalidCastException(string.Format("can not convert type {0} to {1}", this.Type.ToString().ToLower(), "array"));

            return (JsonArray)this;
        }
    }
}
