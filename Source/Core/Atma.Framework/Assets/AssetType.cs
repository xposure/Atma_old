using Atma.Core;
using Atma.Utilities;
using System.Collections.Generic;

namespace Atma.Assets
{
    public struct AssetType
    {
        private static readonly Logger logger = Logger.getLogger(typeof(AssetType));

        private static Dictionary<string, AssetType> _nameLookup = new Dictionary<string, AssetType>();

        public static readonly AssetType NULL = create("NULL");
        public static readonly AssetType MATERIAL = create("MATERIAL");
        public static readonly AssetType TEXTURE = create("TEXTURE");
        public static readonly AssetType FONT = create("FONT");

        private static int typeId = 0;

        public static AssetType find(string name)
        {
            Contract.RequiresNotNull(name, "name");

            var normalizedName = name.ToLower();
            AssetType at;
            if (_nameLookup.TryGetValue(normalizedName, out at))
                return at;

            return NULL;
        }

        public static AssetType create(string name)
        {
            Contract.RequiresNotNull(name, "name");

            var normalizedName = name.ToLower();
            Contract.Requires(!_nameLookup.ContainsKey(normalizedName), "name", "The asset type was already registered");

            var at = new AssetType(typeId++, name);
            _nameLookup.Add(normalizedName, at);
            return at;
        }

        public readonly int id;
        public readonly string name;

        private AssetType(int _id, string _name)
        {
            id = _id;
            name = _name;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is AssetType)
                return Equals((AssetType)obj);

            return false;
        }

        public bool Equals(AssetType other)
        {
            return this.id == other.id;// && this.name == other.name;
        }

        public override int GetHashCode()
        {
            return id;
        }

        public static bool operator ==(AssetType a, AssetType b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(AssetType a, AssetType b)
        {
            return a.id != b.id;
        }

        public override string ToString()
        {
            return string.Format("AssetType: {0}", this.name);
        }
    }
}
