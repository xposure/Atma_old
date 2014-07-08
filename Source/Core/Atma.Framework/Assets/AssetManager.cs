using Atma.Core;
using Atma.Engine;
using System;
using System.Collections.Generic;

namespace Atma.Assets
{
    public class AssetManager
    {
        public static readonly GameUri Uri = "engine:assets";

        private static readonly Logger logger = Logger.getLogger(typeof(AssetManager));

        private Dictionary<int, AssetFactory<IAssetData, IAsset<IAssetData>>> _factories = new Dictionary<int, AssetFactory<IAssetData, IAsset<IAssetData>>>();

        public void setFactory<DATA, ASSET>(AssetType type, Func<AssetUri, DATA, ASSET> factory)
            where DATA : IAssetData
            where ASSET : IAsset<DATA>
        {
            _factories.Add(type.id, new AssetFactory<IAssetData, IAsset<IAssetData>>((uri, data) =>
            {
                return (IAsset<IAssetData>)factory(uri, (DATA)data);
            }));
        }



        //public void setFactory<DATA, ASSET>(AssetType type, AssetFactory<DATA, ASSET> factory)
        //    where DATA : IAssetData
        //    where ASSET : IAsset
        //{
        //    //_factories.Add(type.id, factory);
        //}

        public T loadAsset<T, U>(AssetUri uri, U data)
            where T : IAsset
            where U : IAssetData
        {
            if (!uri.isValid())
            {
                logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            AssetFactory<IAssetData, IAsset<IAssetData>> factory;
            if (!_factories.TryGetValue(uri.type.id, out factory))
            {
                logger.warn("Unsupported asset type: {0}", uri.type);
                return default(T);
            }

            //
            var t = factory(uri, data);

            if (t is T)
                return (T)t;

            if (t != null)
                logger.error("factory returned a type '{0} 'that wasn't of T", t.GetType());

            return default(T);
        }

        public T generateAsset<T, U>(AssetUri uri, U data)
            where T : IAsset
            where U : IAssetData
        {
            if (!uri.isValid())
            {
                logger.warn("Invalid asset uri: {0}", uri);
                return default(T);
            }

            AssetFactory<IAssetData, IAsset<IAssetData>> factory;
            if (!_factories.TryGetValue(uri.type.id, out factory))
            {
                logger.warn("Unsupported asset type: {0}", uri.type);
                return default(T);
            }

            var t = factory(uri, data);

            if (t is T)
                return (T)t;

            if (t != null)
                logger.error("factory returned a type '{0} 'that wasn't of T", t.GetType());

            return default(T);
        }
    }
}
