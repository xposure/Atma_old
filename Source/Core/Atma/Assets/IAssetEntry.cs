using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Atma
{
    public interface IAssetEntry
    {
        AssetUri uri { get; }
        Stream getReadStream();
    }
}
