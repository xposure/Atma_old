//#region Using Statements
//using Atma.Collections;
//using Atma.Engine;
//using Atma.Managers;
//using Atma.Systems;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
////using Microsoft.Xna.Framework.GamerServices;
//using System.Diagnostics;
//#endregion


//namespace Atma
//{
//    public class Root
//    {
//        private static Root _instance = null;
//        public static Root instance
//        {
//            get
//            {
//                if (_instance == null)
//                    throw new ArgumentNullException("_instance");
//                return _instance;
//            }
//        }

//        public Root()
//        {
//            _instance = this;
//        }


//        #region Entities
//        //public event Action<IEntity> add;
//        //public event Action<IEntity> remove;

//        //private List<IEntity> _entities = new List<IEntity>(1024);
//        //private List<IEntity> _addEntities = new List<IEntity>(64);
//        //private List<IEntity> _removeEntities = new List<IEntity>(64);

//        //private List<ISystem> _systems = new List<ISystem>();
//        //private List<ISystem> _initSystems = new List<ISystem>();
//        //private List<ISystem> _destroySystems = new List<ISystem>();

//        #endregion


//        //public readonly LogManager logging = new LogManager();
//        //public readonly TimeManager time = new TimeManager();
//        //public readonly ResourceManager resources = new ResourceManager();
//        //public Atma.Graphics.GraphicSubsystem graphics ;//= new RenderManager();
//        //public readonly RenderManager graphics = new RenderManager();
//        //public readonly ScreenManager screen = new ScreenManager();
//        //public readonly InputManager input = new InputManager();
//        //public readonly GUIManager gui = new GUIManager();

//        //internal  EventManager eventManager = new EventManager();

//        //internal  readonly MessageObjectCache messageCache = new MessageObjectCache();

//        internal ContentManager content;
//        //internal readonly List<GameObject> destroyList = new List<GameObject>();
//        //internal readonly List<GameObject> firstInit = new List<GameObject>();
//        //internal readonly Dictionary<int, GameObject> indexedGO = new Dictionary<int, GameObject>();

//        //internal readonly QuadTree<Collider> physicsQuadTree = new QuadTree<Collider>(new Vector2(16, 16), 20);
//        //internal readonly QuadTree<Collider2> physicsQuadTree2 = new QuadTree<Collider2>(new Vector2(16, 16), 20);
//        //internal readonly QuadTree<Rigidbody> rigidbodiesQuadTree = new QuadTree<Rigidbody>(new Vector2(16, 16), 5);

//        //internal readonly List<GameObject> newGameObjects = new List<GameObject>();

//        //internal List<GameObject> enabledList = new List<GameObject>();
//        //public GameObject RootObject = new GameObject("Root Game Object");

//        private Stopwatch timer = new Stopwatch();
//        public long lastUpdate = 0;
//        public long totalUpdate = 0;
//        public long lastFixedUpdate = 0;
//        public long totalFixedUpdate = 0;

//        //public int totalGOs { get { return indexedGO.Count; } }

//        private int uniqueId = 0;
//        public int nextId()
//        {
//            return uniqueId++;
//        }
//        //internal  List<GameObject> visibleList = new List<GameObject>();

//        private double accumulator = 0.0;
//        public bool isRunning = false;

//        internal void start(GraphicsDevice device, ContentManager content)
//        {
//            _instance = this;
//            this.content = content;
//            this.content.RootDirectory = string.Empty;

//            //time.init();


//            //var display = CoreRegistry.require<Atma.Graphics.DisplayDevice>(Atma.Graphics.DisplayDevice.Uri);
//            //display.init();

//            //RootObject = new GameObject("root");
            
//            //input.init();
//            //resources.init();
//            //gui.init();

//            //screen.SetResolution(graphics.graphicsDevice.DisplayMode.Height, graphics.graphicsDevice.DisplayMode.Width, false);

//            reload();
//            //messageCache.cache("update");
//            //messageCache.cache("fixedupdate");
//            //messageCache.cache("render");

//            //System.Threading.Thread.CurrentThread.Name = "Client Thread";
//            //try
//            //{
//            //    isRunning = true;
//            //    var sw = new Stopwatch();
//            //    sw.Start();
//            //    while (isRunning)
//            //    {
//            //        //if (WindowEventMonitor.Instance.MessagePump != null)
//            //        //    WindowEventMonitor.Instance.MessagePump();

//            //        update(sw.Elapsed.TotalSeconds);
//            //        System.Threading.Thread.Sleep(10);
//            //    }
//            //    sw.Stop();

//            //}
//            //finally
//            //{
//            //    cleanup();
//            //}
//        }

//        //public void addEntity(IEntity e)
//        //{
//        //    _addEntities.Add(e);
//        //}

//        //public void removeEntity(IEntity e)
//        //{
//        //    _removeEntities.Add(e);
//        //}

//        //public void addSystem(ISystem system)
//        //{
//        //    //_systems.Add(system);
//        //    _initSystems.Add(system);
//        //}

//        //public void removeSystem(ISystem system)
//        //{
//        //    _destroySystems.Add(system);
//        //    //_systems.Remove(system);
//        //}

//        internal void update(double newTime)
//        {
//            _instance = this;

