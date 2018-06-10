using System;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class EmptyTile : BaseTile
    {
        public EmptyTile(Vector2i position, Vector2f size, Func<Vector2i, Vector2f> getScreenPos) : base(position, size, getScreenPos, new Color(50,50,50), Color.Black)
        {
        }
    }
}