using Lite.Lib.Interface;
using SFML.System;

namespace Lite
{
    public interface ITileFactory
    {
        ITile CreateTile(int x, int y, int tileSize, Vector2i boardSize, TileType type);
        int GetTileSize(Vector2i gridSize);
    }
}