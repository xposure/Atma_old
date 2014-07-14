using Microsoft.Xna.Framework;

namespace Atma.Graphics
{
    public struct Viewport
    {
        /// <summary>
        /// Attributes 
        /// </summary>
        private int x;
        private int y;
        private int width;
        private int height;
        private float minDepth;
        private float maxDepth;

        #region Properties
        public int Height
        {
            get
            {
                return this.height;
            }
            set
            {
                height = value;
            }
        }

        public float MaxDepth
        {
            get
            {
                return this.maxDepth;
            }
            set
            {
                maxDepth = value;
            }
        }

        public float MinDepth
        {
            get
            {
                return this.minDepth;
            }
            set
            {
                minDepth = value;
            }
        }

        public int Width
        {
            get
            {
                return this.width;
            }
            set
            {
                width = value;
            }
        }

        public int Y
        {
            get
            {
                return this.y;

            }
            set
            {
                y = value;
            }
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        #endregion

        public float AspectRatio
        {
            get
            {
                if ((height != 0) && (width != 0))
                {
                    return (((float)width) / ((float)height));
                }
                return 0f;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                Rectangle rectangle;
                rectangle.X = x;
                rectangle.Y = y;
                rectangle.Width = width;
                rectangle.Height = height;
                return rectangle;
            }

            set
            {
                x = value.X;
                y = value.Y;
                width = value.Width;
                height = value.Height;
            }
        }

        public Rectangle TitleSafeArea
        {
            get
            {
                return new Rectangle(x, y, width, height);
            }
        }

        public Viewport(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.minDepth = 0.0f;
            this.maxDepth = 1.0f;
        }

        public Viewport(Rectangle bounds)
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height)
        {
        }

        public Vector3 Project(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = Matrix.Multiply(Matrix.Multiply(world, view), projection);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector.X = vector.X / a;
                vector.Y = vector.Y / a;
                vector.Z = vector.Z / a;
            }
            vector.X = (((vector.X + 1f) * 0.5f) * this.width) + this.x;
            vector.Y = (((-vector.Y + 1f) * 0.5f) * this.height) + this.y;
            vector.Z = (vector.Z * (this.maxDepth - this.minDepth)) + this.minDepth;
            return vector;
        }

        public Vector3 Unproject(Vector3 source, Matrix projection, Matrix view, Matrix world)
        {
            Matrix matrix = Matrix.Invert(Matrix.Multiply(Matrix.Multiply(world, view), projection));
            source.X = (((source.X - this.x) / ((float)this.width)) * 2f) - 1f;
            source.Y = -((((source.Y - this.y) / ((float)this.height)) * 2f) - 1f);
            source.Z = (source.Z - this.minDepth) / (this.maxDepth - this.minDepth);
            Vector3 vector = Vector3.Transform(source, matrix);
            float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
            if (!WithinEpsilon(a, 1f))
            {
                vector.X = vector.X / a;
                vector.Y = vector.Y / a;
                vector.Z = vector.Z / a;
            }
            return vector;

        }

        public static implicit operator Microsoft.Xna.Framework.Graphics.Viewport(Viewport a)
        {
            return new Microsoft.Xna.Framework.Graphics.Viewport(a.x, a.y, a.width, a.height);
        }

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
        }

        public override string ToString()
        {
            return "{X:" + x + " Y:" + y + " Width:" + width + " Height:" + height + " MinDepth:" + minDepth + " MaxDepth:" + maxDepth + "}";
        }
    }
}
