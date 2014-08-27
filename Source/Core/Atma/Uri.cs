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

namespace Atma.Core
{
    #region Uri
    /// <summary>
    /// Uris are used to identify resources, like assets and systems introduced by mods. Uris can then be serialized/deserialized to and from Strings.
    /// Uris are case-insensitive. They have a normalised form which is lower-case (using English casing).
    /// Uris are immutable.
    /// 
    /// All uris include a module name as part of their structure.
    /// </summary>
    public interface IUri : IComparable<IUri>, IEquatable<IUri>
    {

        /// <summary>
        /// The name of the module the resource in question resides in.
        /// </summary>
        string moduleName { get; }

        /// <summary>
        /// The normalised form of the module name. Generally this means lower case.
        /// </summary>
        string normalisedModuleName { get; }

        /// <returns>The normalised form of the uri. Generally this means lower case.</returns>
        string toNormalisedString();

        /// <summary>
        /// </summary>
        /// <returns>Whether this uri represents a valid, well formed uri.</returns>
        bool isValid();

    }
    #endregion Uri


}
