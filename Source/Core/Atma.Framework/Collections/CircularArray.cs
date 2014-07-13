//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Atma.Collections
//{
//    public class CircularArray<T>
//    {
//        private int start;
//        public int Start
//        {
//            get { return start; }
//            set { start = value % list.Length; }
//        }

//        public int Count { get; set; }
//        public int Capacity { get { return list.Length; } }
//        private T[] list;

//        public CircularArray(int capacity)
//        {
//            list = new T[capacity];
//        }

//        public T this[int i]
//        {
//            get { return list[(start + i) % list.Length]; }
//            set { list[(start + i) % list.Length] = value; }
//        }

//        public void Add(T t)
//        {
//            if (this.Count == this.Capacity)
//            {
//                this[0] = t;
//                this.Start++;
//            }
//            else
//            {
//                this[this.Count] = t;
//                this.Count++;
//            }
//        }
//    }
//}
