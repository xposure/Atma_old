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

//using Purple.Math;
//using Purple.Collections;

//namespace Purple.AI {
//  //=================================================================
//  /// <summary>
//  /// This class may be used for creating a new gesture easily.
//  /// </summary>
//  /// <remarks>
//  ///   <para>Author: Markus Wöß</para>
//  ///   <para>Since: 0.7</para>  
//  /// </remarks>
//  //=================================================================
//  public class GestureBuilder {
//    //---------------------------------------------------------------
//    #region Variables and Properties
//    //---------------------------------------------------------------
//    FixedBag bag;
//    float currentTime;
//    float lastParticle;

//    /// <summary>
//    /// The maximum number of points that can be added to the gesture per second.
//    /// </summary>
//    public float PointsPerSecond {
//      get {
//        return pointsPerSecond;
//      }
//      set {
//        pointsPerSecond = value;
//      }      
//    }
//    float pointsPerSecond = 100;

//    /// <summary>
//    /// Length of the path of the current gesture.
//    /// </summary>
//    public float PathLength {
//      get {
//        return pathLength;
//      }
//    }
//    float pathLength;

//    /// <summary>
//    /// Returns the number of inserted points.
//    /// </summary>
//    public int Count {
//      get {
//        return bag.Count;
//      }
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Initialisation
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Class that hels with building gestures.
//    /// </summary>
//    /// <param name="maxPoints">The maximum number of inserted points.</param>
//    public GestureBuilder(int maxPoints) {
//      bag = new FixedBag(maxPoints);
//      Reset();
//    }    
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Methods
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Adds a new point to the gesture path.
//    /// </summary>
//    /// <param name="point">Adds a point to the gesture path.</param>
//    public void AddPoint(Vector2 point) {
//      bag.Add(point);
//      if (bag.Count > 1)
//        pathLength += (point - (Vector2)bag[bag.Count-2]).Length();
//    }

//    /// <summary>
//    /// Adds a new point if a certain time since the last point elapsed.
//    /// </summary>
//    /// <param name="point"></param>
//    /// <param name="deltaTime"></param>
//    /// <returns>True if the point was inserted - false if it was too soon to accept a new point.</returns>
//    public bool AddPoint(Vector2 point, float deltaTime) {
//      currentTime += deltaTime;
//      if (lastParticle + 1.0f/pointsPerSecond < currentTime) {
//        AddPoint(point);
//        lastParticle = currentTime;
//        return true;
//      }
//      return false;
//    }

//    /// <summary>
//    /// Resets the current gesture.
//    /// </summary>
//    public void Reset() {
//      bag.Clear();
//      pathLength = 0.0f;
//      currentTime = 0.0f;
//      lastParticle = float.NegativeInfinity;
//    }

//    /// <summary>
//    /// Fills a gesture with the data of the GestureBuilder.
//    /// </summary>
//    /// <param name="gesture">The gesture to fill.</param>
//    public void Fill(Gesture gesture) {
//      gesture.Init( (Vector2[])bag.ToArray(typeof(Vector2)));
//    }

//    /// <summary>
//    /// Creates a gesture from the gathered points.
//    /// </summary>
//    /// <param name="size">Number of key points to use for the gesture.</param>
//    /// <returns>The created gesture.</returns>
//    public Gesture Create(int size) {
//      Gesture gesture = new Gesture(size);
//      Fill(gesture);
//      return gesture;
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------
//  }
//}
