using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Entities
{
    public abstract class Tile : Entity
    {
        public static int TileSize => 100;

        protected Tile()
        {
            _rect = new RectangleShape();
        }

        public void SetPosition(Vector2i pos)
        {
            Position = pos;
        }
        protected abstract Color Color { get; }
        public abstract TileType TileType { get; }

        private readonly RectangleShape _rect;
        protected override void DestroyMe()
        {
            
        }

        protected override void DrawMe(RenderTarget target, RenderStates states)
        {
            _rect.FillColor = Color;
            _rect.Draw(target, states);
        }

        protected override void UpdateMe()
        {
            _rect.Size = new Vector2f(TileSize,TileSize);
            _rect.Position = new Vector2f(Position.X * TileSize, Position.Y * TileSize);
        }
    }

    public enum TileType
    {
        Empty,
        Wall,
        Door,
    }
}