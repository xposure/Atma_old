using System;
using System.Globalization;
using System.Text;

namespace Atma.Json
{
    /// <summary>
    /// This class encodes and decodes JSON strings.
    /// Spec. details, see http://www.json.org/
    /// 
    /// JSON uses Arrays and Objects. These correspond here to the datatypes ArrayList and Hashtable.
    /// All numbers are parsed to doubles.
    /// </summary>
    public class JsonReader
    {
        public const int TOKEN_NONE = 0;
        public const int TOKEN_CURLY_OPEN = 1;
        public const int TOKEN_CURLY_CLOSE = 2;
        public const int TOKEN_SQUARED_OPEN = 3;
        public const int TOKEN_SQUARED_CLOSE = 4;
        public const int TOKEN_COLON = 5;
        public const int TOKEN_COMMA = 6;
        public const int TOKEN_STRING = 7;
        public const int TOKEN_NUMBER = 8;
        public const int TOKEN_TRUE = 9;
        public const int TOKEN_FALSE = 10;
        public const int TOKEN_NULL = 11;

        private const int BUILDER_CAPACITY = 2000;

        public static JsonValue Parse(string json)
        {
            var success = true;
            var _jv = JsonDecode(json, ref success) as JsonValue;

            if (!success)
                return null;

            return _jv;
        }

        public static JsonObject ParseObject(string json)
        {
            var success = true;
            return ParseObject(json, ref success);
        }

        public static JsonObject ParseObject(string json, ref bool success)
        {
            var _jv = JsonDecode(json, ref success);

            if (_jv != null && _jv is JsonObject)
                return (JsonObject)_jv;
           
            return null;
        }

        //public static JsonArray ParseArray(string json)
        //{
        //    var success = true;
        //    return ParseArray(json, ref success);
        //}

        //public static JsonArray ParseArray(string json, ref bool success)
        //{
        //    var _jv = JsonDecode(json, ref success);

        //    if (_jv is JsonInstance)
        //    {
        //        var jv = (JsonInstance)_jv;
        //        if (jv.Value is JsonArray)
        //            return (JsonArray)jv.Value;
        //    }

        //    return null;
        //}

        //public static JsonValue2 ParseValue(string json)
        //{
        //    var success = true;
        //    return ParseValue(json, ref success);
        //}

        //public static JsonValue2 ParseValue(string json, ref bool success)
        //{
        //    var _jv = JsonDecode(json, ref success);

        //    if (_jv is JsonValue2)
        //    {
        //        var jv = (JsonInstance)_jv;
        //        if (jv.Value is JsonInstance)
        //            return (JsonInstance)jv.Value;
        //    }

        //    return null;
        //}

        /// <summary>
        /// Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        private static object JsonDecode(string json)
        {
            bool success = true;
            return JsonDecode(json, ref success);
        }

        /// <summary>
        /// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <param name="success">Successful parse?</param>
        /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
        private static object JsonDecode(string json, ref bool success)
        {
            success = true;
            if (json != null)
            {
                char[] charArray = json.ToCharArray();
                int index = 0;
                object value = ParseValue(charArray, ref index, ref success);
                return value;
            }
            else
            {
                return null;
            }
        }

        protected static JsonObject ParseObject(char[] json, ref int index, ref bool success)
        {
            //Hashtable table = new Hashtable();
            var jo = new JsonObject();
            int token;

            // {
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                token = LookAhead(json, index);
                if (token == JsonReader.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JsonReader.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JsonReader.TOKEN_CURLY_CLOSE)
                {
                    NextToken(json, ref index);
                    return jo;
                }
                else
                {

                    // name
                    var name = ParseString(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    // :
                    token = NextToken(json, ref index);
                    if (token != JsonReader.TOKEN_COLON)
                    {
                        success = false;
                        return null;
                    }

                    // value
                    var value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    jo[name.Data] = value;
                }
            }

            return jo;
        }

        protected static JsonArray ParseArray(char[] json, ref int index, ref bool success)
        {
            var array = new JsonArray();

            // [
            NextToken(json, ref index);

            bool done = false;
            while (!done)
            {
                int token = LookAhead(json, index);
                if (token == JsonReader.TOKEN_NONE)
                {
                    success = false;
                    return null;
                }
                else if (token == JsonReader.TOKEN_COMMA)
                {
                    NextToken(json, ref index);
                }
                else if (token == JsonReader.TOKEN_SQUARED_CLOSE)
                {
                    NextToken(json, ref index);
                    break;
                }
                else
                {
                    object value = ParseValue(json, ref index, ref success);
                    if (!success)
                    {
                        return null;
                    }

                    array.Add(value);
                }
            }

            return array;
        }

        protected static JsonValue ParseValue(char[] json, ref int index, ref bool success)
        {
            switch (LookAhead(json, index))
            {
                case JsonReader.TOKEN_STRING:
                    return ParseString(json, ref index, ref success);
                case JsonReader.TOKEN_NUMBER:
                    return ParseNumber(json, ref index, ref success);
                case JsonReader.TOKEN_CURLY_OPEN:
                    return ParseObject(json, ref index, ref success);
                case JsonReader.TOKEN_SQUARED_OPEN:
                    return ParseArray(json, ref index, ref success);
                case JsonReader.TOKEN_TRUE:
                    NextToken(json, ref index);
                    return new JsonBool(true);
                case JsonReader.TOKEN_FALSE:
                    NextToken(json, ref index);
                    return new JsonBool(false);
                case JsonReader.TOKEN_NULL:
                    NextToken(json, ref index);
                    return new JsonNull();
                case JsonReader.TOKEN_NONE:
                    break;
            }

            success = false;
            return new JsonNull();
        }

