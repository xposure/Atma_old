using Atma.Core;
using Atma.Utilities;

namespace Atma.Assets
{
    public struct AssetUri : IUri
    {
        /// <summary>
        /// The character(s) use to separate the module name from other parts of the Uri
        /// </summary>    
        public static readonly char MODULE_SEPARATOR = ':';
        public static readonly char TYPE_SEPARATOR = ':';

        private string normalisedName;
        private string name;

        #region Constructors
        /// <summary>
        /// Creates a SimpleUri from a string in the format "module:object". If the string does not match this format, it will be marked invalid
        /// </summary>
        /// <param name="simpleUri">module:object string</param>
        public AssetUri(string simpleUri)
            : this()
        {
            string[] split = simpleUri.Split(MODULE_SEPARATOR);
            if (split.Length == 3)
            {
                moduleName = split[0];
                normalisedModuleName = UriUtil.normalise(split[0]);
                type = AssetType.find(split[1]);
                objectName = split[2];
                normalisedObjectName = UriUtil.normalise(split[2]);
                normalisedName = normalisedModuleName + MODULE_SEPARATOR +  UriUtil.normalise(type.name) + TYPE_SEPARATOR + normalisedObjectName;
                name = moduleName + MODULE_SEPARATOR + type.name + TYPE_SEPARATOR + objectName;
            }
        }

        /// <summary>
        /// Creates a SimpleUri for the given module:object combo
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="objectName"></param>
        public AssetUri(AssetType _type, string _moduleName, string _objectName)
            : this()
        {
            Contract.RequiresNotEmpty(_moduleName, "moduleName");
            Contract.RequiresNotEmpty(_objectName, "objectName");

            type = _type;
            moduleName = _moduleName;
            objectName = _objectName;
            normalisedModuleName = UriUtil.normalise(_moduleName);
            normalisedObjectName = UriUtil.normalise(_objectName);
            normalisedName = normalisedModuleName + MODULE_SEPARATOR +  UriUtil.normalise(type.name) + TYPE_SEPARATOR + normalisedObjectName;
            name = moduleName + MODULE_SEPARATOR + type.name + TYPE_SEPARATOR + objectName;
        }
        #endregion Constructors

        #region Properties

        public AssetType type { get; private set; }

        public string moduleName { get; private set; }

        public string normalisedModuleName { get; private set; }

        public string objectName { get; private set; }

        public string normalisedObjectName { get; private set; }

        #endregion Properties

        #region Methods

        public bool isValid()
        {
            return type != AssetType.NULL && !string.IsNullOrEmpty(normalisedModuleName) && !string.IsNullOrEmpty(normalisedObjectName);
        }

        public string toNormalisedString()
        {
            if (!isValid())
            {
                return string.Empty;
            }
            return normalisedName;
        }

        public override string ToString()
        {
            if (!isValid())
            {
                return string.Empty;
            }
            return name;
        }

        public int CompareTo(IUri other)
        {
            return string.Compare(normalisedName, other.toNormalisedString());
        }

        public bool Equals(IUri other)
        {
            return CompareTo(other) == 0;
        }

        public bool Equals(AssetUri uri)
        {
            return this.normalisedName == uri.normalisedName;
        }

        public override int GetHashCode()
        {
            return toNormalisedString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is IUri)
                return GetHashCode() == ((IUri)obj).GetHashCode();

            return false;
        }

        #endregion Methods

        #region Operators

        public static implicit operator string(AssetUri uri)
        {
            return uri.normalisedName;
        }

        public static implicit operator AssetUri(string val)
        {
            return new AssetUri(val);
        }

        public static bool operator ==(AssetUri a, AssetUri b)
        {
            if (a.isValid() && b.isValid())
                return a.normalisedName == b.normalisedName;

            return false;
        }

        public static bool operator !=(AssetUri a, AssetUri b)
        {
            if (a.isValid() && b.isValid())
                return a.normalisedName != b.normalisedName;

            return false;
        }
        #endregion
    }

}
