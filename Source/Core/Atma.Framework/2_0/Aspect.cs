using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma._2_0
{
    public class Aspect
    {
        private static uint _thingIndex = 0;
        public readonly uint id;

        public HideFlags hideFlags;

        protected Aspect()
        {
            id = _thingIndex++;
            hideFlags = HideFlags.None;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as Aspect);
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
