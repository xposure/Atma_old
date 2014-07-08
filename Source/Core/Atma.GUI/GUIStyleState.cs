﻿using Atma.Assets;
using Atma.Engine;
using Atma.Graphics;
using Atma.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma
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
                    var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
                    var data = new MaterialData();
                    data.SetSamplerState(SamplerState.LinearClamp);
                    _material = resources.createMaterialFromTexture(texture, data);
                    //_material.SetSamplerState(SamplerState.LinearClamp);
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
