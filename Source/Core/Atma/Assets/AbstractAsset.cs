using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma
{
    public abstract class AbstractAsset<T> : DestroyableObject,  IAsset<T>
        where T: IAssetData
    {
        public AssetUri uri { get; private set; }

        public bool isDisposed { get; private set; }

        public AbstractAsset(AssetUri uri)
        {
            this.uri = uri;

            if (!uri.isValid())
                return;
        }

        public void dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
                ondispose();
            }
        }        

        ~AbstractAsset()
        {
            dispose();
        }

        public abstract void reload(T t);

        protected abstract void ondispose();

        public override bool Equals(object obj)
        {
            if (obj is AbstractAsset<T>)
                return Equals(obj as AbstractAsset<T>);

            if (obj is IAsset)
                return Equals(obj as IAsset);

            return false;
        }

        public bool Equals(IAsset<T> p)
        {
            // If parameter is null, return false. 
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case. 
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false. 
            if (this.GetType() != p.GetType())
                return false;

            return this.uri == p.uri;
        }

        public override int GetHashCode()
        {
            return uri.GetHashCode();
        }

        public static bool operator ==(AbstractAsset<T> a, AbstractAsset<T> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                if (object.ReferenceEquals(b, null))
                    return true;

                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(AbstractAsset<T> a, AbstractAsset<T> b)
        {
            return !(a == b);
        }
    }
}
