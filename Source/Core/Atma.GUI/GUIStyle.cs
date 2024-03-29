﻿using Atma.Assets;
using Atma.Engine;
using Atma.Fonts;
using Atma.Graphics;
using Atma.Managers;
using Microsoft.Xna.Framework;

namespace Atma
{
    public class GUIStyle : GameSystem
    {
        public static readonly GUIStyle none = new GUIStyle();

        public string name;
        public float fixedWidth;
        public float fixedHeight;
        public GUIStyleState normal = new GUIStyleState(Color.White);
        public GUIStyleState hover = new GUIStyleState(Color.Wheat);
        public GUIStyleState active = new GUIStyleState(Color.Wheat);
        public GUIStyleState focused = new GUIStyleState(Color.Wheat);
        public GUIStyleState disabled = new GUIStyleState(Color.Gray);
        public GUIStyleState onNormal = new GUIStyleState(Color.White);
        public GUIStyleState onHover = new GUIStyleState(Color.Wheat);
        public GUIStyleState onActive = new GUIStyleState(Color.Wheat);
        public GUIStyleState onFocused = new GUIStyleState(Color.Wheat);
        //public GUIStyleState disabledNormal = new GUIStyleState(Color.Gray);
        //public GUIStyleState disabledHover = new GUIStyleState(Color.Gray);
        //public GUIStyleState disabledActive = new GUIStyleState(Color.Gray);
        public RectOffset border = new RectOffset() { min = Vector2.One * 6, max = Vector2.One * 6 };
        public RectOffset margin;
        public RectOffset padding = new RectOffset() { min = Vector2.One * 6, max = Vector2.One * 6 };
        public RectOffset overflow;
        public GUIAnchor alignment = GUIAnchor.LowerRight;
        public ImagePosition imagePosition = ImagePosition.TextOnly;
        public bool wordWrap = true;
        public TextClipping clipping;
        public Vector2 contentOffset;
        public bool stretchWidth;
        public bool stretchHeight;
        public string fontName;// = "content/fonts/arial.fnt";
        public float fontSize = 1;
        public int lineHeight;

        private Font _font;
        private Font font
        {
            get
            {
                if (string.IsNullOrEmpty(fontName))
                    return CoreRegistry.require<GUIManager>().defaultFont;
                //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
                //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
                if (_font == null)
                    _font = assets.getFont(fontName);
                return _font;
            }
        }

        internal void Draw(Atma.Graphics.GraphicSubsystem graphics, AxisAlignedBox position, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            Draw(graphics, position, GUIContent.none, isHover, isActive, on, hasKeyboardFocus);
        }

        internal void Draw(Atma.Graphics.GraphicSubsystem graphics, AxisAlignedBox position, string text, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            Draw(graphics, position, new GUIContent(text), isHover, isActive, on, hasKeyboardFocus);
        }

        internal void Draw(Atma.Graphics.GraphicSubsystem graphics, AxisAlignedBox position, Material image, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            Draw(graphics, position, new GUIContent(image), isHover, isActive, on, hasKeyboardFocus);
        }

        internal void Draw(Atma.Graphics.GraphicSubsystem graphics, AxisAlignedBox position, GUIContent content, bool isHover, bool isActive, bool on, bool hasKeyboardFocus)
        {
            Draw(graphics, content, new GUIDrawArguments() { position = position, isHover = isHover, isActive = isActive, on = on, hasKeyboardFocus = hasKeyboardFocus });
        }

