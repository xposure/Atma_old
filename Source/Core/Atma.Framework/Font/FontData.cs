using Atma.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Resources;
using System.IO;

namespace Atma.Font
{
    //public class FontData : FontFile, IAssetData
    //{

    //}

    public class FontDataLoader : IAssetDataLoader<FontData>
    {
        public FontData load(Stream stream)
        {
            return FontLoader.Load(stream);
        }
    }
}
