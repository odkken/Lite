using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface ITile : Drawable
    {
        bool Activated { get; set; }
        void Undo();
        Vector2i Coord { get; }
        int X { get; }
        int Y { get; }
        bool Satisfied { get; }
        void Disable();
    }
}