        internal void Draw(Atma.Graphics.GraphicSubsystem graphics, GUIContent content, GUIDrawArguments drawArgs)
        {
            var size = drawArgs.position.Size;
            if (fixedWidth > 0f)
                size.X = fixedWidth;
            if (fixedHeight > 0f)
                size.Y = fixedHeight;

            drawArgs.content = AxisAlignedBox.FromRect(drawArgs.position.Minimum + padding.min, size - padding.max - padding.min);
            var textSize = Vector2.Zero;
            if (!string.IsNullOrEmpty(content.text))
            {
                if (wordWrap)
                    textSize = font.MeasureString(content.text, drawArgs.content.Size);
                else
                    textSize = font.MeasureString(content.text);
            }

            var remainingWidth = drawArgs.content.Width - textSize.X;
            var remainingHeight = drawArgs.content.Height - textSize.Y;

            var drawIcon = content.icon != null;
            var drawText = !string.IsNullOrEmpty(content.text);

            var state = getState(drawArgs);

            if (remainingWidth <= 0 && imagePosition == ImagePosition.ImageLeft)
            {
                textSize.X = drawArgs.content.Width;
                drawIcon = false;
            }
            else if (remainingHeight <= 0 && imagePosition == ImagePosition.ImageAbove)
            {
                textSize.Y = drawArgs.content.Height;
                drawIcon = false;
            }

            var material = state.material;// Root.instance.resources.createMaterialFromTexture("content/textures/gui/box.png")
            if (material != null)
            {
                if (border.min == Vector2.Zero && border.max == Vector2.Zero)
                    graphics.Draw(material, drawArgs.position, state.backgroundColor);
                else
                {
                    var src = AxisAlignedBox.FromRect(Vector2.Zero, material.textureSize);
                    var dstRects = drawArgs.position.fromRectOffset(border);
                    var srcRects = src.fromRectOffset(border);
                    for (var i = 0; i < dstRects.Length; i++)
                        if (dstRects[i].Width > 0 && dstRects[i].Height > 0)
                            graphics.Draw(material, dstRects[i], srcRects[i], state.backgroundColor);
                }
            }

            if (drawIcon && imagePosition == ImagePosition.ImageLeft)
            {
                AxisAlignedBox icon, text;
                computeTextAndIconLeft(content, drawArgs.content, textSize, remainingWidth, out icon, out text);

                graphics.Draw(content.icon, icon, Color.White);
                font.DrawText(graphics.currentRenderQueue, text.Minimum, this.fontSize, content.text, state.textColor, 0f, text.Width);
            }
            else if (drawIcon && imagePosition == ImagePosition.ImageAbove)
            {
                AxisAlignedBox icon, text;
                computeTextAndIconAbove(content, drawArgs.content, textSize, remainingHeight, out icon, out text);

                graphics.Draw(content.icon, icon, Color.White);
                if (wordWrap)
                    font.DrawWrappedOnWordText(graphics.currentRenderQueue, text.Minimum, this.fontSize, content.text, state.textColor, 0f, text.Size);
                else
                    font.DrawText(graphics.currentRenderQueue, text.Minimum, this.fontSize, content.text, state.textColor, 0f, text.Width);
            }
            else if (drawText && imagePosition != ImagePosition.ImageOnly)
            {
                var position = computePosition(drawArgs.content, textSize);
                if (wordWrap)
                    font.DrawWrappedOnWordText(graphics.currentRenderQueue, position.Minimum, this.fontSize, content.text, state.textColor, 0f, position.Size);
                else
                    font.DrawText(graphics.currentRenderQueue, position.Minimum, this.fontSize, content.text, state.textColor, 0f, position.Width);
            }
            else if (drawIcon && imagePosition != ImagePosition.TextOnly)
            {
                var position = computePosition(drawArgs.content, content.icon.textureSize);
                graphics.Draw(content.icon, position, Color.White);
                graphics.DrawRect(position, Color.Blue);
            }

        }

