using Atma.Engine;
using Atma.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Atma.Fonts;
using Atma.Assets;

namespace Atma.Fonts
{
    public static class FontExentions
    {
        //internal void DrawText(int renderQueue, Vector2 pos, float scale, string text, Color color)
        //{
        //    DrawText(renderQueue, pos, scale, text, color, 0);
        //}

        public static bool CanFit(this IFont font, string text, float width)
        {
            return false;
        }

        public static Vector2 MeasureString(this IFont font, string text, Vector2 maxSize)
        {
            var size = Vector2.Zero;
            var currentSize = Vector2.Zero;
            var current = 0;

            while (current != -1 && current < text.Length)
            {
                current = font.FindWordIndexFromBounds(current, maxSize.X, text, out currentSize);

                if (size.X < currentSize.X)
                    size.X = currentSize.X;

                size.Y += currentSize.Y;
            }

            return size;
        }

        public static Vector2 MeasureMinWrappedString(this IFont font, string text)
        {
            var size = Vector2.Zero;
            var width = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                var fc = font.get(c);
                if (fc != null)
                {
                    width += fc.XAdvance;

                    if (c == ' ')
                    {
                        size.Y = font.getLineHeight();
                        if (width > size.X)
                            size.X = width;
                    }
                }
            }

            return size;
        }

        public static Vector2 MeasureString(this IFont font, string text)
        {
            float dx = 0;
            float dy = 0;
            foreach (char c in text)
            {
                var fc = font.get(c);
                if (fc != null)
                {
                    dx += fc.XAdvance;
                    if (fc.Height + fc.YOffset > dy)
                        dy = fc.Height + fc.YOffset;
                }
            }
            return new Vector2(dx, dy);
        }

        public static int FindWordIndexFromBounds(this IFont font, int start, float width, string text, out Vector2 size)
        {
            var index = -1;
            size = Vector2.Zero;

            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];

                var fc = font.get(c);
                if (fc != null)
                {
                    if (size.X + fc.XAdvance > width)
                        return index;

                    size.X += fc.XAdvance;

                    if (c == ' ')
                        index = i + 1;

                    if (fc.Height + fc.YOffset > size.Y)
                        size.Y = fc.Height + fc.YOffset;
                }
            }

            index = text.Length;

            return index;
        }

        public static void DrawText(this IFont font, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth)
        {
            font.DrawText(renderQueue, pos, scale, text, color, depth, null);
        }

        public static void DrawText(this IFont font, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, float? width)
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);
            foreach (char c in text)
            {
                var fc = font.get(c);
                if (fc != null)
                {
                    if (width.HasValue && dx + fc.XAdvance * scale > width.Value + pos.X)
                        break;

                    var sourceRectangle = AxisAlignedBox.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                    var destRectangle = AxisAlignedBox.FromRect(dx + fc.XOffset * scale, dy + fc.YOffset * scale, fc.Width * scale, fc.Height * scale);
                    //graphics.GL.source(sourceRectangle);
                    //graphics.GL.quad(destRectangle);
                    //var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    graphics.Draw(renderQueue, fc.material, destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);
                    //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
                    dx += fc.XAdvance * scale;

                }
                //graphics.GL.pop();
            }
        }

        public static void DrawWrappedOnWordText(this IFont font, int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, Vector2 size)
        {
            var graphics = CoreRegistry.require<Atma.Graphics.GraphicSubsystem>(Atma.Graphics.GraphicSubsystem.Uri);
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);

            var currentSize = Vector2.Zero;
            var current = 0;

            while (current != -1 && current < text.Length)
            {
                var start = current;
                current = font.FindWordIndexFromBounds(current, size.X, text, out currentSize);

                if (current > 0)
                {
                    for (int i = start; i < current; i++)
                    {
                        var c = text[i];
                        var fc = font.get(c);
                        if (fc != null)
                        {
                            var sourceRectangle = AxisAlignedBox.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                            var destRectangle = AxisAlignedBox.FromRect(dx + fc.XOffset * scale, dy + fc.YOffset * scale, fc.Width * scale, fc.Height * scale);
                            //var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                            graphics.Draw(renderQueue, fc.material, destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);
                            //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
                            dx += fc.XAdvance * scale;
                        }
                    }

                    dx = (float)Math.Floor(pos.X);
                    dy += font.getLineHeight();
                }

                size.Y += currentSize.Y;
            }
        }

    }

    public class Font : AbstractAsset<FontData>, IFont
    {
        private Dictionary<char, FontCharacter> _characterMap;
        private FontData _fontFile;

        private int maxHeight = 0;
        private Material[] _material;
        //private TextureRef _texture;
        private AssetManager _manager;

        public Font(AssetUri uri, FontData fontFile, AssetManager manager/*, string fontTexture*/)
            : base(uri)
        {
            _fontFile = fontFile;
            _manager = manager;

            reload(fontFile);

        }

        public override void reload(FontData t)
        {
            _material = new Material[_fontFile.Pages.Count];

            var mdata = new MaterialData();
            mdata.SetBlendState(BlendState.Opaque);
            mdata.SetSamplerState(SamplerState.PointClamp);

            for (var i = 0; i < _material.Length; i++)
            {
                var file = t.Pages[i].File;
                var index = file.LastIndexOf('.');
                if (index > -1)
                    file = file.Substring(0, index);

                mdata.texture = new GameUri(uri.moduleName, file);
                _material[i] = _manager.createMaterial(new GameUri(uri.moduleName, uri.objectName + "_" + i), mdata);
            }

            //_material = CoreRegistry.require<ResourceManager>(ResourceManager.Uri).createMaterialFromTexture(fontTexture, fontTexture);
            //_material.SetBlendState( BlendState.NonPremultiplied);
            //_material.textureName = fontTexture;

            //_texture = Root.instance.resources.findTexture(fontTexture);

            maxHeight = 0;
            _characterMap = new Dictionary<char, FontCharacter>();
            foreach (var fontCharacter in _fontFile.Chars)
            {
                var c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
                fontCharacter.material = _material[fontCharacter.Page];
                if (fontCharacter.Height + fontCharacter.YOffset > maxHeight)
                    maxHeight = fontCharacter.Height + fontCharacter.YOffset;
            }
        }

        protected override void ondispose()
        {
        }

        public int getWidth(string text)
        {
            var w = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var ch = get(text[i]);
                if (ch != null)
                    w += ch.XAdvance;
            }

            return w;
        }

        public int getLineHeight()
        {
            return maxHeight;
        }

        public bool has(char ch)
        {
            return _characterMap.ContainsKey(ch);
        }

        public FontCharacter get(char ch)
        {
            return _characterMap.get(ch);
        }
    }


}