using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Core;

namespace Atma._2_0
{
    public class Aspect
    {
        private static Logger _logger = Logger.getLogger(typeof(Aspect));
        
        private static uint _aspectIndex = 0;
        private static Dictionary<uint, Aspect> _aspects = new Dictionary<uint, Aspect>();
        private static HashSet<uint> _delayDestroy = new HashSet<uint>();

        public readonly uint id;

        public HideFlags hideFlags;

        protected Aspect()
        {
            id = _aspectIndex++;
            hideFlags = HideFlags.None;
        }

        public void destroy()
        {
            _delayDestroy.Add(this.id);
        }

        public void destroyNow()
        {
            destroyNow(this);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Aspect);
        }

        internal static void processDestroyed()
        {
            while (_delayDestroy.Count > 0)
            {
                //lets do this in case somehow destroy creates new destroys
                var list = _delayDestroy.ToArray();
                _delayDestroy.Clear();

                foreach (var id in list)
                {
                    Aspect aspect;
                    if (!_aspects.TryGetValue(id, out aspect))
                        _logger.warn("aspect {0} was marked for destroy but was missing", id);

                    destroyNow(aspect);
                }
            }
        }

        internal static void destroyNow(Aspect aspect)
        {
            if (aspect)
                _logger.warn("tried to destroy a null aspect");
            else
            {
                _aspects.Remove(aspect.id);

                //needed in case destroy was called and then destroyNow - were going to let the warnings catch it
                //_delayDestroy.Remove(aspect.id);
            }
        }

        public static bool Compare(Aspect left, Aspect right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.id == right.id;
        }

        public static implicit operator bool(Aspect exists)
        {
            return !Aspect.Compare(exists, null);
        }

        public static bool operator ==(Aspect x, Aspect y)
        {
            return Aspect.Compare(x, y);
        }

        public static bool operator !=(Aspect x, Aspect y)
        {
            return !Aspect.Compare(x, y);
        }

    }
}
