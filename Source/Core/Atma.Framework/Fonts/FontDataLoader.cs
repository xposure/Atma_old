using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Atma.Assets;

namespace Atma.Fonts
{
    public class FontDataLoader : IAssetDataLoader<FontData>
    {
        public FontData load(Stream stream)
        {
            var deserializer = new XmlSerializer(typeof(FontData));
            var file = (FontData)deserializer.Deserialize(stream);
            return file;
        }
    }
}