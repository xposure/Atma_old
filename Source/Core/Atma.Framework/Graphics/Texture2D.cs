using Atma.Assets;
using Atma.Engine;
using Microsoft.Xna.Framework;
using System.IO;

namespace Atma.Graphics
{
    public class Texture2D : AbstractAsset<TextureData>
    {
        private TextureData _data;
        public Microsoft.Xna.Framework.Graphics.Texture2D texture;

        public Texture2D(AssetUri uri, int width, int height)
            : base(uri)
        {
            var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
            texture = new Microsoft.Xna.Framework.Graphics.Texture2D(graphics.graphicsDevice, width, height);
        }

        public Texture2D(AssetUri uri, TextureData data)
            : base(uri)
        {
            reload(data);
        }

        public override void reload(TextureData data)
        {
            _data = data;
            setup();
        }

        private void setup()
        {
            if (texture != null)
                texture.Dispose();

            using (var ms = new MemoryStream(_data.bytes))
            {
                var graphics = CoreRegistry.require<GraphicSubsystem>(GraphicSubsystem.Uri);
                texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(graphics.graphicsDevice, ms);
            }
        }

        public int width { get { return texture == null ? 0 : texture.Width; } }

        public int height { get { return texture == null ? 0 : texture.Height; } }

        public void getData<T>(ref T[] data)
            where T : struct
        {
            texture.GetData(data);
        }

        public void getData<T>(ref T[] data, int startIndex, int elementCount)
            where T : struct
        {
            texture.GetData(data, startIndex, elementCount);
        }

        public void setData<T>(T[] data)
            where T : struct
        {
            texture.SetData(data);
        }

        public void setData<T>(T[] data, int startIndex, int elementCount)
            where T : struct
        {
            texture.SetData(data, startIndex, elementCount);
        }

        public void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch, Renderable item)
        {
            if (texture != null)
            {
                //var mgl = MonoGL.instance;

                Rectangle srcRectangle;
                if (item.sourceRectangle.IsNull)
                    srcRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                else
                    srcRectangle = item.sourceRectangle.ToRect();

                var _origin = (new Vector2(srcRectangle.Width, srcRectangle.Height) * item.pivot);// +new Vector2(srcRectangle.Value.X, srcRectangle.Value.Y);
                var origin = new Vector2(_origin.X, _origin.Y);

                //var p = item.position + item.scale * _origin;
                var p = item.position;

                //if (item.type == GLRenderableType.Quad)
                p += item.scale * item.pivot;

                var s = item.scale;
                var destRectangle = p.ToRectangle(s);

                var depth = item.depth;
                //if (depthRange > 0f)
                //    depth = (item.depth - _minDepth) / depthRange;

                var color = new Color(item.color.R, item.color.G, item.color.B, item.color.A);
                //color.A = 0;
                batch.Draw(texture,
                    destRectangle,
                    srcRectangle, color, item.rotation, origin,
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f - depth);
            }
        }

        //public void draw(GLRenderable item)
        //{
        //    if (texture != null)
        //    {
        //        //var mgl = MonoGL.instance;

        //        Rectangle srcRectangle;
        //        if (item.sourceRectangle.IsNull)
        //            srcRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        //        else
        //            srcRectangle = item.sourceRectangle.ToRect();

        //        var _origin = (new Vector2(srcRectangle.Width, srcRectangle.Height) * item.pivot);// +new Vector2(srcRectangle.Value.X, srcRectangle.Value.Y);
        //        var origin = new Vector2(_origin.X, _origin.Y);

        //        //var p = item.position + item.scale * _origin;
        //        var p = item.position;

        //        //if (item.type == GLRenderableType.Quad)
        //        p += item.scale * item.pivot;

        //        var s = item.scale;
        //        var destRectangle = p.ToRectangle(s);

        //        var depth = 0f;
        //        if (mgl.depthRange > 0f)
        //            depth = (item.depth - mgl._minDepth) / mgl.depthRange;

        //        var color = new Color(item.color.R, item.color.G, item.color.B, item.color.A);
        //        //color.A = 0;
        //        mgl.batch.Draw(texture,
        //            destRectangle,
        //            srcRectangle, color, item.rotation, origin,
        //            Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 1f - depth);
        //    }
        //}

        protected override void ondispose()
        {
            if (texture != null)
                texture.Dispose();

        }

    }

}
