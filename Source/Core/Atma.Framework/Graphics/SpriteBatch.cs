using System;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Graphics
{

    public class SpriteBatch2
    {

        readonly SpriteBatcher _batcher;

        SpriteSortMode _sortMode;
        Effect _effect;
        bool _beginCalled;

        Rectangle _tempRect = new Rectangle(0, 0, 0, 0);
        Vector2 _texCoordTL = new Vector2(0, 0);
        Vector2 _texCoordBR = new Vector2(0, 0);

        public GraphicsDevice GraphicsDevice { get; private set; }

        public SpriteBatch2(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice == null)
            {
                throw new ArgumentException("graphicsDevice");
            }

            this.GraphicsDevice = graphicsDevice;

            _batcher = new SpriteBatcher(graphicsDevice);

            _beginCalled = false;
        }

        public void Begin(SpriteSortMode sortMode)
        {
            Begin(sortMode, null);
        }

        public void Begin(SpriteSortMode sortMode, Effect effect)
        {
            if (_beginCalled)
                throw new InvalidOperationException("Begin cannot be called again until End has been successfully called.");

            // defaults
            _sortMode = sortMode;
            _effect = effect;

            // Setup things now so a user can chage them.
            if (sortMode == SpriteSortMode.Immediate)
                Setup();

            _beginCalled = true;
        }

        public void End()
        {
            _beginCalled = false;

            if (_sortMode != SpriteSortMode.Immediate)
                Setup();

#if PSM   
            GraphicsDevice.BlendState.PlatformApplyState(GraphicsDevice);
#endif

            _batcher.DrawBatch(_sortMode);
        }

        private void Setup()
        {
            if (_effect != null)
                _effect.CurrentTechnique.Passes[0].Apply();
        }

        void CheckValid(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException("texture");
            if (!_beginCalled)
                throw new InvalidOperationException("Draw was called, but Begin has not yet been called. Begin must be called successfully before you can call Draw.");
        }

        void CheckValid(SpriteFont spriteFont, string text)
        {
            if (spriteFont == null)
                throw new ArgumentNullException("spriteFont");
            if (text == null)
                throw new ArgumentNullException("text");
            if (!_beginCalled)
                throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        }

        void CheckValid(SpriteFont spriteFont, StringBuilder text)
        {
            if (spriteFont == null)
                throw new ArgumentNullException("spriteFont");
            if (text == null)
                throw new ArgumentNullException("text");
            if (!_beginCalled)
                throw new InvalidOperationException("DrawString was called, but Begin has not yet been called. Begin must be called successfully before you can call DrawString.");
        }

        public void drawTri(Texture2D texture, Vector2 p0, Vector2 p1, Vector2 p2, Color? color = null, float depth = 0f)
        {
            CheckValid(texture);

            var item = _batcher.CreateBatchItem();

            if (!color.HasValue)
                color = Color.White;

            item.Depth = depth;
            item.Texture = texture;

            item.Set(p0, p1, p2, p0, color.Value, Vector2.Zero, new Vector2(1, 0), Vector2.One, new Vector2(0, 1));
            Graphics.GraphicSubsystem.spritesRendered++;
        }

        public void drawQuad(Texture2D texture, Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl, Color? color = null, float depth = 0f)
        {
            CheckValid(texture);

            var item = _batcher.CreateBatchItem();

            if (!color.HasValue)
                color = Color.White;

            item.Depth = depth;
            item.Texture = texture;

            item.Set(tl, tr, bl, br, color.Value, Vector2.Zero, new Vector2(1, 0), Vector2.One, new Vector2(0, 1));
            Graphics.GraphicSubsystem.spritesRendered++;
        }

        public void drawCircle(Texture2D texture, Vector2 center, float radius, float width = 1f, int segments = 10, Color? color = null, float depth = 0f)
        {
            var step = MathHelper.TwoPi / segments;

            var lp = new Vector2(Utility.Cos(0), Utility.Sin(0)) * radius + center;
            var p = Vector2.Zero;
            for (var i = 1; i <= segments; i++)
            {
                var current = step * i;
                p.X = Utility.Cos(current) * radius + center.X;
                p.Y = Utility.Sin(current) * radius + center.Y;

                drawLine(texture, lp, p, color: color, width: width, depth: depth);

                lp = p;
            }
        }



        public void drawRect(Texture2D texture, Vector2 p0, Vector2 p1, Color? color = null, float width = 1f, float depth = 0f)
        {
            var points = new Vector2[4];
            points[0] = p0.Floor();
            points[2] = p1.Floor();
            points[1] = new Vector2(points[2].X, points[0].Y);
            points[3] = new Vector2(points[0].X, points[2].Y);

            for (var i = 0; i < points.Length; i++)
                drawLine(texture, points[i], points[(i + 1) % points.Length], color: color, width: width, depth: depth);
        }

        public void drawShape(Texture2D texture, Shape shape, Color? color = null, float width = 1f, float depth = 0f)
        {
            var points = shape.derivedVertices;
            for (var i = 0; i < points.Length; i++)
                drawLine(texture, points[i], points[(i + 1) % points.Length], color: color, width: width, depth: depth);
        }

        public void drawLine(Texture2D texture, Vector2 start, Vector2 end, Color? color = null, float width = 1f, float depth = 0f)
        {
            //end.Y = -end.Y;
            //start.Y = -start.Y;
            var diff = end - start;
            //var srcRect = new AxisAlignedBox(start.X, start.Y - width * 0.5f, start.X + diff.Length(), start.Y + width * 0.5f);
            //var srcRect = new AxisAlignedBox(start.X, start.Y, start.X + width, start.Y + diff.Length());
            var len = diff.Length();
            diff.Normalize();

            var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            draw(texture, start, new Vector2(width, len),
                color: color, rotation: rotation, origin: new Vector2(0.5f, 0f), depth: depth);
        }

        public void draw(Texture2D texture,
            Vector2 position,
            Vector2 size,
            AxisAlignedBox? sourceRectangle = null,
            Color? color = null,
            float rotation = 0f,
            Vector2? origin = null,
            float depth = 0,
            Microsoft.Xna.Framework.Graphics.SpriteEffects effect = Microsoft.Xna.Framework.Graphics.SpriteEffects.None
            )
        {
            if (texture != null)
            {
                var adjustedSize = size;
                if (!sourceRectangle.HasValue || sourceRectangle.Value.IsNull)
                {
                    adjustedSize /= texture.size;
                    Draw(texture,
                        position: position, scale: adjustedSize, depth: depth,
                        rotation: rotation, color: color, origin: texture.size * origin, effect: effect);
                }
                else
                {
                    adjustedSize /= sourceRectangle.Value.Size;
                    Draw(texture,
                        position: position, scale: adjustedSize, depth: depth, sourceRectangle: sourceRectangle.Value.ToRect(),
                        rotation: rotation, color: color, origin: texture.size * origin, effect: effect);
                }
            }
        }

        // Overload for calling Draw() with named parameters
        /// <summary>
        /// This is a MonoGame Extension method for calling Draw() using named parameters.  It is not available in the standard XNA Framework.
        /// </summary>
        /// <param name='texture'>
        /// The Texture2D to draw.  Required.
        /// </param>
        /// <param name='position'>
        /// The position to draw at.  If left empty, the method will draw at drawRectangle instead.
        /// </param>
        /// <param name='drawRectangle'>
        /// The rectangle to draw at.  If left empty, the method will draw at position instead.
        /// </param>
        /// <param name='sourceRectangle'>
        /// The source rectangle of the texture.  Default is null
        /// </param>
        /// <param name='origin'>
        /// Origin of the texture.  Default is Vector2.Zero
        /// </param>
        /// <param name='rotation'>
        /// Rotation of the texture.  Default is 0f
        /// </param>
        /// <param name='scale'>
        /// The scale of the texture as a Vector2.  Default is Vector2.One
        /// </param>
        /// <param name='color'>
        /// Color of the texture.  Default is Color.White
        /// </param>
        /// <param name='effect'>
        /// SpriteEffect to draw with.  Default is SpriteEffects.None
        /// </param>
        /// <param name='depth'>
        /// Draw depth.  Default is 0f.
        /// </param>
        public void Draw(Texture2D texture,
                Vector2? position = null,
                Rectangle? sourceRectangle = null,
                Vector2? origin = null,
                float rotation = 0f,
                Vector2? scale = null,
                Color? color = null,
                SpriteEffects effect = SpriteEffects.None,
                float depth = 0f)
        {

            // Assign default values to null parameters here, as they are not compile-time constants
            if (!color.HasValue)
                color = Color.White;
            if (!origin.HasValue)
                origin = Vector2.Zero;
            if (!scale.HasValue)
                scale = Vector2.One;

            if (!position.HasValue)
            {
                throw new InvalidOperationException("Expected position");
            }
            else if (position != null)
            {
                // Call Draw() using position
                Draw(texture, (Vector2)position, sourceRectangle, (Color)color, rotation, (Vector2)origin, (Vector2)scale, effect, depth);
            }
        }


        public void Draw(Texture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                Vector2 scale,
                SpriteEffects effect,
                float depth)
        {
            CheckValid(texture);

            var w = texture.width * scale.X;
            var h = texture.height * scale.Y;
            if (sourceRectangle.HasValue)
            {
                w = sourceRectangle.Value.Width * scale.X;
                h = sourceRectangle.Value.Height * scale.Y;
            }

            DrawInternal(texture,
                new Vector4(position.X, position.Y, w, h),
                sourceRectangle,
                color,
                rotation,
                origin * scale,
                effect,
                depth,
                true);
        }

        public void Draw(Texture2D texture,
                Vector2 position,
                Rectangle? sourceRectangle,
                Color color,
                float rotation,
                Vector2 origin,
                float scale,
                SpriteEffects effect,
                float depth)
        {
            CheckValid(texture);

            var w = texture.width * scale;
            var h = texture.height * scale;
            if (sourceRectangle.HasValue)
            {
                w = sourceRectangle.Value.Width * scale;
                h = sourceRectangle.Value.Height * scale;
            }

            DrawInternal(texture,
                new Vector4(position.X, position.Y, w, h),
                sourceRectangle,
                color,
                rotation,
                origin * scale,
                effect,
                depth,
                true);
        }

        public void Draw(Texture2D texture,
            Rectangle destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effect,
            float depth)
        {
            CheckValid(texture);

            DrawInternal(texture,
                  new Vector4(destinationRectangle.X,
                              destinationRectangle.Y,
                              destinationRectangle.Width,
                              destinationRectangle.Height),
                  sourceRectangle,
                  color,
                  rotation,
                  new Vector2(origin.X * ((float)destinationRectangle.Width / (float)((sourceRectangle.HasValue && sourceRectangle.Value.Width != 0) ? sourceRectangle.Value.Width : texture.width)),
                                    origin.Y * ((float)destinationRectangle.Height) / (float)((sourceRectangle.HasValue && sourceRectangle.Value.Height != 0) ? sourceRectangle.Value.Height : texture.height)),
                  effect,
                  depth,
                  true);
        }

        internal void DrawInternal(Texture2D texture,
            Vector4 destinationRectangle,
            Rectangle? sourceRectangle,
            Color color,
            float rotation,
            Vector2 origin,
            SpriteEffects effect,
            float depth,
            bool autoFlush)
        {
            var item = _batcher.CreateBatchItem();

            item.Depth = depth;
            item.Texture = texture;

            if (sourceRectangle.HasValue)
            {
                _tempRect = sourceRectangle.Value;
            }
            else
            {
                _tempRect.X = 0;
                _tempRect.Y = 0;
                _tempRect.Width = texture.width;
                _tempRect.Height = texture.height;
            }

            _texCoordTL.X = _tempRect.X / (float)texture.width;
            _texCoordTL.Y = _tempRect.Y / (float)texture.height;
            _texCoordBR.X = (_tempRect.X + _tempRect.Width) / (float)texture.width;
            _texCoordBR.Y = (_tempRect.Y + _tempRect.Height) / (float)texture.height;

            if ((effect & SpriteEffects.FlipVertically) != 0)
            {
                var temp = _texCoordBR.Y;
                _texCoordBR.Y = _texCoordTL.Y;
                _texCoordTL.Y = temp;
            }
            if ((effect & SpriteEffects.FlipHorizontally) != 0)
            {
                var temp = _texCoordBR.X;
                _texCoordBR.X = _texCoordTL.X;
                _texCoordTL.X = temp;
            }

            item.Set(destinationRectangle.X,
                    destinationRectangle.Y,
                    -origin.X,
                    -origin.Y,
                    destinationRectangle.Z,
                    destinationRectangle.W,
                    (float)Math.Sin(rotation),
                    (float)Math.Cos(rotation),
                    color,
                    _texCoordTL,
                    _texCoordBR);

            Graphics.GraphicSubsystem.spritesRendered++;
            if (autoFlush)
            {
                FlushIfNeeded();
            }
        }

        // Mark the end of a draw operation for Immediate SpriteSortMode.
        internal void FlushIfNeeded()
        {
            if (_sortMode == SpriteSortMode.Immediate)
            {
                _batcher.DrawBatch(_sortMode);
            }
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
        {
            Draw(texture, position, sourceRectangle, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

        public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        {
            Draw(texture, destinationRectangle, sourceRectangle, color, 0, Vector2.Zero, SpriteEffects.None, 0f);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(texture, position, null, color);
        }

        public void Draw(Texture2D texture, Rectangle rectangle, Color color)
        {
            Draw(texture, rectangle, null, color);
        }


        static internal readonly byte[] Bytecode = LoadEffectResource(
#if DIRECTX
"Microsoft.Xna.Framework.Graphics.Effect.Resources.SpriteEffect.dx11.mgfxo"
#elif PSM
            "Microsoft.Xna.Framework.PSSuite.Graphics.Resources.SpriteEffect.cgx" //FIXME: This shader is totally incomplete
#else
            "Microsoft.Xna.Framework.Graphics.Effect.Resources.SpriteEffect.ogl.mgfxo"
#endif
);

        internal static byte[] LoadEffectResource(string name)
        {
#if WINRT
            var assembly = typeof(Effect).GetTypeInfo().Assembly;
#else
            var assembly = typeof(Effect).Assembly;
#endif
            var stream = assembly.GetManifestResourceStream(name);
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}

