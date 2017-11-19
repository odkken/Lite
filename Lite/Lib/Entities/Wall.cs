using System;
using Lite.Lib.GameCore;
using SFML.Graphics;

namespace Lite.Lib.Entities
{
    internal class Wall : Tile
    {
        protected override Color Color => Color.Cyan;
        public override TileType TileType => TileType.Wall;
    }
}