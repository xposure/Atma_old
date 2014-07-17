using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atma.Graphics
{
    internal class SpriteBatchItem
    {
        public Texture2D Texture;
        public float Depth;

        public VertexPositionColorTexture vertexTL;
        public VertexPositionColorTexture vertexTR;
        public VertexPositionColorTexture vertexBL;
        public VertexPositionColorTexture vertexBR;
        public SpriteBatchItem()
        {
            vertexTL = new VertexPositionColorTexture();
            vertexTR = new VertexPositionColorTexture();
            vertexBL = new VertexPositionColorTexture();
            vertexBR = new VertexPositionColorTexture();
        }

        public void Set(Vector2 tl, Vector2 tr, Vector2 bl, Color color, Vector2 texTL, Vector2 texTR, Vector2 texBL)
        {
            vertexTL.Position.X = tl.X;
            vertexTL.Position.Y = tl.Y;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = color;
            vertexTL.TextureCoordinate.X = texTL.X;
            vertexTL.TextureCoordinate.Y = texTL.Y;

            vertexTR.Position.X = tr.X;
            vertexTR.Position.Y = tr.Y;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texTR.X;
            vertexTR.TextureCoordinate.Y = texTR.Y;

            vertexBL.Position.X = bl.X;
            vertexBL.Position.Y = bl.Y;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texBL.X;
            vertexBL.TextureCoordinate.Y = texBL.Y;

        }

        public void Set(Vector2 tl, Vector2 tr, Vector2 bl, Vector2 br, Color color, Vector2 texTL, Vector2 texTR, Vector2 texBR, Vector2 texBL)
        {
            vertexTL.Position.X = tl.X;
            vertexTL.Position.Y = tl.Y;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = color;
            vertexTL.TextureCoordinate.X = texTL.X;
            vertexTL.TextureCoordinate.Y = texTL.Y;

            vertexTR.Position.X = tr.X;
            vertexTR.Position.Y = tr.Y;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texTR.X;
            vertexTR.TextureCoordinate.Y = texTR.Y;

            vertexBL.Position.X = bl.X;
            vertexBL.Position.Y = bl.Y;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texBL.X;
            vertexBL.TextureCoordinate.Y = texBL.Y;

            vertexBR.Position.X = br.X;
            vertexBR.Position.Y = br.Y;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = color;
            vertexBR.TextureCoordinate.X = texBR.X;
            vertexBR.TextureCoordinate.Y = texBR.Y;
        }

        public void Set(float x, float y, float w, float h, Color color, Vector2 texCoordTL, Vector2 texCoordBR)
        {
            vertexTL.Position.X = x;
            vertexTL.Position.Y = y;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = color;
            vertexTL.TextureCoordinate.X = texCoordTL.X;
            vertexTL.TextureCoordinate.Y = texCoordTL.Y;

            vertexTR.Position.X = x + w;
            vertexTR.Position.Y = y;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texCoordBR.X;
            vertexTR.TextureCoordinate.Y = texCoordTL.Y;

            vertexBL.Position.X = x;
            vertexBL.Position.Y = y + h;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texCoordTL.X;
            vertexBL.TextureCoordinate.Y = texCoordBR.Y;

            vertexBR.Position.X = x + w;
            vertexBR.Position.Y = y + h;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = color;
            vertexBR.TextureCoordinate.X = texCoordBR.X;
            vertexBR.TextureCoordinate.Y = texCoordBR.Y;
        }

        public void Set(float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color, Vector2 texCoordTL, Vector2 texCoordBR)
        {
            // TODO, Should we be just assigning the Depth Value to Z?
            // According to http://blogs.msdn.com/b/shawnhar/archive/2011/01/12/spritebatch-billboards-in-a-3d-world.aspx
            // We do.
            vertexTL.Position.X = x + dx * cos - dy * sin;
            vertexTL.Position.Y = y + dx * sin + dy * cos;
            vertexTL.Position.Z = Depth;
            vertexTL.Color = color;
            vertexTL.TextureCoordinate.X = texCoordTL.X;
            vertexTL.TextureCoordinate.Y = texCoordTL.Y;

            vertexTR.Position.X = x + (dx + w) * cos - dy * sin;
            vertexTR.Position.Y = y + (dx + w) * sin + dy * cos;
            vertexTR.Position.Z = Depth;
            vertexTR.Color = color;
            vertexTR.TextureCoordinate.X = texCoordBR.X;
            vertexTR.TextureCoordinate.Y = texCoordTL.Y;

            vertexBL.Position.X = x + dx * cos - (dy + h) * sin;
            vertexBL.Position.Y = y + dx * sin + (dy + h) * cos;
            vertexBL.Position.Z = Depth;
            vertexBL.Color = color;
            vertexBL.TextureCoordinate.X = texCoordTL.X;
            vertexBL.TextureCoordinate.Y = texCoordBR.Y;

            vertexBR.Position.X = x + (dx + w) * cos - (dy + h) * sin;
            vertexBR.Position.Y = y + (dx + w) * sin + (dy + h) * cos;
            vertexBR.Position.Z = Depth;
            vertexBR.Color = color;
            vertexBR.TextureCoordinate.X = texCoordBR.X;
            vertexBR.TextureCoordinate.Y = texCoordBR.Y;
        }
    }
}

