using System.Text;

namespace Atma.Json
{
    public class JsonString : JsonValue
    {
        public string Data { get; set; }
        public JsonString(string data)
        {
            this.Data = data;
        }

        public override void Write(StringBuilder sb)
        {
            sb.Append('\"');
            Helpers.EscapeWithStringBuilder(sb, Data);
            sb.Append('\"');
        }

        public override JsonTypes Type { get { return JsonTypes.String; } }
 
        public override string ToString()
        {
            return Helpers.Escape(Data);
        }
    }

}
