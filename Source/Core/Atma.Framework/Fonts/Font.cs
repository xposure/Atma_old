using Atma.Engine;
using Atma.Graphics;
using Atma.Managers;
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
    public class Font : AbstractAsset<FontData>
    {
        private Dictionary<char, FontCharacter> _characterMap;
        private FontData _fontFile;

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

        public int MaxLineHeight { get; private set; }

        //internal void DrawText(int renderQueue, Vector2 pos, float scale, string text, Color color)
        //{
        //    DrawText(renderQueue, pos, scale, text, color, 0);
        //}

        public bool CanFit(string text, float width)
        {
            return false;
        }

        public Vector2 MeasureString(string text, Vector2 maxSize)
        {
            var size = Vector2.Zero;
            var currentSize = Vector2.Zero;
            var current = 0;

            while (current != -1 && current < text.Length)
            {
                current = FindWordIndexFromBounds(current, maxSize.X, text, out currentSize);

                if (size.X < currentSize.X)
                    size.X = currentSize.X;

                size.Y += currentSize.Y;
            }

            return size;
        }

        public Vector2 MeasureMinWrappedString(string text)
        {
            var size = Vector2.Zero;
            var width = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];

                FontCharacter fc;
                if (_characterMap.TryGetValue(c, out fc))
                {
                    width += fc.XAdvance;

                    if (c == ' ')
                    {
                        size.Y = MaxLineHeight;
                        if (width > size.X)
                            size.X = width;
                    }
                }
            }

            return size;
        }

        public Vector2 MeasureString(string text)
        {
            float dx = 0;
            float dy = 0;
            foreach (char c in text)
            {
                FontCharacter fc;
                if (_characterMap.TryGetValue(c, out fc))
                {
                    dx += fc.XAdvance;
                    if (fc.Height + fc.YOffset > dy)
                        dy = fc.Height + fc.YOffset;
                }
            }
            return new Vector2(dx, dy);
        }

        public int FindWordIndexFromBounds(int start, float width, string text, out Vector2 size)
        {
            var index = -1;
            size = Vector2.Zero;

            for (int i = start; i < text.Length; i++)
            {
                var c = text[i];

                FontCharacter fc;
                if (_characterMap.TryGetValue(c, out fc))
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

        public void DrawText(int renderQueue, Vector2 pos, float scale, string text, Color color, float depth)
        {
            DrawText(renderQueue, pos, scale, text, color, depth, null);
        }

        public void DrawText(int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, float? width)
        {
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);
            foreach (char c in text)
            {
                FontCharacter fc;
                //Root.instance.graphics.GL.push();
                //Root.instance.graphics.GL.material(_material);
                //Root.instance.graphics.GL.depth(depth);
                if (_characterMap.TryGetValue(c, out fc))
                {
                    if (width.HasValue && dx + fc.XAdvance * scale > width.Value + pos.X)
                        break;

                    var sourceRectangle = AxisAlignedBox.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                    var destRectangle = AxisAlignedBox.FromRect(dx + fc.XOffset * scale, dy + fc.YOffset * scale, fc.Width * scale, fc.Height * scale);
                    //Root.instance.graphics.GL.source(sourceRectangle);
                    //Root.instance.graphics.GL.quad(destRectangle);
                    //var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    Root.instance.graphics.Draw(renderQueue, _material[fc.Page], destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);
                    //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
                    dx += fc.XAdvance * scale;

                }
                //Root.instance.graphics.GL.pop();
            }
        }

        public void DrawWrappedOnWordText(int renderQueue, Vector2 pos, float scale, string text, Color color, float depth, Vector2 size)
        {
            float dx = (float)Math.Floor(pos.X);
            float dy = (float)Math.Floor(pos.Y);

            var currentSize = Vector2.Zero;
            var current = 0;

            while (current != -1 && current < text.Length)
            {
                var start = current;
                current = FindWordIndexFromBounds(current, size.X, text, out currentSize);

                if (current > 0)
                {
                    for (int i = start; i < current; i++)
                    {
                        var c = text[i];
                        FontCharacter fc;
                        if (_characterMap.TryGetValue(c, out fc))
                        {
                            var sourceRectangle = AxisAlignedBox.FromRect(fc.X, fc.Y, fc.Width, fc.Height);
                            var destRectangle = AxisAlignedBox.FromRect(dx + fc.XOffset * scale, dy + fc.YOffset * scale, fc.Width * scale, fc.Height * scale);
                            //var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                            Root.instance.graphics.Draw(renderQueue, _material[fc.Page], destRectangle, sourceRectangle, color, 0f, new Vector2(0f, 0f), SpriteEffects.None, depth);
                            //spriteBatch.Draw(_texture, position, sourceRectangle, Color.White);
                            dx += fc.XAdvance * scale;
                        }
                    }

                    dx = (float)Math.Floor(pos.X);
                    dy += MaxLineHeight;
                }

                size.Y += currentSize.Y;
            }
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

            _characterMap = new Dictionary<char, FontCharacter>();

            foreach (var fontCharacter in _fontFile.Chars)
            {
                char c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
                if (fontCharacter.Height + fontCharacter.YOffset > MaxLineHeight)
                    MaxLineHeight = fontCharacter.Height + fontCharacter.YOffset;
            }
        }

        protected override void ondispose()
        {
        }
    }


}