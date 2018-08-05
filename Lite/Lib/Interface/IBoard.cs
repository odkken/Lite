using System;
using System.Collections;
using System.Numerics;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.Interface
{
    public interface IBoard : Drawable
    {
        (int, int, ITile) GetTileFromScreenCoord(Vector2f fractionalScreenCoord);
        void Update(float dt);
        void LoadLevel(string name);
    }
}