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
using Atma.Collections;

namespace Atma.AI.Pathfinding
{

    //=================================================================
    /// <summary>
    /// Implementation of the famous A* algorithm used for searching
    /// the shortes path between two nodes, locations, ...
    /// </summary>
    /// <remarks>
    ///   <para>Author: Markus Wöß</para>
    ///   <para>Since: 0.2</para>
    /// </remarks>
    //=================================================================
    public class AStar
    {

        //---------------------------------------------------------------
        #region internal class, structures, interfaces, ..
        //---------------------------------------------------------------
        //=================================================================
        /// <summary>
        /// represents one possible move/state change
        /// </summary>
        /// <remarks>
        ///   <para>Author: Markus Wöß</para>
        ///   <para>Since: 0.2</para>
        /// </remarks>
        //=================================================================
        public struct Move
        {
            /// <summary>
            /// the waypoint to move to
            /// </summary>
            public IWaypoint Waypoint;

            /// <summary>
            /// the cost to move to the waypoint
            /// </summary>
            public float Cost;

            /// <summary>
            /// create one instance of move
            /// </summary>
            /// <param name="wp">to move to</param>
            /// <param name="cost">to move to waypoint</param>
            public Move(IWaypoint wp, float cost)
            {
                this.Waypoint = wp;
                this.Cost = cost;
            }
        }

        //=================================================================
        /// <summary>
        /// represents one location/state of the environment, the AStar algorithm is operating on
        /// GetHashCode() and Equals() must be overriden => dependent on location/state of IWaypoint
        /// </summary>
        /// <remarks>
        ///   <para>Author: Markus Wöß</para>
        ///   <para>Since: 0.2</para>
        /// </remarks>
        //=================================================================
        public interface IWaypoint
        {
            /// <summary>
            /// returns the estimated cost/distance between the current and the destination waypoint
            /// </summary>
            /// <param name="to">the destination waypoint</param>
            /// <param name="agent">to calculate cost for</param>
            /// <returns>the estimated cost to the destination</returns>
            float EstimateCost(AStar.IWaypoint to, AStar.IAgent agent);

            /// <summary>
            /// get possible moves
            /// </summary>
            /// <param name="agent">agent to get moves for</param>
            /// <returns>array of moves</returns>
            AStar.Move[] GetMoves(AStar.IAgent agent);

            /// <summary>
            /// returns true if waypoint is valid
            /// </summary>
            /// <returns>true if waypoint is valid</returns>
            bool IsValid();
        }

        //=================================================================
        /// <summary>
        /// an agent may be a car, boat, soldier, ...
        /// the costs for a passing a certain path may be dependent on the agent
        /// </summary>
        /// <remarks>
        ///   <para>Author: Markus Wöß</para>
        ///   <para>Since: 0.2</para>
        /// </remarks>
        //=================================================================
        public interface IAgent
        {
        }

        //=================================================================
        /// <summary>
        /// Node is an internal structure used by the AStar class to calculate the shortest path
        /// </summary>
        /// <remarks>
        ///   <para>Author: Markus Wöß</para>
        ///   <para>Since: 0.2</para>
        /// </remarks>
        //=================================================================
        private class Node
        {
            //---------------------------------------------------------------
            #region Variables, Properties
            //---------------------------------------------------------------
            /// <summary>
            /// represents how far we have already gone
            /// </summary>
            public float G;

            /// <summary>
            /// estimate of how far is left
            /// </summary>
            public float H;

            /// <summary>
            /// the total distance (G+H)
            /// </summary>
            public float F
            {
                get
                {
                    return G + H;
                }
            }

            /// <summary>
            /// parent node
            /// </summary>
            public Node Parent;

            /// <summary>
            /// the current waypoint
            /// </summary>
            public IWaypoint Waypoint;
            //---------------------------------------------------------------
            #endregion
            //---------------------------------------------------------------

            //---------------------------------------------------------------
            #region Initialisation
            //---------------------------------------------------------------
            /// <summary>
            /// creates an instance of node with a certain waypoint
            /// </summary>
            /// <param name="waypoint">to create node for</param>
            public Node(IWaypoint waypoint)
            {
                this.Waypoint = waypoint;
                Parent = null;
                G = 0;
                H = 0;
            }
            //---------------------------------------------------------------
            #endregion
            //---------------------------------------------------------------
        }
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------

        //---------------------------------------------------------------
        #region Variables
        //---------------------------------------------------------------
        PriorityQueue<float, Node> open;  // list of nodes
        Hashtable closed;     // list of closed nodes
        Hashtable existing;   // if waypoint is contained by open or closed list
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------

