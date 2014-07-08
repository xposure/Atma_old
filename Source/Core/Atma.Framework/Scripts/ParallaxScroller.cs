
namespace Atma
{
    public class ParallaxScroller : Script
    {
        public float speed = 1f;

        private void render()
        {
            transform.Position = (mainCamera.transform.DerivedPosition * speed);
        }
    }
}
