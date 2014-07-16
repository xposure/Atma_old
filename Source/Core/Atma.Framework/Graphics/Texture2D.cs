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

        public void enable()
        {
            texture.GraphicsDevice.Textures[0] = this.texture;
        }

        protected override void ondispose()
        {
            if (texture != null)
                texture.Dispose();

        }

    }

}