        protected static JsonString ParseString(char[] json, ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
            char c;

            EatWhitespace(json, ref index);

            // "
            c = json[index++];

            bool complete = false;
            while (!complete)
            {

                if (index == json.Length)
                {
                    break;
                }

                c = json[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {

                    if (index == json.Length)
                    {
                        break;
                    }
                    c = json[index++];
                    if (c == '"')
                    {
                        s.Append('"');
                    }
                    else if (c == '\\')
                    {
                        s.Append('\\');
                    }
                    else if (c == '/')
                    {
                        s.Append('/');
                    }
                    else if (c == 'b')
                    {
                        s.Append('\b');
                    }
                    else if (c == 'f')
                    {
                        s.Append('\f');
                    }
                    else if (c == 'n')
                    {
                        s.Append('\n');
                    }
                    else if (c == 'r')
                    {
                        s.Append('\r');
                    }
                    else if (c == 't')
                    {
                        s.Append('\t');
                    }
                    else if (c == 'u')
                    {
                        int remainingLength = json.Length - index;
                        if (remainingLength >= 4)
                        {
                            // parse the 32 bit hex into an integer codepoint
                            uint codePoint;
                            if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint)))
                            {
                                return new JsonString(string.Empty);
                            }
                            // convert the integer codepoint to a unicode char and add to string
                            s.Append(Char.ConvertFromUtf32((int)codePoint));
                            // skip 4 chars
                            index += 4;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    s.Append(c);
                }

            }

            if (!complete)
            {
                success = false;
                return null;
            }

            return new JsonString(s.ToString());
        }

        protected static JsonNumber ParseNumber(char[] json, ref int index, ref bool success)
        {
            EatWhitespace(json, ref index);

            int lastIndex = GetLastIndexOfNumber(json, index);
            int charLength = (lastIndex - index) + 1;

            float number;
            success = float.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);

            index = lastIndex + 1;
            return new JsonNumber(number);
        }

        protected static int GetLastIndexOfNumber(char[] json, int index)
        {
            int lastIndex;

            for (lastIndex = index; lastIndex < json.Length; lastIndex++)
            {
                if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
                {
                    break;
                }
            }
            return lastIndex - 1;
        }

        protected static void EatWhitespace(char[] json, ref int index)
        {
            for (; index < json.Length; index++)
            {
                if (" \t\n\r".IndexOf(json[index]) == -1)
                {
                    break;
                }
            }
        }

        protected static int LookAhead(char[] json, int index)
        {
            int saveIndex = index;
            return NextToken(json, ref saveIndex);
        }

        protected static int NextToken(char[] json, ref int index)
        {
            EatWhitespace(json, ref index);

            if (index == json.Length)
            {
                return JsonReader.TOKEN_NONE;
            }

            char c = json[index];
            index++;
            switch (c)
            {
                case '{':
                    return JsonReader.TOKEN_CURLY_OPEN;
                case '}':
                    return JsonReader.TOKEN_CURLY_CLOSE;
                case '[':
                    return JsonReader.TOKEN_SQUARED_OPEN;
                case ']':
                    return JsonReader.TOKEN_SQUARED_CLOSE;
                case ',':
                    return JsonReader.TOKEN_COMMA;
                case '"':
                    return JsonReader.TOKEN_STRING;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return JsonReader.TOKEN_NUMBER;
                case ':':
                    return JsonReader.TOKEN_COLON;
            }
            index--;

            int remainingLength = json.Length - index;

            // false
            if (remainingLength >= 5)
            {
                if (json[index] == 'f' &&
                    json[index + 1] == 'a' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 's' &&
                    json[index + 4] == 'e')
                {
                    index += 5;
                    return JsonReader.TOKEN_FALSE;
                }
            }

            // true
            if (remainingLength >= 4)
            {
                if (json[index] == 't' &&
                    json[index + 1] == 'r' &&
                    json[index + 2] == 'u' &&
                    json[index + 3] == 'e')
                {
                    index += 4;
                    return JsonReader.TOKEN_TRUE;
                }
            }

            // null
            if (remainingLength >= 4)
            {
                if (json[index] == 'n' &&
                    json[index + 1] == 'u' &&
                    json[index + 2] == 'l' &&
                    json[index + 3] == 'l')
                {
                    index += 4;
                    return JsonReader.TOKEN_NULL;
                }
            }

            return JsonReader.TOKEN_NONE;
        }

        /// <summary>
        /// Determines if a given object is numeric in any way
        /// (can be integer, double, null, etc). 
        /// 
        /// Thanks to mtighe for pointing out Double.TryParse to me.
        /// </summary>
        protected static bool IsNumeric(object o)
        {
            double result;

            return (o == null) ? false : Double.TryParse(o.ToString(), out result);
        }
    }
}
