
using Atma.Engine;
using Atma.Graphics;
namespace Atma.Assets
{
    public static class Assets
    {
        private static T get<T, U>(AssetManager assets, AssetUri uri)
            where T: IAsset<U>
            where U: IAssetData
        {
            return assets.loadAsset<T, U>(uri);
        }

        //public static Material getMaterial(GameUri uri)
        //{
        //    var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

        //    return assets.loadAsset<Material, MaterialData>( uri);
        //}

        public static Texture2D getTexture(this AssetManager assets, GameUri uri)
        {
            return get<Texture2D, TextureData>(assets, new AssetUri(AssetType.TEXTURE, uri.moduleName, uri.objectName));
            //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            //return assets.loadAsset<Texture2D, TextureData>(uri);
        }
        //public static IMaterial generateAsset(MaterialData data)
        //{
        //    var uri = new AssetUri(AssetType.MATERIAL, "Temp", Guid.NewGuid().ToString());
        //    return CoreRegistry.require<AssetManager>(AssetManager.Uri).generateAsset<IMaterial, MaterialData>(uri, data);
        //}
    }
}
