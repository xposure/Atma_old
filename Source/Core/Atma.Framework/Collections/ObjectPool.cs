using System.Collections.Generic;

namespace Atma.Collections
{
    public class ObjectPool<T>
        where T: new()
    {
        private List<T> objects;// = new List<T>();
        private Stack<int> freedObjects;// = new Stack<int>();
        private List<int> activeObjects;// = new List<int>();

        public ObjectPool()
            : this(10)
        {

        }

        public ObjectPool(int initialCapacity)
        {
            objects = new List<T>(initialCapacity);
            freedObjects = new Stack<int>(initialCapacity);
            activeObjects = new List<int>(initialCapacity);

            while (initialCapacity-- > 0)
            {
                freedObjects.Push(initialCapacity);
                objects.Add(new T());
            }
        }

        public int GetFreeItem()
        {
            int index = 0;
            if (freedObjects.Count > 0)
                index = freedObjects.Pop();
            else
            {
                objects.Add(new T());
                index = objects.Count - 1;
            }

            activeObjects.Add(index);
            return index;
        }

        public void FreeItem(int item)
        {
            freedObjects.Push(item);
            var index = activeObjects.IndexOf(item);
            if (index > -1)
                activeObjects[index] = activeObjects[activeObjects.Count - 1];

            activeObjects.RemoveAt(activeObjects.Count - 1);                
        }

        public IEnumerable<int> activeItems
        {
            get
            {
                foreach (var id in activeObjects)
                    yield return id;
            }
        }

        public void Clear()
        {
            activeObjects.Clear();
            freedObjects.Clear();
            for (int i = objects.Capacity - 1; i >= 0; i--)
                freedObjects.Push(i);
        }

        public int Count { get { return activeObjects.Count; } }

        public int indexOf(int index)
        {
            return activeObjects[index];            
        }

        public T this[int index]
        {
            get
            {
                return objects[index];
            }
            set
            {
                objects[index] = value;
            }
        }
    }

}
