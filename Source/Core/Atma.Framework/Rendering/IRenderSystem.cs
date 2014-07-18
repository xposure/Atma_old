using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma.Rendering
{
    public interface IRenderSystem : ICore
    {
        void renderOpaque();
        //void renderAlphaReject();
        void renderAlphaBlend();
        void renderOverlay();
        void renderShadows(); 
        //void renderFirstPerson();
        //void renderShadows();
    }
}
