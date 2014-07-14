using Atma.Graphics;

namespace Atma.MonoGame.Graphics
{
    public class RenderToTexture : IRenderTarget
    {
        public Microsoft.Xna.Framework.Graphics.RenderTarget2D target;

        public int width { get { return target.Width; } }

        public int height { get { return target.Height; } }

        public RenderToTexture(Microsoft.Xna.Framework.Graphics.RenderTarget2D rt2d)
        {
            target = rt2d;
        }

        public void enable()
        {
            //MonoGL.instance._device.SetRenderTarget(target);
        }

        public void Dispose()
        {
            target.Dispose();
            target = null;
        }
    }
}
