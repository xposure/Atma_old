﻿using Atma;

namespace Atma
{
    public static class Assets
    {
        private static T get<T, U>(AssetManager assets, AssetUri uri)
            where T : IAsset<U>
            where U : IAssetData
        {
            return assets.loadAsset<T, U>(uri);
        }

        private static T create<T, U>(AssetManager assets, AssetUri uri, U data)
            where T : IAsset<U>
            where U : IAssetData
        {
            return assets.generateAsset<T, U>(uri, data);
        }

        //public static Material createMaterial(this AssetManager assets, GameUri uri, MaterialData data)
        //{
        //    return create<Material, MaterialData>(assets, new AssetUri(AssetType.MATERIAL, uri.moduleName, uri.objectName), data);
        //}

        //public static Material getMaterial(this AssetManager assets, GameUri uri)
        //{
        //    return get<Material, MaterialData>(assets, new AssetUri(AssetType.MATERIAL, uri.moduleName, uri.objectName));
        //}

        public static Texture createTexture(this AssetManager assets, GameUri uri, TextureData data)
        {
            return create<Texture, TextureData>(assets, new AssetUri(AssetType.TEXTURE, uri.moduleName, uri.objectName), data);
        }

        public static Texture getTexture(this AssetManager assets, GameUri uri)
        {
            return get<Texture, TextureData>(assets, new AssetUri(AssetType.TEXTURE, uri.moduleName, uri.objectName));
        }

        //public static Font getFont(this AssetManager assets, GameUri uri)
        //{
        //    return get<Font, FontData>(assets, new AssetUri(AssetType.FONT, uri.moduleName, uri.objectName));
        //}

    }
}
