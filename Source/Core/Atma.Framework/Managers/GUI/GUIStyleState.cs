using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using zSprite.Resources;

namespace zSprite
{
    public class GUIStyleState
    {
        public string texture;

        private Material _material;

        //kdpublic TextureRef texture;
        public Color textColor = Color.White;
        public Color backgroundColor = Color.White;

        public Material material
        {
            get
            {
                if (_material == null && !string.IsNullOrEmpty(texture))
                {
                    _material = Root.instance.resources.createMaterialFromTexture(texture);
                    _material.SetSamplerState(SamplerState.LinearClamp);
                }

                return _material;
            }
        }

        public GUIStyleState()
        {

        }

        public GUIStyleState(Color textColor)
        {
            this.textColor = textColor;
        }
        public GUIStyleState(string texture, Color textColor, Color backgroundColor)
        {
            this.texture = texture;
            this.textColor = textColor;
            this.backgroundColor = backgroundColor;
        }
    }
}
