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

namespace Purple.AI.FSM
{
  //=================================================================
  /// <summary>
  /// Abstract interface for a state transition.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
	public interface ITransition
	{
    /// <summary>
    /// The state to change from.
    /// </summary>
    IState From {get;}

    /// <summary>
    /// The state to change to.
    /// </summary>
    IState To {get;}
	}

  //=================================================================
  /// <summary>
  /// Standard implementation of an <see cref="ITransition"/>.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
  public class Transition : ITransition {
    //---------------------------------------------------------------
    #region Variables and Properties
    //---------------------------------------------------------------
    /// <summary>
    /// The state to change from.
    /// </summary>
    public IState From {
      get {
        return from;
      }
    }
    IState from;

    /// <summary>
    /// The state to change to.
    /// </summary>
    public IState To {
      get {
        return to;
      }
    }
    IState to;
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Initialisation
    //---------------------------------------------------------------
    /// <summary>
    /// Creates a new transition between to states.
    /// </summary>
    /// <param name="from">The from state.</param>
    /// <param name="to">The state to change to.</param>
    public Transition(IState from, IState to) {
      if (from == null)
        throw new ArgumentNullException("from", "From state mustn't be null!");
      if (to == null)
        throw new ArgumentNullException("to", "To state mustn't be null!");
      this.from = from;
      this.to = to;
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Methods
    //---------------------------------------------------------------
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------
  }
}
