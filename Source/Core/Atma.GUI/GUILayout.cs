﻿using Atma.Managers;
using Microsoft.Xna.Framework;
using System;

namespace Atma
{
    public sealed class GUILayout
    {
        private GUIManager gui;
        private LayoutCache current = new LayoutCache();

        public static AxisAlignedBox kDummyRect = new AxisAlignedBox(0, 0, 1, 1);

        internal GUILayout(GUIManager gui)
        {
            this.gui = gui;
        }

        private AxisAlignedBox DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            if (style.isHeightDependantOnWidth)
            {
                current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
            }
            else
            {
                Vector2 vector = style.calcSize(content);
                current.topLevel.Add(new GUILayoutEntry(vector.X, vector.X, vector.Y, vector.Y, style, options));
                return new AxisAlignedBox(Vector2.Zero, vector);
            }
            return kDummyRect;
        }

        private AxisAlignedBox DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options)
        {
            current.topLevel.Add(new GUILayoutEntry(minWidth, maxWidth, minHeight, maxHeight, style, options));
            return kDummyRect;
        }

        public void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            gui.label2(GetRect(content, style, options), content, style);
        }

        public AxisAlignedBox GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetRect(content, style, options);
        }


        //public struct GUILayout2
        //{

        //    private bool isVertical = true;

        //    public void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        //    {
        //        gui.label2(GetRect(content, style, options), content, style);
        //    }
        //}
    }

    public sealed class GUILayoutOld
    {
        private GUIManagerOld gui;
        private LayoutCache current = new LayoutCache();

        public static AxisAlignedBox kDummyRect = new AxisAlignedBox(0, 0, 1, 1);

        internal GUILayoutOld(GUIManagerOld gui)
        {
            this.gui = gui;
        }

        private AxisAlignedBox DoGetRect(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            if (style.isHeightDependantOnWidth)
            {
                current.topLevel.Add(new GUIWordWrapSizer(style, content, options));
            }
            else
            {
                Vector2 vector = style.calcSize(content);
                current.topLevel.Add(new GUILayoutEntry(vector.X, vector.X, vector.Y, vector.Y, style, options));
                return new AxisAlignedBox(Vector2.Zero, vector);
            }
            return kDummyRect;
        }

        private AxisAlignedBox DoGetRect(float minWidth, float maxWidth, float minHeight, float maxHeight, GUIStyle style, GUILayoutOption[] options)
        {
            current.topLevel.Add(new GUILayoutEntry(minWidth, maxWidth, minHeight, maxHeight, style, options));
            return kDummyRect;
        }

        public void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        {
            gui.label2(GetRect(content, style, options), content, style);
        }

        public AxisAlignedBox GetRect(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return DoGetRect(content, style, options);
        }


        //public struct GUILayout2
        //{

        //    private bool isVertical = true;

        //    public void DoLabel(GUIContent content, GUIStyle style, GUILayoutOption[] options)
        //    {
        //        gui.label2(GetRect(content, style, options), content, style);
        //    }
        //}
    }


    public struct GUILayout3
    {
        internal GUIManagerOld gui;
        internal Vector2 position;
        internal GUIAnchor anchor;

        public void label(object arg)
        {
            label(1f, arg);
        }

        public void label(string text)
        {
            label(1f, text);
        }

        public void label(string text, params object[] args)
        {
            label(string.Format(text, args));
        }

        public void label(float scale, object arg)
        {
            if (arg == null)
                throw new ArgumentNullException();

            label(scale, arg.ToString());
        }

        public void label(float scale, string text)
        {

        }

        public void label(float scale, string text, params object[] args)
        {
            label(scale, string.Format(text, args));
        }

        //private void processLabel(
    }

}
