using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma
{
    public interface IAssetSource 
    {
        string id { get; }
        void init();
        IEnumerable<IAssetEntry> list();
        IEnumerable<IAssetEntry> list(AssetType type);
        IAssetEntry get(AssetUri uri);
    }
}
