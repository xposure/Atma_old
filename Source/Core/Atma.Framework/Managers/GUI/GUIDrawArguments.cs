using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zSprite
{
    public struct GUIDrawArguments
    {
        public AxisAlignedBox position;
        public AxisAlignedBox content;
        public bool isHover;
        public bool isActive;
        public bool on;
        public bool hasKeyboardFocus;
    }
}