        internal void computeTextAndIconLeft(GUIContent content, AxisAlignedBox area, Vector2 textSize, float remainingWidth, out AxisAlignedBox icon, out AxisAlignedBox text)
        {
            var textSizeOver2 = textSize / 2;
            var positionSizeOver2 = area.Size / 2;
            var drawPosition = area.Minimum;
            switch (alignment)
            {
                case GUIAnchor.MiddleLeft:
                    drawPosition.Y = area.Y0 + positionSizeOver2.Y - textSizeOver2.Y;
                    break;
                case GUIAnchor.LowerLeft:
                    drawPosition.Y = area.Y1 - textSize.Y;
                    break;
                case GUIAnchor.UpperCenter:
                    drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                    break;
                case GUIAnchor.MiddleCenter:
                    drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                    goto case GUIAnchor.MiddleLeft;
                case GUIAnchor.LowerCenter:
                    drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                    goto case GUIAnchor.LowerLeft;
                case GUIAnchor.UpperRight:
                    drawPosition.X = area.X1 - textSize.X;
                    break;
                case GUIAnchor.MiddleRight:
                    drawPosition.X = area.X1 - textSize.X;
                    goto case GUIAnchor.MiddleLeft;
                case GUIAnchor.LowerRight:
                    drawPosition.X = area.X1 - textSize.X;
                    goto case GUIAnchor.LowerLeft;
            }

            var scaleSize = Vector2.One * Utility.Min(remainingWidth, area.Height);
            var iconSize = Utility.ScaleToSize(content.icon.textureSize, scaleSize);

            var iconSizeOver2 = iconSize / 2;
            var iconPosition = area.Minimum;
            switch (alignment)
            {
                case GUIAnchor.UpperLeft:
                    drawPosition.X += iconSize.X;
                    drawPosition.Y += iconSizeOver2.Y - textSizeOver2.Y;
                    break;
                case GUIAnchor.MiddleLeft:
                    drawPosition.X += iconSize.X;
                    iconPosition.Y += positionSizeOver2.Y - iconSizeOver2.Y;
                    break;
                case GUIAnchor.LowerLeft:
                    drawPosition.X += iconSize.X;
                    iconPosition.Y += area.Height - iconSize.Y;
                    drawPosition.Y -= iconSizeOver2.Y - textSizeOver2.Y;
                    break;
                case GUIAnchor.UpperRight:
                    drawPosition.Y += iconSizeOver2.Y - textSizeOver2.Y;
                    iconPosition.X = drawPosition.X - iconSize.X;
                    break;
                case GUIAnchor.MiddleRight:
                    iconPosition.X = drawPosition.X - iconSize.X;
                    iconPosition.Y += positionSizeOver2.Y - iconSizeOver2.Y;
                    break;
                case GUIAnchor.LowerRight:
                    iconPosition.X = drawPosition.X - iconSize.X;
                    iconPosition.Y += area.Height - iconSize.Y;
                    drawPosition.Y -= iconSizeOver2.Y - textSizeOver2.Y;
                    break;
                case GUIAnchor.UpperCenter:
                    {
                        var size = (area.Width - (iconSize.X + textSize.X)) / 2;

                        iconPosition.X += size;
                        drawPosition.X = iconPosition.X + iconSize.X;
                        drawPosition.Y += iconSizeOver2.Y - textSizeOver2.Y;
                        break;
                    }
                case GUIAnchor.MiddleCenter:
                    {
                        var size = (area.Width - (iconSize.X + textSize.X)) / 2;

                        iconPosition.X += size;
                        drawPosition.X = iconPosition.X + iconSize.X;
                        iconPosition.Y += positionSizeOver2.Y - iconSizeOver2.Y;
                        break;
                    }
                case GUIAnchor.LowerCenter:
                    {
                        var size = (area.Width - (iconSize.X + textSize.X)) / 2;

                        iconPosition.X += size;
                        drawPosition.X = iconPosition.X + iconSize.X;
                        iconPosition.Y += area.Height - iconSize.Y;
                        drawPosition.Y -= iconSizeOver2.Y - textSizeOver2.Y;
                        break;
                    }
            }

            icon = AxisAlignedBox.FromRect(iconPosition + contentOffset, iconSize);
            text = AxisAlignedBox.FromRect(drawPosition + contentOffset, textSize);
        }

