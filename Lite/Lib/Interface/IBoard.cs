using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface IBoard : Drawable
    {
        (int, int, ITile) GetTileFromScreenCoord(Vector2f fractionalScreenCoord);
        void Update(float dt);
        bool LoadLevel(string name);
        Vector2i Size { get; }
        IEnumerable<ITile> Tiles { get; }
    }
}