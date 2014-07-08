using Microsoft.Xna.Framework;

namespace Atma.Graphics
{
    public struct GLRenderable : IRadixKey
    {
        //public long id;
        public int key;
        public float depth;
        public float rotation;
        public Vector2 pivot;
        public Vector2 position;
        public Vector2 scale;
        public Color color;
        public AxisAlignedBox scissorRect;
        public AxisAlignedBox sourceRectangle;
        public GLRenderableType type;
        public Material material;
        public Texture2D texture;
        public IEffect effect;
        //public SpriteEffects effect;
        public bool applyScissor;

        public int Key { get { return key; } }
        //public TextureRef texture;
    }
}
