using System;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    internal class KeyTile : BaseTile
    {
        public KeyTile(Vector2i position, Vector2f size, Func<Vector2i, Vector2f> getScreenPos) : base(position, size, getScreenPos, new Color (50,150,150), Color.Black)
        {
        }
    }
}