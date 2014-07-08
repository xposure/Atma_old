using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
        a.X = Utility.Ceiling(a.X);
        a.Y = Utility.Ceiling(a.Y);
        return a;
    }

    public static float Dot(this Vector2 a, Vector2 b)
    {
        return Vector2.Dot(a, b);
    }

    public static Vector2 Floor(this Vector2 a)
    {
        a.X = Utility.Ceiling(a.X) - 1;
        a.Y = Utility.Ceiling(a.Y) - 1;
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

    public static Vector2 Rotate(this Vector2 src, Vector2 pivot, float amount)
    {
        if (amount == 0)
            return src;

        var rot = pivot.GetRotation(src);
        rot += amount;

        var dir = new Vector2((float)Math.Cos(rot), (float)Math.Sin(rot));
        return dir * (src - pivot).Length() + pivot;
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

    public static BoundingBox ToBoundingBox(this Vector2 pos, Vector2 size)
    {
        return new BoundingBox(new Vector3(pos, 0f), new Vector3(pos + size, 0f));
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
        return Utility.ATan2(pos.Y, pos.X);
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

    #endregion Vector3

    #region Vector4

    public static Vector4 Lerp(this Vector4 a, Vector4 b, float t)
    {
        return new Vector4(MathHelper.Lerp(a.X, b.X, t), MathHelper.Lerp(a.Y, b.Y, t), MathHelper.Lerp(a.Z, b.Z, t), MathHelper.Lerp(a.W, b.W, t));
    }

    public static Vector4 ToNormalized(this Vector4 v)
    {
        v.Normalize();
        return v;
    }

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
        return new Color(Utility.Max(c.R - other.R, 0), Utility.Max(c.G - other.G, 0), Utility.Max(c.B - other.B, 0), Utility.Max(c.A - other.A, 0));
    }

    public static Color Subtract(this Color c, int amt)
    {
        return c.Subtract(new Color(amt, amt, amt, 0));
    }

    #endregion Color

    #region Matrix

    public static Matrix Lerp(this Matrix a, Matrix b, float t)
    {
        return new Matrix(a.M11.Lerp(b.M11, t), a.M12.Lerp(b.M12, t), a.M13.Lerp(b.M13, t), a.M14.Lerp(b.M14, t),
                            a.M21.Lerp(b.M21, t), a.M22.Lerp(b.M22, t), a.M23.Lerp(b.M23, t), a.M24.Lerp(b.M24, t),
                            a.M31.Lerp(b.M31, t), a.M32.Lerp(b.M32, t), a.M33.Lerp(b.M33, t), a.M34.Lerp(b.M34, t),
                            a.M41.Lerp(b.M41, t), a.M42.Lerp(b.M42, t), a.M43.Lerp(b.M43, t), a.M44.Lerp(b.M44, t));
    }

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

    public static Rectangle GetRectangle(this Texture2D texture)
    {
        return new Rectangle(0, 0, texture.Width, texture.Height);
    }

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

    public static BoundingBox Intersection(this BoundingBox a, BoundingBox b)
    {
        //        if (vector.x >= minVector.x && vector.x <= maxVector.x &&
        //vector.y >= minVector.y && vector.y <= maxVector.y &&
        //vector.z >= minVector.z && vector.z <= maxVector.z)
        if (!a.Intersects2(b))
            return new BoundingBox();

        Vector3 intMin = Vector3.Zero;
        Vector3 intMax = Vector3.Zero;

        Vector3 b2max = b.Max;
        Vector3 b2min = b.Min;

        Vector3 maxVector = a.Max;
        Vector3 minVector = a.Min;

        if (b2max.X > maxVector.X && maxVector.X > b2min.X)
            intMax.X = maxVector.X;
        else
            intMax.X = b2max.X;

        if (b2max.Y > maxVector.Y && maxVector.Y > b2min.Y)
            intMax.Y = maxVector.Y;
        else
            intMax.Y = b2max.Y;

        if (b2max.Z > maxVector.Z && maxVector.Z > b2min.Z)
            intMax.Z = maxVector.Z;
        else
            intMax.Z = b2max.Z;

        if (b2min.X < minVector.X && minVector.X < b2max.X)
            intMin.X = minVector.X;
        else
            intMin.X = b2min.X;

        if (b2min.Y < minVector.Y && minVector.Y < b2max.Y)
            intMin.Y = minVector.Y;
        else
            intMin.Y = b2min.Y;

        if (b2min.Z < minVector.Z && minVector.Z < b2max.Z)
            intMin.Z = minVector.Z;
        else
            intMin.Z = b2min.Z;

        return new BoundingBox(intMin, intMax);
    }

    public static bool Intersects2(this BoundingBox a, BoundingBox box2)
    {
        Vector3 maxVector = a.Max;
        Vector3 minVector = a.Min;

        // Use up to 6 separating planes
        if (maxVector.X <= box2.Min.X)
            return false;
        if (maxVector.Y <= box2.Min.Y)
            return false;
        if (maxVector.Z <= box2.Min.Z)
            return false;

        if (minVector.X >= box2.Max.X)
            return false;
        if (minVector.Y >= box2.Max.Y)
            return false;
        if (minVector.Z >= box2.Max.Z)
            return false;

        // otherwise, must be intersecting
        return true;
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
        where U: new()
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