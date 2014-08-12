using Atma.Engine;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Viewport = Atma.Graphics.Viewport;
using Atma.Assets;
using Atma.Fonts;
using Atma.Rendering;
using Atma.Input;

namespace Atma.Managers
{

    internal class GUIGroupLayout2
    {
        //public 
        public void doLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {

            //gui.label2(GetRect(content, style, options), content, style);
        }
    }

    public struct GUIRenderable
    {
        public GUIContent content;
        public GUIDrawArguments arguments;
    }

    public class GUIGroup
    {
        public List<GUIRenderable> items = new List<GUIRenderable>();

        private Vector2 position = Vector2.Zero;

        internal void reset()
        {
            items.Clear();
        }
    }

    public sealed class GUIManager : GameSystem
    {
        public GUISkin skin = new GUISkin();

        public readonly GUILayout layout;

        public Color buttonBackground = Color.DarkBlue;
        public Color buttonBackgroundHover = Color.Blue;
        public Color buttonBorder = Color.Wheat;
        public Color buttonBorderHover = Color.White;
        public Color buttonTextColor = Color.Wheat;
        public Color buttonTextColorHover = Color.White;
        public Color labelTextColor = Color.White;
        public AxisAlignedBox lastHitRect = AxisAlignedBox.Null;

        private RenderTarget2D target;
        private Vector2 _normalizedViewSize = Vector2.One;
        private Viewport _viewport = new Viewport();
        private Vector2 buttonPadding = new Vector2(5, 5);
        private Stack<Rectangle> clips = new Stack<Rectangle>();
        internal Font defaultFont;
        private Vector2 groupOffset = Vector2.Zero;
        private Stack<Vector2> groups = new Stack<Vector2>();
        private Matrix viewMatrix;
        private bool viewMatrixDirty = true;

        public event Action<GUIManager> onRender;

        private OrthoCamera camera = new OrthoCamera() { normalizedOrigin = Vector2.Zero };
        //private List<

        //private 

        public bool mouseUsed = false;


        public GUIManager()
        {
            layout = new GUILayout(this);
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

        public Viewport viewport
        {
            get
            {
                if (viewMatrixDirty)
                    ReCreateViewMatrix();
                return _viewport;
            }
        }

        public void beginClip(Vector2 p, Vector2 s)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            var r = new Rectangle((int)(p.X + groupOffset.X), (int)(p.Y + groupOffset.Y), (int)s.X, (int)s.Y);
            if (graphics.scissorEnabled)
                clips.Push(graphics.scissorRect);

            //graphics.beginScissor(r);
        }

        public void beginGroup(Vector2 p)
        {
            groups.Push(p);
            groupOffset += p;
        }

        public bool buttonold(Vector2 p, string text)
        {
            var rect = defaultFont.MeasureString(text);
            return buttonold(0, 1f, p.ToAABB(rect), defaultFont, 0f, text);
        }

        public bool buttonold(AxisAlignedBox rect, string text)
        {
            return buttonold(0, 1f, rect, defaultFont, 0f, text);
        }

        public bool buttonold(int renderQueue, float scale, AxisAlignedBox rect, Font font, float depth, string text)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            rect.SetExtents(rect.Minimum + groupOffset, rect.Maximum + groupOffset + buttonPadding + buttonPadding);
            //var input = CoreRegistry.require<InputSystem>(InputSystem.Uri);
            var mp = input.MousePosition;
            var gp = screenToGUI(mp);

            var isOver = rect.Contains(gp);

            var bg = isOver ? skin.button.hover.texture : skin.button.normal.texture;
            var border = isOver ? buttonBorderHover : buttonBorder;
            var color = isOver ? buttonTextColorHover : buttonTextColor;

