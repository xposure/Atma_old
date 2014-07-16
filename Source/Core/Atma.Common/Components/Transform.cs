using Atma.Entities;
using Atma.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma;

namespace Atma.Common.Components
{
    /// <summary>
    ///     Denotes the spaces which a transform can be relative to.
    /// </summary>
    public enum TransformSpace
    {
        /// <summary>
        ///     Transform is relative to the local space.
        /// </summary>
        Local,

        /// <summary>
        ///     Transform is relative to the space of the parent node.
        /// </summary>
        Parent,

        /// <summary>
        ///     Transform is relative to world space.
        /// </summary>
        World
    };

    public class Transform : Component
    {

        public static readonly Vector2 up = -Vector2.UnitY;
        public static readonly Vector2 down = Vector2.UnitY;
        public static readonly Vector2 forward = -Vector2.UnitY;
        public static readonly Vector2 backward = Vector2.UnitY;
        public static readonly Vector2 left = -Vector2.UnitX;
        public static readonly Vector2 right = Vector2.UnitX;

        internal Transform parent;
        private List<Transform> children;

        /// <summary>Flag to indicate own transform from parent is out of date.</summary>
        protected bool needUpdate;

        /// <summary>Orientation of this node relative to its parent.</summary>
        protected float orientation;

        /// <summary>World orientation of this node based on parents orientation.</summary>
        protected float derivedOrientation;

        /// <summary></summary>
        protected float depth;

        /// <summary></summary>
        protected float derivedDepth;

        /// <summary>Position of this node relative to its parent.</summary>
        protected Vector2 position;

        /// <summary></summary>
        protected Vector2 derivedPosition;

        /// <summary></summary>
        protected Vector2 scale = Vector2.One;

        /// <summary></summary>
        protected Vector2 derivedScale = Vector2.One;

        private bool _inheritPosition = true;
        public bool inheritPosition
        {
            get { return _inheritPosition; }
            set
            {
                _inheritPosition = value;
                NeedUpdate();
            }
        }

        /// <summary></summary>
        private bool _inheritScale = true;
        public bool inheritScale
        {
            get { return _inheritScale; }
            set
            {
                _inheritScale = value;
                NeedUpdate();
            }
        }

        /// <summary></summary>
        private bool _inheritDepth = true;
        public bool inheritDepth
        {
            get { return _inheritDepth; }
            set
            {
                _inheritDepth = value;
                NeedUpdate();
            }
        }

        /// <summary></summary>
        private bool _inheritOrientation = true;
        public bool inheritOrientation
        {
            get { return _inheritOrientation; }
            set
            {
                _inheritOrientation = value;
                NeedUpdate();
            }
        }

        /// <summary>
        ///    A Quaternion representing the nodes orientation.
        /// </summary>
        public float Orientation
        {
            get { return MathHelper.WrapAngle(orientation); }
            set
            {
                orientation = value;
                //orientation.Normalize(); // avoid drift
                //modulus radains
                NeedUpdate();
            }
        }

        /// <summary></summary>
        public float Depth
        {
            get { return depth; }
            set
            {
                depth = value;
                NeedUpdate();
            }
        }

