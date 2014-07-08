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

//using Microsoft.Xna.Framework;

//namespace Purple.AI
//{
//  //=================================================================
//  /// <summary>
//  /// This class stores one gesture command.
//  /// </summary>
//  /// <remarks>
//  ///   <para>Author: Markus Wöß</para>
//  ///   <para>Since: 0.5</para>  
//  /// </remarks>
//  //=================================================================
//    public class Gesture
//    {
//    //---------------------------------------------------------------
//    #region Variables and Properties
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Name of the gesture.
//    /// </summary>
//    [Purple.Serialization.Serialize(true)]
//    public string Name {
//      get {
//        return name;
//      }
//      set {
//        name = value;
//      }
//    }
//    string name = "";

//    /// <summary>
//    /// The segments of the gesture.
//    /// </summary>
//    [Purple.Serialization.Serialize()]
//    public Vector2[] Segments {
//      get {
//        return segments;
//      }
//      set {
//        segments = value;
//      }
//    }
//    Vector2[] segments = null;

//    /// <summary>
//    /// The bounding box of the gesture.
//    /// </summary>
//    [Purple.Serialization.Serialize()]
//    public AABB BoundingBox {
//      get {
//        return boundingBox;
//      }
//      set {
//        boundingBox = value;
//      }
//    }
//    AABB boundingBox = new AABB();

//    /// <summary>
//    /// Flag that indicates if the gesture is enabled and should be used for matching.
//    /// </summary>
//    public bool Enabled {
//      set {
//        enabled = value;
//      }
//      get {
//        return enabled;
//      }
//    }
//    bool enabled = true;
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Initialisation
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Create a new instance of a <see cref="Gesture"/> object.
//    /// </summary>
//    public Gesture( ) : this(12) {
//    }

//    /// <summary>
//    /// Create a new instance of a <see cref="Gesture"/> object.
//    /// </summary>
//    /// <param name="segmentNum">The number of segments to split path into.</param>
//    public Gesture( int segmentNum ) {
//      segments = new Vector2[segmentNum];
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------

//    //---------------------------------------------------------------
//    #region Methods
//    //---------------------------------------------------------------
//    /// <summary>
//    /// Initializes a certain gesture from an array of points.
//    /// </summary>
//    /// <param name="points">The array of points to create gesture from.</param>
//    public void Init(Vector2[] points) {
//      // calculate total length of path
//      float totalLength = 0.0f;
//      for (int i=0; i<points.Length-1; i++)
//        totalLength += (points[i+1] - points[i]).Length();

//      boundingBox = new AABB();

//      Vector2 lastPoint = points[0];
//      float nextPos = 0.0f;
//      float currentPos = 0.0f;
//      float lastPos = 0.0f;
//      float stepSize = totalLength / (segments.Length);
//      int s=0;
//      for (int i=0; i<points.Length-1; i++) {
//        lastPos = nextPos;
//        nextPos += (points[i+1] - points[i]).Length();
//        while ((nextPos - currentPos) >= stepSize && s < segments.Length) {
//          currentPos += stepSize;
//          Vector2 newPoint = points[i] + (points[i+1] - points[i])*(currentPos-lastPos);
//          segments[s] = Vector2.Unit(newPoint - lastPoint);
//          lastPoint = newPoint;
//          s++;
//        }
//      }
//      if (s < segments.Length)
//        segments[s] = Vector2.Unit(points[points.Length-1] - lastPoint);

//      Vector2 point = Vector2.Zero;
//      for (int i=0; i<segments.Length; i++) {
//        point += segments[i];
//        boundingBox.Grow( point.X, point.Y, 0.0f );
//      }
//    }

//    /// <summary>
//    /// Calculates the matching value of two gestures.
//    /// </summary>
//    /// <param name="gesture">The gesture to test the current with.</param>
//    /// <returns>The matching value of two gestures.</returns>
//    public float Match(Gesture gesture) {
//      if (gesture.Segments.Length != segments.Length)
//        return 0.0f;

//      float sum = 0.0f;
//      for (int i=0; i<segments.Length; i++)
//        sum += Vector2.Dot( segments[i], gesture.Segments[i]);
//      return sum/segments.Length;
//    }

//    /// <summary>
//    /// Creates a clone of the gesture.
//    /// </summary>
//    /// <returns>The cloned gesture.</returns>
//    public Gesture Clone() {
//      Gesture gesture = new Gesture( this.Segments.Length );
//      Array.Copy( segments, 0, gesture.segments, 0, segments.Length );
//      gesture.name = name;
//      gesture.boundingBox = boundingBox;
//      return gesture;
//    }
//    //---------------------------------------------------------------
//    #endregion
//    //---------------------------------------------------------------
//    }
//}