            //graphics.beginScissor(rect);
            graphics.Draw(renderQueue, rect, buttonBackground, depth);
            //graphics.batch.drawRect(null, rect.Minimum, rect.Maximum, color: buttonBorder);
            graphics.DrawRect(renderQueue, rect, buttonBorder);
            graphics.DrawText(renderQueue, font, scale, rect.Minimum + buttonPadding, text, color, depth);
            //graphics.endScissor();

            return input.IsLeftMouseDown && isOver;
            //return false;
        }

        public void endClip()
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            if (!graphics.scissorEnabled)
                throw new Exception("called end clip without the scissorrect enabled");

            //graphics.endScissor();
            if (clips.Count > 0)
            {
                var r = clips.Pop();
                //graphics.beginScissor(r);
            }
        }

        public void endGroup()
        {
            var p = groups.Pop();
            groupOffset -= p;
        }

        #region label

        public void label2(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), skin.label);
        }

        public void label2(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), style);
        }

        public void label2(AxisAlignedBox p, string text)
        {
            doLabel(p, new GUIContent(text), skin.label);
        }

        public void label2(AxisAlignedBox p, string text, params object[] args)
        {
            doLabel(p, new GUIContent(string.Format(text, args)), skin.label);
        }

        public void label2(AxisAlignedBox p, string text, GUIStyle style)
        {
            doLabel(p, new GUIContent(text), style);
        }

        public void label2(AxisAlignedBox p, Material image)
        {
            doLabel(p, new GUIContent(image), skin.label);
        }

        public void label2(AxisAlignedBox p, Material image, GUIStyle style)
        {
            doLabel(p, new GUIContent(image), style);
        }

        public void label2(AxisAlignedBox p, GUIContent content)
        {
            doLabel(p, content, skin.label);
        }

        public void label2(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            doLabel(p, content, style);
        }

        private void doLabel(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            style.Draw(graphics, p, content, false, false, false, false);
        }

        #endregion

        public void line(Vector2 start, Vector2 end, Color? color = null, float width = 1f, float depth = 1f)
        {
            if (!color.HasValue)
                color = Color.White;

            //var tl = new Vector2(start.x

            //graphics.batch.drawQuad(null, 
            var def = assets.getMaterial("engine:default");


            //end.Y = -end.Y;
            //start.Y = -start.Y;
            var diff = end - start;
            //var srcRect = new AxisAlignedBox(start.X, start.Y - width * 0.5f, start.X + diff.Length(), start.Y + width * 0.5f);
            //var srcRect = new AxisAlignedBox(start.X, start.Y, start.X + width, start.Y + diff.Length());

            diff.Normalize();
            var perp = diff.Perp();
            var hw = width * 0.5f * perp;
            var tl = start + hw;
            var bl = start - hw;
            var tr = end + hw;
            var br = end - hw;
            graphics.batch.drawQuad(def.texture, tl, tr, br, bl, color: color, depth: depth);
            //var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            //Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);


            //graphics.DrawLine(0, null, start, end, color.Value, width, depth);
        }

        public void lines(Vector2[] points, Color? color = null, float width = 1f, float depth = 1f)
        {
            for (var i = 0; i < points.Length; i++)
            {
                var p0 = points[i];
                var p1 = points[(i + 1) % points.Length];
                line(p0, p1, color: color, width: width, depth: depth);
            }
        }

        public void tri(Vector2 p0, Vector2 p1, Vector2 p2, Color? color = null, float depth = 1f)
        {
            if (!color.HasValue)
                color = Color.White;

            //var tl = new Vector2(start.x

            //graphics.batch.drawQuad(null, 
            var def = assets.getMaterial("engine:default");

            graphics.batch.drawTri(def.texture, p0, p1, p2, color: color, depth: depth);
            //var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            //Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);


            //graphics.DrawLine(0, null, start, end, color.Value, width, depth);
        }

        public void triWire(Vector2 p0, Vector2 p1, Vector2 p2, Color? color = null, float width = 1f, float depth = 1f)
        {
            if (!color.HasValue)
                color = Color.White;

            GraphicsPath gp = new GraphicsPath();
            gp.clear();
            gp.AddPoint(p0);
            gp.AddPoint(p1);
            gp.AddPoint(p2);
            drawPath(gp);
            //var tl = new Vector2(start.x

            ////graphics.batch.drawQuad(null, 
            //var def = assets.getMaterial("engine:default");

            //graphics.batch.drawLine(def.texture, p0, p1, color: color, depth: depth, width: width);
            //graphics.batch.drawLine(def.texture, p1, p2, color: color, depth: depth, width: width);
            //graphics.batch.drawLine(def.texture, p2, p0, color: color, depth: depth, width: width);
            ////var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            ////Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);
            //if (graphics)
            //{

            //}

            //graphics.DrawLine(0, null, start, end, color.Value, width, depth);
        }

        public void quadWire(Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl, Color? color = null, float width = 1f, float depth = 1f)
        {
            if (!color.HasValue)
                color = Color.White;

            //var tl = new Vector2(start.x

            //graphics.batch.drawQuad(null, 
            var def = assets.getMaterial("engine:default");

            graphics.batch.drawLine(def.texture, tl, tr, color: color, depth: depth, width: width);
            graphics.batch.drawLine(def.texture, tr, br, color: color, depth: depth, width: width);
            graphics.batch.drawLine(def.texture, br, bl, color: color, depth: depth, width: width);
            graphics.batch.drawLine(def.texture, bl, tr, color: color, depth: depth, width: width);
            //var rotation = (float)Math.Atan2(diff.Y, diff.X) - MathHelper.PiOver2;
            //Draw(renderQueue, material, srcRect, AxisAlignedBox.Null, color, rotation, new Vector2(0.5f, 0), SpriteEffects.None, depth);


            //graphics.DrawLine(0, null, start, end, color.Value, width, depth);

        }

        public void drawRoundRect(float x, float y, float width, float height, float radius, float border = 1f, Color? color = null)
        {
            GraphicsPath gp = new GraphicsPath();
            gp.clear();
            //gp.AddLine(x + radius, y, x + width - radius, y); // Line
            gp.AddArc(x + width - radius, y + radius, radius, 270, 90); // Corner

            //gp.AddLine(x + width, y + radius, x + width, y + height - radius); // Line
            gp.AddArc(x + width - radius, y + height - radius, radius, 0, 90); // Corner

            //gp.AddLine(x + width - radius, y + height, x + radius, y + height); // Line
            gp.AddArc(x + radius, y + height - radius, radius, 90, 90); // Corner

            //gp.AddLine(x, y + height - radius, x, y + radius); // Line
            gp.AddArc(x + radius, y + radius, radius, 180, 90); // Corner

            fillPath(gp, color: Color.White);
            drawPath(gp, width: border, color: color);
        }

        public void fillPath(GraphicsPath path, Color? color = null, float depth = 1f)
        {
            var def = assets.getMaterial("engine:default");
            for (var i = 1; i < path.count; i++)
            {
                var p0 = path[0];
                var p1 = path[i];
                var p2 = path[(i + 1) % path.count];

                tri(p0, p1, p2, color: color, depth: depth);
            }
        }

        public void drawPath(GraphicsPath path, Color? color = null, float width = 1f, float depth = 1f)
        {
            var def = assets.getMaterial("engine:default");
            for (var i = 0; i < path.count; i++)
            {
                var l0 = path[(i - 1) % path.count];
                var l1 = path[i];
                var l2 = path[(i + 1) % path.count];
                var l3 = path[(i + 2) % path.count];

                var d0 = l1 - l0;
                var d1 = l2 - l1;
                var d2 = l3 - l2;

                d0.Normalize();
                d1.Normalize();
                d2.Normalize();

                var e0 = d0.Perp();
                var e1 = d1.Perp();
                var e2 = d2.Perp();

                var r0 = e0 + e1;
                var r1 = e1 + e2;

                r0.Normalize();
                r1.Normalize();

                var p0 = l1 + r0 * width;
                var p1 = l1;
                var p2 = l2;
                var p3 = l2 + r1 * width;


                graphics.batch.drawQuad(def.texture, p0, p3, p2, p1, color: color, depth: depth);
            }

            //for (var i = 0; i < path.count; i++)
            //line(path[i], path[(i + 1) % path.count], color: color, width: width, depth: depth);
        }

        #region box
        public void box(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), skin.box);
        }

        public void box(AxisAlignedBox p)
        {
            doBox(p, GUIContent.none, skin.box);
        }

        public void box(AxisAlignedBox p, GUIStyle style)
        {
            doBox(p, GUIContent.none, style);
        }

        public void box(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), style);
        }

        public void box(AxisAlignedBox p, string text)
        {
            doBox(p, new GUIContent(text), skin.box);
        }

        public void box(AxisAlignedBox p, string text, params object[] args)
        {
            doBox(p, new GUIContent(string.Format(text, args)), skin.box);
        }

        public void box(AxisAlignedBox p, string text, GUIStyle style)
        {
            doBox(p, new GUIContent(text), style);
        }

        public void box(AxisAlignedBox p, Material image)
        {
            doBox(p, new GUIContent(image), skin.box);
        }

        public void box(AxisAlignedBox p, Material image, GUIStyle style)
        {
            doBox(p, new GUIContent(image), style);
        }

        public void box(AxisAlignedBox p, GUIContent content)
        {
            doBox(p, content, skin.box);
        }

        public void box(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            doBox(p, content, style);
        }

        private void doBox(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>();
            style.Draw(graphics, p, content, false, false, false, false);
        }
        #endregion

        #region button
        public bool button(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), skin.button);
        }

        public bool button(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), style);
        }

        public bool button(AxisAlignedBox p, string text)
        {
            return doButton(p, new GUIContent(text), skin.button);
        }

        public bool button(AxisAlignedBox p, string text, params object[] args)
        {
            return doButton(p, new GUIContent(string.Format(text, args)), skin.button);
        }

        public bool button(AxisAlignedBox p, string text, GUIStyle style)
        {
            return doButton(p, new GUIContent(text), style);
        }

        public bool button(AxisAlignedBox p, Material image)
        {
            return doButton(p, new GUIContent(image), skin.button);
        }

        public bool button(AxisAlignedBox p, Material image, GUIStyle style)
        {
            return doButton(p, new GUIContent(image), style);
        }

        public bool button(AxisAlignedBox p, GUIContent content)
        {
            return doButton(p, content, skin.button);
        }

        public bool button(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            return doButton(p, content, style);
        }

        private bool doButton(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            var input = CoreRegistry.require<InputSystem>();
            var mp = screenToGUI(input.MousePosition);
            var isHover = p.Contains(mp);
            var isActive = !mouseUsed && isHover && input.IsLeftMouseDown;
            var wasActive = !mouseUsed && isHover && input.WasLeftMousePressed;

            if (isActive || wasActive)
                mouseUsed = true;

            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>();
            style.Draw(graphics, p, content, isHover, isActive, false, false);

            return wasActive;
        }
        #endregion button

        //public GUILayout layout(Vector2 position)
        //{
        //    return layout(position, GUIAnchor.UpperLeft);
        //}

        //public GUILayout layout(Vector2 position, GUIAnchor anchor)
        //{
        //    var layout = new GUILayout();
        //    layout.position = groupOffset + position;
        //    layout.gui = this;
        //    layout.anchor = anchor;

        //    return layout;

        //}

        public Vector2 screenToGUI(Vector2 p)
        {
            return Vector2.Transform(p, camera.inverseViewMatrix);
        }

        public void init()
        {
            //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            defaultFont = assets.getFont("bullethell:arial");
            updateViewport();
            ReCreateViewMatrix();
        }

        public void render()
        {
            camera.scale = new Vector2(2, 2);
            camera.lookThrough();

            var viewMatrix = //Matrix.CreateRotationZ((float)Math.PI) * Matrix.CreateRotationY((float)Math.PI) *
                  Matrix.CreateTranslation(new Vector3(-(int)0, -(int)0, 0)) *
                  Matrix.CreateScale(1, 1, 1);// *
            //Matrix.CreateTranslation(new Vector3(_display.width * 0.5f, _display.height * 0.5f, 0));

            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            if (groups.Count != 0)
                throw new Exception("missing endGroup call");

            if (clips.Count != 0)
                throw new Exception("missing endClip call");

            //var size = new Vector2(display.device.Viewport.Width, display.device.Viewport.Height);
            //graphics.GL.begin(new Atma.MonoGame.Graphics.RenderToScreen(), Atma.Graphics.SortMode.Material, ViewMatrix, viewport);
            //graphics.GL.translate(-size / 2f);
            //graphics.GL.translate
            //updateViewport();

            display.device.BlendState = BlendState.AlphaBlend;
            display.device.DepthStencilState = DepthStencilState.None;
            graphics.begin(SpriteSortMode.Deferred);

            //Event.Invoke("ongui");
            if (onRender != null)
                onRender(this);

            graphics.end();//viewMatrix, viewport);
            //graphics.graphicsDevice.SetRenderTarget(null);
            //graphics.render(ViewMatrix);

            //graphics.GL.end();
            mouseUsed = false;
        }

        //public bool input()
        //{
        //    return false;
        //}

        private void ReCreateViewMatrix()
        {
            updateViewport();
            //viewMatrix = Matrix.CreateScale(new Vector3(new Vector2(viewport.Width, viewport.Height) / normalizedViewSize, 1f));
            viewMatrix = Matrix.CreateScale(new Vector3(/*new Vector2(viewport.Width, viewport.Height) / */normalizedViewSize, 1f));
            //* Matrix.CreateTranslation(new Vector3(_viewport.Width, _viewport.Height, 0) / 2f);
            viewMatrixDirty = false;
        }

        private void updateViewport()
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            //_viewport.Width = (int)(display.device.PresentationParameters.Bounds.Width * _normalizedViewSize.X);
            //_viewport.Height = (int)(display.device.PresentationParameters.Bounds.Height * _normalizedViewSize.Y);
            //_viewport.Width = (int)(20 * _normalizedViewSize.X);
            //_viewport.Height = (int)(20 * _normalizedViewSize.Y);
            _viewport.Width = (int)(camera.scale.X * _normalizedViewSize.X);
            _viewport.Height = (int)(camera.scale.X * _normalizedViewSize.Y);

            if (target == null || target.Width < _viewport.Width || target.Height < _viewport.Height)
            {
                if (target != null)
                    target.Dispose();

                var w = Helpers.NextPow(_viewport.Width);
                var h = Helpers.NextPow(_viewport.Height);

                target = new RenderTarget2D(display.device, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            }
        }

        public static readonly GameUri Uri = "subsystem:gui";

    }


    public sealed class GUIManagerOld : GameSystem
    {
        public GUISkin skin = new GUISkin();

        public readonly GUILayoutOld layout;

        public Color buttonBackground = Color.DarkBlue;
        public Color buttonBackgroundHover = Color.Blue;
        public Color buttonBorder = Color.Wheat;
        public Color buttonBorderHover = Color.White;
        public Color buttonTextColor = Color.Wheat;
        public Color buttonTextColorHover = Color.White;
        public Color labelTextColor = Color.White;
        public AxisAlignedBox lastHitRect = AxisAlignedBox.Null;

        private RenderTarget2D target;
        private Vector2 _normalizedViewSize = Vector2.One;
        private Viewport _viewport = new Viewport();
        private Vector2 buttonPadding = new Vector2(5, 5);
        private Stack<Rectangle> clips = new Stack<Rectangle>();
        internal Font defaultFont;
        private Vector2 groupOffset = Vector2.Zero;
        private Stack<Vector2> groups = new Stack<Vector2>();
        private Matrix viewMatrix;
        private bool viewMatrixDirty = true;

        public event Action<GUIManagerOld> onRender;

        private OrthoCamera camera = new OrthoCamera() { normalizedOrigin = Vector2.Zero };
        //private List<

        //private 

        public bool mouseUsed = false;


        public GUIManagerOld()
        {
            layout = new GUILayoutOld(this);
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

        public Viewport viewport
        {
            get
            {
                if (viewMatrixDirty)
                    ReCreateViewMatrix();
                return _viewport;
            }
        }

        public void beginClip(Vector2 p, Vector2 s)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            var r = new Rectangle((int)(p.X + groupOffset.X), (int)(p.Y + groupOffset.Y), (int)s.X, (int)s.Y);
            if (graphics.scissorEnabled)
                clips.Push(graphics.scissorRect);

            //graphics.beginScissor(r);
        }

        public void beginGroup(Vector2 p)
        {
            groups.Push(p);
            groupOffset += p;
        }

        public bool buttonold(Vector2 p, string text)
        {
            var rect = defaultFont.MeasureString(text);
            return buttonold(0, 1f, p.ToAABB(rect), defaultFont, 0f, text);
        }

        public bool buttonold(AxisAlignedBox rect, string text)
        {
            return buttonold(0, 1f, rect, defaultFont, 0f, text);
        }

        public bool buttonold(int renderQueue, float scale, AxisAlignedBox rect, Font font, float depth, string text)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            rect.SetExtents(rect.Minimum + groupOffset, rect.Maximum + groupOffset + buttonPadding + buttonPadding);
            //var input = CoreRegistry.require<InputSystem>(InputSystem.Uri);
            var mp = input.MousePosition;
            var gp = screenToGUI(mp);

            var isOver = rect.Contains(gp);

            var bg = isOver ? skin.button.hover.texture : skin.button.normal.texture;
            var border = isOver ? buttonBorderHover : buttonBorder;
            var color = isOver ? buttonTextColorHover : buttonTextColor;

            //graphics.beginScissor(rect);
            graphics.Draw(renderQueue, rect, buttonBackground, depth);
            //graphics.batch.drawRect(null, rect.Minimum, rect.Maximum, color: buttonBorder);
            graphics.DrawRect(renderQueue, rect, buttonBorder);
            graphics.DrawText(renderQueue, font, scale, rect.Minimum + buttonPadding, text, color, depth);
            //graphics.endScissor();

            return input.IsLeftMouseDown && isOver;
            //return false;
        }

        public void endClip()
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            if (!graphics.scissorEnabled)
                throw new Exception("called end clip without the scissorrect enabled");

            //graphics.endScissor();
            if (clips.Count > 0)
            {
                var r = clips.Pop();
                //graphics.beginScissor(r);
            }
        }

        public void endGroup()
        {
            var p = groups.Pop();
            groupOffset -= p;
        }

        #region label

        public void label2(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), skin.label);
        }

        public void label2(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doLabel(p, new GUIContent(arg.ToString()), style);
        }

        public void label2(AxisAlignedBox p, string text)
        {
            doLabel(p, new GUIContent(text), skin.label);
        }

        public void label2(AxisAlignedBox p, string text, params object[] args)
        {
            doLabel(p, new GUIContent(string.Format(text, args)), skin.label);
        }

        public void label2(AxisAlignedBox p, string text, GUIStyle style)
        {
            doLabel(p, new GUIContent(text), style);
        }

        public void label2(AxisAlignedBox p, Material image)
        {
            doLabel(p, new GUIContent(image), skin.label);
        }

        public void label2(AxisAlignedBox p, Material image, GUIStyle style)
        {
            doLabel(p, new GUIContent(image), style);
        }

        public void label2(AxisAlignedBox p, GUIContent content)
        {
            doLabel(p, content, skin.label);
        }

        public void label2(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            doLabel(p, content, style);
        }

        private void doLabel(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            style.Draw(graphics, p, content, false, false, false, false);
        }

        #endregion

        #region box
        public void box(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), skin.box);
        }

        public void box(AxisAlignedBox p)
        {
            doBox(p, GUIContent.none, skin.box);
        }

        public void box(AxisAlignedBox p, GUIStyle style)
        {
            doBox(p, GUIContent.none, style);
        }

        public void box(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            doBox(p, new GUIContent(arg.ToString()), style);
        }

        public void box(AxisAlignedBox p, string text)
        {
            doBox(p, new GUIContent(text), skin.box);
        }

        public void box(AxisAlignedBox p, string text, params object[] args)
        {
            doBox(p, new GUIContent(string.Format(text, args)), skin.box);
        }

        public void box(AxisAlignedBox p, string text, GUIStyle style)
        {
            doBox(p, new GUIContent(text), style);
        }

        public void box(AxisAlignedBox p, Material image)
        {
            doBox(p, new GUIContent(image), skin.box);
        }

        public void box(AxisAlignedBox p, Material image, GUIStyle style)
        {
            doBox(p, new GUIContent(image), style);
        }

        public void box(AxisAlignedBox p, GUIContent content)
        {
            doBox(p, content, skin.box);
        }

        public void box(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            doBox(p, content, style);
        }

        private void doBox(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>();
            style.Draw(graphics, p, content, false, false, false, false);
        }
        #endregion

        #region button
        public bool button(AxisAlignedBox p, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), skin.button);
        }

        public bool button(AxisAlignedBox p, object arg, GUIStyle style)
        {
            if (arg == null)
                throw new ArgumentNullException("arg");

            return doButton(p, new GUIContent(arg.ToString()), style);
        }

        public bool button(AxisAlignedBox p, string text)
        {
            return doButton(p, new GUIContent(text), skin.button);
        }

        public bool button(AxisAlignedBox p, string text, params object[] args)
        {
            return doButton(p, new GUIContent(string.Format(text, args)), skin.button);
        }

        public bool button(AxisAlignedBox p, string text, GUIStyle style)
        {
            return doButton(p, new GUIContent(text), style);
        }

        public bool button(AxisAlignedBox p, Material image)
        {
            return doButton(p, new GUIContent(image), skin.button);
        }

        public bool button(AxisAlignedBox p, Material image, GUIStyle style)
        {
            return doButton(p, new GUIContent(image), style);
        }

        public bool button(AxisAlignedBox p, GUIContent content)
        {
            return doButton(p, content, skin.button);
        }

        public bool button(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            return doButton(p, content, style);
        }

        private bool doButton(AxisAlignedBox p, GUIContent content, GUIStyle style)
        {
            var input = CoreRegistry.require<InputSystem>();
            var mp = screenToGUI(input.MousePosition);
            var isHover = p.Contains(mp);
            var isActive = !mouseUsed && isHover && input.IsLeftMouseDown;
            var wasActive = !mouseUsed && isHover && input.WasLeftMousePressed;

            if (isActive || wasActive)
                mouseUsed = true;

            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>();
            style.Draw(graphics, p, content, isHover, isActive, false, false);

            return wasActive;
        }
        #endregion button

        public void label(Vector2 p, string text)
        {
            label(0, 1f, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(Vector2 p, string text, Color color)
        {
            label(0, 1f, p, defaultFont, 0f, color, text);
        }

        public void label(Vector2 p, float scale, string text)
        {
            label(0, scale, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(int renderQueue, float scale, Vector2 p, Font font, float depth, Color color, string text, params object[] args)
        {
            label(renderQueue, scale, p, font ?? defaultFont, depth, color, string.Format(text, args));
        }

        public void label(int renderQueue, float scale, Vector2 p, Font font, float depth, Color color, string text)
        {
            // var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            graphics.DrawText(renderQueue, font ?? defaultFont, scale, p + groupOffset, text, color, depth);
        }

        public void label(int renderQueue, float scale, AxisAlignedBox rect, Font font, float depth, Color color, string text)
        {
            //graphics.beginScissor(rect);
            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            graphics.DrawText(renderQueue, font ?? defaultFont, scale, rect.Minimum + groupOffset, text, color, depth);
            //graphics.endScissor();
        }

        //public GUILayout layout(Vector2 position)
        //{
        //    return layout(position, GUIAnchor.UpperLeft);
        //}

        //public GUILayout layout(Vector2 position, GUIAnchor anchor)
        //{
        //    var layout = new GUILayout();
        //    layout.position = groupOffset + position;
        //    layout.gui = this;
        //    layout.anchor = anchor;

        //    return layout;

        //}

        public Vector2 screenToGUI(Vector2 p)
        {
            return Vector2.Transform(p, Matrix.Invert(ViewMatrix));
        }

        public void init()
        {
            //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            defaultFont = assets.getFont("bullethell:arial");
            updateViewport();
            ReCreateViewMatrix();
        }

        public void render()
        {
            camera.lookThrough();

            var viewMatrix = //Matrix.CreateRotationZ((float)Math.PI) * Matrix.CreateRotationY((float)Math.PI) *
                  Matrix.CreateTranslation(new Vector3(-(int)0, -(int)0, 0)) *
                  Matrix.CreateScale(1, 1, 1);// *
            //Matrix.CreateTranslation(new Vector3(_display.width * 0.5f, _display.height * 0.5f, 0));

            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            if (groups.Count != 0)
                throw new Exception("missing endGroup call");

            if (clips.Count != 0)
                throw new Exception("missing endClip call");

            //var size = new Vector2(display.device.Viewport.Width, display.device.Viewport.Height);
            //graphics.GL.begin(new Atma.MonoGame.Graphics.RenderToScreen(), Atma.Graphics.SortMode.Material, ViewMatrix, viewport);
            //graphics.GL.translate(-size / 2f);
            //graphics.GL.translate
            //updateViewport();

            display.device.BlendState = BlendState.AlphaBlend;
            display.device.DepthStencilState = DepthStencilState.None;

            graphics.begin(SpriteSortMode.Deferred);

            //Event.Invoke("ongui");
            if (onRender != null)
                onRender(this);

            graphics.end();//viewMatrix, viewport);
            //graphics.graphicsDevice.SetRenderTarget(null);
            //graphics.render(ViewMatrix);

            //graphics.GL.end();
            mouseUsed = false;
        }

        //public bool input()
        //{
        //    return false;
        //}

        private void ReCreateViewMatrix()
        {
            updateViewport();
            //viewMatrix = Matrix.CreateScale(new Vector3(new Vector2(viewport.Width, viewport.Height) / normalizedViewSize, 1f));
            viewMatrix = Matrix.CreateScale(new Vector3(/*new Vector2(viewport.Width, viewport.Height) / */normalizedViewSize, 1f));
            //* Matrix.CreateTranslation(new Vector3(_viewport.Width, _viewport.Height, 0) / 2f);
            viewMatrixDirty = false;
        }

        private void updateViewport()
        {

            //var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            _viewport.Width = (int)(display.device.PresentationParameters.Bounds.Width * _normalizedViewSize.X);
            _viewport.Height = (int)(display.device.PresentationParameters.Bounds.Height * _normalizedViewSize.Y);

            if (target == null || target.Width < _viewport.Width || target.Height < _viewport.Height)
            {
                if (target != null)
                    target.Dispose();

                var w = Helpers.NextPow(_viewport.Width);
                var h = Helpers.NextPow(_viewport.Height);

                target = new RenderTarget2D(display.device, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            }
        }

        public static readonly GameUri Uri = "subsystem:gui";

    }
}