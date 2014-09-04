using System;
using System.Collections.Generic;

namespace Atma
{
    public static class zSpriteExtensions
    {
        public static bool CheckFlags(this int src, int flags)
        {
            return (src & flags) == flags;
        }

        #region float

        public static float Lerp(this float a, float b, float n)
        {
            return (a * (1 - n) + b * n);
        }

        #endregion float

        #region Vector2

        public static Vector2 Ceiling(this Vector2 a)
        {
            a.X = MathHelper.Ceiling(a.X);
            a.Y = MathHelper.Ceiling(a.Y);
            return a;
        }

        public static float Dot(this Vector2 a, Vector2 b)
        {
            return Vector2.Dot(a, b);
        }

        public static Vector2 Floor(this Vector2 a)
        {
            a.X = MathHelper.Ceiling(a.X) - 1;
            a.Y = MathHelper.Ceiling(a.Y) - 1;
            return a;
        }

        public static float GetRotation(this Vector2 src, Vector2 dst)
        {
            if (src == dst)
                return (float)Math.Atan2(0, 0);

            var diff = (dst - src).ToNormalized();
            return (float)Math.Atan2(diff.Y, diff.X);
        }

        public static float GetRotation(this Vector2 src)
        {
            var diff = src.ToNormalized();
            return (float)Math.Atan2(diff.Y, diff.X);
        }

        public static Vector2 Lerp(this Vector2 a, Vector2 b, float t)
        {
            return new Vector2(MathHelper.Lerp(a.X, b.X, t), MathHelper.Lerp(a.Y, b.Y, t));
        }

        public static Vector2 Perp(this Vector2 p)
        {
            return new Vector2(p.Y, -p.X);
        }

        public static Vector2 Rotate(this Vector2 src, Vector2 pivot, Real amount)
        {
            if (amount == 0)
                return src;

            var rot = pivot.GetRotation(src);
            rot += amount;

            var dir = new Vector2(MathHelper.Cos(rot), MathHelper.Sin(rot));
            return dir * (src - pivot).Length + pivot;
        }

        public static AxisAlignedBox ToAABB(this Vector2 pos, Vector2 size)
        {
            return new AxisAlignedBox(pos, pos + size);
        }

        public static AxisAlignedBox ToAABB(this Vector2 pos, float size)
        {
            return new AxisAlignedBox(pos, pos + new Vector2(size, size));
        }

        public static AxisAlignedBox ToAABB(this Vector2 pos, int size)
        {
            return new AxisAlignedBox(pos, pos + new Vector2(size, size));
        }

        public static AxisAlignedBox ToAABB(this Vector2 pos, float width, float height)
        {
            return new AxisAlignedBox(pos, pos + new Vector2(width, height));
        }

        public static AxisAlignedBox ToAABB(this Vector2 pos, int width, int height)
        {
            return new AxisAlignedBox(pos, pos + new Vector2(width, height));
        }

        public static Vector2 ToNormalized(this Vector2 v)
        {
            if (v == Vector2.Zero)
                return Vector2.Zero;
            v.Normalize();
            return v;
        }

        public static float ToAngle(this Vector2 pos)
        {
            return MathHelper.ATan2(pos.Y, pos.X);
        }

        public static Point ToPoint(this Vector2 pos)
        {
            //return new Point((int)pos.X, (int)pos.Y);
            return new Point((int)Math.Floor(pos.X), (int)Math.Floor(pos.Y));
        }

