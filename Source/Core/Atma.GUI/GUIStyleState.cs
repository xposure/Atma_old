using System.Collections.Generic;
using Atma.Assets;
using Atma.Engine;
using Atma.Graphics;
using Atma.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma
{
    public struct GUISize
    {
        public Vector2 min, max;
    }

    public class GUIRenderStyle
    {

    }

    public abstract class GUINode
    {
        protected List<GUINode> _children;

        public void layout()
        {

        }

        public void paint()
        {
            
        }

    }

    public class GUIDocument : GUINode
    {

    }



    public abstract class GUIRenderObject
    {
        public abstract void layout();
        public abstract void paint();

        protected abstract GUISize calculateSize();
    }

    public class GUIRenderText
    {

    }
        
    
    public class GUIStyle2
    {
        
    }

    public class GUIStyleState: GameSystem
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
                    //var assets = CoreRegistry.require<AssetManager>(AssetManager.Uri);

                    //var resources = CoreRegistry.require<ResourceManager>(ResourceManager.Uri);
                    var data = new MaterialData();
                    data.SetSamplerState(SamplerState.LinearClamp);
                    data.texture = texture;
                    _material = assets.createMaterial(new GameUri("gui", texture), data);
                    //_material = resources.createMaterialFromTexture(texture, data);
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
