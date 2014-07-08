using System.Text;

namespace Atma.Json
{
    public class JsonBool : JsonValue
    {
        public JsonBool()
        {

        }

        public JsonBool(bool value)
        {
            this.Value = value;
        }

        public bool Value = false;
        public override void Write(StringBuilder sb)
        {
            sb.Append(Value ? "true" : "false");
        }

        public override JsonTypes Type { get { return JsonTypes.Bool; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }

        //public static implicit operator JsonBool(bool val)
        //{
        //    if (val)
        //        return new JsonBool(true);
        //    else
        //        return new JsonBool(false);
        //}

        //public static implicit operator bool(JsonBool val)
        //{
        //    return val.Value;
        //}
    }

}
