using Microsoft.Xna.Framework;

namespace Atma.Graphics
{
    public struct GraphicsState
    {
        public float depth, rotation;
        public Vector2 position, scale;
        public AxisAlignedBox source;
        public Texture2D texture;
        public Color color;

        public static GraphicsState empty
        {
            get
            {
                return new GraphicsState()
                {
                    position = Vector2.Zero,
                    rotation = 0,
                    //rotation = -Utility.PIOverTwo,
                    color = Color.White,
                    depth = 0f,
                    scale = Vector2.One,
                    source = AxisAlignedBox.Null
                };
            }
        }
    }
}
