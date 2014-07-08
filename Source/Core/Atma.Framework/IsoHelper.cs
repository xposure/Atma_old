using Microsoft.Xna.Framework;

namespace Atma
{
    public static class IsoHelper            
    {
        public static Vector2 ToDiamondIso(float x, float y)
        {
            return ToDiamondIso(x, y, 0);
        }

        public static Vector2 ToDiamondIso(Vector2 p)
        {
            return ToDiamondIso(p.X, p.Y, 0);
        }

        public static Vector2 ToDiamondIso(Vector2 p, float z)
        {
            return ToDiamondIso(p.X, p.Y, z);
        }

        public static Vector2 ToDiamondIso(Vector3 p)
        {
            return ToDiamondIso(p.X, p.Y, p.Z);
        }

        public static Vector2 ToDiamondIso(float x, float y, float z)
        {
            return new Vector2(x - y - 1 , (x + y) / 2 - z);
        }

        public static Vector2 FromDiamondIso(Vector2 iso)
        {
            return FromDiamondIso(iso.X, iso.Y, 0);
        }

        public static Vector2 FromDiamondIso(Vector2 iso, float z)
        {
            return FromDiamondIso(iso.X, iso.Y, z);
        }

        public static Vector2 FromDiamondIso(Vector3 iso)
        {
            return FromDiamondIso(iso.X, iso.Y, iso.Z);
        }

        public static Vector2 FromDiamondIso(float x, float y)
        {
            return FromDiamondIso(x, y, 0);
        }

        public static Vector2 FromDiamondIso(float x, float y, float z)
        {
            var _y = (2 * y - x) / 2 + z;
            return new Vector2(x + _y, _y);
        }

    }
}
