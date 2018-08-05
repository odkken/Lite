using System;
using System.Collections.Generic;
using System.Linq;
using Lite.Lib;
using Lite.Lib.GameCore;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public class RectWithIntPosition
    {
        public Vector2i Position { get; set; }
        public RectangleShape Rect { get; set; }
    }
    public class Character : Drawable
    {
        private readonly IInput _input;
        private readonly Vector2i _size;
        private readonly Func<Vector2i, Vector2f> _posToScreenCoord;
        private readonly Func<Vector2i, bool> _canMoveTo;
        private readonly int _outlineThickness;
        private readonly Func<Vector2i, List<Vector2i>> _getAdjacentKeys;
        private readonly Action<Vector2i> _removeKey;
        private readonly List<RectWithIntPosition> _rects;
        private double _maxMovesPerSec = 8;

        public Character(IInput input, Vector2i size, List<RectWithIntPosition> rects, Func<Vector2i, Vector2f> posToScreenCoord, Func<Vector2i, bool> canMoveTo, int outlineThickness, Func<Vector2i, List<Vector2i>> getAdjacentKeys, Action<Vector2i> removeKey)
        {
            _input = input;
            _size = size;
            _posToScreenCoord = posToScreenCoord;
            _canMoveTo = canMoveTo;
            _outlineThickness = outlineThickness;
            _getAdjacentKeys = getAdjacentKeys;
            _removeKey = removeKey;
            _rects = rects;
        }

        private double _lastMoveTime;
        public void Update(float dt)
        {
            var moveDt = 1.0 / _maxMovesPerSec;
            var time = Core.TimeInfo.CurrentTime;
            if (time - _lastMoveTime < moveDt)
                return;
            var delta = new Vector2i();
            if (_input.IsKeyDown(Keyboard.Key.W) || _input.IsKeyDown(Keyboard.Key.Up))
                delta.Y--;
            if (_input.IsKeyDown(Keyboard.Key.A) || _input.IsKeyDown(Keyboard.Key.Left))
                delta.X--;
            if (_input.IsKeyDown(Keyboard.Key.S) || _input.IsKeyDown(Keyboard.Key.Down))
                delta.Y++;
            if (_input.IsKeyDown(Keyboard.Key.D) || _input.IsKeyDown(Keyboard.Key.Right))
                delta.X++;

            foreach (var rectWithIntPosition in _rects)
            {
                var newPos = rectWithIntPosition.Position + delta;
                if (!_canMoveTo(newPos)) return;
            }

            foreach (var rectWithIntPosition in _rects)
            {
                rectWithIntPosition.Position += delta;
            }

            while (true)
            {
                var rectsToAdd = _rects.SelectMany(a => _getAdjacentKeys(a.Position)).Distinct().ToList();
                foreach (var item in rectsToAdd)
                {
                    _removeKey(item);
                    _rects.Add(new RectWithIntPosition
                    {
                        Rect = new RectangleShape(new Vector2f(_size.X, _size.Y))
                        {
                            OutlineThickness = _outlineThickness,
                            OutlineColor = Color.Black,
                            FillColor = Color.Cyan
                        },
                        Position = item
                    });
                }
                if (!rectsToAdd.Any())
                    break;
            }
            if (delta.SquareMagnitude() != 0)
                _lastMoveTime = time;
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