//            #region Entities
//            //for (var i = 0; i < _destroySystems.Count; i++)
//            //{
//            //    _destroySystems[i].destroy();
//            //    _systems.Remove(_destroySystems[i]);
//            //}

//            //_destroySystems.Clear();

//            //for (var i = 0; i < _initSystems.Count; i++)
//            //{
//            //    _initSystems[i].init();
//            //    _systems.Add(_initSystems[i]);
//            //}

//            //_initSystems.Clear();

//            //while (_removeEntities.Count > 0)
//            //{
//            //    var next = _removeEntities[_removeEntities.Count - 1];
//            //    var index = _entities.IndexOf(next);

//            //    if (index == -1)
//            //        throw new ArgumentOutOfRangeException("index");

//            //    if (_addEntities.Count > 0)
//            //    {
//            //        _entities[index] = _addEntities[_addEntities.Count - 1];
//            //        _addEntities.RemoveAt(_addEntities.Count - 1);

//            //        if (add != null)
//            //            add(_entities[index]);
//            //    }

//            //    if (remove != null)
//            //        remove(_removeEntities[_removeEntities.Count - 1]);

//            //    _removeEntities.RemoveAt(_removeEntities.Count - 1);
//            //}

//            //while (_addEntities.Count > 0)
//            //{
//            //    _entities.Add(_addEntities[_addEntities.Count - 1]);
//            //    _addEntities.RemoveAt(_addEntities.Count - 1);

//            //    if (add != null)
//            //        add(_entities[_entities.Count - 1]);
//            //}

//            //for (var i = 0; i < _systems.Count; i++)
//            //    _systems[i].update();

//            #endregion

//            //foreach (var go in destroyList)
//            //    go.internalDestroy();

//            //destroyList.Clear();


//            //while (Event.Count("init") > 0)
//            //{
//            //    Event.Invoke("init");
//            //    Event.Clear("init");


//            //    foreach (var newgo in newGameObjects)
//            //        newgo.parent.forceSendMessage("childadded", newgo);

//            //    newGameObjects.Clear();
//            //}

//            //Root.instance.input.update();

//            //var fixedDelta = 1.0 / 60;
//            //var time = CoreRegistry.require<Atma.Core.TimeBase>(Atma.Core.TimeBase.Uri);
//            //foreach (var tick in time.tick())
//            //{

//            //    lastUpdate = 0;
//            //    timer.Reset();
//            //    timer.Start();

//            //    Event.Invoke("beforeupdate");
//            //    Event.Invoke("update");
//            //    Event.Invoke("afterupdate");

//            //    timer.Stop();
//            //    lastUpdate = timer.ElapsedMilliseconds;
//            //    totalUpdate += lastUpdate;
//            //    //time.rawTimeInMs
//            //    //double frameTime = newTime - time.realTimeInMs;

//            //    // note: max frame time to avoid spiral of death          
//            //    //if (frameTime > time.maximumDeltaTimeD /* * time.timeScale */)
//            //    //    frameTime = time.maximumDeltaTimeD /* * time.timeScale */;
//            //    var delta = time.deltaL;
//            //    accumulator += tick;

//            //    if (accumulator >= fixedDelta)
//            //    {
//            //        while (accumulator >= fixedDelta)
//            //        {
//            //            accumulator -= fixedDelta;

//            //            time.updateDelta((long)(fixedDelta * 1000));
//            //            //time.updateFixed(time.fixedDeltaTimeD);

//            //            lastFixedUpdate = 0;
//            //            timer.Reset();
//            //            timer.Start();

//            //            //call fixed update   
//            //            Event.Invoke("beforefixedupdate");
//            //            Event.Invoke("fixedupdate");
//            //            Event.Invoke("afterfixedupdate");

//            //            timer.Stop();
//            //            lastFixedUpdate = timer.ElapsedMilliseconds;
//            //            totalFixedUpdate += lastFixedUpdate;
//            //            //messageCache.invoke("fixedupdate");
//            //            //Console.WriteLine("fixed update");
//            //        }
//            //    }
//            //}
//            //messageCache.invoke("update");

//            //call update
//            //Console.WriteLine("update {0}", time.smoothedTimeDeltaD);
//        }

//        internal void draw()
//        {
//            _instance = this;

//            //for (var i = 0; i < _systems.Count; i++)
//            //    _systems[i].render();

//            //graphics.drawCallsThisFrame = 0;
//            //graphics.spritesSubmittedThisFrame = 0;

//            //Event.Invoke("render");
//            //messageCache.invoke("render");
//            //Camera.drawAll();
//            //gui.render();
//            //physicsQuadTree2.Render(Color.Blue, Color.Green);
//            //graphics.drawCalls = graphics.drawCallsThisFrame;
//            //graphics.spritesSubmitted = graphics.spritesSubmittedThisFrame;

//        }

//        internal void cleanup()
//        {
//            _instance = this;
//            //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
//            //resources.cleanup();
//        }

//        internal void reload()
//        {
//            _instance = this;

//            //if (RootObject != null)
//            //    RootObject.destroy();

//            //foreach (var go in destroyList)
//            //    go.internalDestroy();

//            //RootObject = new GameObject("root");

//            //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
//            //resources.reload();
//        }

//        //public GameObject find(int id)
//        //{
//        //    GameObject go;
//        //    indexedGO.TryGetValue(id, out go);
//        //    return go;
//        //}

//    }
//}
