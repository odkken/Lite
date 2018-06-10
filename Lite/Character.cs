using System;
using System.Collections.Generic;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    class RectWithIntPosition
    {
        public Vector2i Position { get; set; }
        public RectangleShape Rect { get; set; }
    }
    public class Character : Drawable
    {
        private readonly Func<Vector2i, Vector2f> _posToScreenCoord;
        private readonly List<RectWithIntPosition> _rects;

        public Character(IInput input, Vector2i position, Vector2f size, Func<Vector2i, Vector2f> posToScreenCoord, Func<Vector2i, bool> canMoveTo, int outlineThickness)
        {
            _posToScreenCoord = posToScreenCoord;
            _rects = new List<RectWithIntPosition> { new RectWithIntPosition { Rect = new RectangleShape(size) { OutlineThickness = outlineThickness, OutlineColor = Color.Black, FillColor = Color.Cyan }, Position = position } };
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

                foreach (var rectWithIntPosition in _rects)
                {
                    var newPos = rectWithIntPosition.Position + delta;
                    if (!canMoveTo(newPos)) return;
                }

                foreach (var rectWithIntPosition in _rects)
                {
                    rectWithIntPosition.Position += delta;
                }
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var rect in _rects)
            {
                rect.Rect.Position = _posToScreenCoord(new Vector2i(rect.Position.X, rect.Position.Y));
                rect.Rect.Draw(target, states);
            }
            
        }
    }
}