using System;
using System.Runtime.InteropServices;

namespace Atma.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray2<T>
    {
        private T _value0;
        private T _value1;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _value0 = value;
                        break;
                    case 1:
                        _value1 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray3<T>
    {
        private T _value0;
        private T _value1;
        private T _value2;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    case 2:
                        return _value2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _value0 = value;
                        break;
                    case 1:
                        _value1 = value;
                        break;
                    case 2:
                        _value2 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray4<T>
    {
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    case 2:
                        return _value2;
                    case 3:
                        return _value3;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _value0 = value;
                        break;
                    case 1:
                        _value1 = value;
                        break;
                    case 2:
                        _value2 = value;
                        break;
                    case 3:
                        _value3 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray8<T>
    {
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        private T _value5;
        private T _value6;
        private T _value7;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    case 2:
                        return _value2;
                    case 3:
                        return _value3;
                    case 4:
                        return _value4;
                    case 5:
                        return _value5;
                    case 6:
                        return _value6;
                    case 7:
                        return _value7;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _value0 = value;
                        break;
                    case 1:
                        _value1 = value;
                        break;
                    case 2:
                        _value2 = value;
                        break;
                    case 3:
                        _value3 = value;
                        break;
                    case 4:
                        _value4 = value;
                        break;
                    case 5:
                        _value5 = value;
                        break;
                    case 6:
                        _value6 = value;
                        break;
                    case 7:
                        _value7 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray16<T>
    {
        private T _value0;
        private T _value1;
        private T _value2;
        private T _value3;
        private T _value4;
        private T _value5;
        private T _value6;
        private T _value7;
        private T _value8;
        private T _value9;
        private T _value10;
        private T _value11;
        private T _value12;
        private T _value13;
        private T _value14;
        private T _value15;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return _value0;
                    case 1:
                        return _value1;
                    case 2:
                        return _value2;
                    case 3:
                        return _value3;
                    case 4:
                        return _value4;
                    case 5:
                        return _value5;
                    case 6:
                        return _value6;
                    case 7:
                        return _value7;
                    case 8:
                        return _value8;
                    case 9:
                        return _value9;
                    case 10:
                        return _value10;
                    case 11:
                        return _value11;
                    case 12:
                        return _value12;
                    case 13:
                        return _value13;
                    case 14:
                        return _value14;
                    case 15:
                        return _value15;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        _value0 = value;
                        break;
                    case 1:
                        _value1 = value;
                        break;
                    case 2:
                        _value2 = value;
                        break;
                    case 3:
                        _value3 = value;
                        break;
                    case 4:
                        _value4 = value;
                        break;
                    case 5:
                        _value5 = value;
                        break;
                    case 6:
                        _value6 = value;
                        break;
                    case 7:
                        _value7 = value;
                        break;
                    case 8:
                        _value8 = value;
                        break;
                    case 9:
                        _value9 = value;
                        break;
                    case 10:
                        _value10 = value;
                        break;
                    case 11:
                        _value11 = value;
                        break;
                    case 12:
                        _value12 = value;
                        break;
                    case 13:
                        _value13 = value;
                        break;
                    case 14:
                        _value14 = value;
                        break;
                    case 15:
                        _value15 = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public void SetAll(T value)
        {
            _value0 = value;
            _value1 = value;
            _value2 = value;
            _value3 = value;
            _value4 = value;
            _value5 = value;
            _value6 = value;
            _value7 = value;
            _value8 = value;
            _value9 = value;
            _value10 = value;
            _value11 = value;
            _value12 = value;
            _value13 = value;
            _value14 = value;
            _value15 = value;
        }
    }
}
