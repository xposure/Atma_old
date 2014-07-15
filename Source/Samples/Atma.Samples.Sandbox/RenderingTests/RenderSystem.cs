using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;
using Atma.Graphics;
using Atma.Systems;
using Atma.TwoD.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace Atma.Samples.Sandbox.RenderingTests
{
    public class RenderSystem : IComponentSystem, IRenderSystem
    {
        private DisplayDevice _display;
        private GraphicSubsystem _graphics;
        private WorldRenderer _world;
        private AssetManager _assets;
        private Texture2D _texture;

        private Vector2 p = Vector2.Zero;

        //private SpriteBatch _batch;
        public void init()
        {
            _display = this.display();
            _graphics = this.graphics();
            _world = this.world();
            _assets = this.assets();

            var white = Color.White;
            white.A = 0;

            _texture = new Texture2D(_display.device, 1, 1);
            _texture.SetData(new Color[] { white });
            //_batch = new SpriteBatch(_display.device);
        }

        public void shutdown()
        {
            //_batch.Dispose();
        }

        public void renderOpaque()
        {
            _graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullClockwise, null, _world.viewMatrix);
            _graphics.batch.Draw(_texture, drawRectangle: new Rectangle(0, 0, 20, 20), color: Color.Red, depth: 0.5f);
            _graphics.end();
            //_graphics.begin(SpriteSortMode.Texture, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, _world.viewMatrix);
            //for (var i = 0; i < 20; i++)
            //    for (var k = 0; k < 2048; k++)
            //        _graphics.batch.Draw(_texture, drawRectangle: new Rectangle(0, 0, 20, 20), color: Color.Red, depth: 1f);
            //_graphics.end();
        }

        public void renderAlphaBlend()
        {
        }

        public void renderOverlay()
        {
            var scale = new Vector2(10, 10);
            var state = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W)) p += new Vector2(0, 0.1f) * scale ;
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.S)) p += new Vector2(0, -0.1f) * scale;
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A)) p += new Vector2(-0.1f, 0f) * scale;
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D)) p += new Vector2(0.1f, 0f) * scale;



            var white = Color.FromNonPremultiplied(255, 255, 255, 128);
            //white.A = 0;

            _graphics.begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.DepthRead, RasterizerState.CullClockwise, null, _world.viewMatrix);
            _graphics.batch.Draw(_texture, drawRectangle: new Rectangle((int)p.X , (int)p.Y, 20, 20), color: white, depth: 0f);
            _graphics.end();
        }
    }
}
