
namespace Atma.Assets
{
    public interface IReloadableAsset<T> : IAsset
        where T : IAssetData
    {
        void reload(T data);

    }

    public interface IAsset//<out T>
    //where T: IAssetData
    {
        AssetUri uri { get; }

        //T reload();

        void dispose();

        bool isDisposed { get; }
    }

    public interface IAsset<in T>
        where T : IAssetData
    {
        AssetUri uri { get; }

        void reload(T t);

        void dispose();

        bool isDisposed { get; }
    }
}
