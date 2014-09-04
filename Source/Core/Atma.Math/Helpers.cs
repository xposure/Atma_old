using System.Text;

namespace Atma
{
    public static class Helpers
    {
        public static void EscapeWithStringBuilder(StringBuilder sb, string source)
        {
            var data = source;
            int lastwrite = 0;
            for (int i = 0; i < data.Length; i++)
            {
                var ch = data[i];
                if (ch == '\n' || ch == '\"' || ch == '\\' || ch == '\r' || ch == '\t' || ch == '\f' || ch == '\b')
                {
                    lastwrite = escape(sb, data, lastwrite, i);
                    switch (ch)
                    {
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\b': sb.Append("\\b"); break;
                        case '\t': sb.Append("\\t"); break;
                        default:
                            sb.Append('\\');
                            sb.Append(ch);
                            break;
                    }
                }
            }
            lastwrite = escape(sb, data, lastwrite, data.Length);
        }

        public static string Escape(string source)
        {
            var sb = new StringBuilder();
            EscapeWithStringBuilder(sb, source);
            return sb.ToString();
        }

        private static int escape(StringBuilder sb, string data, int lastwrite, int pos)
        {
            if (pos - lastwrite > 0)
                sb.Append(data, lastwrite, pos - lastwrite);

            return pos + 1;
        }

        public static int NextPow(int x)
        {
            int power = 1;
            while (power < x)
                power <<= 1;

            return power;
        }

        public static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            var p = Vector2.Zero;
            return Intersects(a1, a2, b1, b2, out p);
        }

        // a1 is line1 start, a2 is line1 end, b1 is line2 start, b2 is line2 end
        public static bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            Vector2 b = a2 - a1;
            Vector2 d = b2 - b1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return false;

            Vector2 c = b1 - a1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return false;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return false;

            intersection = a1 + t * b;

            return true;
        }

    }
}