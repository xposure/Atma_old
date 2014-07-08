using System.Text;

namespace Atma.Json
{
    public class JsonNull : JsonValue
    {
        public override void Write(StringBuilder sb)
        {
            sb.Append("null");
        }

        public override JsonTypes Type { get { return JsonTypes.Null; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }
        
    }

}
