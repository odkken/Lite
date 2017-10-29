using System;
using System.Collections.Generic;
using Lite;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class LightRay : Drawable
    {
        private readonly Vector2f _origin;
        private Vector2f _direction;
        private readonly Func<List<Mirror>> _getAllMirrors;
        private readonly Func<bool> _isMouseDown;
        private readonly Func<Vector2f> _getMousePos;
        private List<Beam> _beams;

        public LightRay(Vector2f origin, Vector2f direction, Func<List<Mirror>> getAllMirrors, Func<bool> isMouseDown, Func<Vector2f> getMousePos)
        {
            _origin = origin;
            _direction = direction;
            _getAllMirrors = getAllMirrors;
            _isMouseDown = isMouseDown;
            _getMousePos = getMousePos;
            CalculateBeams();
        }

        void CalculateBeams()
        {
            _beams = new List<Beam>();
            var mirrors = _getAllMirrors();

            var anyIntersections = false;
            var closestIntersection = new Vector2f();
            var info = MathUtil.GetLineInfo(_origin, _direction);
            foreach (var mirror in mirrors)
            {
                if (mirror.TryGetIntersection(info, out Vector2f intersection))
                {

                    if (!anyIntersections || (intersection - _origin).SquareMagnitude() < (closestIntersection - _origin).SquareMagnitude())
                    {
                        anyIntersections = true;
                        closestIntersection = intersection;
                    }
                }
            }

            _beams.Add(new Beam(2, _origin, anyIntersections ? (closestIntersection - _origin) : _direction * 100000, Color.Yellow));

        }

        public void Update()
        {
            if (_isMouseDown())
            {
                _direction = _getMousePos() - _origin;
                CalculateBeams();
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var beam in _beams)
            {
                beam.Draw(target, states);
            }
        }
    }
}