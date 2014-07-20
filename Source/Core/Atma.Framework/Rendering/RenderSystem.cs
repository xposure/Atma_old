using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Rendering
{
    public class RenderSystem : IRenderSystem
    {
        public virtual void renderOpaque()
        {
        }

        public virtual void renderAlphaBlend()
        {
        }

        public virtual void renderOverlay()
        {
        }

        public virtual void renderShadows()
        {
        }
    }
}
