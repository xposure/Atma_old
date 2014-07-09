
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

        public static Material getMaterial(this AssetManager assets, GameUri uri)
        {
            return get<Material, MaterialData>(assets, new AssetUri(AssetType.MATERIAL, uri.moduleName, uri.objectName));
        }

        public static Texture2D getTexture(this AssetManager assets, GameUri uri)
        {
            return get<Texture2D, TextureData>(assets, new AssetUri(AssetType.TEXTURE, uri.moduleName, uri.objectName));
        }

    }
}
