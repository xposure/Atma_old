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

namespace Atma.Collections
{
	/// <summary>
	/// 	A simple container class for returning a pair of objects from a method call 
	/// 	(similar to std::pair, minus the templates).
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class Pair
	{
		public object first;
		public object second;

		public Pair( object first, object second )
		{
			this.first = first;
			this.second = second;
		}
	}
}
