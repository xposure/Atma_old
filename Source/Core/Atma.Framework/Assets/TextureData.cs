using System.IO;

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

}
