using Microsoft.Xna.Framework;

public struct LineSegment
{
    public Vector2 p0;
    public Vector2 p1;

    public LineSegment(Vector2 p0, Vector2 p1)
    {
        this.p0 = p0;
        this.p1 = p1;
    }

    public Vector2 closest(Vector2 p)
    {
        var l2 = (p0 - p1).LengthSquared();
        if (l2 == 0)
            return p0;

        var t = Vector2.Dot(p - p0, p1 - p0) / l2;
        if (t < 0)
            return p0;
        else if (t > 1)
            return p1;

        return p0 + t * (p1 - p0);
    }


    public float distance(Vector2 p)
    {
        var l2 = (p0 - p1).LengthSquared();
        if (l2 == 0)
            return (p0 - p).Length();

        var t = Vector2.Dot(p - p0, p1 - p0) / l2;
        if (t < 0)
            return (p0 - p).Length();
        else if (t > 1)
            return (p1 - p).Length();

        var projection = p0 + t * (p1 - p0);
        return (p - projection).Length();
    }

    public float distanceSquared(Vector2 p)
    {
        var l2 = (p0 - p1).LengthSquared();
        if (l2 == 0)
            return (p0 - p).LengthSquared();

        var t = Vector2.Dot(p - p0, p1 - p0) / l2;
        if (t < 0)
            return (p0 - p).LengthSquared();
        else if (t > 1)
            return (p1 - p).LengthSquared();

        var projection = p0 + t * (p1 - p0);
        return (p - projection).LengthSquared();
    }

    //det = bd.x * ad.y - bd.y * ad.x
    public IntersectResult intersects(Ray ray)
    {
        float t;
        Vector2 hitPoint;
        Vector2 min = p0;
        Vector2 max = p1;

        if (ray.origin.X <= min.X && ray.direction.X > 0)
        {
            t = (min.X - ray.origin.X) / ray.direction.X;

            if (t >= 0)
            {
                // substitue t back into ray and check bounds and distance
                hitPoint = ray.origin + ray.direction * t;

                if (hitPoint.Y >= min.Y && hitPoint.Y <= max.Y)
                    return new IntersectResult(true, t);
            }
        }

        return new IntersectResult(false, 0f);
    }

    public bool Intersects(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, Vector2 vertex4, out float r, out float s)
    {
        //float d = bd.x * ad.y - bd.y * ad.x;
        r = 0;
        s = 0;
        var d = 0f;
        //Make sure the lines aren't parallel
        Vector2 vertex1to2 = vertex2 - vertex1;
        Vector2 vertex3to4 = vertex4 - vertex3;
        //if (vertex1to2.x * -vertex3to4.y + vertex1to2.y * vertex3to4.x != 0)
        //{
        //if (vertex1to2.Y / vertex1to2.X != vertex3to4.Y / vertex3to4.X)
        {
            d = vertex1to2.X * vertex3to4.Y - vertex1to2.Y * vertex3to4.X;
            if (d != 0)
            {
                Vector2 vertex3to1 = vertex1 - vertex3;
                r = (vertex3to1.Y * vertex3to4.X - vertex3to1.X * vertex3to4.Y) / d;
                s = (vertex3to1.Y * vertex1to2.X - vertex3to1.X * vertex1to2.Y) / d;
                return true;
            }
        }
        return false;
    }

    public bool Intersects(LineSegment ls, out float r, out float s)
    {
        var vertex1 = ls.p0;
        var vertex2 = ls.p1;
        var vertex3 = p0;
        var vertex4 = p1;

        return Intersects(p0, p1, ls.p0, ls.p1, out r, out s);
       
    }

    public IntersectResult intersects2(Ray ls)
    {
        IntersectResult result = new IntersectResult();
        float r, s;

        var vertex1 = Vector2.Zero;
        var vertex2 = ls.direction;
        var vertex3 = p0 - ls.origin;
        var vertex4 = p1 - ls.origin;

        if (Intersects(vertex1, vertex2, vertex3, vertex4, out r, out s))
        {
            if (r >= 0)
            {
                if (s >= 0 && s <= 1)
                {
                    return new IntersectResult(true, r);
                }
            }
        }
        return result;
    }
}
