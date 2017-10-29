using System;
using System.Collections.Generic;
using System.Linq;
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
            var intersected = false;
            do
            {
                if (_beams.Any())
                {
                    //does not work, need to start at the END of last beam (origin + representation), and new direction will be angle of reflection (need to calculate/store)
                    intersected = TryGetNextBeam(_beams.Last().Origin, _beams.Last().Representation, _getAllMirrors(),
                        out Beam beam);
                    _beams.Add(beam);
                }
                else
                {

                    intersected = TryGetNextBeam(_origin, _direction, _getAllMirrors(), out Beam beam);
                    _beams.Add(beam);
                }
            } while (intersected);
        }

        bool TryGetNextBeam(Vector2f origin, Vector2f direction, List<Mirror> mirrors, out Beam beam)
        {
            var anyIntersections = false;
            var closestIntersection = new Vector2f();
            foreach (var mirror in mirrors)
            {
                if (mirror.TryGetIntersection(origin, direction, out Vector2f intersection))
                {

                    if (!anyIntersections || (intersection - origin).SquareMagnitude() < (closestIntersection - origin).SquareMagnitude())
                    {
                        anyIntersections = true;
                        closestIntersection = intersection;
                    }
                }
            }
            beam = new Beam(2, origin, anyIntersections ? (closestIntersection - origin) : direction * 100000, Color.Yellow);
            return anyIntersections;
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