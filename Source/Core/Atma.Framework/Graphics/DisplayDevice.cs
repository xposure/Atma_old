using Atma.Engine;
using Atma.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Graphics
{
    public struct Resolution
    {
        public int width;
        public int height;
        //internal int colorDepth;
        public int refreshRate;
    }

    public class DisplayDevice : ISubsystem
    {
        public static readonly GameUri Uri = "subsystem:display";

        //resolutions	 All fullscreen resolutions supported by the monitor (Read Only).
        public Resolution[] resolutions { get; private set; }

        //currentResolution	 The current screen resolution (Read Only).
        public Resolution currentResolution { get; private set; }

        //showCursor	 Should the cursor be visible?
        public bool showCursor { get; set; }

        //lockCursor	 Should the cursor be locked?
        public bool lockCursor { get; set; }

        //width	 The current width of the screen window in pixels (Read Only).
        public int width { get { return currentResolution.width; } }

        //height	 The current height of the screen window in pixels (Read Only).
        public int height { get { return currentResolution.height; } }

        public Vector2 size { get { return new Vector2(width, height); } }

        //dpi	 The current DPI of the screen / device (Read Only).
        public int dpi { get; private set; }

        //fullScreen	 Is the game running fullscreen?
        public bool fullScreen { get; internal set; }

        //autorotateToPortrait	 Allow auto-rotation to portrait?
        //autorotateToPortraitUpsideDown	 Allow auto-rotation to portrait, upside down?
        //autorotateToLandscapeLeft	 Allow auto-rotation to landscape left?
        //autorotateToLandscapeRight	 Allow auto-rotation to landscape right?
        //orientation	 Specifies logical orientation of the screen.
        //sleepTimeout	 A power saving setting, allowing the screen to dim some time after the

        internal bool screenResolutionChangeRequest = true;

        public DisplayDevice()
        {

        }

        private void initResolutions()
        {
            //var vmRegex = new Regex(@"(\d+) x (\d+) @ (\d+)-bit color", RegexOptions.Compiled);
            //var allVideoModes = graphics.root.RenderSystem.ConfigOptions["Video Mode"].PossibleValues;

            //var foundResolutions = new List<Resolution>(allVideoModes.Count);
            //for (var i = 0; i < allVideoModes.Count; i++)
            //{
            //    var m = vmRegex.Match(allVideoModes[i]);
            //    if (m.Success)
            //    {
            //        var r = new Resolution();
            //        var w = Convert.ToInt32(m.Groups[1].Value);
            //        var h = Convert.ToInt32(m.Groups[2].Value);
            //        var b = Convert.ToInt32(m.Groups[3].Value);

            //        r.width = w;
            //        r.height = h;
            //        //r.colorDepth = b;
            //        r.refreshRate = 60;

            //        foundResolutions.Add(r);
            //    }
            //}

            //resolutions = foundResolutions.ToArray();
            //SetResolution(resolutions[0].width, resolutions[0].width, false);
        }

        private GraphicsDeviceManager _graphicsDevice;
        public void setGraphicsDeviceManager(GraphicsDeviceManager gdm)
        {
            _graphicsDevice = gdm;
        }

        public void init()
        {
            //IsFixedTimeStep = false;
            _graphicsDevice.SynchronizeWithVerticalRetrace = false;
            _graphicsDevice.PreferredBackBufferWidth = 1024;
            _graphicsDevice.PreferredBackBufferHeight = 768;
            _graphicsDevice.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            _graphicsDevice.PreparingDeviceSettings += graphics_PreparingDeviceSettings;
            _graphicsDevice.ApplyChanges();

            initResolutions();
            //System.Windows.Forms.Cursor.Show();
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
        }

        public void SetResolution(int width, int height, bool setFullscreen)
        {
            SetResolution(width, height, setFullscreen, 60);
        }

        public void SetResolution(int width, int height, bool setFullscreen, int refreshRate)
        {
            //find closest matching resolution
            var res = new Resolution();
            res.width = width;
            res.height = height;
            res.refreshRate = refreshRate;
            currentResolution = res;
            fullScreen = setFullscreen;
            screenResolutionChangeRequest = true;
        }

        public GameUri uri { get { return Uri; } }

        public void preUpdate(float delta)
        {
        }

        public void postUpdate(float delta)
        {
        }

        public void shutdown()
        {
        }
    }

}
