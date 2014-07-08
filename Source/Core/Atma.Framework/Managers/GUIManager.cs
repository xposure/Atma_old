using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using zSprite.Resources;

namespace zSprite.Managers
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

    public sealed class GUIManager
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
        internal BmFont defaultFont;
        private Vector2 groupOffset = Vector2.Zero;
        private Stack<Vector2> groups = new Stack<Vector2>();
        private Matrix viewMatrix;
        private bool viewMatrixDirty = true;



        //private List<

        //private 

        public bool mouseUsed = false;


        internal GUIManager()
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
            var r = new Rectangle((int)(p.X + groupOffset.X), (int)(p.Y + groupOffset.Y), (int)s.X, (int)s.Y);
            if (Root.instance.graphics.scissorEnabled)
                clips.Push(Root.instance.graphics.scissorRect);

            Root.instance.graphics.beginScissor(r);
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

        public bool buttonold(int renderQueue, float scale, AxisAlignedBox rect, BmFont font, float depth, string text)
        {
            rect.SetExtents(rect.minVector + groupOffset, rect.maxVector + groupOffset + buttonPadding + buttonPadding);
            var mp = Root.instance.input.MousePosition;
            var gp = screenToGUI(mp);

            var isOver = rect.Contains(gp);

            var bg = isOver ? skin.button.hover.texture : skin.button.normal.texture;
            var border = isOver ? buttonBorderHover : buttonBorder;
            var color = isOver ? buttonTextColorHover : buttonTextColor;

            //Root.instance.graphics.beginScissor(rect);
            Root.instance.graphics.Draw(renderQueue, rect, buttonBackground, depth);
            Root.instance.graphics.DrawRect(renderQueue, rect, buttonBorder);
            Root.instance.graphics.DrawText(renderQueue, font, scale, rect.minVector + buttonPadding, text, color, depth);
            //Root.instance.graphics.endScissor();

            return Root.instance.input.IsLeftMouseDown && isOver;
            return false;
        }

        public void endClip()
        {
            if (!Root.instance.graphics.scissorEnabled)
                throw new Exception("called end clip without the scissorrect enabled");

            Root.instance.graphics.endScissor();
            if (clips.Count > 0)
            {
                var r = clips.Pop();
                Root.instance.graphics.beginScissor(r);
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
            style.Draw(Root.instance.graphics, p, content, false, false, false, false);
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
            style.Draw(Root.instance.graphics, p, content, false, false, false, false);
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
            var mp = screenToGUI(Root.instance.input.MousePosition);
            var isHover = p.Contains(mp);
            var isActive = !mouseUsed && isHover && Root.instance.input.IsLeftMouseDown;
            var wasActive = !mouseUsed && isHover && Root.instance.input.WasLeftMousePressed;

            if (isActive || wasActive)
                mouseUsed = true;

            style.Draw(Root.instance.graphics, p, content, isHover, isActive, false, false);

            return wasActive;
        }
        #endregion button

        public void label(Vector2 p, string text)
        {
            label(0, 1f, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(Vector2 p, float scale, string text)
        {
            label(0, scale, p, defaultFont, 0f, skin.label.normal.textColor, text);
        }

        public void label(int renderQueue, float scale, Vector2 p, BmFont font, float depth, Color color, string text, params object[] args)
        {
            label(renderQueue, scale, p, font ?? defaultFont, depth, color, string.Format(text, args));
        }

        public void label(int renderQueue, float scale, Vector2 p, BmFont font, float depth, Color color, string text)
        {
            Root.instance.graphics.DrawText(renderQueue, font ?? defaultFont, scale, p + groupOffset, text, color, depth);
        }

        public void label(int renderQueue, float scale, AxisAlignedBox rect, BmFont font, float depth, Color color, string text)
        {
            Root.instance.graphics.beginScissor(rect);
            Root.instance.graphics.DrawText(renderQueue, font ?? defaultFont, scale, rect.minVector + groupOffset, text, color, depth);
            Root.instance.graphics.endScissor();
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

        internal void init()
        {
            defaultFont = Root.instance.resources.findFont("content/fonts/arial.fnt");
            updateViewport();
            ReCreateViewMatrix();
        }

        internal void render()
        {
            if (groups.Count != 0)
                throw new Exception("missing endGroup call");

            if (clips.Count != 0)
                throw new Exception("missing endClip call");

            Event.Invoke("ongui");
            updateViewport();
            Root.instance.graphics.graphicsDevice.SetRenderTarget(null);
            Root.instance.graphics.graphicsDevice.Viewport = viewport;
            Root.instance.graphics.render(ViewMatrix);

            mouseUsed = false;
        }

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
            _viewport.Width = (int)(Root.instance.graphics.graphicsDevice.PresentationParameters.Bounds.Width * _normalizedViewSize.X);
            _viewport.Height = (int)(Root.instance.graphics.graphicsDevice.PresentationParameters.Bounds.Height * _normalizedViewSize.Y);

            if (target == null || target.Width < _viewport.Width || target.Height < _viewport.Height)
            {
                if (target != null)
                    target.Dispose();

                var w = Helpers.NextPow(_viewport.Width);
                var h = Helpers.NextPow(_viewport.Height);

                target = new RenderTarget2D(Root.instance.graphics.graphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            }
        }
    }
}