        //---------------------------------------------------------------
        #region Properties
        //---------------------------------------------------------------
        /// <summary>
        /// returns the number of closed nodes
        /// </summary>
        public int ClosedNodes
        {
            get
            {
                return closed.Count;
            }
        }

        /// <summary>
        /// number of open nodes
        /// </summary>
        public int OpenNodes
        {
            get
            {
                return open.Count;
            }
        }

        /// <summary>
        /// number of created nodes
        /// </summary>
        public int TotalNodes
        {
            get
            {
                return existing.Count;
            }
        }
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------

        //---------------------------------------------------------------
        #region Initialisation
        //---------------------------------------------------------------
        /// <summary>
        /// creates an instance of the A* pathfinding class
        /// </summary>
        public AStar()
        {
        }
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------

        //---------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------
        /// <summary>
        /// finds a path between two waypoints
        /// </summary>
        /// <param name="start">the start waypoint</param>
        /// <param name="goal">the destination waypoint</param>
        /// <param name="agent">the agent to find path for</param>
        /// <returns>the path in form of an array of moves
        /// or null if there is no connection between from and to</returns>
        public Move[] FindPath(IWaypoint start, IWaypoint goal, IAgent agent)
        {
            // init variables, and add the startNode to the openList
            Init(start, goal, agent);

            // read out the potentially best node, put it to the closed list and calculate moves
            while (open.Count != 0)
            {
                Node parentNode = open.DequeueValue();
                // do we have already reached the goal?
                if (parentNode.Waypoint.Equals(goal))
                    return CreatePath(parentNode);
                Move[] moves = parentNode.Waypoint.GetMoves(agent);

                // traverse all neighbours of the current waypoint
                for (int i = 0; i < moves.Length; i++)
                {
                    IWaypoint currentWaypoint = moves[i].Waypoint;
                    // if this isn't a valid waypoint move on to next
                    if (currentWaypoint.IsValid() && !closed.Contains(currentWaypoint))
                    {

                        // calculate new cost
                        float g = parentNode.G + moves[i].Cost;

                        // if node for current waypoint is already existing => update node if cost is lower
                        if (existing.Contains(currentWaypoint))
                        {
                            Node node = (Node)existing[currentWaypoint];
                            if (g < node.G)
                            {
                                node.G = g;
                                node.H = currentWaypoint.EstimateCost(goal, agent);
                                node.Parent = parentNode;
                            }
                            // if it was already in closed list => remove it
                            // actualy this can only happen if our function overestimates the distance
                            // which may be used to speed up the pathfinding, but may result into a suboptimal path
                            if (closed.Contains(currentWaypoint))
                            {
                                closed.Remove(currentWaypoint);
                                open.Add(node.F, node);
                                //Root.instance.logging.debug("This shouldn't happen for underestimating functions!");
                            }
                        }
                        else
                        {
                            Node node = new Node(currentWaypoint);
                            node.G = g;
                            node.H = currentWaypoint.EstimateCost(goal, agent);
                            node.Parent = parentNode;
                            node.Waypoint = currentWaypoint;
                            open.Add(node.F, node);
                            existing.Add(currentWaypoint, node);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// create the whole path
        /// </summary>
        /// <param name="goal">the goal node</param>
        /// <returns>returns the shortest path</returns>
        private Move[] CreatePath(Node goal)
        {
            // just count the number of elements
            int count = 1;
            Node current = goal;
            while (current.Parent != null)
            {
                current = current.Parent;
                count++;
            }

            // now fill the the array
            Move[] ret = new Move[count];
            current = goal;
            count--;
            ret[count] = new Move(current.Waypoint, current.G);
            while (current.Parent != null)
            {
                current = current.Parent;
                count--;
                ret[count] = new Move(current.Waypoint, current.G);
            }
            return ret;
        }

        /// <summary>
        /// initialised the variables for a new search
        /// </summary>
        /// <param name="start">the waypoint to start from</param>
        /// <param name="goal">the destination waypoint</param>
        /// <param name="agent">the agent to use for path</param>
        private void Init(IWaypoint start, IWaypoint goal, IAgent agent)
        {
            open = new PriorityQueue<float, Node>(256);
            closed = new Hashtable();
            existing = new Hashtable();
            Node startNode = new Node(start);
            startNode.H = start.EstimateCost(goal, agent);
            open.Add(0, startNode);
            existing.Add(start, startNode);
        }
        //---------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------
    }
}
