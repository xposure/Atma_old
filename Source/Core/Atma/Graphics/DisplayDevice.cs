//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Atma.Graphics
//{
//    public struct Resolution
//    {
//        public int width;
//        public int height;
//        //internal int colorDepth;
//        public int refreshRate;
//    }

//    public class DisplayDevice : Object//, ISubsystem
//    {
//        public static readonly GameUri Uri = "subsystem:display";

//        //private GraphicsDeviceManager _graphicsDeviceManager;

//        public event Action<DisplayDevice> onResolutionChange;

//        //resolutions	 All fullscreen resolutions supported by the monitor (Read Only).
//        public Resolution[] resolutions { get; private set; }

//        //currentResolution	 The current screen resolution (Read Only).
//        public Resolution currentResolution { get; private set; }

//        //showCursor	 Should the cursor be visible?
//        public bool showCursor { get { return engine.IsMouseVisible; } set { engine.IsMouseVisible = value; } }

//        //lockCursor	 Should the cursor be locked?
//        public bool lockCursor { get; set; }

//        //width	 The current width of the screen window in pixels (Read Only).
//        public int width { get { return currentResolution.width; } }

//        //height	 The current height of the screen window in pixels (Read Only).
//        public int height { get { return currentResolution.height; } }

//        public Vector2 size { get { return new Vector2(width, height); } }

//        //dpi	 The current DPI of the screen / device (Read Only).
//        public int dpi { get; private set; }

//        //fullScreen	 Is the game running fullscreen?
//        public bool fullScreen { get; internal set; }

//        //autorotateToPortrait	 Allow auto-rotation to portrait?
//        //autorotateToPortraitUpsideDown	 Allow auto-rotation to portrait, upside down?
//        //autorotateToLandscapeLeft	 Allow auto-rotation to landscape left?
//        //autorotateToLandscapeRight	 Allow auto-rotation to landscape right?
//        //orientation	 Specifies logical orientation of the screen.
//        //sleepTimeout	 A power saving setting, allowing the screen to dim some time after the

//        internal bool screenResolutionChangeRequest = true;

//        private void initResolutions()
//        {
//            //var vmRegex = new Regex(@"(\d+) x (\d+) @ (\d+)-bit color", RegexOptions.Compiled);
//            //var allVideoModes = graphics.root.RenderSystem.ConfigOptions["Video Mode"].PossibleValues;

//            //var foundResolutions = new List<Resolution>(allVideoModes.Count);
//            //for (var i = 0; i < allVideoModes.Count; i++)
//            //{
//            //    var m = vmRegex.Match(allVideoModes[i]);
//            //    if (m.Success)
//            //    {
//            //        var r = new Resolution();
//            //        var w = Convert.ToInt32(m.Groups[1].Value);
//            //        var h = Convert.ToInt32(m.Groups[2].Value);
//            //        var b = Convert.ToInt32(m.Groups[3].Value);

//            //        r.width = w;
//            //        r.height = h;
//            //        //r.colorDepth = b;
//            //        r.refreshRate = 60;

//            //        foundResolutions.Add(r);
//            //    }
//            //}

//            //resolutions = foundResolutions.ToArray();
//            //SetResolution(resolutions[0].width, resolutions[0].width, false);
//        }

//        //public GraphicsDevice device { get { return _graphicsDeviceManager.GraphicsDevice; } }

//        public void preInit()
//        {
//            _graphicsDeviceManager = new GraphicsDeviceManager(engine);
//            initResolutions();
//        }

//        public void postInit()
//        {
//            //IsFixedTimeStep = false;

//            _graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
//            _graphicsDeviceManager.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;

//            SetResolution(1024, 768, false);
//            updateScreen();

//        }

//        public void SetResolution(int width, int height, bool setFullscreen)
//        {
//            SetResolution(width, height, setFullscreen, 60);
//        }

//        public void SetResolution(int width, int height, bool setFullscreen, int refreshRate)
//        {
//            //find closest matching resolution
//            var res = new Resolution();
//            res.width = width;
//            res.height = height;
//            res.refreshRate = refreshRate;
//            currentResolution = res;
//            fullScreen = setFullscreen;

//            screenResolutionChangeRequest = true;

//            if (onResolutionChange != null)
//                onResolutionChange(this);
//        }

//        private void updateScreen()
//        {
//            _graphicsDeviceManager.PreferredBackBufferWidth = width;
//            _graphicsDeviceManager.PreferredBackBufferHeight = height;
//            _graphicsDeviceManager.IsFullScreen = fullScreen;
//            _graphicsDeviceManager.ApplyChanges();
//            screenResolutionChangeRequest = false;

//        }

//        public GameUri uri { get { return Uri; } }

//        public void preUpdate(float delta)
//        {
//            if (screenResolutionChangeRequest)
//            {
//                updateScreen();
//            }
//        }

//        public void postUpdate(float delta)
//        {
//            //todo figure out how to enable mouse locking
//            //if (lockCursor)
//            //    Mouse.SetPosition(0, 0);
//        }

//        public void prepareToRender()
//        {
//            device.SetRenderTarget(null);
//            device.Clear(Color.Black);
//        }

//        public void shutdown()
//        {
//        }
//    }
//}
