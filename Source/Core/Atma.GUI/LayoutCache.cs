﻿using System.Collections.Generic;

namespace Atma
{
    internal sealed class LayoutCache
    {
        internal GUILayoutGroup topLevel = new GUILayoutGroup();
        internal Stack<GUILayoutGroup> layoutGroups = new Stack<GUILayoutGroup>();
        internal GUILayoutGroup windows = new GUILayoutGroup();
        internal LayoutCache()
        {
            this.layoutGroups.Push(this.topLevel);
        }
        internal LayoutCache(LayoutCache other)
        {
            this.topLevel = other.topLevel;
            this.layoutGroups = other.layoutGroups;
            this.windows = other.windows;
        }
    }
}
