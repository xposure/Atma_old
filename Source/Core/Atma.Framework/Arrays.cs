using System;
using System.Collections.Generic;

namespace Atma.Common
{
    public static class Arrays
    {
        public struct XY<T>
        {
            public int x;
            public int y;
            public int index;
            //public T value;
        }

        public struct XYZ<T>
        {
            public int x;
            public int y;
            public int z;
            public int index;
            //public T value;
        }

        public static IEnumerable<XY<T>> xy<T>(this T[] t, int width)
        {
            var xy = new XY<T>();

            while (xy.index < t.Length)
            {
                yield return xy;

                xy.x++;
                if (xy.x >= width)
                {
                    xy.x = 0;
                    xy.y++;
                }

                xy.index++;
            }
        }

        public static void set<T>(this T[] t, int width, int x, int y, T value)
        {
            var index = index2(width, x, y);
            t[index] = value;
        }

        public static T get<T>(this T[] t, int width, int x, int y)
        {
            var index = index2(width, x, y);
            return t[index];
        }

        public static IEnumerable<XYZ<T>> xyz<T>(this T[] t, int width, int height)
        {
            var xyz = new XYZ<T>();

            while (xyz.index < t.Length)
            {
                yield return xyz;

                xyz.x++;
                if (xyz.x >= width)
                {
                    xyz.x = 0;
                    xyz.y++;

                    if (xyz.y >= height)
                    {
                        xyz.y = 0;
                        xyz.z++;
                    }
                }

                xyz.index++;
            }
        }

        public static void set<T>(this T[] t, int width, int height, int x, int y, int z, T value)
        {
            var index = index3(width, height, x, y, z);
            t[index] = value;
        }

        public static T get<T>(this T[] t, int width, int height, int x, int y, int z)
        {
            var index = index3(width, height, x, y, z);
            return t[index];
        }

        public static int index2(int width, int x, int y)
        {
            return x + (y * width);
        }

        public static int index3(int width, int height, int x, int y, int z)
        {
            return x + (y * width) + (z * width * height);
        }

        public static T[] create3<T>(int width, int height, int depth)
        {
            return new T[width * height * depth];
        }

        public static T[] create3<T>(int size)
        {
            return new T[size * size * size];
        }

        public static T[] create2<T>(int width, int height)
        {
            return new T[width * height];
        }

        public static T[] create2<T>(int size)
        {
            return new T[size * size];
        }

        public static void quickSort<T>(this T[] elements)
            where T : IComparable<T>
        {
            elements.quickSort(0, elements.Length - 1);
        }

        public static void quickSort<T>(this T[] elements, int left, int right)
            where T : IComparable<T>
        {
            int i = left, j = right;
            T pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    T tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                quickSort<T>(elements, left, j);
            }

            if (i < right)
            {
                quickSort<T>(elements, i, right);
            }
        }


        public static void quickSort(this IComparable[] elements, int left, int right)
        {
            int i = left, j = right;
            IComparable pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                quickSort(elements, left, j);
            }

            if (i < right)
            {
                quickSort(elements, i, right);
            }
        }
    }
}

