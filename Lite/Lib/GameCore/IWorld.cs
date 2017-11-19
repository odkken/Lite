using System.Collections.Generic;
using Lite.Lib.Entities;
using SFML.System;

namespace Lite.Lib.GameCore
{
    public interface IWorld
    {
        void Load(List<string> lines);

        Vector2i PlayerCoord { get; }
        float ScreenFractionPerTile { get; }
        Vector2i GetCoordOfEntity(Entity entity);
        TileState GetTileState(Vector2i vector2I);
        Entity GetEntity(int entityId);
    }

    public enum TileState
    {
        Walkable,
        Pit,
        Wall,
        Door,
        Block
    }
}