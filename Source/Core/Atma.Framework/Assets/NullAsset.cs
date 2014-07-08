using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Assets
{
    public class NullAssetData : IAssetData
    {
        public static readonly NullAssetData NULL = new NullAssetData();
    }

    public class NullAsset : AbstractAsset<NullAssetData>
    {
        public NullAsset()
            : base("asset:null:null")
        {

        }

        public override void reload(NullAssetData t)
        {
        }

        protected override void ondispose()
        {
        }
    }

    public class NullAssetDataLoader : IAssetLoader<NullAssetData>
    {
        public static readonly NullAssetDataLoader NULL = new NullAssetDataLoader();

        public NullAssetData load(System.IO.Stream stream)
        {
            return NullAssetData.NULL;
        }
    }
}