        /// <summary>
        /// The position of the node relative to its parent.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                NeedUpdate();
            }
        }

        /// <summary>
        /// The scaling factor applied to this node.
        /// </summary>
        /// <remarks>
        ///	Scaling factors, unlike other transforms, are not always inherited by child nodes.
        ///	Whether or not scalings affect both the size and position of the child nodes depends on
        ///	the setInheritScale option of the child. In some cases you want a scaling factor of a parent node
        ///	to apply to a child node (e.g. where the child node is a part of the same object, so you
        ///	want it to be the same relative size and position based on the parent's size), but
        ///	not in other cases (e.g. where the child node is just for positioning another object,
        ///	you want it to maintain its own size and relative position). The default is to inherit
        ///	as with other transforms.
        ///
        ///	Note that like rotations, scalings are oriented around the node's origin.
        ///	</remarks>
        public Vector2 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                NeedUpdate();
            }
        }

        public Vector2 Forward
        {
            get { return new Vector2((float)Math.Cos(Orientation /*- MathHelper.PiOver2*/), (float)Math.Sin(Orientation /*- MathHelper.PiOver2*/)); }
            set
            {
                orientation = (float)Math.Atan2(value.Y, value.X);// +MathHelper.PiOver2;
                NeedUpdate();
            }
        }

        public Vector2 DerivedForward
        {
            get { return new Vector2((float)Math.Cos(DerivedOrientation /*- MathHelper.PiOver2*/), (float)Math.Sin(DerivedOrientation /*- MathHelper.PiOver2*/)); }
        }

        public Vector2 DerivedRight
        {
            get { return new Vector2((float)Math.Cos(DerivedOrientation + MathHelper.PiOver2), (float)Math.Sin(DerivedOrientation + MathHelper.PiOver2)); }
        }

        public Vector2 DerivedLeft
        {
            get { return -DerivedRight; }
        }

        public Vector2 DerivedBackward
        {
            get { return -DerivedForward; }
        }

        //public Vector2 Backward
        //{
        //    get { return new Vector2((float)Math.Sin(Orientation - MathHelper.PiOver2), (float)Math.Cos(Orientation - MathHelper.PiOver2)); }
        //    set { orientation = (float)Math.Atan2(value.Y, value.Y) + MathHelper.PiOver2; }
        //}

        //public Vector2 DerivedBackward
        //{
        //    get { return new Vector2((float)Math.Cos(DerivedOrientation - MathHelper.PiOver2), (float)Math.Sin(DerivedOrientation - MathHelper.PiOver2)); }
        //}

        /// <summary>
        /// Gets the orientation of the node as derived from all parents.
        /// </summary>
        public float DerivedOrientation
        {
            get
            {
                if (needUpdate)
                {
                    UpdateFromParent();
                }

                return MathHelper.WrapAngle(derivedOrientation);
            }
            //set
            //{
            //    if (inheritOrientation && parent != null)
            //    {
            //        //orientation = parent.DerivedOrientation.Inverse() * value;
            //        orientation = parent.DerivedOrientation.Inverse() * value;
            //    }
            //    else
            //    {
            //        orientation = value;
            //    }

            //    orientation.Normalize(); // avoid drift
            //    NeedUpdate();
            //}
        }

        /// <summary>
        /// Gets the position of the node as derived from all parents.
        /// </summary>
        public Vector2 DerivedPosition
        {
            get
            {
                if (needUpdate)
                {
                    UpdateFromParent();
                }

                return derivedPosition;
            }
            set
            {
                if (parent != null && _inheritPosition)
                {
                    var matrix = Matrix.CreateRotationZ(parent.DerivedOrientation) *
                                    Matrix.CreateTranslation(new Vector3(parent.DerivedPosition, 0f)) *
                                    Matrix.CreateScale(new Vector3(parent.DerivedScale, 1f));
                    position = Vector2.Transform(value, Matrix.Invert(matrix));

                    //position = -parent.DerivedOrientation * (value - parent.DerivedPosition) / parent.DerivedScale;
                }
                else
                {
                    position = value;
                }

                NeedUpdate();
            }
        }

        /// <summary>
        /// Gets the scaling factor of the node as derived from all parents.
        /// </summary>
        public Vector2 DerivedScale
        {
            get
            {
                if (needUpdate)
                {
                    UpdateFromParent();
                }

                return derivedScale;
            }
            set
            {
                if (inheritScale & parent != null)
                {
                    scale = value / parent.DerivedScale;
                }
                else
                {
                    scale = value;
                }

                NeedUpdate();
            }
        }

        /// <summary></summary>
        public float DerivedDepth
        {
            get
            {
                if (needUpdate)
                {
                    UpdateFromParent();
                }

                return derivedDepth;
            }
        }

        /// <summary>
        /// Scales the node, combining its current scale with the passed in scaling factor.
        /// </summary>
        /// <remarks>
        ///	This method applies an extra scaling factor to the node's existing scale, (unlike setScale
        ///	which overwrites it) combining its current scale with the new one. E.g. calling this
        ///	method twice with Vector2(2,2,2) would have the same effect as setScale(Vector2(4,4,4)) if
        /// the existing scale was 1.
        ///
        ///	Note that like rotations, scalings are oriented around the node's origin.
        ///</remarks>
        /// <param name="scale"></param>
        public void ScaleBy(Vector2 factor)
        {
            scale = scale * factor;
            NeedUpdate();
        }

        /// <summary>
        /// Moves the node along the cartesian axes.
        ///
        ///	This method moves the node by the supplied vector along the
        ///	world cartesian axes, i.e. along world x,y,z
        /// </summary>
        /// <param name="scale">Vector with x,y,z values representing the translation.</param>
        public void Translate(Vector2 translate)
        {
            Translate(translate, TransformSpace.Parent);
        }

        /// <summary>
        /// Moves the node along the cartesian axes.
        ///
        ///	This method moves the node by the supplied vector along the
        ///	world cartesian axes, i.e. along world x,y,z
        /// </summary>
        /// <param name="scale">Vector with x,y,z values representing the translation.</param>
        public void Translate(Vector2 translate, TransformSpace relativeTo)
        {
            switch (relativeTo)
            {
                case TransformSpace.Local:
                    {
                        var matrix =
                                     Matrix.CreateTranslation(new Vector3(translate, 0)) *
                                     Matrix.CreateRotationZ(orientation) *
                                     Matrix.CreateScale(new Vector3(scale, 1));
                        position += new Vector2(matrix.Translation.X, matrix.Translation.Y);
                        break;
                    }
                case TransformSpace.World:
                    {
                        var parentDerivedOrientation = parent == null ? 0f : parent.DerivedOrientation;
                        //var parentDerivedScale = parent == null ? Vector2.One : parent.DerivedScale;
                        var matrix =
                                     Matrix.CreateTranslation(new Vector3(translate, 0)) *
                                     Matrix.CreateRotationZ(-parentDerivedOrientation) *
                                     Matrix.CreateScale(new Vector3(scale, 1));

                        //matrix = Matrix.Invert(matrix);
                        //position = Vector2.Transform(position, matrix);
                        position += new Vector2(matrix.Translation.X, matrix.Translation.Y);
                        break;
                    }
                case TransformSpace.Parent:
                    {
                        //var parentDerivedPosition = parent == null ? Vector2.Zero : parent.DerivedPosition;
                        //var parentDerivedOrientation = parent == null ? 0f : parent.DerivedOrientation;
                        var parentDerivedScale = parent == null ? Vector2.One : parent.DerivedScale;
                        //var matrix = Matrix.CreateRotationZ(parentDerivedOrientation + MathHelper.PiOver2) *
                        //                Matrix.CreateTranslation(new Vector3(translate, 0)) *
                        //                Matrix.CreateScale(new Vector3(parentDerivedScale, 1));

                        //position = Vector2.Transform(position, matrix);
                        //position += translate * parentDerivedScale;
                        position += translate * scale;
                        break;
                    }
            }

            NeedUpdate();
        }

        public void LookAt(Vector2 pos)
        {
            LookAt(pos, TransformSpace.World);
        }

        public void LookAt(Vector2 pos, TransformSpace relativeTo)
        {
            if (this.parent == null)
                relativeTo = TransformSpace.World;

            switch (relativeTo)
            {
                case TransformSpace.Parent:
                    {
                        // Rotations are normally relative to local axes, transform up
                        var dir = pos - parent.position;
                        orientation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;
                        //orientation = rotation * orientation;
                    }
                    break;
                case TransformSpace.World:
                    {
                        var dir = pos - this.DerivedPosition;
                        dir.Normalize();
                        orientation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;
                        //orientation = orientation * DerivedOrientation.Inverse() * rotation * DerivedOrientation;
                    }
                    break;
                case TransformSpace.Local:
                    {
                        // Note the order of the mult, i.e. q comes after
                        //orientation = orientation * rotation;
                        var dir = pos - Position;
                        orientation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;
                        break;
                    }
            }

            NeedUpdate();
        }

        public float GetRotationTo(Vector2 pos, TransformSpace relativeTo)
        {
            switch (relativeTo)
            {
                case TransformSpace.World:
                    {
                        var dir = (pos - DerivedPosition).ToNormalized();
                        var targetRotation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;

                        //flip the rotation
                        if (targetRotation > MathHelper.Pi)
                            targetRotation -= MathHelper.Pi * 2;

                        return targetRotation;
                    }
                case TransformSpace.Parent:
                    {
                        var dp = parent == null ? position : parent.DerivedPosition;
                        var dir = (pos - dp).ToNormalized();
                        var targetRotation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;

                        //flip the rotation
                        if (targetRotation > MathHelper.Pi)
                            targetRotation -= MathHelper.Pi * 2;

                        return targetRotation;
                    }
                case TransformSpace.Local:
                    {
                        var dir = (pos - position).ToNormalized();
                        var targetRotation = (float)Math.Atan2(dir.Y, dir.X);// +MathHelper.PiOver2;

                        //flip the rotation
                        if (targetRotation > MathHelper.Pi)
                            targetRotation -= MathHelper.Pi * 2;

                        return targetRotation;
                    }

            }
            return 0f;
        }

        /// <summary>
        ///		To be called in the event of transform changes to this node that require its recalculation.
        /// </summary>
        /// <remarks>
        ///		This not only tags the node state as being 'dirty', it also requests its parent to
        ///		know about its dirtiness so it will get an update next time.
        /// </remarks>
        protected internal void NeedUpdate()
        {
            if (needUpdate)
                return;

            needUpdate = true;

            //this.gameObject.sendMessage("transformdirty");

            if (children != null)
                foreach (var c in children)
                    c.NeedUpdate();
        }

        public Matrix Matrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3(position, 0f)) *
                        Matrix.CreateRotationZ(orientation) *
                        Matrix.CreateScale(new Vector3(scale, 1f));
            }
        }

        public Matrix DerivedMatrix
        {
            get
            {
                if (needUpdate)
                    UpdateFromParent();

                if (parent != null)
                {
                    return Matrix.CreateTranslation(new Vector3(parent.DerivedPosition, 0f)) *
                            Matrix.CreateRotationZ(this.DerivedOrientation) *
                            Matrix.CreateTranslation(new Vector3(position, 0f)) *
                            Matrix.CreateScale(new Vector3(this.DerivedScale, 1f));
                }

                return Matrix;
            }
        }

        /// <summary>
        ///	Triggers the node to update its combined transforms.
        ///
        ///	This method is called internally by the engine to ask the node
        ///	to update its complete transformation based on its parents
        ///	derived transform.
        /// </summary>
        public void UpdateFromParent()
        {
            if (!needUpdate)
                return;
            //var oldpos = DerivedPosition;
            //var oldrot = DerivedOrientation;
            //var oldscale = DerivedScale;
            //var oldddepth = DerivedDepth;

            if (parent != null)
            {
                // Update orientation
                if (_inheritOrientation)
                    derivedOrientation = parent.DerivedOrientation + orientation;
                else
                    derivedOrientation = orientation;

                // update scale
                if (_inheritScale)
                    derivedScale = parent.DerivedScale * scale;
                else
                    derivedScale = scale;

                if (_inheritDepth)
                    derivedDepth = parent.DerivedDepth + depth;
                else
                    derivedDepth = depth;


                //var parentDerivedPosition = parent == null ? Vector2.Zero : parent.DerivedPosition;

                // Change position vector based on parent's orientation & scale
                var matrix = Matrix.CreateTranslation(new Vector3(position, 0));

                if (_inheritOrientation)
                    matrix *= Matrix.CreateRotationZ(parent.DerivedOrientation);

                //matrix *= Matrix.CreateScale(new Vector3(derivedScale, 1));

                derivedPosition = new Vector2(matrix.Translation.X, matrix.Translation.Y);

                // add parents positition to local altered position
                derivedPosition += parent.DerivedPosition;
            }
            else
            {
                // Root node, no parent
                derivedOrientation = orientation;
                derivedPosition = position;
                derivedScale = scale;
                derivedDepth = depth;
            }

            needUpdate = false;
            //gameObject.sendMessage("transformupdate");
            //if (oldpos != derivedPosition)
            //    this.gameObject.sendMessage("moved", oldpos, derivedPosition);
            //if (oldrot != derivedOrientation)
            //    this.gameObject.sendMessage("rotated", oldrot, derivedOrientation);
            //if (oldscale != derivedScale)
            //    this.gameObject.sendMessage("scaled", oldscale, derivedScale);
            //if (oldddepth != derivedDepth)
            //    this.gameObject.sendMessage("depth", oldddepth, derivedDepth);
        }

        public void setParent(int id)
        {
            var entity = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            var tc = entity.getComponent<Transform>(id, "transform");
            if (tc != null)
            {
                if (tc.children != null)
                {
                    tc.children = new List<Transform>();
                    tc.children.Add(this);
                    parent = tc;
                }
            }
        }

        //private void childadded(GameObject go)
        //{
        //    var transform = go.getScript<TransformComponent>();
        //    if (transform != null)
        //    {
        //        if (children == null)
        //            children = new List<TransformComponent>();

        //        transform.parent = this;
        //        children.Add(transform);
        //        transform.NeedUpdate();
        //    }
        //}

        //private void childremoved(GameObject go)
        //{
        //    var transform = go.getScript<Transform>();
        //    if (transform != null && children != null)
        //    {
        //        transform.parent = null;
        //        children.Remove(transform);
        //        transform.NeedUpdate();
        //    }
        //}

    }

}
