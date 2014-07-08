////*****************************************************************************
////     ____                              ___                __ __      
////    /\  _`\                           /\_ \              _\ \\ \__   
////    \ \ \L\ \ __  __   _ __   _____   \//\ \       __   /\__  _  _\  
////     \ \ ,__//\ \/\ \ /\`'__\/\ '__`\   \ \ \    /'__`\ \/_L\ \\ \L_ 
////      \ \ \/ \ \ \_\ \\ \ \/ \ \ \L\ \   \_\ \_ /\  __/   /\_   _  _\
////       \ \_\  \ \____/ \ \_\  \ \ ,__/   /\____\\ \____\  \/_/\_\\_\/
////        \/_/   \/___/   \/_/   \ \ \/    \/____/ \/____/     \/_//_/ 
////                                \ \_\                                
////                                 \/_/                                            
////                  Purple# - The smart way of programming games
//#region //
//// Copyright (c) 2002-2003 by 
////   Markus Wöß
////   Bunnz@Bunnz.com
////   http://www.bunnz.com
////
//// This library is free software; you can redistribute it and/or
//// modify it under the terms of the GNU Lesser General Public
//// License as published by the Free Software Foundation; either
//// version 2.1 of the License, or (at your option) any later version.
////
//// This library is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//// Lesser General Public License for more details.
////
//// You should have received a copy of the GNU Lesser General Public
//// License along with this library; if not, write to the Free Software
//// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//#endregion
////*****************************************************************************
//using System;
//using System.Collections;

//namespace Purple.AI
//{
//  //=================================================================
//  /// <summary>
//  /// A class for handling (mouse) gestures a la Opera ;-)
//  /// </summary>
//  /// <remarks>
//  ///   <para>Author: Markus Wöß</para>
//  ///   <para>Since: 0.5</para>  
//  /// </remarks>
//  //=================================================================
//    public class Gestures : Purple.Collections.CollectionBase
//    {
//    //---------------------------------------------------------------
//    #region Variables and Properties
//    //---------------------------------------------------------------
//    ArrayList list = new ArrayList();
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Initialisation
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Creates a new <see cref="Gestures"/> object.
//    /// </summary>
//    public Gestures() {
//      collection = list;
//    }

//    /// <summary>
//    /// Creates a new <see cref="Gestures"/> object.
//    /// </summary>
//    /// <param name="gestures">Gestures to use.</param>
//    public Gestures(ICollection gestures) {
//      collection = list;
//      AddRange(gestures);
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Methods
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Adds a <see cref="Gesture"/> to the list.
//    /// </summary>
//    /// <param name="gesture"></param>
//    public void Add(Gesture gesture) {
//      list.Add(gesture);
//    }

//    /// <summary>
//    /// Adds a certain number of <see cref="Gesture"/> objects.
//    /// </summary>
//    /// <param name="gestures">The collection containing the <see cref="Gesture"/> objects.</param>
//    public void AddRange(ICollection gestures) {
//      this.list.AddRange(gestures);
//    }

//    /// <summary>
//    /// Removes a gesture at a certain index.
//    /// </summary>
//    /// <param name="index">The index to remove gesture at.</param>
//    public void RemoveAt(int index) {
//      list[index] = list[list.Count-1];
//      list.RemoveAt(list.Count-1);
//    }

//    /// <summary>
//    /// Tries to find the gesture in the list, that matches the passed gesture.
//    /// </summary>
//    /// <param name="gesture">The gesture to find in the list.</param>
//    /// <returns>The gesture object in the list that matched the passed gesture.</returns>
//    public Gesture Match(Gesture gesture) {
//      float factor;
//      return Match(gesture, out factor);      
//    }

//    /// <summary>
//    /// Tries to find the gesture in the list, that matches the passed gesture.
//    /// </summary>
//    /// <param name="gesture">The gesture to find in the list.</param>
//    /// <param name="matchFactor">The factor the returned gestures matches the passed gesture.</param>
//    /// <returns>The gesture object in the list that matched the passed gesture.</returns>
//    public Gesture Match(Gesture gesture, out float matchFactor) {
//      Gesture retGesture = null;
//      matchFactor = float.NegativeInfinity;
//      for (int i=0; i<list.Count; i++) {
//        Gesture current = list[i] as Gesture;
//        if (current.Enabled) {
//          float match = current.Match(gesture);
//          if (match > matchFactor) {
//            retGesture = current;
//            matchFactor = match;
//          }
//        }
//      }
//      return retGesture;
//    }

//    /// <summary>
//    /// Converts the gestures object to an array.
//    /// </summary>
//    /// <returns>Returns the array.</returns>
//    public Gesture[] ToArray() {
//      return (Gesture[])list.ToArray(typeof(Gesture));
//    }

//    /// <summary>
//    /// Returns a gesture with a certain name.
//    /// </summary>
//    /// <param name="name">Name of the gesture.</param>
//    /// <returns>Returns a gesture with a certain name.</returns>
//    public Gesture Find(string name) {
//      for (int i=0; i<list.Count; i++)
//        if ((list[i] as Gesture).Name == name)
//          return (list[i] as Gesture);
//      return null;
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------
//    }
//}
