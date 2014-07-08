using System;
using System.Text;

namespace Atma.Json
{
    public class JsonDate : JsonValue
    {
        private DateTime dt;
        public JsonDate(DateTime dt)
        {
            this.dt = dt;
        }

        public override void Write(StringBuilder sb)
        {
            //sb.AppendFormat("new Date({0})", Math.Floor((double)dt.Ticks / 10000));
            sb.AppendFormat("\"{0}\"", dt.ToString("s", System.Globalization.CultureInfo.InvariantCulture));
        }

        public override JsonTypes Type { get { return JsonTypes.Date; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Write(sb);
            return sb.ToString();
        }
    }
}
