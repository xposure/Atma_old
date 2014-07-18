using System.IO;
using Atma.Engine;
using Atma.Graphics;
using Microsoft.Xna.Framework;

namespace Atma.Assets
{
    public class TextureData : IAssetData
    {
        private byte[] _data;

        public byte[] bytes { get { return _data; } }

        public TextureData(byte[] data)
        {
            _data = data;
        }

        public static TextureData fromStream(string file)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                return new TextureData(bytes);
            }
        }

        public static TextureData create(int width, int height, Microsoft.Xna.Framework.Color color)
        {
            using (var bmp = new System.Drawing.Bitmap(width, height))
            {
                //SLOW!!!!!!
                var dColor = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
                for (var w = 0; w < width; w++)
                    for (var h = 0; h < height; h++)
                        bmp.SetPixel(w, h, dColor);

                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);

                    var data = new byte[ms.Length];
                    ms.Read(data, 0, data.Length);
                    return new TextureData(data);
                }
            }
        }

  
    }

    public class TextureDataLoader : IAssetDataLoader<TextureData>
    {
        public TextureData load(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            return new TextureData(bytes);
        }
    }
}
