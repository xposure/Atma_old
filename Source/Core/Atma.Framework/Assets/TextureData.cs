using System.IO;
using Atma.Engine;
using Atma.Graphics;
using Atma.MonoGame.Graphics;
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
    }

    public class TextureDataLoader : IAssetLoader<TextureData>
    {
        public TextureData load(Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            return new TextureData(bytes);
        }
    }
}
