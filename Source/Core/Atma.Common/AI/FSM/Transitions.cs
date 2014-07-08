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
  /// A simple collection of <see cref="ITransition"/> objects.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
	public class Transitions : Purple.Collections.CollectionBase
	{
    //---------------------------------------------------------------
    #region Variables and Properties
    //---------------------------------------------------------------
    ArrayList transitions = new ArrayList();

    /// <summary>
    /// Returns the transition at a given index.
    /// </summary>
    public ITransition this[int index] {
      get {
        return (ITransition)transitions[index];
      }
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Initialisation
    //---------------------------------------------------------------
    /// <summary>
    /// Creates a new <see cref="Transitions"/> object.
    /// </summary>
		public Transitions()
		{
      collection = transitions;
		}
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Methods
    //---------------------------------------------------------------
    /// <summary>
    /// Adds a new transition to the list.
    /// </summary>
    /// <param name="trans">Transition to add.</param>
    public void Add(ITransition trans) {
      transitions.Add(trans);
    }

    /// <summary>
    /// Removes a transition from the list.
    /// </summary>
    /// <param name="trans"></param>
    public void Remove(ITransition trans) {
      transitions.Remove(trans);
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------
  }
}
