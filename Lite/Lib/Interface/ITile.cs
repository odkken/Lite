using SFML.Graphics;

namespace Lite.Lib.Interface
{
    public interface ITile : Drawable
    {
        int X { get; }
        int Y { get; }
    }
}