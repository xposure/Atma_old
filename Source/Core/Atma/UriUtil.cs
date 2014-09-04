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



#endregion Namespace Declarations


namespace Atma.Core
{
    public static class UriUtil
    {
        /// <summary>
        /// Normalises a uri or uri part. The normal form is used for comparison/string matching.
        /// This process includes lower-casing the uri.
        /// </summary>
        /// <param name="value">value A uri or uri part</param>
        /// <returns>The normal form of the given value.</returns>
        public static string normalise(string value)
        {
            return value.ToLowerInvariant();
        }
    }
}
