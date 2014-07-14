using Atma.Engine;
using Atma.Entity;
using Atma.Graphics;
using Atma.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xna = Microsoft.Xna.Framework;

namespace Atma.Rendering.Camera
{
    public class CameraComponent : Component
    {
        private Viewport _viewport = new Viewport();

        protected DisplayDevice _device;
        protected Transform _transform;

        public Color clear = Color.CornflowerBlue;

        public static CameraComponent mainCamera { get; private set; }
        public CameraComponent()
        {
            if (mainCamera == null)
                mainCamera = this;
        }

        public Viewport viewport
        {
            get
            {
                if (viewMatrixDirty)
                    ReCreateViewMatrix();
                return _viewport;
            }
        }

        public AxisAlignedBox worldBounds
        {
            get
            {
                var aabb = AxisAlignedBox.FromRect(Vector2.Zero, new Vector2(_viewport.Width, _viewport.Height));
                var inv = Matrix.Invert(ViewMatrix);
                var p0 = Vector2.Transform(aabb.Minimum, inv);
                var p1 = Vector2.Transform(aabb.Maximum, inv);

                aabb.SetExtents(p0, p1);

                return aabb;
            }
        }

        public Vector2 screenToWorld(Vector2 p)
        {
            return Vector2.Transform(p, Matrix.Invert(ViewMatrix));
        }

        public void init(Transform t)
        {
            _transform = t;
        }

        /// <summary>
        /// Recreates our view matrix, then signals that the view matrix
        /// is clean.
        /// </summary>
        public void ReCreateViewMatrix()
        {
            updateViewport();

            var position = _transform.DerivedPosition;
            var scale = _transform.DerivedScale;
            var orientation = _transform.DerivedOrientation;

            viewMatrix =
               Matrix.CreateTranslation(new Vector3(-(int)position.X, -(int)position.Y, 0)) *
               Matrix.CreateRotationZ(orientation) *
               Matrix.CreateScale(scale.X, scale.Y, 1) *
               Matrix.CreateTranslation(new Vector3(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0));

           
            viewMatrixDirty = false;
        }

        private void updateViewport()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            _viewport.Width = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width);
            _viewport.Height = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height);

      
        }


        #region FieldsAndProperties

        //We don't need an update method because the camera only needs updating
        //when we change one of it's parameters.
        //We keep track if one of our matrices is dirty
        //and reacalculate that matrix when it is accesed.
        private bool viewMatrixDirty = true;

        //private bool projectionMatrixDirty = true;

        //private float fieldOfView = 60f;
        //public float FieldOfView
        //{
        //    get { return fieldOfView; }
        //    set
        //    {
        //        projectionMatrixDirty = true;
        //        fieldOfView = value;
        //    }
        //}

        //private float aspectRatio;
        //public float AspectRatio
        //{
        //    get { return aspectRatio; }
        //    set
        //    {
        //        projectionMatrixDirty = true;
        //        aspectRatio = value;
        //    }
        //}

        //private float nearPlane = 0.3f;
        //public float NearPlane
        //{
        //    get { return nearPlane; }
        //    set
        //    {
        //        projectionMatrixDirty = true;
        //        nearPlane = value;
        //    }
        //}

        //private float farPlane = 100f;
        //public float FarPlane
        //{
        //    get { return farPlane; }
        //    set
        //    {
        //        projectionMatrixDirty = true;
        //        farPlane = value;
        //    }
        //}

        //public float MinZoom = 1;
        //public float MaxZoom = float.MaxValue;
        //private float zoom = 1;
        //public float Zoom
        //{
        //    get { return zoom; }
        //    set
        //    {
        //        viewMatrixDirty = true;
        //        zoom = MathHelper.Clamp(value, MinZoom, MaxZoom);
        //    }
        //}

        //private Vector3 position;
        //public Vector3 Position
        //{
        //    get
        //    {
        //        if (viewMatrixDirty)
        //        {
        //            ReCreateViewMatrix();
        //        }
        //        return position;
        //    }
        //}

        //private Vector3 lookAt;
        //public Vector3 LookAt
        //{
        //    get { return lookAt; }
        //    set
        //    {
        //        viewMatrixDirty = true;
        //        lookAt = value;
        //    }
        //}

        #endregion FieldsAndProperties

        #region ICamera Members

        //public Matrix ViewProjectionMatrix
        //{
        //    get { return ViewMatrix * ProjectionMatrix; }
        //}

        private Matrix viewMatrix;

        public Matrix ViewMatrix
        {
            get
            {
                if (viewMatrixDirty)
                {
                    ReCreateViewMatrix();
                }
                return viewMatrix;
            }
        }

        //private Matrix projectionMatrix;
        //public Matrix ProjectionMatrix
        //{
        //    get
        //    {
        //        if (projectionMatrixDirty)
        //        {
        //            ReCreateProjectionMatrix();
        //        }
        //        return projectionMatrix;
        //    }
        //}

        #endregion ICamera Members
    }

}
