using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface ITile : Drawable
    {
        TileType Type { get; set; }
        int X { get; }
        int Y { get; }
        Vector2i PixelPosition { get; }
    }
}