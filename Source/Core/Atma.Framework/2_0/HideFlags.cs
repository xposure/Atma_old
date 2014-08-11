using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atma._2_0
{
    [Flags]
    public enum HideFlags
    {
        None = 0,
        HideInHierarchy = 1,
        HideInInspector = 2,
        DontSave = 4,
        NotEditable = 8,
        HideAndDontSave = 13
    }
}
