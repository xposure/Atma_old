using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atma.Assets;

namespace Atma.Engine.Modules
{
    public interface IModule
    {
        IAssetSource getAssetSource();
    }
}
