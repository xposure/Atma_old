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

using Purple.Serialization;

namespace Purple.AI.FSM
{
  //=================================================================
  /// <summary>
  /// Abstract interface for a simple state of a finite state machine.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
	public interface IState
	{
    /// <summary>
    /// Returns the parent <see cref="IState"/> or null if there is none.
    /// </summary>
    IState Parent  { get; set; }

    /// <summary>
    /// Returns the root <see cref="IState"/>.
    /// </summary>
    IState Root { get; }

    /// <summary>
    /// The sub states.
    /// </summary>
    States Children { get; }

    /// <summary>
    /// Returns the name of the state.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Returns the full name of the state.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// List of possible state transitions
    /// </summary>
    Transitions Transitions { get; }

    /// <summary>
    /// Tests if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.
    /// </summary>
    /// <param name="state"></param>
    /// <returns>True if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.</returns>
    bool IsA(IState state);

    /// <summary>
    /// Returns true if there is a transition from the current to the target <see cref="IState"/>.
    /// </summary>
    /// <param name="target">The target state.</param>
    /// <returns>True if a transition is possible.</returns>
    bool CanSwitch(IState target);

    /// <summary>
    /// Switches from the current to a given <see cref="IState"/>.
    /// </summary>
    /// <param name="target">The target state.</param>
    /// <returns>The current state.</returns>
    IState Switch(IState target);

    /// <summary>
    /// Adds a <see cref="IState"/> object to the list.
    /// </summary>
    /// <param name="state">State to add.</param>
    void Add(IState state);
	}

  //=================================================================
  /// <summary>
  /// Standard implementation of <see cref="IState"/>.
  /// </summary>
  /// <remarks>
  ///   <para>Author: Markus Wöß</para>
  ///   <para>Since: 0.4</para>
  /// </remarks>
  //=================================================================
  public class State : IState {
    //---------------------------------------------------------------
    #region Variables, Properties
    //---------------------------------------------------------------
    /// <summary>
    /// Returns the name of the state.
    /// </summary>
    [Serialize(true)]
    public string Name { 
      get {
        return name;
      }
      set {
        name = value;
      }
    }
    string name = "";

    /// <summary>
    /// Returns the full name of the state.
    /// </summary>
    public string FullName { 
      get {
        if (parent != null)
          return parent.FullName + "." + name;
        return name;
      }
    }

    /// <summary>
    /// List of possible state transitions
    /// </summary>
    public Transitions Transitions { 
      get {
        return transitions;
      }
    }
    Transitions transitions = new Transitions();

    /// <summary>
    /// Returns the parent <see cref="IState"/> or null if there is none.
    /// </summary>
    public IState Parent  { 
      get {
        return parent;
      }
      set {
        parent = value;
      }
    }
    IState parent = null;

    /// <summary>
    /// Returns the root <see cref="IState"/>.
    /// </summary>
    public IState Root { 
      get {
        IState current = this;
        while (current.Parent != null)
          current = current.Parent;
        return current;
      }
    }

    /// <summary>
    /// The sub states.
    /// </summary>
    public States Children { 
      get {
        return children;
      }
    }
    States children;
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Initialisation
    //---------------------------------------------------------------
    /// <summary>
    /// Creates a new instance of a <see cref="State"/> object.
    /// </summary>
    /// <param name="name">Name of state.</param>
    public State(string name) {
      this.name = name;
      children = new States(this);
    }

    /// <summary>
    /// parameterless for Serialization
    /// </summary>
    protected State() {
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    #region Methods
    //---------------------------------------------------------------
    /// <summary>
    /// Adds a new transition to a certain target element.
    /// </summary>
    /// <param name="target">The target state.</param>
    public void AddTransition(IState target) {
      transitions.Add( new Transition(this, target) );
    }

    /// <summary>
    /// Removes the transition to a certain target element.
    /// </summary>
    /// <param name="target">The target state.</param>
    public void RemoveTransition(IState target) {
      transitions.Remove( new Transition(this, target) );
    }

    /// <summary>
    /// Tests if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.
    /// </summary>
    /// <param name="state"></param>
    /// <returns>True if the current <see cref="IState"/> or one of its parent is equal to the 
    /// given state.</returns>
    public bool IsA(IState state) {
      IState current = this;
      do {
        if (current == state)
          return true;
        current = current.Parent;
      } while (current != null);
      return false;
    }

    /// <summary>
    /// Returns true if there is a transition from the current to the target <see cref="IState"/>.
    /// </summary>
    /// <param name="target">The target state.</param>
    /// <returns>True if a transition is possible.</returns>
    public bool CanSwitch(IState target) {
     if (target == null)
        throw new ArgumentNullException("target", "Can't switch to a null reference state");
      for (int i=0; i<transitions.Count; i++) {
        if (target.IsA(transitions[i].To))
          return true;
      }
      if (parent != null)
        return parent.CanSwitch(target);
      return false;
    }

    /// <summary>
    /// Switches from the current to a given <see cref="IState"/>.
    /// </summary>
    /// <param name="target">The target state.</param>
    /// <returns>The current state.</returns>
    public IState Switch(IState target) {
      for (int i=0; i<transitions.Count; i++) {
        if (target.IsA(transitions[i].To))
          return target;
      }
      if (parent != null)
        return parent.Switch(target);
      return null;
    }

    /// <summary>
    /// Adds a <see cref="IState"/> object to the list.
    /// </summary>
    /// <param name="state">State to add.</param>
    public void Add(IState state) {
      children.Add(state);
    }
    //---------------------------------------------------------------
    #endregion
    //---------------------------------------------------------------
  }
}
