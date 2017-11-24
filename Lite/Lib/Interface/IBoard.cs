using SFML.System;

namespace Lite.Lib.Interface
{
    public interface IBoard
    {
        ITile GetTile(Vector2i coord);
        ITile GetTile(int xCoord, int yCoord);
    }
}