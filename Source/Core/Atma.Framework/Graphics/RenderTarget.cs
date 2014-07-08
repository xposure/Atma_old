using System;

namespace Atma.Graphics
{
    public interface IRenderTarget : IDisposable
    {
        void enable();
        int width { get; }
        int height { get; }

    }
}
