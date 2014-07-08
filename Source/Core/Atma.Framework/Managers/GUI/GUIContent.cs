using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using zSprite.Resources;

namespace zSprite
{
    public struct GUIContent
    {
        public Material icon;
        public string text;
        public string toolip;

        public GUIContent(string text)
        {
            this.text = text;
            this.icon = null;
            this.toolip = string.Empty;
        }

        public GUIContent(string text, Material icon)
        {
            this.text = text;
            this.icon = icon;
            this.toolip = string.Empty;
        }

        public GUIContent(Material icon)
        {
            this.text = string.Empty;
            this.icon = icon;
            this.toolip = string.Empty;
        }

        public GUIContent(string text, string tooltip)
        {
            this.text = text;
            this.icon = null;
            this.toolip = tooltip;
        }

        public GUIContent(Material icon, string tooltip)
        {
            this.text = string.Empty;
            this.icon = icon;
            this.toolip = tooltip;
        }

        public GUIContent(string text, Material icon, string tooltip)
        {
            this.text = text;
            this.icon = icon;
            this.toolip = tooltip;
        }

        public static GUIContent none { get { return new GUIContent(); } }
    }

}
