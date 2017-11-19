using System;
using Lite.Lib.GameCore;
using SFML.Graphics;

namespace Lite.Lib.Entities
{
    internal class Door : Empty
    {
        protected override Color Color => Color.Yellow;
        public override TileType TileType => TileType.Door;
    }
}