        public static Rectangle ToRectangle(this Vector2 pos, float x, float y)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, (int)x, (int)y);
        }

        public static Rectangle ToRectangle(this Vector2 pos, int w, int h)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, w, h);
        }

        public static Rectangle ToRectangle(this Vector2 pos, Vector2 size)
        {
            return new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        }

        #endregion Vector2

        #region Vector3

        public static Vector3 Lerp(this Vector3 a, Vector3 b, float t)
        {
            return new Vector3(MathHelper.Lerp(a.X, b.X, t), MathHelper.Lerp(a.Y, b.Y, t), MathHelper.Lerp(a.Z, b.Z, t));
        }

        public static Vector3 ToNormalized(this Vector3 v)
        {
            v.Normalize();
            return v;
        }

        public static Vector3 Cross(this Vector3 a, Vector3 b)
        {
            return a.Cross(b); // Vector3.Cross(a, b);
        }

        public static float Dot(this Vector3 a, Vector3 b)
        {
            return a.Dot(b);// Vector3.Dot(a, b);
        }

        public static float AbsDot(this Vector3 a, Vector3 b)
        {
            return (float)Math.Abs(a.Dot(b));
        }

        public static Vector3 Floor2(this Vector3 a, Vector3 compare)
        {
            if (compare.X < a.X)
            {
                a.X = compare.X;
            }
            if (compare.Y < a.Y)
            {
                a.Y = compare.Y;
            }
            if (compare.Z < a.Z)
            {
                a.Z = compare.Z;
            }

            return a;
        }

        public static Vector3 Ceil2(this Vector3 a, Vector3 compare)
        {
            if (compare.X > a.X)
            {
                a.X = compare.X;
            }
            if (compare.Y > a.Y)
            {
                a.Y = compare.Y;
            }
            if (compare.Z > a.Z)
            {
                a.Z = compare.Z;
            }

            return a;
        }
        #endregion Vector3

        #region Vector4

        public static Vector4 Lerp(this Vector4 a, Vector4 b, float t)
        {
            return new Vector4(MathHelper.Lerp(a.X, b.X, t), MathHelper.Lerp(a.Y, b.Y, t), MathHelper.Lerp(a.Z, b.Z, t), MathHelper.Lerp(a.W, b.W, t));
        }

        //public static Vector4 ToNormalized(this Vector4 v)
        //{
        //    v.Normalize();
        //    return v;
        //}

        //public static Vector3 Cross(this Vector3 a, Vector3 b)
        //{
        //    return Vector4.Cross(a, b);
        //}

        //public static float Dot(this Vector4 a, Vector4 b)
        //{
        //    return Vector4.Dot(a, b);
        //}

        //public static float AbsDot(this Vector4 a, Vector3 b)
        //{
        //    return (float)Math.Abs(Vector3.Dot(a, b));
        //}


        #endregion Vector4

        #region Color

        public static Color Lerp(this Color c, Color other, float n)
        {
            var c0 = c.ToVector4();
            var c1 = other.ToVector4();
            return new Color(c0.Lerp(c1, n));
        }

        public static Color Subtract(this Color c, Color other)
        {
            return new Color(MathHelper.Max(c.R - other.R, 0), MathHelper.Max(c.G - other.G, 0), MathHelper.Max(c.B - other.B, 0), MathHelper.Max(c.A - other.A, 0));
        }

        public static Color Subtract(this Color c, int amt)
        {
            return c.Subtract(new Color(amt, amt, amt, 0));
        }

        #endregion Color

        #region Matrix

        //public static Matrix4 Lerp(this Matrix4 a, Matrix4 b, float t)
        //{
        //    return new Matrix4(a.M11.Lerp(b.m11, t), a.M12.Lerp(b.M12, t), a.M13.Lerp(b.M13, t), a.M14.Lerp(b.M14, t),
        //                        a.M21.Lerp(b.m21, t), a.M22.Lerp(b.M22, t), a.M23.Lerp(b.M23, t), a.M24.Lerp(b.M24, t),
        //                        a.M31.Lerp(b.m31, t), a.M32.Lerp(b.M32, t), a.M33.Lerp(b.M33, t), a.M34.Lerp(b.M34, t),
        //                        a.M41.Lerp(b.M41, t), a.M42.Lerp(b.M42, t), a.M43.Lerp(b.M43, t), a.M44.Lerp(b.M44, t));
        //}

        #endregion Matrix

        #region Rectangle

        public static Vector2 GetPosition(this Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }

        public static Vector2 GetSize(this Rectangle r)
        {
            return new Vector2(r.Width, r.Height);
        }

        public static bool LineIntersects(this Rectangle rect, Vector2 start, Vector2 end)
        {
            //top line
            var a1 = new Vector2(rect.X, rect.Y);
            var a2 = new Vector2(rect.Right, rect.Y);

            //right line
            var b1 = new Vector2(rect.Right, rect.Y);
            var b2 = new Vector2(rect.Right, rect.Bottom);

            //bottom line
            var c1 = new Vector2(rect.X, rect.Bottom);
            var c2 = new Vector2(rect.Right, rect.Bottom);

            //left line
            var d1 = new Vector2(rect.X, rect.Y);
            var d2 = new Vector2(rect.X, rect.Bottom);

            return Helpers.Intersects(start, end, a1, a2) || Helpers.Intersects(start, end, b1, b2) ||
                    Helpers.Intersects(start, end, c1, c2) || Helpers.Intersects(start, end, d1, d2);
        }

        #endregion Rectangle

        #region Texture2D

        //public static Rectangle GetRectangle(this Texture2D texture)
        //{
        //    return new Rectangle(0, 0, texture.Width, texture.Height);
        //}

        #endregion Texture2D

        #region Point

        public static Vector2 ToVector(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }

        #endregion Point

        #region Random
        public static float NextFloat(this Random r)
        {
            return (float)r.NextDouble();
        }

        public static float Next(this Random r, float min, float max)
        {
            return r.NextFloat() * (max - min) + min;
        }
        #endregion

        #region List
        public static void RemoveUnordered<T>(this List<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
                throw new ArgumentOutOfRangeException("index");

            list[index] = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
        }

        #endregion

        /// <summary>
        /// Gets a 3x3 rotation matrix from this Quaternion.
        /// </summary>
        /// <returns></returns>
        public static Atma.Matrix3 ToRotationMatrix(this Quaternion q)
        {
            Atma.Matrix3 rotation = new Atma.Matrix3();

            float tx = 2.0f * q.X;
            float ty = 2.0f * q.Y;
            float tz = 2.0f * q.Z;
            float twx = tx * q.W;
            float twy = ty * q.W;
            float twz = tz * q.W;
            float txx = tx * q.X;
            float txy = ty * q.X;
            float txz = tz * q.X;
            float tyy = ty * q.Y;
            float tyz = tz * q.Y;
            float tzz = tz * q.Z;

            rotation.m00 = 1.0f - (tyy + tzz);
            rotation.m01 = txy - twz;
            rotation.m02 = txz + twy;
            rotation.m10 = txy + twz;
            rotation.m11 = 1.0f - (txx + tzz);
            rotation.m12 = tyz - twx;
            rotation.m20 = txz - twy;
            rotation.m21 = tyz + twx;
            rotation.m22 = 1.0f - (txx + tyy);

            return rotation;
        }

        public static Quaternion FromRotationMatrix(this Atma.Matrix3 matrix)
        {
            int[] next = new int[3] { 1, 2, 0 };
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternion Calculus and Fast Animation".

            Quaternion result = new Quaternion(0, 0, 0, 0);

            float trace = matrix.m00 + matrix.m11 + matrix.m22;

            float root = 0.0f;

            if (trace > 0.0f)
            {
                // |this.w| > 1/2, may as well choose this.w > 1/2
                root = MathHelper.Sqrt(trace + 1.0f); // 2w
                result.W = 0.5f * root;

                root = 0.5f / root; // 1/(4w)

                result.X = (matrix.m21 - matrix.m12) * root;
                result.Y = (matrix.m02 - matrix.m20) * root;
                result.Z = (matrix.m10 - matrix.m01) * root;
            }
            else
            {
                // |result.w| <= 1/2

                int i = 0;
                if (matrix.m11 > matrix.m00)
                {
                    i = 1;
                }
                if (matrix.m22 > matrix[i, i])
                {
                    i = 2;
                }

                int j = next[i];
                int k = next[j];

                root = MathHelper.Sqrt(matrix[i, i] - matrix[j, j] - matrix[k, k] + 1.0f);

                unsafe
                {
                    Real* apkQuat = &result.x;

                    apkQuat[i] = 0.5f * root;
                    root = 0.5f / root;

                    result.W = (matrix[k, j] - matrix[j, k]) * root;

                    apkQuat[j] = (matrix[j, i] + matrix[i, j]) * root;
                    apkQuat[k] = (matrix[k, i] + matrix[i, k]) * root;
                }
            }

            return result;
        }
        /// <summary>
        /// Computes the inverse of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public static Quaternion Inverse(this Quaternion q)
        {
            float norm = q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z;
            if (norm > 0.0f)
            {
                float inverseNorm = 1.0f / norm;
                return new Quaternion(q.W * inverseNorm, -q.X * inverseNorm, -q.Y * inverseNorm, -q.Z * inverseNorm);
            }
            else
            {
                // return an invalid result to flag the error
                return new Quaternion(0, 0, 0, 0);
            }
        }

        public static Vector3 Multiply(this Quaternion quat, Vector3 vector)
        {
            // nVidia SDK implementation
            Vector3 uv, uuv;
            Vector3 qvec = new Vector3(quat.X, quat.Y, quat.Z);

            uv = qvec.Cross(vector);
            uuv = qvec.Cross(uv);
            uv *= (2.0f * quat.W);
            uuv *= 2.0f;

            return vector + uv + uuv;

            // get the rotation matrix of the Quaternion and multiply it times the vector
            //return quat.ToRotationMatrix() * vector;
        }

        //public static BoundingBox Intersection(this BoundingBox a, BoundingBox b)
        //{
        //    //        if (vector.x >= minVector.x && vector.x <= maxVector.x &&
        //    //vector.y >= minVector.y && vector.y <= maxVector.y &&
        //    //vector.z >= minVector.z && vector.z <= maxVector.z)
        //    if (!a.Intersects2(b))
        //        return new BoundingBox();

        //    Vector3 intMin = Vector3.Zero;
        //    Vector3 intMax = Vector3.Zero;

        //    Vector3 b2max = b.Max;
        //    Vector3 b2min = b.Min;

        //    Vector3 maxVector = a.Max;
        //    Vector3 minVector = a.Min;

        //    if (b2max.X > maxVector.X && maxVector.X > b2min.X)
        //        intMax.X = maxVector.X;
        //    else
        //        intMax.X = b2max.X;

        //    if (b2max.Y > maxVector.Y && maxVector.Y > b2min.Y)
        //        intMax.Y = maxVector.Y;
        //    else
        //        intMax.Y = b2max.Y;

        //    if (b2max.Z > maxVector.Z && maxVector.Z > b2min.Z)
        //        intMax.Z = maxVector.Z;
        //    else
        //        intMax.Z = b2max.Z;

        //    if (b2min.X < minVector.X && minVector.X < b2max.X)
        //        intMin.X = minVector.X;
        //    else
        //        intMin.X = b2min.X;

        //    if (b2min.Y < minVector.Y && minVector.Y < b2max.Y)
        //        intMin.Y = minVector.Y;
        //    else
        //        intMin.Y = b2min.Y;

        //    if (b2min.Z < minVector.Z && minVector.Z < b2max.Z)
        //        intMin.Z = minVector.Z;
        //    else
        //        intMin.Z = b2min.Z;

        //    return new BoundingBox(intMin, intMax);
        //}

        //public static bool Intersects2(this BoundingBox a, BoundingBox box2)
        //{
        //    Vector3 maxVector = a.Max;
        //    Vector3 minVector = a.Min;

        //    // Use up to 6 separating planes
        //    if (maxVector.X <= box2.Min.X)
        //        return false;
        //    if (maxVector.Y <= box2.Min.Y)
        //        return false;
        //    if (maxVector.Z <= box2.Min.Z)
        //        return false;

        //    if (minVector.X >= box2.Max.X)
        //        return false;
        //    if (minVector.Y >= box2.Max.Y)
        //        return false;
        //    if (minVector.Z >= box2.Max.Z)
        //        return false;

        //    // otherwise, must be intersecting
        //    return true;
        //}

        public static bool remove<T, U>(this Dictionary<T, U> target, T t, U u)
        {
            if (target == null)
                return false;

            return target.remove(t, u);
        }

        public static U get<T, U>(this Dictionary<T, U> target, T t)
        {
            if (target == null)
                return default(U);

            U u;
            if (target.TryGetValue(t, out u))
                return u;

            return default(U);
        }

        public static U getOrCreate<T, U>(this Dictionary<T, U> target, T t)
            where U : new()
        {
            if (target == null)
                return default(U);

            U u;
            if (!target.TryGetValue(t, out u))
            {
                u = new U();
                target.Add(t, u);
                return u;
            }

            return u;
        }

    }
}