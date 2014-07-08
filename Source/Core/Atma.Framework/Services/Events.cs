using System;
using System.Collections.Generic;
using System.Text;

namespace Atma
{
    public static partial class Event
    {
        //[De("DEBUG")]
        public static string Build()
        {
            var sb = new StringBuilder(50000);
            for (var i = 0; i <= 10; i++)
            {
                build(sb, i);
            }

            return sb.ToString();
        }

        //[Conditional("DEBUG")]
        private static void build(StringBuilder sb, int args)
        {
            var arg1 = new StringBuilder();
            var arg2 = new StringBuilder();
            var arg3 = new StringBuilder();

            for (var i = 0; i < args; i++)
            {
                if (i == 0)
                {
                    arg1.Append('<');
                    arg2.Append(',');
                }

                arg1.AppendFormat("T{0}", i);
                arg2.AppendFormat("T{0} a{0}", i);
                arg3.AppendFormat("a{0}", i);
                if (i < args - 1)
                {
                    arg1.Append(',');
                    arg2.Append(',');
                    arg3.Append(',');
                }

                if (i == args - 1)
                    arg1.Append('>');

            }

            sb.AppendFormat(code, arg1, arg2, arg3);
        }

        private static string code = @"
    public static partial class Event{0}
    {{
        private class Listeners
        {{
            public readonly string Function;
            public readonly List<Action{0}> Callbacks = new List<Action{0}>();
            public readonly Dictionary<int, List<Action{0}>> CallbackGrouping = new Dictionary<int, List<Action{0}>>();

            public Listeners(string func)
            {{
                this.Function = func;
            }}
        }}

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action{0} action)
        {{
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {{
                l = new Listeners(func);
                listeners.Add(func, l);
            }}

            l.Callbacks.Add(action);

            List<Action{0}> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {{
                groupList = new List<Action{0}>();
                l.CallbackGrouping.Add(group, groupList);
            }}

            groupList.Add(action);
        }}

        public static void UnRegister(int group, string func, Action{0} action)
        {{
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {{
                l.Callbacks.Remove(action);

                List<Action{0}> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {{
                    groupList.Remove(action);
                }}
            }}
        }}

        public static void Invoke(string func{1})
        {{
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {{
                foreach (var c in l.Callbacks)
                    c({2});
            }}
        }}

        public static void Invoke(int group, string func{1})
        {{
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {{
                List<Action{0}> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {{
                    foreach (var c in groupList)
                        c({2});
                }}
            }}
        }}

        public static void Clear(string func)
        {{
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {{
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }}
        }}
    }}";
    }


    public static partial class Event
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action> Callbacks = new List<Action>();
            public readonly Dictionary<int, List<Action>> CallbackGrouping = new Dictionary<int, List<Action>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                for (var i = 0; i < l.Callbacks.Count; i++ )
                    l.Callbacks[i]();
            }
        }

        public static int Count(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
                return l.Callbacks.Count;
           
            return 0;
        }

        public static void Invoke(int group, string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c();
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0>> Callbacks = new List<Action<T0>>();
            public readonly Dictionary<int, List<Action<T0>>> CallbackGrouping = new Dictionary<int, List<Action<T0>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0);
            }
        }

        public static void Invoke(int group, string func, T0 a0)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1>> Callbacks = new List<Action<T0, T1>>();
            public readonly Dictionary<int, List<Action<T0, T1>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2>> Callbacks = new List<Action<T0, T1, T2>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3>> Callbacks = new List<Action<T0, T1, T2, T3>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4>> Callbacks = new List<Action<T0, T1, T2, T3, T4>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4, T5>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4, T5>> Callbacks = new List<Action<T0, T1, T2, T3, T4, T5>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4, T5> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4, T5>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4, T5>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4, T5> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4, T5>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4, a5);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4, T5>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4, a5);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4, T5, T6>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4, T5, T6>> Callbacks = new List<Action<T0, T1, T2, T3, T4, T5, T6>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4, T5, T6>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4, T5, T6>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4, T5, T6>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4, a5, a6);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4, T5, T6>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4, a5, a6);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4, T5, T6, T7>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4, T5, T6, T7>> Callbacks = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4, T5, T6, T7>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4, T5, T6, T7>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4, a5, a6, a7);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4, T5, T6, T7>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4, a5, a6, a7);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4, T5, T6, T7, T8>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> Callbacks = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4, a5, a6, a7, a8);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4, a5, a6, a7, a8);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }
    public static partial class Event<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        private class Listeners
        {
            public readonly string Function;
            public readonly List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> Callbacks = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
            public readonly Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>> CallbackGrouping = new Dictionary<int, List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>>();

            public Listeners(string func)
            {
                this.Function = func;
            }
        }

        private static readonly Dictionary<string, Listeners> listeners = new Dictionary<string, Listeners>();

        public static void Register(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            Listeners l = null;
            if (!listeners.TryGetValue(func, out l))
            {
                l = new Listeners(func);
                listeners.Add(func, l);
            }

            l.Callbacks.Add(action);

            List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> groupList = null;
            if (!l.CallbackGrouping.TryGetValue(group, out groupList))
            {
                groupList = new List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>();
                l.CallbackGrouping.Add(group, groupList);
            }

            groupList.Add(action);
        }

        public static void UnRegister(int group, string func, Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.Callbacks.Remove(action);

                List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    groupList.Remove(action);
                }
            }
        }

        public static void Invoke(string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                foreach (var c in l.Callbacks)
                    c(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
            }
        }

        public static void Invoke(int group, string func, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                List<Action<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> groupList = null;
                if (l.CallbackGrouping.TryGetValue(group, out groupList))
                {
                    foreach (var c in groupList)
                        c(a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
                }
            }
        }

        public static void Clear(string func)
        {
            Listeners l = null;
            if (listeners.TryGetValue(func, out l))
            {
                l.CallbackGrouping.Clear();
                l.Callbacks.Clear();
            }
        }
    }


}