        internal void computeTextAndIconAbove(GUIContent content, AxisAlignedBox area, Vector2 textSize, float remainingHeight, out AxisAlignedBox icon, out AxisAlignedBox text)
        {
            var textSizeOver2 = textSize / 2;
            var positionSizeOver2 = area.Size / 2;
            var drawPosition = area.Minimum;
            var scaleSize = Vector2.One * Utility.Min(remainingHeight, area.Height);
            var iconSize = Utility.ScaleToSize(content.icon.textureSize, scaleSize);
            var iconSizeOver2 = iconSize / 2;
            var iconPosition = area.Minimum;

            switch (alignment)
            {
                case GUIAnchor.UpperLeft:
                    drawPosition.Y += iconSize.Y;
                    break;
                case GUIAnchor.MiddleLeft:
                    {
                        var size = (area.Height - (iconSize.Y + textSize.Y)) / 2;

                        iconPosition.Y += size;
                        drawPosition.Y = iconPosition.Y + iconSize.Y;
                    }
                    break;
                case GUIAnchor.LowerLeft:
                    drawPosition.Y = area.Y1 - textSize.Y;
                    iconPosition.Y = drawPosition.Y - iconSize.Y;
                    break;
                case GUIAnchor.UpperRight:
                    drawPosition.X = area.X1 - textSize.X;
                    drawPosition.Y += iconSize.Y;
                    iconPosition.X += area.Width - iconSize.X;
                    break;
                case GUIAnchor.MiddleRight:
                    {
                        var sizeH = (area.Height - (iconSize.Y + textSize.Y)) / 2;

                        drawPosition.X = area.X1 - textSize.X;
                        iconPosition.X = area.X1 - iconSize.X;
                        iconPosition.Y += sizeH;
                        drawPosition.Y = iconPosition.Y + iconSize.Y;

                    }
                    break;
                case GUIAnchor.LowerRight:
                    drawPosition.Y = area.Y1 - textSize.Y;
                    drawPosition.X = area.X1 - textSize.X;
                    iconPosition.X = area.X1 - iconSize.X;
                    iconPosition.Y = drawPosition.Y - iconSize.Y;
                    break;
                case GUIAnchor.UpperCenter:
                    drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                    iconPosition.X += positionSizeOver2.X - iconSizeOver2.X;
                    drawPosition.Y += iconSize.Y;
                    break;
                case GUIAnchor.MiddleCenter:
                    {
                        var sizeH = (area.Height - (iconSize.Y + textSize.Y)) / 2;

                        iconPosition.X += positionSizeOver2.X - iconSizeOver2.X;
                        iconPosition.Y += sizeH;
                        drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                        drawPosition.Y = iconPosition.Y + iconSize.Y;

                        break;
                    }
                case GUIAnchor.LowerCenter:
                    drawPosition.Y = area.Y1 - textSize.Y;
                    drawPosition.X = area.X0 + positionSizeOver2.X - textSizeOver2.X;
                    iconPosition.X += positionSizeOver2.X - iconSizeOver2.X;
                    iconPosition.Y = drawPosition.Y - iconSize.Y;
                    break;
            }

            switch (alignment)
            {
                case GUIAnchor.LowerLeft:
                case GUIAnchor.MiddleLeft:
                case GUIAnchor.UpperLeft:
                    if (textSize.X > iconSize.X)
                        iconPosition.X += textSizeOver2.X - iconSizeOver2.X;
                    else
                        drawPosition.X += iconSizeOver2.X - textSizeOver2.X;

                    break;
                case GUIAnchor.LowerRight:
                case GUIAnchor.MiddleRight:
                case GUIAnchor.UpperRight:
                    if (textSize.X > iconSize.X)
                        iconPosition.X -= textSizeOver2.X - iconSizeOver2.X;
                    else
                        drawPosition.X -= iconSizeOver2.X - textSizeOver2.X;

                    break;
            }




            icon = AxisAlignedBox.FromRect(iconPosition + contentOffset, iconSize);
            text = AxisAlignedBox.FromRect(drawPosition + contentOffset, textSize);
        }

        internal AxisAlignedBox computePosition(AxisAlignedBox area, Vector2 size)
        {
            var sizeOver2 = size / 2;
            var positionSizeOver2 = area.Size / 2;
            var position = area.Minimum;
            switch (alignment)
            {
                case GUIAnchor.MiddleLeft:
                    position.Y = area.Y0 + positionSizeOver2.Y - sizeOver2.Y;
                    break;
                case GUIAnchor.LowerLeft:
                    position.Y = area.Y1 - size.Y;
                    break;
                case GUIAnchor.UpperCenter:
                    position.X = area.X0 + positionSizeOver2.X - sizeOver2.X;
                    break;
                case GUIAnchor.MiddleCenter:
                    position.X = area.X0 + positionSizeOver2.X - sizeOver2.X;
                    goto case GUIAnchor.MiddleLeft;
                case GUIAnchor.LowerCenter:
                    position.X = area.X0 + positionSizeOver2.X - sizeOver2.X;
                    goto case GUIAnchor.LowerLeft;
                case GUIAnchor.UpperRight:
                    position.X = area.X1 - size.X;
                    break;
                case GUIAnchor.MiddleRight:
                    position.X = area.X1 - size.X;
                    goto case GUIAnchor.MiddleLeft;
                case GUIAnchor.LowerRight:
                    position.X = area.X1 - size.X;
                    goto case GUIAnchor.LowerLeft;
            }

            return AxisAlignedBox.FromRect(position + contentOffset, size);
        }

