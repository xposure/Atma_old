using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Viewport = Atma.Graphics.Viewport;
using Atma.Engine;
using Atma.Entity;

namespace Atma
{
    public class Camera : Component
    {
        public Color clear = Color.CornflowerBlue;

        protected Transform _transform = null;

        protected Atma.Graphics.IRenderTarget target;

        private static List<Camera> _allCameras = new List<Camera>();

        private static SpriteBatch batch;

        private Vector2 _normalizedViewPosition = Vector2.Zero;

        private Vector2 _normalizedViewSize = Vector2.One;

        //public Vector2 clipPlane = new Vector2(0.3f, 1000f);
        //public Rectangle AABB = new Rectangle(0, 0, 1, 1);
        private Viewport _viewport = new Viewport();

        public Camera()
        {
            if (mainCamera == null)
                mainCamera = this;
            //_allCameras.Add(this);
        }

        public static IEnumerable<Camera> allActiveCameras
        {
            get
            {
                foreach (var c in allCameras)
                    //if (c.enabled)
                        yield return c;
            }
        }

        public static IEnumerable<Camera> allCameras { get { return _allCameras; } }

        public static Camera current { get; internal set; }

        public static Camera mainCamera { get; private set; }

        public Vector2 normalizedViewPosition
        {
            get { return _normalizedViewPosition; }
            set
            {
                _normalizedViewPosition = value;
                viewMatrixDirty = true;
            }
        }

        public Vector2 normalizedViewSize
        {
            get { return _normalizedViewSize; }
            set
            {
                _normalizedViewSize = value;
                viewMatrixDirty = true;
            }
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
                var p0 = Vector2.Transform(aabb.minVector, inv);
                var p1 = Vector2.Transform(aabb.maxVector, inv);

                aabb.SetExtents(p0, p1);

                return aabb;
                //return new Rectangle(
                //    -halfWidth + (int)_transform.DerivedPosition.X,
                //    -halfHeight + (int)_transform.DerivedPosition.Y,
                //    _viewport.Width,
                //    _viewport.Height);
            }
        }

        //protected internal override void removeFromEvents()
        //{
        //    base.removeFromEvents();
        //    Event.UnRegister(gameObject.id, "init", init);
        //}

        //protected internal override void addToEvents()
        //{
        //    base.addToEvents();
        //    _allCameras.Add(this);
        //    Event.Register(gameObject.id, "init", init);
        //}

        public Vector2 screenToWorld(Vector2 p)
        {
            return Vector2.Transform(p, Matrix.Invert(ViewMatrix));
        }

        //protected GraphicsDevice graphicsDevice { get { return graphics.graphicsDevice; } }

        public static void drawAll()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            //foreach (var camera in allActiveCameras)
            //    camera.draw();

