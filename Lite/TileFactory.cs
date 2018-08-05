using System;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.System;

namespace Lite
{
    public class TileFactory : ITileFactory
    {

        public int GetTileSize(Vector2i gridSize)
        {
            return _getTileSize(gridSize);
        }
        private readonly Func<int, Vector2i, Vector2i, Vector2f> _getScreenPos;
        private readonly Func<Vector2i, int> _getTileSize;

        public TileFactory(Func<Vector2i, int> getTileSize, Func<int, Vector2i, Vector2i, Vector2f> getScreenPos)
        {
            _getTileSize = getTileSize;
            _getScreenPos = getScreenPos;
        }

        public ITile CreateTile(int x, int y, int tileSize, Vector2i boardSize, TileType type)
        {
            return new Tile(new Vector2i(x, y), new Vector2f(tileSize, tileSize), (i, vector2I) => _getScreenPos(i, vector2I, boardSize), type);
        }
    }
}