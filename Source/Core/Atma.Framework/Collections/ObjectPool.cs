using System.Collections.Generic;

namespace Atma.Collections
{
    public class ObjectPool<T>
        where T: new()
    {
        private List<T> objects = new List<T>();
        private Stack<int> freedObjects = new Stack<int>();
        private List<int> activeObjects = new List<int>();

        public ObjectPool()
            : this(10)
        {

        }

        public ObjectPool(int initialCapacity)
        {
            while (initialCapacity-- > 0)
            {
                freedObjects.Push(initialCapacity);
                objects.Add(new T());
            }
        }

        public int GetFreeItem()
        {
            if (freedObjects.Count > 0)
                return freedObjects.Pop();

            objects.Add(new T());
            return objects.Count - 1;
        }

        public void FreeItem(int item)
        {
            freedObjects.Push(item);
        }

        public void Clear()
        {
            freedObjects.Clear();
            for (int i = objects.Capacity - 1; i >= 0; i--)
                freedObjects.Push(i);
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