        internal GUIStyleState getState(GUIDrawArguments drawArgs)
        {
            if (drawArgs.on)
            {
                if (drawArgs.hasKeyboardFocus)
                    return onFocused;
                else if (drawArgs.isActive)
                    return onActive;
                else if (drawArgs.isHover)
                    return onHover;
                else
                    return onNormal;
            }
            else
            {
                if (drawArgs.hasKeyboardFocus)
                    return focused;
                else if (drawArgs.isActive)
                    return active;
                else if (drawArgs.isHover)
                    return hover;
            }

            return normal;
        }

        public Vector2 calcSize(GUIContent content)
        {            
            var drawText = !string.IsNullOrEmpty(content.text);
            var textSize = Vector2.Zero;
            if (drawText)
                textSize = font.MeasureString(content.text);

            var drawIcon = content.icon != null;
            var iconSize = Vector2.Zero;
            if (drawIcon)
                iconSize = content.icon.textureSize;

            if (drawIcon && imagePosition == ImagePosition.ImageLeft)
                return new Vector2(iconSize.X + textSize.X, Utility.Max(iconSize.Y, textSize.Y));
            else if (drawIcon && imagePosition == ImagePosition.ImageAbove)
                return new Vector2(Utility.Max(iconSize.X, textSize.X), iconSize.Y + textSize.Y);
            else if (drawText && imagePosition != ImagePosition.ImageOnly)
                return textSize;
            else if (drawIcon && imagePosition != ImagePosition.TextOnly)
                return iconSize;

            return Vector2.Zero;
        }
      
        public void CalcMinMaxWidth(GUIContent content, out float minWidth, out float maxWidth)
        {
            minWidth = fixedWidth;
            maxWidth = fixedWidth;

            if (fixedWidth != 0)
                return;

            var drawText = !string.IsNullOrEmpty(content.text);
            var textSize = Vector2.Zero;
            if (drawText)
                textSize = font.MeasureString(content.text);

            var drawIcon = content.icon != null;
            var iconSize = Vector2.Zero;
            if (drawIcon)
                iconSize = content.icon.textureSize;

            if (drawIcon && imagePosition == ImagePosition.ImageLeft && drawText)
            {
                minWidth = iconSize.X + textSize.X;
                maxWidth = minWidth;
            }
            else if (drawIcon && imagePosition == ImagePosition.ImageAbove && drawText)
            {
                minWidth = font.MeasureMinWrappedString(content.text).X;
                maxWidth = textSize.X;

                minWidth = Utility.Max(minWidth, iconSize.X);
                maxWidth = Utility.Max(maxWidth, iconSize.X);
            }
            else if (drawText && imagePosition != ImagePosition.ImageOnly)
            {
                minWidth = font.MeasureMinWrappedString(content.text).X;
                maxWidth = textSize.X;
            }
            else if (drawIcon && imagePosition != ImagePosition.TextOnly)
            {
                minWidth = iconSize.X;
                maxWidth = iconSize.X;
            }
            
        }

        public float CalcHeight(GUIContent content, float width)
        {
            if (fixedHeight != 0f)
                return fixedHeight;

            var textSize = Vector2.Zero;
            if (!string.IsNullOrEmpty(content.text))
            {
                if (wordWrap)
                    textSize = font.MeasureString(content.text, new Vector2(width, 0));
                else
                    textSize = font.MeasureString(content.text);
            }

            var remainingWidth = width - textSize.X;

            var drawIcon = content.icon != null;
            var drawText = !string.IsNullOrEmpty(content.text);
            
            var iconSize = Vector2.Zero;
            if (drawIcon)
                iconSize = content.icon.textureSize;

            if (remainingWidth <= 0 && imagePosition == ImagePosition.ImageLeft)
            {
                textSize.X = width;
                drawIcon = false;
            }

            if (drawIcon && imagePosition == ImagePosition.ImageLeft)
                return Utility.Max(textSize.Y, iconSize.Y);
            else if (drawIcon && imagePosition == ImagePosition.ImageAbove)
                return iconSize.Y + textSize.Y;
            else if (drawText && imagePosition != ImagePosition.ImageOnly)
                return textSize.Y;
            else if (drawIcon && imagePosition != ImagePosition.TextOnly)
                return iconSize.Y;
            
            return 0f;
        }

