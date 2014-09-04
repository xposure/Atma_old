#region GPLv3 License

/*
Atma
Copyright © 2014 Atma Project Team

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License V3
as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
General Public License V3 for more details.

You should have received a copy of the GNU General Public License V3
along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/

#endregion

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion Namespace Declarations


namespace Atma
{
    /// <summary>
    /// 
    /// </summary>
    internal static class Proclaim
    {
        /// <summary>
        /// Asserts if this statement is reached.
        /// </summary>
        /// <exception cref="InvalidOperationException">Code is supposed to be unreachable.</exception>
        public static Exception Unreachable
        {
            get
            {
                Debug.Assert(false, "Unreachable");
                return new InvalidOperationException("Code is supposed to be unreachable.");
            }
        }

        /// <summary>
        /// Asserts if any argument is <c>null</c>.
        /// </summary>
        /// <param name="vars"></param>
        public static void NotNull(params object[] vars)
        {
            var result = true;
            foreach (var obj in vars)
            {
                result &= (obj != null);
            }
            Debug.Assert(result);
        }

        /// <summary>
        /// Asserts if the string is <c>null</c> or zero length.
        /// </summary>
        /// <param name="str"></param>
        public static void NotEmpty(string str)
        {
            Debug.Assert(!String.IsNullOrEmpty(str));
        }

        /// <summary>
        /// Asserts if the collection is <c>null</c> or the <c>Count</c> is zero.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void NotEmpty<T>(ICollection<T> items)
        {
            Debug.Assert(items != null && items.Count > 0);
        }

        /// <summary>
        /// Asserts if any item in the collection is <c>null</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public static void NotNullItems<T>(IEnumerable<T> items) where T : class
        {
            Debug.Assert(items != null);
            foreach (object item in items)
            {
                Debug.Assert(item != null);
            }
        }
    }
}
