﻿using Atma.Graphics;
using Atma.Managers;
using System;

namespace Atma
{
    public interface IGUIElement
    {
        GUIContent getContent();
        AxisAlignedBox getRect();
        GUIStyle getStyle();
        bool isHovering { get; }
        bool isActive { get; }
        bool isOn { get; }
        bool isFocused { get; }
        void render(GUIManagerOld gui);
    }

    public abstract class GUIElement : IGUIElement
    {
        public string text;
        public Material icon;
        public string tooltip;
        public GUIStyle style;

        public GUIContent getContent()
        {
            return new GUIContent(text, icon, tooltip);
        }

        public abstract AxisAlignedBox getRect();

        public abstract GUIStyle getStyle();

        public abstract bool isHovering { get; }
        public abstract bool isActive { get; }
        public abstract bool isOn { get; }
        public abstract bool isFocused { get; }

        public abstract void render(GUIManagerOld gui);
    }

    public class GUIButton : IGUIElement
    {
        public GUIContent getContent()
        {
            throw new NotImplementedException();
        }

        public AxisAlignedBox getRect()
        {
            throw new NotImplementedException();
        }

        public GUIStyle getStyle()
        {
            throw new NotImplementedException();
        }

        public bool isHovering
        {
            get { throw new NotImplementedException(); }
        }

        public bool isActive
        {
            get { throw new NotImplementedException(); }
        }

        public bool isOn
        {
            get { throw new NotImplementedException(); }
        }

        public bool isFocused
        {
            get { throw new NotImplementedException(); }
        }

        public void render(GUIManagerOld gui)
        {
            throw new NotImplementedException();
        }
    }
}