        public Vector2 CalcSize(GUIContent content, float width)
        {
            var height = CalcHeight(content, width);

            return new Vector2(width, height) + padding.size;
        }

        public bool isHeightDependantOnWidth { get { return fixedHeight == 0f && this.wordWrap && this.imagePosition != ImagePosition.ImageOnly; } }


        //public static GUIStyle 
        //public Vector2 CalcScreenSize(Vector2 contentSize)
        //{
        //    return new Vector2((this.fixedWidth == 0f) ? Utility.Ceiling(contentSize.X + this.padding.xWidth) : this.fixedWidth, (this.fixedHeight == 0f) ? Utility.Ceiling(contentSize.Y + this.padding.yHeight) : this.fixedHeight);
        //}

        public static GUIStyle defaultLabel
        {
            get
            {
                var style = new GUIStyle();
                style.name = "label";
                style.normal = new GUIStyleState(new Color(230, 230, 230, 256));
                style.margin = new RectOffset(4, 4, 4, 4);
                style.padding = new RectOffset(0, 0, 3, 3);
                style.alignment = GUIAnchor.UpperLeft;
                style.wordWrap = true;
                style.clipping = TextClipping.Clip;
                style.imagePosition = ImagePosition.ImageLeft;
                style.stretchWidth = true;
                style.stretchHeight = false;

                return style;
            }
        }

        public static GUIStyle defaultBox
        {
            get
            {
                var style = new GUIStyle();
                style.name = "button";
                style.normal = new GUIStyleState("content/textures/gui/box.png", new Color(230, 230, 230, 255), new Color(64, 64, 192, 255));
                style.border = new RectOffset(6, 6, 6, 6);
                style.margin = new RectOffset(4, 4, 4, 4);
                style.padding = new RectOffset(4, 4, 4, 4);
                style.alignment = GUIAnchor.UpperCenter;
                style.wordWrap = true;
                style.clipping = TextClipping.Clip;
                style.imagePosition = ImagePosition.ImageLeft;
                style.stretchWidth = true;
                style.stretchHeight = false;

                return style;
            }
        }

        public static GUIStyle defaultButton
        {
            get
            {
                var style = new GUIStyle();
                style.name = "button";
                style.normal = new GUIStyleState("content/textures/gui/button.png", new Color(230, 230, 230, 255), new Color(64, 64, 192, 255));
                style.hover = new GUIStyleState("content/textures/gui/button-hover.png", new Color(255, 255, 255, 255), new Color(64, 64, 224, 255));
                style.active = new GUIStyleState("content/textures/gui/button-on-active.png", new Color(230, 230, 230, 255), new Color(64, 64, 255, 255));
                style.onNormal = new GUIStyleState("content/textures/gui/button-on.png", new Color(230, 230, 230, 255), new Color(64, 64, 192, 255));
                style.onHover = new GUIStyleState("content/textures/gui/button-on-hover.png", new Color(230, 230, 230, 255), new Color(64, 64, 224, 255));
                style.onActive = new GUIStyleState("content/textures/gui/button-on-active.png", new Color(230, 230, 230, 255), new Color(64, 64, 255, 255));
                style.border = new RectOffset(6, 6, 6, 4);
                style.margin = new RectOffset(4, 4, 4, 4);
                style.padding = new RectOffset(6, 6, 3, 3);
                style.alignment = GUIAnchor.MiddleCenter;
                style.wordWrap = false;
                style.clipping = TextClipping.Clip;
                style.imagePosition = ImagePosition.ImageLeft;
                style.stretchWidth = true;
                style.stretchHeight = false;

                return style;
            }
        }

        private static GUIStyle _spaceStyle;
        internal static GUIStyle spaceStyle
        {
            get
            {
                if (_spaceStyle == null)
                {
                    _spaceStyle = new GUIStyle();
                    _spaceStyle.stretchWidth = false;
                }
                return _spaceStyle;
            }
        }
    }
}
