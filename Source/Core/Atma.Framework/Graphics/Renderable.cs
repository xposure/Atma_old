using Microsoft.Xna.Framework;

namespace Atma.Graphics
{
    public struct Renderable 
    {
        public float depth;
        public float rotation;
        public Vector2 pivot;
        public Vector2 position;
        public Vector2 scale;
        public Color color;
        public AxisAlignedBox scissorRect;
        public AxisAlignedBox sourceRectangle;
        public Texture2D texture;
    }
}
