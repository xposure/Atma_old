//*****************************************************************************
//     ____                              ___                __ __      
//    /\  _`\                           /\_ \              _\ \\ \__   
//    \ \ \L\ \ __  __   _ __   _____   \//\ \       __   /\__  _  _\  
//     \ \ ,__//\ \/\ \ /\`'__\/\ '__`\   \ \ \    /'__`\ \/_L\ \\ \L_ 
//      \ \ \/ \ \ \_\ \\ \ \/ \ \ \L\ \   \_\ \_ /\  __/   /\_   _  _\
//       \ \_\  \ \____/ \ \_\  \ \ ,__/   /\____\\ \____\  \/_/\_\\_\/
//        \/_/   \/___/   \/_/   \ \ \/    \/____/ \/____/     \/_//_/ 
//                                \ \_\                                
//                                 \/_/                                            
//                  Purple# - The smart way of programming games
#region //
// Copyright (c) 2002-2003 by 
//   Markus Wöß
//   Bunnz@Bunnz.com
//   http://www.bunnz.com
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
#endregion
//*****************************************************************************
using System;
using System.Collections;

namespace Purple.AI.FSM
{
  //=================================================================
  /// <summary>
  /// A collection of <see cref="IState"/> objects.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
	public class States : Purple.Collections.CollectionBase
	{
    //---------------------------------------------------------------
    #region Variables and Properties
    //---------------------------------------------------------------
    /// <summary>
    /// Hashtable containing name and IState object.
    /// </summary>
    protected Hashtable states = new Hashtable();
    IState parent = null;

    /// <summary>
    /// Returns the <see cref="IState"/> with a certain name.
    /// </summary>
    public virtual IState this[string name] {
      get {
        return (IState)states[name];
      }
    }

    /// <summary>
    /// Returns the <see cref="IState"/> for a given index.
    /// </summary>
    public IState this[int index] {
      get {
        int i =0;
        foreach (DictionaryEntry entry in states) {
          if (i == index)
            return (IState)entry.Value;
          i++;
        }
        return null;
      }
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Initialisation
    //---------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="States"/> object.
    /// </summary>
    /// <param name="parent">Parent state.</param>
    public States(IState parent) {
      collection = states;
      this.parent = parent;
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Methods
    //---------------------------------------------------------------
    /// <summary>
    /// Adds a <see cref="IState"/> object to the list.
    /// </summary>
    /// <param name="state">State to add.</param>
    public void Add(IState state) {
      states.Add(state.Name, state);
      state.Parent = parent;
    }

    /// <summary>
    /// Removes a <see cref="IState"/> from the list.
    /// </summary>
    /// <param name="state">State to remove.</param>
    public void Remove(IState state) {
      states.Remove(state.Name);
      state.Parent = null;
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------
	}
}
