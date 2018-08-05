using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface ITile : Drawable
    {
        int X { get; }
        int Y { get; }
        Vector2i PixelPosition { get; }
        void SetColor(Color color);
    }
}