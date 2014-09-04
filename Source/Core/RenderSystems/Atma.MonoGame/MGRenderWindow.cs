using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Atma.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.MonoGame
{
    public class MGRenderWindow : RenderWindow
    {
        //protected bool isFullScreen;
        private MGRenderSystem _renderSystem;
        private RenderForm _form;
        internal GraphicsDevice _device;
        private SpriteBatch test;
        private Texture2D tex;

        public MGRenderWindow(MGRenderSystem renderSystem)
        {
            _renderSystem = renderSystem;
        }

        public override void resize(int width, int height)
        {
            width = width < 16 ? 16 : width;
            height = height < 16 ? 16 : height;
            this.Height = height;
            this.Width = width;
            //createXnaResources();
        }

        private void createXnaResources()
        {
            //if (!isFullScreen)
            {
                //PresentationParameters p = new PresentationParameters(); // (_device.PresentationParameters);//swapchain
                //p.BackBufferWidth = Width;
                //p.BackBufferHeight = Height;

                _form.Width = Width;
                _form.Height = Height;

                //_swapChain.Dispose();
                //_swapChain = new XFG.SwapChain( _device, p );
                /*_stencilBuffer.Dispose();
                _stencilBuffer = new XFG.DepthStencilBuffer(
                    _device,
                    width, height,
                    _device.PresentationParameters.AutoDepthStencilFormat,
                    _device.PresentationParameters.MultiSampleType,
                    _device.PresentationParameters.MultiSampleQuality
                    );*/

                // customAttributes[ "SwapChain" ] = _swapChain;
                test = new SpriteBatch(_device);
                tex = new Texture2D(_device, 1, 1);
                tex.SetData(new Microsoft.Xna.Framework.Color[] { Microsoft.Xna.Framework.Color.White });
            }
        }

        public override void prepareToRender()
        {
            _device.SetRenderTarget(null);
        }

        public override void update()
        {
            //_device
            _device.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            _device.Viewport = new Viewport(0, 0, Width, Height);
            test.Begin();
            test.Draw(tex, new Microsoft.Xna.Framework.Rectangle(100,100,200,200), Microsoft.Xna.Framework.Color.White);
            test.End();
            _device.Present();
        }

        public override void create(string name, int width, int height, bool fullScreen)
        {

            _form = new RenderForm(this);
            this.Width = width;
            this.Height = height;
            //_form.Top = 0;
            //_form.Left = 0;'
            _form.Text = name;
            _form.Show();

            var service = GraphicsDeviceService.AddRef(_form.Handle, width, height);
            _device = service.GraphicsDevice;

            MGWin32Platform.instance.RegisterWindow(this);

            createXnaResources();
            //return _form.Handle;
        }

        protected override void onDispose()
        {
            if (_form != null)
            {
                _renderSystem.DisposeWindow(this);

                MGWin32Platform.instance.UnregisterWindow(this);

                _form.Dispose();
                _form = null;
            }

            _device = null;
        }
    }
}
