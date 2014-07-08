using Atma.Engine;
using Atma.Graphics;
using Atma.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Atma.Collections
{
    public interface IQuadObject
    {
        AxisAlignedBox Bounds { get; }
        //event EventHandler BoundsChanged;
    }

    public class QuadTree<T> where T : IQuadObject
    {
        //private readonly bool sort;
        private readonly Vector2 minLeafSize;
        private readonly int maxObjectsPerLeaf;
        private QuadNode rootNode = null;
        private Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();
        //private Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();
        public QuadNode RootNode { get { return rootNode; } }
        private object syncLock = new object();
        //private int objectSortId = 0;

        public QuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf)
        {
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }

        //public int GetSortOrder(T quadObject)
        //{
        //    //lock (objectSortOrder)
        //    {
        //        if (!objectSortOrder.ContainsKey(quadObject))
        //            return -1;
        //        else
        //        {
        //            return objectSortOrder[quadObject];
        //        }
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="minLeafSize">The smallest Vector2 a leaf will split into</param>
        ///// <param name="maxObjectsPerLeaf">Maximum number of objects per leaf before it forces a split into sub quadrants</param>
        ///// <param name="sort">Whether or not queries will return objects in the order in which they were added</param>
        //public QuadTree(Vector2 minLeafSize, int maxObjectsPerLeaf, bool sort)
        //    : this(minLeafSize, maxObjectsPerLeaf)
        //{
        //    this.sort = sort;

        //}

        public void Insert(T quadObject)
        {
            //lock (syncLock)
            {
                //if (sort & !objectSortOrder.ContainsKey(quadObject))
                //{
                //    objectSortOrder.Add(quadObject, objectSortId++);
                //}

                AxisAlignedBox bounds = quadObject.Bounds;
                if (rootNode == null)
                {
                    var rootSize = new Vector2(Utility.Ceiling(bounds.Width / minLeafSize.X),
                                            Utility.Ceiling(bounds.Height / minLeafSize.Y));
                    var multiplier = Utility.Max(rootSize.X, rootSize.Y);
                    rootSize = new Vector2(minLeafSize.X * multiplier, minLeafSize.Y * multiplier);
                    var center = new Vector2(bounds.X0 + bounds.Width / 2, bounds.Y0 + bounds.Height / 2);
                    var rootOrigin = new Vector2(center.X - rootSize.X / 2, center.Y - rootSize.Y / 2);
                    rootNode = new QuadNode(new AxisAlignedBox(rootOrigin, rootOrigin + rootSize));
                }

                while (!rootNode.Bounds.Contains(bounds))
                {
                    ExpandRoot(bounds);
                }

                InsertNodeObject(rootNode, quadObject);
            }
        }

        public List<T> Query(AxisAlignedBox bounds)
        {
            //lock (syncLock)
            {
                List<T> results = new List<T>();
                if (rootNode != null)
                    Query(bounds, rootNode, results);
                //if (sort)
                //    results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
                return results;
            }
        }

        public List<QuadIntersection> Query(Ray ray)
        {
            //lock (syncLock)
            {
                var results = new List<QuadIntersection>();
                if (rootNode != null)
                    Query(ray, rootNode, results);
                //if (sort)
                //    results.Sort((a, b) => { return objectSortOrder[a.Item].CompareTo(objectSortOrder[b.Item]); });
                return results;
            }
        }

        private void Query(AxisAlignedBox bounds, QuadNode node, List<T> results)
        {
            //lock (syncLock)
            {
                if (node == null) return;

                if (bounds.Intersects(node.Bounds))
                {
                    foreach (T quadObject in node.Objects)
                    {
                        if (bounds.Intersects(quadObject.Bounds))
                            results.Add(quadObject);
                    }

                    foreach (QuadNode childNode in node.Nodes)
                    {
                        Query(bounds, childNode, results);
                    }
                }
            }
        }

        private void Query(Ray ray, QuadNode node, List<QuadIntersection> results)
        {
            //lock (syncLock)
            {
                if (node == null) return;

                var ir = ray.Intersects(node.Bounds);
                if (ir.Hit)
                {
                    foreach (T quadObject in node.Objects)
                    {
                        ir = ray.Intersects(quadObject.Bounds);
                        if (ir.Hit)
                            results.Add(new QuadIntersection() { Distance = ir.Distance, Item = quadObject });
                    }

                    foreach (QuadNode childNode in node.Nodes)
                    {
                        Query(ray, childNode, results);
                    }
                }
            }
        }

        private void ExpandRoot(AxisAlignedBox newChildBounds)
        {
            //lock (syncLock)
            {
                bool isNorth = rootNode.Bounds.Y0 < newChildBounds.Y0;
                bool isWest = rootNode.Bounds.X0 < newChildBounds.X0;

                Direction rootDirection;
                if (isNorth)
                {
                    rootDirection = isWest ? Direction.NW : Direction.NE;
                }
                else
                {
                    rootDirection = isWest ? Direction.SW : Direction.SE;
                }

                var newX = (rootDirection == Direction.NW || rootDirection == Direction.SW)
                                  ? rootNode.Bounds.X0
                                  : rootNode.Bounds.X0 - rootNode.Bounds.Width;
                var newY = (rootDirection == Direction.NW || rootDirection == Direction.NE)
                                  ? rootNode.Bounds.Y0
                                  : rootNode.Bounds.Y0 - rootNode.Bounds.Height;
                var newRootBounds = AxisAlignedBox.FromRect(newX, newY, rootNode.Bounds.Width * 2, rootNode.Bounds.Height * 2);
                var newRoot = new QuadNode(newRootBounds);
                SetupChildNodes(newRoot);
                newRoot[rootDirection] = rootNode;
                rootNode = newRoot;
            }
        }

        private void InsertNodeObject(QuadNode node, T quadObject)
        {
            //lock (syncLock)
            {
                if (!node.Bounds.Contains(quadObject.Bounds))
                    throw new Exception("This should not happen, child does not fit within node bounds");

                if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
                {
                    SetupChildNodes(node);

                    List<T> childObjects = new List<T>(node.Objects);
                    List<T> childrenToRelocate = new List<T>();

                    foreach (T childObject in childObjects)
                    {
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode == null)
                                continue;

                            if (childNode.Bounds.Contains(childObject.Bounds))
                            {
                                childrenToRelocate.Add(childObject);
                            }
                        }
                    }

                    foreach (T childObject in childrenToRelocate)
                    {
                        RemoveQuadObjectFromNode(childObject);
                        InsertNodeObject(node, childObject);
                    }
                }

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        if (childNode.Bounds.Contains(quadObject.Bounds))
                        {
                            InsertNodeObject(childNode, quadObject);
                            return;
                        }
                    }
                }

                AddQuadObjectToNode(node, quadObject);
            }
        }

        private void ClearQuadObjectsFromNode(QuadNode node)
        {
            //lock (syncLock)
            {
                List<T> quadObjects = new List<T>(node.Objects);
                foreach (T quadObject in quadObjects)
                {
                    RemoveQuadObjectFromNode(quadObject);
                }
            }
        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {
            //lock (syncLock)
            {
                QuadNode node = objectToNodeLookup[quadObject];
                node.quadObjects.Remove(quadObject);
                objectToNodeLookup.Remove(quadObject);
                //quadObject.BoundsChanged -= new EventHandler(quadObject_BoundsChanged);
            }
        }

        private void AddQuadObjectToNode(QuadNode node, T quadObject)
        {
            //lock (syncLock)
            {
                node.quadObjects.Add(quadObject);
                objectToNodeLookup.Add(quadObject, node);
                //quadObject.BoundsChanged += new EventHandler(quadObject_BoundsChanged);
            }
        }

        public void UpdateBounds(T quadObject)
        {
            QuadNode node = objectToNodeLookup[quadObject];
            if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes())
            {
                RemoveQuadObjectFromNode(quadObject);
                Insert(quadObject);
                if (node.Parent != null)
                {
                    CheckChildNodes(node.Parent);
                }
            }
        }

        private void SetupChildNodes(QuadNode node)
        {
            //lock (syncLock)
            {
                if (minLeafSize.X <= node.Bounds.Width / 2 && minLeafSize.Y <= node.Bounds.Height / 2)
                {
                    node[Direction.NW] = new QuadNode(node.Bounds.X0, node.Bounds.Y0, node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.NE] = new QuadNode(node.Bounds.X0 + node.Bounds.Width / 2, node.Bounds.Y0,
                                                      node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.SW] = new QuadNode(node.Bounds.X0, node.Bounds.Y0 + node.Bounds.Height / 2,
                                                      node.Bounds.Width / 2,
                                                      node.Bounds.Height / 2);
                    node[Direction.SE] = new QuadNode(node.Bounds.X0 + node.Bounds.Width / 2,
                                                      node.Bounds.Y0 + node.Bounds.Height / 2,
                                                      node.Bounds.Width / 2, node.Bounds.Height / 2);

                }
            }
        }

        public void Clear()
        {
            var objects = objectToNodeLookup.Keys.ToArray();
            foreach (var obj in objects)
                this.RemoveQuadObjectFromNode(obj);
        }

        public void Remove(T quadObject)
        {
            //lock (syncLock)
            {
                //if (sort && objectSortOrder.ContainsKey(quadObject))
                //{
                //    objectSortOrder.Remove(quadObject);
                //}

                if (!objectToNodeLookup.ContainsKey(quadObject))
                    throw new KeyNotFoundException("QuadObject not found in dictionary for removal");

                QuadNode containingNode = objectToNodeLookup[quadObject];
                RemoveQuadObjectFromNode(quadObject);

                if (containingNode.Parent != null)
                    CheckChildNodes(containingNode.Parent);
            }
        }

        private void CheckChildNodes(QuadNode node)
        {
            //lock (syncLock)
            {
                if (GetQuadObjectCount(node) <= maxObjectsPerLeaf)
                {
                    // Move child objects into this node, and delete sub nodes
                    List<T> subChildObjects = GetChildObjects(node);
                    foreach (T childObject in subChildObjects)
                    {
                        if (!node.Objects.Contains(childObject))
                        {
                            RemoveQuadObjectFromNode(childObject);
                            AddQuadObjectToNode(node, childObject);
                        }
                    }
                    if (node[Direction.NW] != null)
                    {
                        node[Direction.NW].Parent = null;
                        node[Direction.NW] = null;
                    }
                    if (node[Direction.NE] != null)
                    {
                        node[Direction.NE].Parent = null;
                        node[Direction.NE] = null;
                    }
                    if (node[Direction.SW] != null)
                    {
                        node[Direction.SW].Parent = null;
                        node[Direction.SW] = null;
                    }
                    if (node[Direction.SE] != null)
                    {
                        node[Direction.SE].Parent = null;
                        node[Direction.SE] = null;
                    }

                    if (node.Parent != null)
                        CheckChildNodes(node.Parent);
                    else
                    {
                        // Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
                        int numQuadrantsWithObjects = 0;
                        QuadNode nodeWithObjects = null;
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode != null && GetQuadObjectCount(childNode) > 0)
                            {
                                numQuadrantsWithObjects++;
                                nodeWithObjects = childNode;
                                if (numQuadrantsWithObjects > 1) break;
                            }
                        }
                        if (numQuadrantsWithObjects == 1)
                        {
                            foreach (QuadNode childNode in node.Nodes)
                            {
                                if (childNode != nodeWithObjects)
                                    childNode.Parent = null;
                            }
                            rootNode = nodeWithObjects;
                        }
                    }
                }
            }
        }

        private List<T> GetChildObjects(QuadNode node)
        {
            //lock (syncLock)
            {
                List<T> results = new List<T>();
                results.AddRange(node.quadObjects);
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                        results.AddRange(GetChildObjects(childNode));
                }
                return results;
            }
        }

        public int GetQuadObjectCount()
        {
            //lock (syncLock)
            {
                if (rootNode == null)
                    return 0;
                int count = GetQuadObjectCount(rootNode);
                return count;
            }
        }

        private int GetQuadObjectCount(QuadNode node)
        {
            //lock (syncLock)
            {
                int count = node.Objects.Count;
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        count += GetQuadObjectCount(childNode);
                    }
                }
                return count;
            }
        }

        public int GetQuadNodeCount()
        {
            //lock (syncLock)
            {
                if (rootNode == null)
                    return 0;
                int count = GetQuadNodeCount(rootNode, 1);
                return count;
            }
        }

        private int GetQuadNodeCount(QuadNode node, int count)
        {
            //lock (syncLock)
            {
                if (node == null) return count;

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                        count++;
                }
                return count;
            }
        }

        public List<QuadNode> GetAllNodes()
        {
            //lock (syncLock)
            {
                List<QuadNode> results = new List<QuadNode>();
                if (rootNode != null)
                {
                    results.Add(rootNode);
                    GetChildNodes(rootNode, results);
                }
                return results;
            }
        }

        private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
        {
            //lock (syncLock)
            {
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        results.Add(childNode);
                        GetChildNodes(childNode, results);
                    }
                }
            }
        }

        public void Render(Color start, Color end)
        {
            if (rootNode != null)
            {
                var material = CoreRegistry.require<ResourceManager>(ResourceManager.Uri).findMaterial("basewhite");
                rootNode.Render(material);
            }
        }

        public struct QuadIntersection
        {
            public float Distance;
            public T Item;
        }

        public class QuadNode
        {
            private static int _id = 0;
            public readonly int ID = _id++;

            public QuadNode Parent { get; internal set; }

            private QuadNode[] _nodes = new QuadNode[4];
            public QuadNode this[Direction direction]
            {
                get
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            return _nodes[0];
                        case Direction.NE:
                            return _nodes[1];
                        case Direction.SW:
                            return _nodes[2];
                        case Direction.SE:
                            return _nodes[3];
                        default:
                            return null;
                    }
                }
                set
                {
                    switch (direction)
                    {
                        case Direction.NW:
                            _nodes[0] = value;
                            break;
                        case Direction.NE:
                            _nodes[1] = value;
                            break;
                        case Direction.SW:
                            _nodes[2] = value;
                            break;
                        case Direction.SE:
                            _nodes[3] = value;
                            break;
                    }
                    if (value != null)
                        value.Parent = this;
                }
            }

            public ReadOnlyCollection<QuadNode> Nodes;

            internal List<T> quadObjects = new List<T>();
            public ReadOnlyCollection<T> Objects;

            public AxisAlignedBox Bounds { get; internal set; }

            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }

            public QuadNode(AxisAlignedBox bounds)
            {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
                Objects = new ReadOnlyCollection<T>(quadObjects);
            }

            public QuadNode(float x, float y, float width, float height)
                : this(AxisAlignedBox.FromRect(x, y, width, height))
            {

            }

            public void Render(Material material)
            {
                Root.instance.graphics.GL.push();
                Root.instance.graphics.GL.material(material);
                Root.instance.graphics.GL.color(Color.Red);
                Root.instance.graphics.GL.quad(Bounds);
                Root.instance.graphics.GL.pop();
                //Root.instance.graphics.DrawRect(19, material, Bounds.minVector, Bounds.maxVector, Color.Red);
                if (_nodes != null)
                {
                    foreach (var node in _nodes)
                        if (node != null)
                            node.Render(material);
                }
            }
        }


    }

    public enum Direction : int
    {
        NW = 0,
        NE = 1,
        SW = 2,
        SE = 3
    }
}
