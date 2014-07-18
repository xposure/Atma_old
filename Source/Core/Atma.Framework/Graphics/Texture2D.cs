using Atma.Assets;
using Atma.Engine;
using Microsoft.Xna.Framework;
using System;
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
            var display = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri);
            texture = new Microsoft.Xna.Framework.Graphics.Texture2D(display.device, width, height);
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
                var display = CoreRegistry.require<DisplayDevice>(DisplayDevice.Uri);
                texture = Microsoft.Xna.Framework.Graphics.Texture2D.FromStream(display.device, ms);
            }
        }

        public int width { get { return texture == null ? 0 : texture.Width; } }

        public int height { get { return texture == null ? 0 : texture.Height; } }

        public Vector2 size { get { return new Vector2(width, height); } }

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

        public static Texture2D createMetaball(AssetUri uri, int radius, Func<float, int, int, float> falloff, Func<float, int, int, Color> colorPicker)
        {
            int length = radius * 2;
            var colors = new Color[length * length];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    float distance = Vector2.Distance(Vector2.One,
                        new Vector2(x, y) / radius);
                    float alpha = falloff(distance, x, y);

                    var color = colorPicker(distance, x, y);
                    color = Color.FromNonPremultiplied(color.R, color.G, color.B, (byte)MathHelper.Clamp(alpha * 256f + 0.5f, 0f, 255f));
                    colors[y * length + x] = color;

                }
            }

            var tex = new Texture2D(uri, length, length);
            tex.texture.SetData(colors);
            return tex;
        }

        public static Color colorWhite(float distance, int x, int y)
        {
            return Color.FromNonPremultiplied(255, 255, 255, 255);
        }

        public static Color colorBlack(float distance, int x, int y)
        {
            return Color.FromNonPremultiplied(0, 0, 0, 0);
        }

        public static float circleFalloff(float distance, int x, int y)
        {
            if (0 < distance && distance < 1f / 3f)
            {
                return 1 - (3 * (distance * distance));
            }
            else if (1f / 3f < distance && distance < 1f)
            {
                return (3f / 2f) * ((1f - distance) * (1f - distance));
            }

            return 0f;
        }

        protected override void ondispose()
        {
            if (texture != null)
                texture.Dispose();

        }

    }

}
