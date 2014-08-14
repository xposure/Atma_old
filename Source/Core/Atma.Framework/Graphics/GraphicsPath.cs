using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Graphics
{
    public class GraphicsPath
    {
        private List<Vector2> _points = new List<Vector2>();

        public int count { get { return _points.Count; } }

        public Vector2 this[int index]
        {
            get
            {
                if(index < 0)
                    index += _points.Count;

                if (index >= 0 && index < _points.Count)
                {
                    return _points[index];
                }

                return Vector2.Zero;
            }
        }

        public void clear()
        {
            _points.Clear();
        }


        public void AddLine(Vector2 p0, Vector2 p1)
        {
            _points.Add(p0);
            _points.Add(p1);
        }

        public void AddLine(float x0, float y0, float x1, float y1)
        {
            _points.Add(new Vector2(x0, y0));
            _points.Add(new Vector2(x1, y1));
        }

        public void AddPoint(Vector2 p)
        {
            _points.Add(p);
        }

        public void AddPoint(float x, float y)
        {
            _points.Add(new Vector2(x, y));
        }

        public void AddArc(float x0, float y0, float radius, int startAngle, int sweepAngle)
        {
            const int segments = 5;
            var step = Utility.DegreesToRadians(sweepAngle) / segments;
            var sweep = Utility.DegreesToRadians(startAngle) + step;
            var position = new Vector2(x0, y0);
            for (var i = 0; i < segments; i++)
            {
                var direction = new Vector2(Utility.Cos(sweep), Utility.Sin(sweep));
                _points.Add(direction * radius + position);
                sweep += step;
            }
        }

        public void AddArc(Vector2 position, float radius, float startAngle, int segments)
        {
            var step = MathHelper.PiOver2 / segments;
            var sweep = startAngle + step;
            for (var i = 0; i < segments; i++)
            {
                var direction = new Vector2(Utility.Cos(sweep), Utility.Sin(sweep));
                _points.Add(direction * radius + position);
                sweep += step;
            }
        }
    }
}
