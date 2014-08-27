using Atma.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Sources
{
    public abstract class AbstractSource : IAssetSource
    {
        private readonly static Logger logger = Logger.getLogger(typeof(AbstractSource));

        private Dictionary<AssetUri, IAssetEntry> _entries = new Dictionary<AssetUri, IAssetEntry>();
        private Dictionary<int, List<IAssetEntry>> _entryByTypes = new Dictionary<int, List<IAssetEntry>>();

        public AbstractSource(string id)
        {
            this.id = id;
        }

        public void init()
        {
            _entries.Clear();

            foreach (var list in _entryByTypes)
                list.Value.Clear();

            _entryByTypes.Clear();

            load();
        }


        protected abstract void load();

        protected  void addEntry(IAssetEntry ae)
        {
            if (_entries.ContainsKey(ae.uri))
                logger.warn("{0} already existed", ae.uri);

            _entries[ae.uri] = ae;

            List<IAssetEntry> byType;
            if (!_entryByTypes.TryGetValue(ae.uri.type.id, out byType))
            {
                byType = new List<IAssetEntry>();
                _entryByTypes.Add(ae.uri.type.id, byType);
            }

            byType.Add(ae);
        }

        public string id { get; private set; }

        public IEnumerable<IAssetEntry> list()
        {
            foreach (var ae in _entries.Values)
                yield return ae;
        }

        public IEnumerable<IAssetEntry> list(AssetType type)
        {
            List<IAssetEntry> byType;
            if (_entryByTypes.TryGetValue(type.id, out byType))
            {
                foreach (var ae in byType)
                    yield return ae;
            }
        }

        public IAssetEntry get(AssetUri uri)
        {
            IAssetEntry ae;
            if (_entries.TryGetValue(uri, out ae))
                return ae;

            return null;
        }

        protected AssetUri getAssetUri(string relativePath)
        {
            relativePath = relativePath.ToLower();
            String[] parts = relativePath.Split(new char[] { '/' }, 2);
            if (parts.Length > 1)
            {
                //int lastSepIndex = parts[1].IndexOf("/");
                //if (lastSepIndex != -1)
                //{
                //    parts[1] = parts[1].Substring(lastSepIndex + 1);
                //}
                int extensionSeparator = parts[1].LastIndexOf(".");
                if (extensionSeparator != -1)
                {
                    var name = parts[1].Substring(0, extensionSeparator);
                    var extension = parts[1].Substring(extensionSeparator + 1);
                    AssetType assetType;
                    if (AssetType.getTypeFor(parts[0], extension, out assetType))
                    {
                        return assetType.getUri(id, name);
                    }
                }
            }

            return null;
        }
    }
}
