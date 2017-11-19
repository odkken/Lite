using System.Collections.Generic;
using Lite.Lib.Entities;
using SFML.System;

namespace Lite.Lib.GameCore
{
    public interface IWorld
    {
        void Load(int level);

        Vector2i PlayerCoord { get; }
        Vector2i GetCoordOfEntity(Entity entity);
        Entity GetEntity(int entityId);
        void LoadNextLevel();
        Tile GetTileAt(Vector2i position);
    }
}