            //graphics.graphicsDevice.SetRenderTarget(null);
            //graphics.graphicsDevice.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(graphics.graphicsDevice.PresentationParameters.Bounds);
            ////graphics.graphicsDevice.Clear(Color.Black);
            //if (batch == null)
            //    batch = new SpriteBatch(graphics.graphicsDevice);
            //batch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            //foreach (var camera in allActiveCameras)
            //{
            //    var dstx = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width * camera._normalizedViewPosition.X);
            //    var dsty = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height * camera._normalizedViewPosition.Y);
            //    var dstw = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width * camera._normalizedViewSize.X);
            //    var dsth = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height * camera._normalizedViewSize.Y);
            //    var dstr = new Rectangle(dstx, dsty, dstw, dsth);
            //    batch.Draw(((Atma.MonoGame.Graphics.RenderToTexture)camera.target).target, dstr, new Rectangle(0, 0, camera._viewport.Width, camera._viewport.Height), Color.White);
            //}
            ////batch.DrawString(
            //batch.End();

        }

        private void destroy()
        {
            _allCameras.Remove(this);
        }

        //public static Matrix TransoformMatrix
        //{
        //    get { return Matrix.CreateTranslation(new Vector3(-Position, 0)); }
        //}
        public void begin()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            var vp = viewport;
            var vm = ViewMatrix;
            graphics.GL.begin(target, Atma.Graphics.SortMode.Material, vm, vp);
            graphics.GL.clear(clear);
        }

        public void end()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            graphics.GL.end();

        }

        public void draw()
        {
            current = this;
            //Event.Invoke("beforerender");
            
            ondraw();
            postprocess();
        }

        protected virtual void ondraw()
        {
            
            //graphics.graphicsDevice.Clear(clear);
            //Event.Invoke("render");
            //graphics.graphicsDevice.SetRenderTarget(target);
            //var vp = new Viewport(
            //graphics.render(ViewMatrix);
        }

        protected virtual void postprocess()
        {

        }

        public void init(Transform t)
        {
            _allCameras.Add(this);

            //var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            //_transform = em.getComponent<Transform>(this.id, "transform");

            _transform = t;
        }

        /// <summary>
        /// Recreates our view matrix, then signals that the view matrix
        /// is clean.
        /// </summary>
        private void ReCreateViewMatrix()
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

            //viewMatrix = Matrix.CreateOrthographic
            ////Calculate the relative position of the camera
            //var position = Vector3.Transform(Vector3.Backward, Matrix.CreateFromYawPitchRoll(0,0, roll));
            ////Convert the relative position to the absolute position
            ////position *= zoom;
            //position += lookAt;

            ////Calculate a new viewmatrix
            //viewMatrix = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            viewMatrixDirty = false;
        }

        private void transformdirty()
        {
            viewMatrixDirty = true;
        }

        private void updateViewport()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            //var target = new RenderTarget2D(graphics.graphicsDevice, 1024, 1024);
            //target.Width
            //_viewport.X = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width * normalizedViewPosition.X);
            //_viewport.Y = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height * normalizedViewPosition.Y);
            _viewport.Width = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width * _normalizedViewSize.X);
            _viewport.Height = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height * _normalizedViewSize.Y);

            if (target == null || target.width < _viewport.Width || target.height < _viewport.Height)
            {
                if (target != null)
                    target.Dispose();

                var w = Helpers.NextPow(_viewport.Width);
                var h = Helpers.NextPow(_viewport.Height);

                //target = new Atma.MonoGame.Graphics.RenderToTexture(new RenderTarget2D(graphics.graphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents));
                target = new Atma.MonoGame.Graphics.RenderToScreen();
            }

            // Calculate the position of the four corners in world space by applying
            //// The world matrix to the four corners in object space (0, 0, width, height)
            //var matrix = ViewMatrix;
            //Vector2 tl = Vector2.Transform(Vector2.Zero, matrix);
            //Vector2 tr = Vector2.Transform(new Vector2(_viewport.Width, 0), matrix);
            //Vector2 bl = Vector2.Transform(new Vector2(0, _viewport.Height), matrix);
            //Vector2 br = Vector2.Transform(new Vector2(_viewport.Width, _viewport.Height), matrix);

            //// Find the minimum and maximum "corners" based on the ones above
            //float minX = MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X)));
            //float maxX = MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X)));
            //float minY = MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y)));
            //float maxY = MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y)));
            //Vector2 min = new Vector2(minX, minY);
            //Vector2 max = new Vector2(maxX, maxY);

            //// And create the AABB
            //AABB = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        //private void update()
        //{
        //    ReCreateViewMatrix();
        //}

        //private void transformupdate()
        //{
        //    viewMatrixDirty = true;
        //}
        //public bool intersects(Rectangle r)
        //{
        //    return AABB.Intersects(r);
        //}
        ///// <summary>
        ///// Recreates our projection matrix, then signals that the projection
        ///// matrix is clean.
        ///// </summary>
        //private void ReCreateProjectionMatrix()
        //{
        //    projectionMatrix = Matrix.CreateOrthographic(_viewport.Width, _viewport.Height, nearPlane, farPlane);
        //    //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, nearPlane, farPlane);
        //    projectionMatrixDirty = false;
        //}

        #region HelperMethods

        ///// <summary>
        ///// Moves the camera and lookAt at to the right,
        ///// as seen from the camera, while keeping the same height
        ///// </summary>
        //public void MoveCameraRight(float amount)
        //{
        //    Vector3 right = Vector3.Normalize(LookAt - Position); //calculate forward
        //    right = Vector3.Cross(right, Vector3.Up); //calculate the float right
        //    right.Y = 0;
        //    right.Normalize();
        //    LookAt += right * amount;
        //}

        ///// <summary>
        ///// Moves the camera and lookAt forward,
        ///// as seen from the camera, while keeping the same height
        ///// </summary>
        //public void MoveCameraForward(float amount)
        //{
        //    Vector3 forward = Vector3.Normalize(LookAt - Position);
        //    forward.Y = 0;
        //    forward.Normalize();
        //    LookAt += forward * amount;
        //}

        #endregion HelperMethods

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