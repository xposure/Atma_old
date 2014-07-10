using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Atma
{


    public class Script : EventListener
    {
        protected static Random random = new Random();
        //public GUIManager gui { get { return Root.instance.gui; } }
        //public InputManager input { get { return Root.instance.input; } }
        //public LogManager log { get { return Root.instance.logging; } }
        //public Atma.Graphics.GraphicSubsystem graphics { get { return graphics; } }
        //public RenderManager graphics { get { return graphics; } }
        //public ResourceManager resources { get { return Root.instance.resources; } }
        //public ScreenManager screen { get { return Root.instance.screen; } }
        //public TimeManager time { get { return Root.instance.time; } }

        public Camera mainCamera { get { return Camera.mainCamera; } }
        public Camera currentCamera { get { return Camera.current; } }

        public Transform transform { get { return gameObject.transform; } }

        public GameObject rootObject { get { return Root.instance.RootObject; } }

        public virtual GameObject gameObject { get; internal set; }
        public override int id { get { return gameObject.id; } }
        public override bool parentEnabled { get { return gameObject.enabled; } }
    }

    public abstract class EventListener
    {
        #region ActionTypes
        public readonly static Type[] ActionTypes = { 
                typeof(Action),
                typeof(Action<>),
                typeof(Action<,>),
                typeof(Action<,,>),
                typeof(Action<,,,>),
                typeof(Action<,,,,>),
                typeof(Action<,,,,,>),
                typeof(Action<,,,,,,>),
                typeof(Action<,,,,,,,>),
                typeof(Action<,,,,,,,,>)
                //typeof(Action<,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,,,,,>),
                //typeof(Action<,,,,,,,,,,,,,,,>)
            };
        #endregion

        #region EventTypes
        public readonly static Type[] EventTypes = { 
                typeof(Event),
                typeof(Event<>),
                typeof(Event<,>),
                typeof(Event<,,>),
                typeof(Event<,,,>),
                typeof(Event<,,,,>),
                typeof(Event<,,,,,>),
                typeof(Event<,,,,,,>),
                typeof(Event<,,,,,,,>),
                typeof(Event<,,,,,,,,>)
                //typeof(Event<,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,,,,,>),
                //typeof(Event<,,,,,,,,,,,,,,,>)
            };
        #endregion

        #region Event Auto Wire Up Reflection Code

        //calls a method (event) on a specific script, used by the gameloop
        private delegate object ActionBuilder(object target);

        //used to suspend and unsuspend a script from the gameloop
        private delegate void EventWrapper(int id, object caller, bool unregister);

        private struct AutoWireUp
        {
            /// <summary>
            /// helper methods to all events to suspend and unsuspend events from the gameloop
            /// </summary>
            public EventWrapper[] wrappers;

            /// <summary>
            /// builds a delegate to the script's methods (events) that can be called fast (without reflection)
            /// this is cached statically so that all future components of the same type can quickly create a referece to the method
            /// </summary>
            public ActionBuilder[] builders;

            /// <summary>
            /// list of all events the script has access to for the gameloop
            /// </summary>
            public string[] funcs;
        }

        private struct EventTracker
        {
            /// <summary>
            /// is subscribed to the gameloop
            /// </summary>
            public bool isRegistered;

            /// <summary>
            /// fast call delegate to the components event
            /// </summary>
            public object caller;
        }

        /// <summary>
        /// cached reflection logic to suspend and unsuspend from the gameloop (once per class)
        /// this class uses dynamic IL to make sure reflection isn't a performance issue
        /// </summary>
        private static Dictionary<Type, AutoWireUp> autoWireUps = new Dictionary<Type, AutoWireUp>();

        /// <summary>
        ///helper code that references the events the script supports 
        ///this class also tracks if the event is suspended or unsuspended
        /// </summary>
        private EventTracker[] _eventTrackers;

        /// <summary>
        /// Will suspend a function from being called during the game loop
        /// </summary>
        /// <param name="func">event to suspend for the gameloop</param>
        protected void suspendEvent(string func)
        {
            var type = this.GetType();
            AutoWireUp wrapper;
            if (_eventTrackers != null && autoWireUps.TryGetValue(type, out wrapper))
            {
                for (var i = 0; i < wrapper.wrappers.Length; i++)
                {
                    if (_eventTrackers[i].isRegistered && wrapper.funcs[i] == func)
                    {
                        _eventTrackers[i].isRegistered = false;

                        //tell the game loop we don't want to receive this event any more
                        wrapper.wrappers[i](id, _eventTrackers[i].caller, true);
                    }
                }
            }
        }

        /// <summary>
        /// Will resume a funcion to be called during the game loop
        /// </summary>
        /// <param name="func">event to resume for the gameloop</param>
        protected void resumeEvent(string func)
        {
            var type = this.GetType();
            AutoWireUp wrapper;
            if (_eventTrackers != null && autoWireUps.TryGetValue(type, out wrapper))
            {
                for (var i = 0; i < wrapper.wrappers.Length; i++)
                {
                    if (!_eventTrackers[i].isRegistered && wrapper.funcs[i] == func)
                    {
                        _eventTrackers[i].isRegistered = true;

                        //tell the game loop we want to receive this event
                        wrapper.wrappers[i](id, _eventTrackers[i].caller, false);
                    }
                }
            }
        }

        /// <summary>
        /// Will suspend all functions from the game loop
        /// </summary>
        protected internal virtual void removeFromEvents()
        {
            var type = this.GetType();
            AutoWireUp wrapper;
            if (_eventTrackers != null && autoWireUps.TryGetValue(type, out wrapper))
            {
                for (var i = 0; i < wrapper.wrappers.Length; i++)
                {
                    if (_eventTrackers[i].isRegistered)
                    {
                        _eventTrackers[i].isRegistered = false;
                        wrapper.wrappers[i](id, _eventTrackers[i].caller, true);
                    }
                }
            }
        }

        /// <summary>
        /// Will resume all function to the game loop
        /// </summary>
        protected internal virtual void addToEvents()
        {
            var type = this.GetType();
            AutoWireUp wrapper;

            //check if the reflection code has already been cached
            if (!autoWireUps.TryGetValue(type, out wrapper))
            {
                wrapper = new AutoWireUp();

                //get all methods that are not public and aren't getters/setters
                var method = new List<MethodInfo>(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)).Where(x => !x.IsSpecialName).ToList();
                var parentType = type.BaseType;

                //find all parent method's that aren't public and aren't getters/setters
                while (parentType != typeof(EventListener))
                {
                    var methods = parentType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    foreach (var m in methods)
                        //we want the parents to run first, insert(0) is bad but its a one time init per class
                        method.Insert(0, m);
                    parentType = parentType.BaseType;
                }

                wrapper.wrappers = new EventWrapper[method.Count];
                wrapper.builders = new ActionBuilder[method.Count];
                wrapper.funcs = new string[method.Count];

                for (var i = 0; i < method.Count; i++)
                {
                    //build all the dynamic il code
                    wrapper.wrappers[i] = eventWrapper(type, method[i]);
                    wrapper.builders[i] = actionBuilder(type, method[i]);
                    wrapper.funcs[i] = method[i].Name;
                }

                autoWireUps.Add(type, wrapper);
            }

            if (_eventTrackers == null)
            {
                _eventTrackers = new EventTracker[wrapper.builders.Length];
                for (var i = 0; i < wrapper.builders.Length; i++)
                {
                    //create a delegate to the function to be called per class for the game loop
                    _eventTrackers[i] = new EventTracker();
                    _eventTrackers[i].caller = wrapper.builders[i](this);
                }
            }

            for (var i = 0; i < wrapper.wrappers.Length; i++)
            {
                //this willl register the methods to the dispatcher handler of the game loop
                if (!_eventTrackers[i].isRegistered)
                {
                    _eventTrackers[i].isRegistered = true;
                    wrapper.wrappers[i](id, _eventTrackers[i].caller, false);
                }
            }

        }

        /// <summary>
        /// This method will return a delegate that gameloop can quickly invoke
        /// </summary>
        /// <param name="targetType">Script's type</param>
        /// <param name="m">Script's method (event) that should be invoked</param>
        /// <returns>Delegate the gameloop can call</returns>
        private static ActionBuilder actionBuilder(Type targetType, MethodInfo m)
        {
            //get the params to the method that wants to be dynamically invoked
            var types = m.GetParameters().Select(x => x.ParameterType).ToArray();
            var actionType = ActionTypes[types.Length];

            if (types.Length > 0)
            {
                //make sure that if the method has generic params that they are resolved
                actionType = actionType.MakeGenericType(types);
            }

            //get the ctor for the Action class
            var actionctor = actionType.GetConstructors()[0];

            DynamicMethod dynam =
                new DynamicMethod(
                ""
                , typeof(object)
                , new Type[] { typeof(object) }
                , targetType
                , true);

            ILGenerator il = dynam.GetILGenerator();

            //call the method
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldftn, m);
            il.Emit(OpCodes.Newobj, actionctor);
            il.Emit(OpCodes.Ret);

            //builds a delegate that can call this method without reflection (fast)
            return (ActionBuilder)dynam.CreateDelegate(typeof(ActionBuilder));
        }

        /// <summary>
        /// Creates a delegate that can suspend and unsuspend a specific script from the game loop
        /// </summary>
        /// <param name="targetType">Script's type</param>
        /// <param name="m">Script's method to suspend or unsuspend from</param>
        /// <returns></returns>
        private static EventWrapper eventWrapper(Type targetType, MethodInfo m)
        {
            var types = m.GetParameters().Select(x => x.ParameterType).ToArray();
            var eventType = EventTypes[types.Length];

            if (types.Length > 0)
            {
                eventType = eventType.MakeGenericType(types);
            }

            //get the methods to do the actual register and register to the game loop (event manager)
            var eventRegister = eventType.GetMethod("Register", BindingFlags.Static | BindingFlags.Public);
            var eventUnregister = eventType.GetMethod("UnRegister", BindingFlags.Static | BindingFlags.Public);

            DynamicMethod dynam =
                new DynamicMethod(
                ""
                , typeof(void)
                , new Type[] { typeof(int), typeof(object), typeof(bool) }
                , targetType
                , true);

            ILGenerator il = dynam.GetILGenerator();
            var lblExit1 = il.DefineLabel();


            //this IL will take an input param and either register and unregister
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldstr, m.Name);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue_S, lblExit1);

            //requesting to register to the gameloop
            il.Emit(OpCodes.Call, eventRegister);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(lblExit1);

            //requesting to underegister from the gameloop
            il.Emit(OpCodes.Call, eventUnregister);
            il.Emit(OpCodes.Ret);

            //delegate to quickly register and unregister from the gameloop
            return (EventWrapper)dynam.CreateDelegate(typeof(EventWrapper));
        }

        #endregion

        internal bool _enabled = true;

        public virtual bool enabled
        {
            get
            {
                if (!_enabled)
                    return false;

                if (!parentEnabled)
                    return false;

                return true;
            }
            set
            {
                if (_enabled != value && this.enabled)
                {
                    if (value)
                        //Game.messageCache.add(this);
                        addToEvents();
                    else
                        //Game.messageCache.remove(this);
                        removeFromEvents();
                }
                _enabled = value;
            }
        }


        public virtual bool parentEnabled { get { return true; } }

        public abstract int id { get; }
    }

}
