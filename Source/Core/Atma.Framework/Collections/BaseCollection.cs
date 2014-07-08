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

using System.Collections;

#endregion Namespace Declarations

namespace Atma.Collections
{
	/// <summary>
	///		Serves as a basis for strongly typed collections in the math lib.
	/// </summary>
	/// <remarks>
	///		Can't wait for Generics in .Net Framework 2.0!   
	/// </remarks>
	abstract public class BaseCollection : ICollection, IEnumerable, IEnumerator
	{
		/// <summary></summary>
		protected ArrayList objectList;

		//		protected int nextUniqueKeyCounter;

		private const int INITIAL_CAPACITY = 50;

		#region Constructors

		/// <summary>
		///		
		/// </summary>
		public BaseCollection()
		{
			objectList = new ArrayList( INITIAL_CAPACITY );
		}

		#endregion

		/// <summary>
		///		
		/// </summary>
		public object this[ int index ] { get { return objectList[ index ]; } set { objectList[ index ] = value; } }

		/// <summary>
		///		Adds an item to the collection.
		/// </summary>
		/// <param name="item"></param>
		protected void Add( object item )
		{
			objectList.Add( item );
		}

		/// <summary>
		///		Clears all objects from the collection.
		/// </summary>
		public void Clear()
		{
			objectList.Clear();
		}

		/// <summary>
		///		Removes the item from the collection.
		/// </summary>
		/// <param name="item"></param>
		public void Remove( object item )
		{
			int index = objectList.IndexOf( item );

			if( index != -1 )
			{
				objectList.RemoveAt( index );
			}
		}

		#region Implementation of ICollection

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo( System.Array array, int index )
		{
			objectList.CopyTo( array, index );
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsSynchronized { get { return objectList.IsSynchronized; } }

		/// <summary>
		/// 
		/// </summary>
		public int Count { get { return objectList.Count; } }

		/// <summary>
		/// 
		/// </summary>
		public object SyncRoot { get { return objectList.SyncRoot; } }

		#endregion

		#region Implementation of IEnumerable

		public System.Collections.IEnumerator GetEnumerator()
		{
			return (IEnumerator)this;
		}

		#endregion

		#region Implementation of IEnumerator

		private int position = -1;

		/// <summary>
		///		Resets the in progress enumerator.
		/// </summary>
		public void Reset()
		{
			// reset the enumerator position
			position = -1;
		}

		/// <summary>
		///		Moves to the next item in the enumeration if there is one.
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			position += 1;

			if( position >= objectList.Count )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		///		Returns the current object in the enumeration.
		/// </summary>
		public object Current { get { return objectList[ position ]; } }

		#endregion
	}
}
