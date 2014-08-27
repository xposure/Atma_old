using Atma.Core;
using Atma.Utilities;

namespace Atma
{
    #region GameUri
    public struct GameUri : IUri
    {
        /// <summary>
        /// The character(s) use to separate the module name from other parts of the Uri
        /// </summary>    
        public static readonly char MODULE_SEPARATOR = ':';

        private string normalisedName;
        private string name;

        #region Constructors
        /// <summary>
        /// Creates a SimpleUri from a string in the format "module:object". If the string does not match this format, it will be marked invalid
        /// </summary>
        /// <param name="simpleUri">module:object string</param>
        public GameUri(string simpleUri)
            : this()
        {
            string[] split = simpleUri.Split(MODULE_SEPARATOR);
            if (split.Length == 2)
            {
                moduleName = split[0];
                normalisedModuleName = UriUtil.normalise(split[0]);
                objectName = split[1];
                normalisedObjectName = UriUtil.normalise(split[1]);
                normalisedName = normalisedModuleName + MODULE_SEPARATOR + normalisedObjectName;
                name = moduleName + MODULE_SEPARATOR + objectName;
            }
        }

        /// <summary>
        /// Creates a SimpleUri for the given module:object combo
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="objectName"></param>
        public GameUri(string _moduleName, string _objectName)
            : this()
        {
            Contract.RequiresNotEmpty(_moduleName, "moduleName");
            Contract.RequiresNotEmpty(_objectName, "objectName");
            moduleName = _moduleName;
            objectName = _objectName;
            normalisedModuleName = UriUtil.normalise(_moduleName);
            normalisedObjectName = UriUtil.normalise(_objectName);
            normalisedName =  normalisedModuleName + MODULE_SEPARATOR + normalisedObjectName;
            name = _moduleName + MODULE_SEPARATOR + _objectName;

        }
        #endregion Constructors

        #region Properties

        public string moduleName { get; private set; }

        public string normalisedModuleName { get; private set; }

        public string objectName { get; private set; }

        public string normalisedObjectName { get; private set; }

        #endregion Properties

        #region Methods

        public bool isValid()
        {
            return !string.IsNullOrEmpty(normalisedModuleName) && !string.IsNullOrEmpty(normalisedObjectName);
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

        public bool Equals(GameUri uri)
        {
            return this.normalisedName == uri.normalisedName;
        }


        #endregion Methods

        #region Operators

        public static implicit operator string(GameUri uri)
        {
            return uri.toNormalisedString();
        }

        public static implicit operator GameUri(string val)
        {
            return new GameUri(val);
        }

        public static bool operator ==(GameUri a, GameUri b)
        {
            return a.normalisedName == b.normalisedName;
        }

        public static bool operator !=(GameUri a, GameUri b)
        {
            return a.normalisedName != b.normalisedName;
        }
        #endregion
    }
    #endregion GameUri
}
