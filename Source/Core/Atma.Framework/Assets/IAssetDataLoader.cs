﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Atma.Assets
{
    public interface IAssetDataLoader<out T>
        where T: IAssetData
    {
        T load(Stream stream);
    }
}
