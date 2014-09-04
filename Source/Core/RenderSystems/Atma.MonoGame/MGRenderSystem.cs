using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Graphics;

using XNA = Microsoft.Xna.Framework;
using XFG = Microsoft.Xna.Framework.Graphics;

namespace Atma.MonoGame
{
    public class MGRenderSystem : RenderSystem
    {
        private XFG.GraphicsDevice _device;
        private List<RenderWindow> _windows = new List<RenderWindow>();
        // However many GraphicsDeviceControl instances you have, they all share
        // the same underlying GraphicsDevice, managed by this helper service.
        //GraphicsDeviceService graphicsDeviceService;

        public override RenderWindow CreateWindow(bool isFullscreen, int width, int height)
        {
            var window = new MGRenderWindow(this);

            //var form = MGWin32Platform.instance.CreateForm(window);

            //form.Size = new System.Drawing.Size(width, height);
            //form.Show();

             window.create("test", width, height, isFullscreen);
             _device = window._device;
             _windows.Add(window);
            
            return window;
        }

        public void DisposeWindow(RenderWindow window)
        {
            _windows.Remove(window);
        }

        public override void Initialize(bool autoCreateWindow)
        {
            MGWin32Platform.Initialize();

            if (autoCreateWindow)
            {
                var window = CreateWindow(false, 640, 480);
            }
        }

        public void render()
        {
            foreach (var w in _windows)
                w.update();
        }

        //private XFG.GraphicsDevice initMonoGame(bool isFullscreen, int width, int height)
        //{
        //                if( _device != null )
        //    {
        //        return _device;
        //    }

        //    XFG.GraphicsDevice newDevice;

        //    // if this is the first window, get the device and do other initialization
        //    XFG.PresentationParameters presentParams = new XFG.PresentationParameters();
        //    presentParams.IsFullScreen = isFullscreen;
        //    //presentParams.BackBufferCount = 1;
        //    //presentParams.EnableAutoDepthStencil = depthBuffer;
        //    presentParams.BackBufferFormat = XFG.SurfaceFormat.Color;
        //    presentParams.BackBufferWidth = width;
        //    presentParams.BackBufferHeight = height;
        //    //presentParams.MultiSampleType = XFG.MultiSampleType.None;
        //    //presentParams.RenderTargetUsage = SwapEffect = XFG.SwapEffect.Copy;

        //    return 
        //}
    }
}
