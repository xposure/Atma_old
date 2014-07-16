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
  /// An object representing a <see cref="StateMachine"/>.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
	public class StateMachine : States
	{
    //---------------------------------------------------------------
    #region Variables and Properties
    //---------------------------------------------------------------
    /// <summary>
    /// Returns the current state.
    /// </summary>
    public IState Current {
      get {
        return current;
      }
      set {
        current = value;
      }
    }
    IState current = null;

    /// <summary>
    /// Returns the <see cref="IState"/> with a certain name.
    /// </summary>
    public override IState this[string fullName] {
      get {
        string[] names = fullName.Split('.');
        IState state = (IState)states[names[0]];
        for (int i=1; i<names.Length; i++)
          state = (IState)state.Children[names[i]];
        return state;
      }
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Initialisation
    //---------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of a <see cref="StateMachine"/>.
    /// </summary>
    public StateMachine() : base(null) {
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Methods
    //---------------------------------------------------------------
    /// <summary>
    /// Adds a new <see cref="ITransition"/>.
    /// </summary>
    /// <param name="transition">Transition to add.</param>
    public void AddTransition(ITransition transition) {
      this[transition.From.FullName].Transitions.Add(transition);
    }

    /// <summary>
    /// Creates a shallow copy of the <see cref="StateMachine"/>.
    /// </summary>
    /// <returns>A shallow copy of the <see cref="StateMachine"/>.</returns>
    public StateMachine Clone() {
      StateMachine machine = new StateMachine();
      machine.Current = this.Current;
      machine.states = this.states;
      return machine;
    }

    /// <summary>
    /// Returns true if there is a transition from the current to the target <see cref="IState"/>.
    /// </summary>
    /// <param name="fullName">The full name of the target state.</param>
    /// <returns>True if a transition is possible.</returns>
    public bool CanSwitch(string fullName) {
      return current.CanSwitch( this[fullName] );
    }

    /// <summary>
    /// Switches from the current to a given <see cref="IState"/>.
    /// </summary>
    /// <param name="fullName">The full name of the target state.</param>
    /// <returns>The current state.</returns>
    public IState Switch(string fullName) {
      current = current.Switch( this[fullName] );
      return current;
    }

    /// <summary>
    /// Sets a new <see cref="IState"/>.
    /// </summary>
    /// <param name="fullName">The full name of the target state.</param>
    /// <returns>The current state.</returns>
    public IState Set(string fullName) {
      current = this[fullName];
      return current;
    }

    /// <summary>
    /// Tests if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.
    /// </summary>
    /// <param name="fullName">Full name of the state.</param>
    /// <returns>True if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.</returns>
    public bool IsA(string fullName) {
      return current.IsA( this[fullName] );
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------
	}
}
