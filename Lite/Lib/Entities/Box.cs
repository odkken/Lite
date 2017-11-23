using System;
using SFML.Graphics;
using Lite.Lib.GameCore;
using SFML.System;

namespace Lite.Lib.Entities
{
    internal class Box : Entity
    {
        private RectangleShape shape = new RectangleShape { FillColor = Color.Green };
        protected override void DestroyMe()
        {

        }

        protected override void DrawMe(RenderTarget target, RenderStates states)
        {
            shape.Draw(target, states);
        }

        public override Vector2i Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                UpdateMe();
            }
        }

        protected override void UpdateMe()
        {
            var belowPos = GameWorld.GetTileAt(Position + new Vector2i(0, 1));
            if (belowPos is Empty && (belowPos as Empty).Contents == null)
                Position += new Vector2i(0, 1);
            shape.Size = new Vector2f(Tile.TileSize, Tile.TileSize);
            shape.Position = (Vector2f)(Position * Tile.TileSize);
        }
    }
}