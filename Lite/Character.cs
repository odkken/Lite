using System;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public class Character : Drawable
    {
        private readonly Func<Vector2i, Vector2f> _posToScreenCoord;
        private RectangleShape _rect;
        public Character(IInput input, Vector2i position, Vector2f size, Func<Vector2i, Vector2f> posToScreenCoord, Func<Vector2i, bool> canMoveTo, int outlineThickness)
        {
            _posToScreenCoord = posToScreenCoord;
            _rect = new RectangleShape(size){OutlineThickness = outlineThickness, OutlineColor = Color.Black, FillColor = Color.Cyan};
            X = position.X;
            Y = position.Y;
            input.KeyPressed += args =>
            {
                var delta = new Vector2i();
                switch (args.Code)
                {
                    case Keyboard.Key.W:
                        delta.Y--;
                        break;
                    case Keyboard.Key.A:
                        delta.X--;
                        break;
                    case Keyboard.Key.S:
                        delta.Y++;
                        break;
                    case Keyboard.Key.D:
                        delta.X++;
                        break;
                    default:
                        return;
                }
                var destination = new Vector2i(X, Y) + delta;
                if (!canMoveTo(destination)) return;
                X = destination.X;
                Y = destination.Y;
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _rect.Position = _posToScreenCoord(new Vector2i(X, Y));
            _rect.Draw(target, states);
        }

        public int X { get; private set; }

        public int Y { get; private set; }
    }
}