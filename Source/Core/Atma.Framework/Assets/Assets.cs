
using Atma.Engine;
using Atma.Graphics;
namespace Atma.Assets
{
    public static class Assets
    {
        public static Material getMaterial(AssetUri uri)
        {
            var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            return assets.loadAsset<Material, MaterialData>(uri);
        }

        public static Texture2D getTexture(this AssetManager assets, AssetUri uri)
        {
            //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);
            return assets.loadAsset<Texture2D, TextureData>(uri);
        }
        //public static IMaterial generateAsset(MaterialData data)
        //{
        //    var uri = new AssetUri(AssetType.MATERIAL, "Temp", Guid.NewGuid().ToString());
        //    return CoreRegistry.require<AssetManager>(AssetManager.Uri).generateAsset<IMaterial, MaterialData>(uri, data);
        //}
    }
}
