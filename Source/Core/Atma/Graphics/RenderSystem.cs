using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Graphics
{
    public abstract class RenderSystem
    {
        public virtual void beginFrame()
        {

        }

        public virtual void update(RenderWindow window)
        {
            window.prepareToRender();
            window.update();
        }

        public virtual void endFrame()
        {

        }

        public abstract RenderWindow CreateWindow(bool isFullscreen, int width, int height);

        public abstract void Initialize(bool autoCreateWindow);
    }
}
