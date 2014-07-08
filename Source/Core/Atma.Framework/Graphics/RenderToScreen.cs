using Atma.Graphics;

namespace Atma.MonoGame.Graphics
{
    public class RenderToScreen : IRenderTarget
    {
        public int width { get { return 0; } }
        public int height { get { return 0; } }

        public void enable()
        {
            MonoGL.instance._device.SetRenderTarget(null);
        }

        public void Dispose()
        {

        }

    }
}
