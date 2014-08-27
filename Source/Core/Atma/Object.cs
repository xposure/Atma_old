using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Core;

namespace Atma._2_0
{
    public class Object
    {
        //internal static Logger _logger = Logger.getLogger(typeof(Aspect));

        internal static uint _aspectIndex = 0;
        internal static Dictionary<uint, Object> _aspects = new Dictionary<uint, Object>();

        public readonly uint id;

        //public HideFlags hideFlags;

        protected Object()
        {
            id = _aspectIndex++;
            //hideFlags = HideFlags.None;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Object);
        }

        public static bool Compare(Object left, Object right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.id == right.id;
        }

        public static implicit operator bool(Object exists)
        {
            return !Object.Compare(exists, null);
        }

        public static bool operator ==(Object x, Object y)
        {
            return Object.Compare(x, y);
        }

        public static bool operator !=(Object x, Object y)
        {
            return !Object.Compare(x, y);
        }

    }
}
