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

#endregion Namespace Declarations

namespace Atma.Utilities
{
    /// <summary>
    /// Factory class for some exception classes that have variable constructors based on the 
    /// framework that is targeted. Rather than use <c>#if</c> around the different constructors
    /// use the least common denominator, but wrap it in an easier to use method.
    /// </summary>
    internal static class ExceptionFactory
    {
        /// <summary>
        /// Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ArgumentOutOfRangeException CreateArgumentOutOfRangeException(string name, object value, string message)
        {
            return new ArgumentOutOfRangeException(name, string.Format("{0} (actual value is '{1}')", message, value));
        }

        /// <summary>
        /// Factory for the <c>ArgumentOutOfRangeException</c>
        /// </summary>
        /// <returns></returns>
        public static ArgumentNullException CreateArgumentItemNullException(int index, string arrayName)
        {
            return new ArgumentNullException(String.Format("{0}[{1}]", arrayName, index));
        }
    }
}
