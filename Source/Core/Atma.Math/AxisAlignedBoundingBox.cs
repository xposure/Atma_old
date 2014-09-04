#region LGPL License

/*
Axiom Graphics Engine Library
Copyright © 2003-2011 Axiom Project Team

The overall design, and a majority of the core engine and rendering code
contained within this library is a derivative of the open source Object Oriented
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.
Many thanks to the OGRE team for maintaining such a high quality project.

The math library included in this project, in addition to being a derivative of
the works of Ogre, also include derivative work of the free portion of the
Wild Magic mathematics source code that is distributed with the excellent
book Game Engine Design.
http://www.wild-magic.com/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion LGPL License

#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AxisAlignedBox.cs 2353 2010-12-30 03:50:52Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using Atma;
using System;
using System.Diagnostics;


#endregion Namespace Declarations

/// <summary>
///		A 3D box aligned with the x/y/z axes.
/// </summary>
/// <remarks>
///		This class represents a simple box which is aligned with the
///	    axes. It stores 2 points as the extremeties of
///	    the box, one which is the minima of all 3 axes, and the other
///	    which is the maxima of all 3 axes. This class is typically used
///	    for an axis-aligned bounding box (AABB) for collision and
///	    visibility determination.
/// </remarks>
public struct AxisAlignedBox : ICloneable
{
    #region Fields

    internal Vector2 maxVector;
    internal Vector2 minVector;
    private Vector2[] corners;
    private bool isInfinite;
    private bool isNull;

    #endregion Fields

    #region Constructors

    public AxisAlignedBox(float x0, float y0, float x1, float y1)
    {
        corners = new Vector2[4];
        minVector.X = x0;
        minVector.Y = y0;
        maxVector.X = x1;
        maxVector.Y = y1;
        isNull = false;
        isInfinite = false;
        UpdateCorners();
    }

    public AxisAlignedBox(Vector2 min, Vector2 max)
    {
        corners = new Vector2[4];
        minVector = min;
        maxVector = max;
        isNull = false;
        isInfinite = false;
        UpdateCorners();
    }

    public AxisAlignedBox(AxisAlignedBox box)
    {
        corners = new Vector2[4];
        minVector = box.minVector;
        maxVector = box.maxVector;
        isNull = box.IsNull;
        isInfinite = box.IsInfinite;
        UpdateCorners();
    }

    #endregion Constructors

    #region Public methods

    public float Height
    {
        get { return maxVector.Y - minVector.Y; }
    }

    public float Width
    {
        get { return maxVector.X - minVector.X; }
    }

    public float X0 { get { return minVector.X; } }

    public float X1 { get { return maxVector.X; } }

    public float Y0 { get { return minVector.Y; } }

    public float Y1 { get { return maxVector.Y; } }

    /// <summary>
    ///     Return new bounding box from the supplied dimensions.
    /// </summary>
    /// <param name="center">Center of the new box</param>
    /// <param name="size">Entire size of the new box</param>
    /// <returns>New bounding box</returns>
    public static AxisAlignedBox FromDimensions(Vector2 center, Vector2 size)
    {
        Vector2 halfSize = .5f * size;

        return new AxisAlignedBox(center - halfSize, center + halfSize);
    }

    public static AxisAlignedBox FromRect(float x, float y, float w, float h)
    {
        var min = new Vector2(x, y);
        var max = new Vector2(w, h) + min;
        return new AxisAlignedBox(min, max);
    }

    public static AxisAlignedBox FromRect(Vector2 min, float w, float h)
    {
        var max = new Vector2(w, h) + min;
        return new AxisAlignedBox(min, max);
    }

    public static AxisAlignedBox FromRect(Vector2 min, Vector2 size)
    {
        return new AxisAlignedBox(min, min + size);
    }

    public void Inflate(float x, float y)
    {
        var hx = x / 2f;
        var hy = y / 2f;
        minVector.X -= hx;
        minVector.Y -= hy;
        maxVector.X += hx;
        maxVector.Y += hy;
        UpdateCorners();
    }

    public void Inflate(Vector2 size)
    {
        Inflate(size.X, size.Y);
    }

    /// <summary>
    ///		Allows for merging two boxes together (combining).
    /// </summary>
    /// <param name="box">Source box.</param>
    public void Merge(AxisAlignedBox box)
    {
        if (box.IsNull)
        {
            // nothing to merge with in this case, just return
            return;
        }
        else if (box.IsInfinite)
        {
            this.IsInfinite = true;
        }
        else if (this.IsNull)
        {
            SetExtents(box.Minimum, box.Maximum);
        }
        else if (!this.IsInfinite)
        {
            if (box.minVector.X < minVector.X)
                minVector.X = box.minVector.X;
            if (box.maxVector.X > maxVector.X)
                maxVector.X = box.maxVector.X;

            if (box.minVector.Y < minVector.Y)
                minVector.Y = box.minVector.Y;
            if (box.maxVector.Y > maxVector.Y)
                maxVector.Y = box.maxVector.Y;

            UpdateCorners();
        }
    }

    /// <summary>
    ///		Extends the box to encompass the specified point (if needed).
    /// </summary>
    /// <param name="point"></param>
    public void Merge(Vector2 point)
    {
        if (isNull || isInfinite)
        {
            // if null, use this point
            SetExtents(point, point);
        }
        else
        {
            if (point.X > maxVector.X)
                maxVector.X = point.X;
            else if (point.X < minVector.X)
                minVector.X = point.X;

            if (point.Y > maxVector.Y)
                maxVector.Y = point.Y;
            else if (point.Y < minVector.Y)
                minVector.Y = point.Y;

            UpdateCorners();
        }
    }

    /// <summary>
    ///    Scales the size of the box by the supplied factor.
    /// </summary>
    /// <param name="factor">Factor of scaling to apply to the box.</param>
    public void Scale(Vector2 factor)
    {
        SetExtents(minVector * factor, maxVector * factor);
    }

    /// <summary>
    ///		Sets both Minimum and Maximum at once, so that UpdateCorners only
    ///		needs to be called once as well.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void SetExtents(Vector2 min, Vector2 max)
    {
        isNull = false;
        isInfinite = false;

        minVector = min;
        maxVector = max;

        UpdateCorners();
    }

    /// <summary>
    ///
    /// </summary>
    internal void UpdateCorners()
    {
        // The order of these items is, using right-handed co-ordinates:
        // Minimum Z face, starting with Min(all), then anticlockwise
        //   around face (looking onto the face)
        // Maximum Z face, starting with Max(all), then anticlockwise
        //   around face (looking onto the face)
        corners[0] = minVector;
        corners[1].X = minVector.X;
        corners[1].Y = maxVector.Y;
        corners[2].X = maxVector.X;
        corners[2].Y = maxVector.Y;
        corners[3].X = maxVector.X;
        corners[3].Y = minVector.Y;

        //corners[4] = maxVector;
        //corners[5].X = minVector.X;
        //corners[5].Y = maxVector.Y;
        //corners[6].X = minVector.X;
        //corners[6].Y = minVector.Y;
        //corners[7].X = maxVector.X;
        //corners[7].Y = minVector.Y;
    }

    #endregion Public methods

    #region Contain methods

    /// <summary>
    /// Tests whether the given point contained by this box.
    /// </summary>
    /// <param name="v"></param>
    /// <returns>True if the vector is contained inside the box.</returns>
    public bool Contains(Vector2 v)
    {
        if (IsNull)
            return false;
        if (IsInfinite)
            return true;

        return Minimum.X <= v.X && v.X <= Maximum.X &&
               Minimum.Y <= v.Y && v.Y <= Maximum.Y;
    }

    public bool Contains(AxisAlignedBox box)
    {
        return Contains(box.minVector) && Contains(box.maxVector);
    }

    #endregion Contain methods

    #region Intersection Methods

    public MinimumTranslationVector collide(AxisAlignedBox box2)
    {
        var overlap = Intersection(box2);
        if (!overlap.IsNull)
        {
            var axis = new Axis();
            var minOverlap = 0.0;
            var adjust = overlap.Size;

            if (adjust.X < adjust.Y)
            {
                minOverlap = adjust.X;
                if (overlap.Center.X < box2.Center.X)
                    axis.edge = new Vector2(overlap.X0, overlap.Y0) - new Vector2(overlap.X0, overlap.Y1);
                else
                    axis.edge = new Vector2(overlap.X1, overlap.Y1) - new Vector2(overlap.X1, overlap.Y0);
            }
            else
            {
                minOverlap = adjust.Y;
                if (overlap.Center.Y > box2.Center.Y)
                    axis.edge = new Vector2(overlap.X0, overlap.Y0) - new Vector2(overlap.X1, overlap.Y0);
                else
                    axis.edge = new Vector2(overlap.X1, overlap.Y1) - new Vector2(overlap.X0, overlap.Y1);
            }

            axis.unit = axis.edge.Perpendicular;
            axis.normal = axis.unit.ToNormalized();
            return new MinimumTranslationVector(axis, minOverlap);
        }
        return MinimumTranslationVector.Zero;
    }

    public MinimumTranslationVector collideX(AxisAlignedBox box2)
    {
        var overlap = Intersection(box2);
        if (!overlap.IsNull)
        {
            var axis = new Axis();
            var minOverlap = 0.0;
            var adjust = overlap.Size;

            minOverlap = adjust.X;
            if (overlap.Center.X < box2.Center.X)
                axis.edge = new Vector2(overlap.X0, overlap.Y0) - new Vector2(overlap.X0, overlap.Y1);
            else
                axis.edge = new Vector2(overlap.X1, overlap.Y1) - new Vector2(overlap.X1, overlap.Y0);

            axis.unit = axis.edge.Perpendicular;
            axis.normal = axis.unit.ToNormalized();
            return new MinimumTranslationVector(axis, minOverlap);
        }
        return MinimumTranslationVector.Zero;
    }

    public MinimumTranslationVector collideY(AxisAlignedBox box2)
    {
        var overlap = Intersection(box2);
        if (!overlap.IsNull)
        {
            var axis = new Axis();
            var minOverlap = 0.0;
            var adjust = overlap.Size;

            minOverlap = adjust.Y;
            if (overlap.Center.Y > box2.Center.Y)
                axis.edge = new Vector2(overlap.X0, overlap.Y0) - new Vector2(overlap.X1, overlap.Y0);
            else
                axis.edge = new Vector2(overlap.X1, overlap.Y1) - new Vector2(overlap.X0, overlap.Y1);

            axis.unit = axis.edge.Perpendicular;
            axis.normal = axis.unit.ToNormalized();
            return new MinimumTranslationVector(axis, minOverlap);
        }
        return MinimumTranslationVector.Zero;
    }

    /// <summary>
    ///		Calculate the area of intersection of this box and another
    /// </summary>
    public AxisAlignedBox Intersection(AxisAlignedBox b2)
    {
        if (!Intersects(b2))
            return AxisAlignedBox.Null;

        Vector2 intMin = Vector2.Zero;
        Vector2 intMax = Vector2.Zero;

        Vector2 b2max = b2.maxVector;
        Vector2 b2min = b2.minVector;

        if (b2max.X > maxVector.X && maxVector.X > b2min.X)
            intMax.X = maxVector.X;
        else
            intMax.X = b2max.X;
        if (b2max.Y > maxVector.Y && maxVector.Y > b2min.Y)
            intMax.Y = maxVector.Y;
        else
            intMax.Y = b2max.Y;

        if (b2min.X < minVector.X && minVector.X < b2max.X)
            intMin.X = minVector.X;
        else
            intMin.X = b2min.X;
        if (b2min.Y < minVector.Y && minVector.Y < b2max.Y)
            intMin.Y = minVector.Y;
        else
            intMin.Y = b2min.Y;

        return new AxisAlignedBox(intMin, intMax);
    }

    /// <summary>
    ///		Returns whether or not this box intersects another.
    /// </summary>
    /// <param name="box2"></param>
    /// <returns>True if the 2 boxes intersect, false otherwise.</returns>
    public bool Intersects(AxisAlignedBox box2)
    {
        // Early-fail for nulls
        if (this.IsNull || box2.IsNull)
            return false;

        if (this.IsInfinite || box2.IsInfinite)
            return true;

        // Use up to 6 separating planes
        if (this.maxVector.X <= box2.minVector.X)
            return false;
        if (this.maxVector.Y <= box2.minVector.Y)
            return false;

        if (this.minVector.X >= box2.maxVector.X)
            return false;
        if (this.minVector.Y >= box2.maxVector.Y)
            return false;

        // otherwise, must be intersecting
        return true;
    }

    public bool Intersects(Circle circle)
    {
        return Intersects(circle.Center) ||
            circle.Intersects(corners[0]) ||
            circle.Intersects(corners[1]) ||
            circle.Intersects(corners[2]) ||
            circle.Intersects(corners[3]);
    }

    public bool Intersects(Ray2 ray)
    {
        return MathHelper.Intersects(ray, this).Hit;
    }

    /// <summary>
    ///		Tests whether this box intersects a sphere.
    /// </summary>
    /// <param name="sphere"></param>
    /// <returns>True if the sphere intersects, false otherwise.</returns>
    //public bool Intersects(Sphere sphere)
    //{
    //    return MathHelper.Intersects(sphere, this);
    //}

    /// <summary>
    ///
    /// </summary>
    /// <param name="plane"></param>
    /// <returns>True if the plane intersects, false otherwise.</returns>
    //public bool Intersects(Plane plane)
    //{
    //    return MathHelper.Intersects(plane, this);
    //}

    /// <summary>
    ///		Tests whether the vector point is within this box.
    /// </summary>
    /// <param name="vector"></param>
    /// <returns>True if the vector is within this box, false otherwise.</returns>
    public bool Intersects(Vector2 vector)
    {
        return (vector.X >= minVector.X && vector.X <= maxVector.X &&
            vector.Y >= minVector.Y && vector.Y <= maxVector.Y);
    }

    #endregion Intersection Methods

    #region Properties

    /// <summary>
    ///		Returns a null box
    /// </summary>
    public static AxisAlignedBox Null
    {
        get
        {
            AxisAlignedBox nullBox = new AxisAlignedBox(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));
            nullBox.IsNull = true;
            nullBox.isInfinite = false;
            return nullBox;
        }
    }

    /// <summary>
    ///    Get/set the center point of this bounding box.
    /// </summary>
    public Vector2 Center
    {
        get
        {
            return (minVector + maxVector) * 0.5f;
        }
        set
        {
            Vector2 halfSize = .5f * Size;
            minVector = value - halfSize;
            maxVector = value + halfSize;
            UpdateCorners();
        }
    }

    /// <summary>
    ///		Returns an array of 8 corner points, useful for
    ///		collision vs. non-aligned objects.
    /// </summary>
    /// <remarks>
    ///		If the order of these corners is important, they are as
    ///		follows: The 4 points of the minimum Z face (note that
    ///		because we use right-handed coordinates, the minimum Z is
    ///		at the 'back' of the box) starting with the minimum point of
    ///		all, then anticlockwise around this face (if you are looking
    ///		onto the face from outside the box). Then the 4 points of the
    ///		maximum Z face, starting with maximum point of all, then
    ///		anticlockwise around this face (looking onto the face from
    ///		outside the box). Like this:
    ///		<pre>
    ///		      -z
    ///		   1-----2
    ///		  /|+y  /|
    ///		 / |   / | +x
    ///		5-----4  |
    ///	 -x	|  0--|--3
    ///		| /   | /
    ///		|/ -y |/
    ///		6-----7
    ///		   +z
    ///		</pre>
    /// </remarks>
    public Vector2[] Corners
    {
        get
        {
            Debug.Assert(!isNull && !isInfinite, "Cannot get the corners of a null or infinite box.");

            return corners;
        }
    }

    public Vector2 HalfSize
    {
        get
        {
            if (isNull)
                return Vector2.Zero;

            if (isInfinite)
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            return (Maximum - Minimum) * 0.5f;
        }
    }

    /// <summary>
    /// Returns true if the box is infinite.
    /// </summary>
    public bool IsInfinite
    {
        get
        {
            return isInfinite;
        }
        set
        {
            isInfinite = value;
            if (value)
                isNull = false;
        }
    }

    /// <summary>
    ///		Get/set the value of whether this box is null (i.e. not dimensions, etc).
    /// </summary>
    public bool IsNull
    {
        get
        {
            return isNull;
        }
        set
        {
            isNull = value;
            if (value)
                isInfinite = false;
        }
    }

    /// <summary>
    ///		Get/set the maximum corner of the box.
    /// </summary>
    public Vector2 Maximum
    {
        get
        {
            return maxVector;
        }
        set
        {
            isNull = false;
            maxVector = value;
            UpdateCorners();
        }
    }

    /// <summary>
    ///		Get/set the minimum corner of the box.
    /// </summary>
    public Vector2 Minimum
    {
        get
        {
            return minVector;
        }
        set
        {
            isNull = false;
            minVector = value;
            UpdateCorners();
        }
    }

    /// <summary>
    ///     Get/set the size of this bounding box.
    /// </summary>
    public Vector2 Size
    {
        get
        {
            return maxVector - minVector;
        }
        set
        {
            Vector2 center = Center;
            Vector2 halfSize = .5f * value;
            minVector = center - halfSize;
            maxVector = center + halfSize;
            UpdateCorners();
        }
    }

    /// <summary>
    ///     Calculate the volume of this box
    /// </summary>
    public float Volume
    {
        get
        {
            if (isNull)
                return 0.0f;

            if (isInfinite)
                return float.PositiveInfinity;

            Vector2 diff = Maximum - Minimum;
            return diff.X * diff.Y;
        }
    }

    #endregion Properties

    #region Operator Overloads

    public static bool operator !=(AxisAlignedBox left, AxisAlignedBox right)
    {
        //if ((object.ReferenceEquals(left, null) || left.isNull) &&
        //    (object.ReferenceEquals(right, null) || right.isNull))
        //    return false;

        //else if ((object.ReferenceEquals(left, null) || left.isNull) ||
        //         (object.ReferenceEquals(right, null) || right.isNull))
        //    return true;

        return left.minVector != right.minVector || left.maxVector != right.maxVector;
        //return
        //    (left.corners[0] != right.corners[0] || left.corners[1] != right.corners[1] || left.corners[2] != right.corners[2] ||
        //    left.corners[3] != right.corners[3] || left.corners[4] != right.corners[4] || left.corners[5] != right.corners[5] ||
        //    left.corners[6] != right.corners[6] || left.corners[7] != right.corners[7]);
    }

    public static bool operator ==(AxisAlignedBox left, AxisAlignedBox right)
    {
        //if ((object.ReferenceEquals(left, null) || left.isNull) &&
        //    (object.ReferenceEquals(right, null) || right.isNull))
        //    return true;

        //else if ((object.ReferenceEquals(left, null) || left.isNull) ||
        //         (object.ReferenceEquals(right, null) || right.isNull))
        //    return false;

        return left.minVector == right.minVector && left.maxVector == right.maxVector;
        //(left.corners[0] == right.corners[0] && left.corners[1] == right.corners[1] && left.corners[2] == right.corners[2] &&
        //left.corners[3] == right.corners[3] && left.corners[4] == right.corners[4] && left.corners[5] == right.corners[5] &&
        //left.corners[6] == right.corners[6] && left.corners[7] == right.corners[7]);
    }

    public override bool Equals(object obj)
    {
        return obj is AxisAlignedBox && this == (AxisAlignedBox)obj;
    }

    public override int GetHashCode()
    {
        if (isNull)
            return 0;

        return corners[0].GetHashCode() ^ corners[1].GetHashCode() ^ corners[2].GetHashCode() ^ corners[3].GetHashCode();// ^
        //corners[4].GetHashCode() ^ corners[5].GetHashCode() ^ corners[6].GetHashCode() ^ corners[7].GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("{0}:{1}", this.minVector, this.maxVector);
    }

    #endregion Operator Overloads

    #region ICloneable Members

    public object Clone()
    {
        return new AxisAlignedBox(this);
    }

    #endregion ICloneable Members

    public AxisAlignedBox[] fromRectOffset(RectOffset offset)
    {
        var inner = new AxisAlignedBox(minVector + offset.min, maxVector - offset.max);

        var rects = new AxisAlignedBox[9];

        rects[0] = new AxisAlignedBox(corners[0], inner.corners[0]);
        rects[2] = new AxisAlignedBox(MathHelper.Min(corners[1], inner.corners[1]), MathHelper.Max(corners[1], inner.corners[1]));
        rects[6] = new AxisAlignedBox(MathHelper.Min(corners[3], inner.corners[3]), MathHelper.Max(corners[3], inner.corners[3]));
        rects[8] = new AxisAlignedBox(inner.corners[2], corners[2]);

        rects[1] = new AxisAlignedBox(rects[0].corners[1], inner.corners[1]);
        rects[3] = new AxisAlignedBox(rects[0].corners[3], inner.corners[3]);
        rects[4] = inner;
        rects[5] = new AxisAlignedBox(rects[2].corners[3], rects[8].corners[1]);
        rects[7] = new AxisAlignedBox(rects[6].corners[1], rects[8].corners[3]);
        return rects;
    }



    public void RotateAndContain(Vector2 pivot, float r)
    {
        if (!isNull && !isInfinite && r != 0)
        {
            var rotatedCorners = this.ToOBB(pivot, r);
            var center = (rotatedCorners[0] + rotatedCorners[1] + rotatedCorners[2] + rotatedCorners[3]) / 4f;
            this.minVector = center;
            this.maxVector = center;
            UpdateCorners();

            this.Merge(rotatedCorners[0]);
            this.Merge(rotatedCorners[1]);
            this.Merge(rotatedCorners[2]);
            this.Merge(rotatedCorners[3]);
        }
    }

    public Vector2[] ToOBB(Vector2 pivot, float r)
    {
        if (!isNull && !isInfinite && r != 0)
        {
            var rotatedCorners = new Vector2[4];
            for (var i = 0; i < corners.Length; i++)
            {
                rotatedCorners[i] = corners[i].Rotate(pivot, r);
            }
            return rotatedCorners;
        }
        return corners;
    }

    public Rectangle ToRect()
    {
        if (IsNull)
            throw new NullReferenceException();

        return new Rectangle((int)minVector.X, (int)minVector.Y, (int)(maxVector.X - minVector.X), (int)(maxVector.Y - minVector.Y));
    }
}