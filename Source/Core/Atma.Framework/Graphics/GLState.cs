using Microsoft.Xna.Framework;

namespace Atma.Graphics
{
    public partial class GL
    {
        protected struct GLState
        {
            public float depth, rotation;
            public Vector2 position, scale;
            public AxisAlignedBox source;
            public Material material;
            public Texture2D texture;
            public Color color;

            //public bool isClipping { get { return !clip.IsNull; } }

            public static GLState empty
            {
                get
                {
                    return new GLState()
                    {
                        position = Vector2.Zero,
                        rotation = 0,
                        //rotation = -Utility.PIOverTwo,
                        color = Color.White,
                        depth = 0f,
                        scale = Vector2.One,
                        material = null,
                        source = AxisAlignedBox.Null
                    };
                }
            }
        }
    }
}
