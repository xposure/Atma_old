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
using Atma.Assets;

namespace Atma.Samples.BulletHell.Systems
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

        //private void transformdirty()
        //{
        //    viewMatrixDirty = true;
        //}

        private void updateViewport()
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);

            _viewport.Width = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Width);
            _viewport.Height = (int)(graphics.graphicsDevice.PresentationParameters.Bounds.Height);

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

    public class CameraSystem : IComponentSystem, IRenderSubscriber
    {
        public static readonly GameUri Uri = "system:camera";

        private DisplayDevice _display;
        private Microsoft.Xna.Framework.Graphics.SpriteBatch _batch;

        private RenderQueue _opaqueQueue = new TextureRenderQueue();
        private RenderQueue _alphaRejectQueue = new TextureRenderQueue();
        private RenderQueue _alphaQueue = new DepthRenderQueue();
        private RenderQueue _additiveQueue = new TextureRenderQueue();

        public event Action<CameraComponent> prepareToRender;
        public event Action<CameraComponent, RenderQueue> renderOpaque;
        public event Action<CameraComponent, RenderQueue> renderAlphaReject;
        public event Action<CameraComponent, RenderQueue> renderAlpha;
        public event Action<CameraComponent, RenderQueue> renderAdditive;

        public int draws = 0;

        public void render()
        {
            draws = 0;

            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            var em = CoreRegistry.require<EntityManager>(EntityManager.Uri);
            foreach (var id in em.getWithComponents("camera", "transform"))
            {
                var entity = em.createRef(id);
                var camera = entity.getComponent<CameraComponent>("camera");
                var transform = entity.getComponent<Transform>("transform");

                camera.ReCreateViewMatrix();

                var matrix = camera.ViewMatrix;
                var viewport = camera.viewport;

                _display.device.Viewport = new Xna.Graphics.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
                _display.device.Clear(camera.clear);

                if (prepareToRender != null)
                {
                    prepareToRender(camera);
                }

                {
                    _batch.Begin(Xna.Graphics.SpriteSortMode.Texture,
                     Xna.Graphics.BlendState.Opaque,
                     Xna.Graphics.SamplerState.LinearClamp,
                     Xna.Graphics.DepthStencilState.Default,
                     Xna.Graphics.RasterizerState.CullCounterClockwise, null, matrix);

                    var mat = assets.getMaterial("engine:default");
                    var item = new Renderable();
                    item.color = Color.White;
                    item.depth = 0;
                    item.position = new Vector2(100, 100);
                    item.scale = new Vector2(100, 100);
                    item.texture = mat.texture;

                    for (var i = 0; i < 20000; i++)
                    {
                        _batch.Draw(item.texture.texture, new Rectangle(0, 0, 100, 100), Color.White);
                        //mat.texture.draw(_batch, item);
                        draws++;
                    }
                    _batch.End();
                }

                if (renderOpaque != null)
                {
                    _opaqueQueue.reset();
                    renderOpaque(camera, _opaqueQueue);

                    _batch.Begin(Xna.Graphics.SpriteSortMode.Texture,
                        Xna.Graphics.BlendState.Opaque,
                        Xna.Graphics.SamplerState.PointClamp,
                        Xna.Graphics.DepthStencilState.Default,
                        Xna.Graphics.RasterizerState.CullCounterClockwise, null, matrix);

                    foreach (var item in _opaqueQueue.items)
                    {
                        item.texture.draw(_batch, item);
                        draws++;
                    }

                    _batch.End();
                }

                if (renderAlphaReject != null)
                {
                    //graphics.begin();
                    //renderAlphaReject(camera, _alphaRejectQueue);
                    //graphics.renderQueue(_alphaRejectQueue);
                    //graphics.end(matrix, viewport);
                }

                if (renderAlpha != null)
                {
                    //graphics.begin();
                    //renderAlpha(camera, _alphaQueue);
                    //graphics.renderQueue(_alphaQueue);
                    //graphics.end(matrix, viewport);
                }

                if (renderAdditive != null)
                {
                    //_additiveQueue.reset();
                    //renderAdditive(camera, _additiveQueue);

                    //_batch.Begin(Xna.Graphics.SpriteSortMode.Texture,
                    //    Xna.Graphics.BlendState.Additive,
                    //    Xna.Graphics.SamplerState.PointClamp,
                    //    Xna.Graphics.DepthStencilState.None,
                    //    Xna.Graphics.RasterizerState.CullCounterClockwise, null, matrix);

                    //foreach (var item in _additiveQueue.items)
                    //{
                    //    item.texture.draw(_batch, item);
                    //    draws++;
                    //}

                    //_batch.End();
                }
            }
        }

        public void init()
        {
            _display = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri);
            _batch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(_display.device);
        }

        public void shutdown()
        {
        }
    }
}
