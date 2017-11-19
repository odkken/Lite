using System;
using Lite.Lib.Entities;
using SFML.Graphics;

namespace Lite.Lib.GameCore
{
    internal class Empty : Tile
    {
        protected override Color Color => Color.Transparent;

        public void StoreEntity(Entity entity)
        {
            Contents = entity;
        }
        public override TileType TileType => TileType.Empty;

        public Entity Contents { get; private set; }

        public void RemoveEntity(Entity block)
        {
            if(Contents != block)
                throw new Exception("Not my shit!");
            Contents = null;
        }
    }
}