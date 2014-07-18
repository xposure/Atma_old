using Atma.Engine;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Atma.Assets;
using Atma.Fonts;


namespace Atma.Rendering
{
    public class OrthoCamera : GameSystem//, ICore
    {
        private Effect _spriteEffect;
        private readonly EffectParameter _matrixTransform;
        private readonly EffectPass _spritePass;

        private bool _isDirty = true;

        private Vector2 _normalizedOrigin = new Vector2(0.5f, 0.5f);
        private Vector2 _normalizedTopLeft = new Vector2(0, 0);
        private Vector2 _normalizedBottomRight = new Vector2(1, 1);

        private Atma.Graphics.Viewport _viewport = new Atma.Graphics.Viewport(0, 0, 1, 1);
        private Matrix _viewMatrix;
        private Matrix _inverseViewMatrix;
        private Matrix _projection;

        private Vector2 _position = Vector2.Zero;
        private Vector2 _scale = Vector2.One;
        private float _orientation = 0f;

        public Color clear = Color.CornflowerBlue;

        public Vector2 normalizedOrigin
        {
            get { return _normalizedOrigin; }
            set
            {
                if (_normalizedOrigin != value)
                {
                    _normalizedOrigin = value;
                    _isDirty = true;
                }
            }
        }

        public Vector2 normalizedTopLeft
        {
            get { return _normalizedTopLeft; }
            set
            {
                if (_normalizedTopLeft != value)
                {
                    _isDirty = true;
                    _normalizedTopLeft = value;
                }
            }
        }

        public Vector2 normalizedBottomRight
        {
            get { return _normalizedBottomRight; }
            set
            {
                if (_normalizedBottomRight != value)
                {
                    _isDirty = true;
                    _normalizedBottomRight = value;
                }
            }
        }

        public Atma.Graphics.Viewport viewport
        {
            get
            {
                if (_isDirty)
                    update();

                return _viewport;
            }
        }

        public Matrix viewMatrix
        {
            get
            {
                if (_isDirty)
                    update();
                return _viewMatrix;
            }
        }

        public Matrix inverseViewMatrix
        {
            get
            {
                if (_isDirty)
                    update();
                return _inverseViewMatrix;
            }
        }

        public Matrix projection { get { return _projection; } }

        public Vector2 position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                    _isDirty = true;

                _position = value;
            }
        }

        public Vector2 scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                    _isDirty = true;

                _scale = value;
            }
        }

        public float orientation
        {
            get { return _orientation; }
            set
            {
                if (_orientation != value)
                    _isDirty = true;

                _orientation = value;
            }
        }

        public AxisAlignedBox worldBounds
        {
            get
            {
                if (_isDirty)
                    update();

                var aabb = AxisAlignedBox.FromRect(Vector2.Zero, new Vector2(_viewport.Width, _viewport.Height));
                var inv = Matrix.Invert(_viewMatrix);
                var p0 = Vector2.Transform(aabb.Minimum, inv);
                var p1 = Vector2.Transform(aabb.Maximum, inv);

                aabb.SetExtents(p0, p1);

                return aabb;
            }
        }

        public AxisAlignedBox screenBounds
        {
            get
            {
                if (_isDirty)
                    update();

                return AxisAlignedBox.FromRect(_viewport.X, _viewport.Y, _viewport.Width, _viewport.Height);
            }
        }

        public OrthoCamera()
        {
            _spriteEffect = new Effect(display.device, SpriteBatch2.Bytecode);
            _matrixTransform = _spriteEffect.Parameters["MatrixTransform"];
            _spritePass = _spriteEffect.CurrentTechnique.Passes[0];
        }

        private void update()
        {
            _isDirty = false;

            _viewport.X = (int)(display.width * _normalizedTopLeft.X);
            _viewport.Y = (int)(display.height * _normalizedTopLeft.Y);
            _viewport.Width = (int)(display.width * _normalizedBottomRight.X);
            _viewport.Height = (int)(display.height * _normalizedBottomRight.Y);


            _viewMatrix =
               Matrix.CreateTranslation(new Vector3(-(int)_position.X, -(int)_position.Y, 0)) *
               Matrix.CreateRotationZ(_orientation) *
               Matrix.CreateScale(_scale.X, _scale.Y, 1) *
               Matrix.CreateTranslation(new Vector3(_viewport.Width * _normalizedOrigin.X, _viewport.Height * _normalizedOrigin.Y, 0));

            _inverseViewMatrix = Matrix.Invert(_viewMatrix);

            Matrix projection;
#if PSM || DIRECTX
            Matrix.CreateOrthographicOffCenter(0, _viewport.Width, _viewport.Height, 0, -1, 0, out projection);
#else
            _viewMatrix.M41 += -0.5f * _viewMatrix.M11;
            _viewMatrix.M42 += -0.5f * _viewMatrix.M22;
            // GL requires a half pixel offset to match DX.
            Matrix.CreateOrthographicOffCenter(0, _viewport.Width, _viewport.Height, 0, 0, 1, out projection);
            projection.M41 += -0.5f * projection.M11;
            projection.M42 += -0.5f * projection.M22;
#endif
            Matrix.Multiply(ref _viewMatrix, ref projection, out projection);
            _projection = projection;
        }

        public void lookThrough()
        {
            if (_isDirty)
                update();

            _matrixTransform.SetValue(projection);
            _spritePass.Apply();
        }

        public Vector2 screenToWorld(Vector2 p)
        {
            return Vector2.Transform(p, Matrix.Invert(_viewMatrix));
        }

        public Vector2 worldToScreen(Vector2 p)
        {
            return Vector2.Transform(p, _viewMatrix);
        }
    }
}
