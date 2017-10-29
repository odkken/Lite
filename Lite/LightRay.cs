using System;
using System.Collections.Generic;
using Lite;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public class LightRay : Drawable
    {
        private readonly List<Beam> _beams;

        public LightRay(Vector2f origin, Vector2f direction, Func<List<Mirror>> getAllMirrors)
        {
            _beams = new List<Beam>();
            var mirrors = getAllMirrors();

            var anyIntersections = false;
            var closestIntersection = new Vector2f();
            var info = MathUtil.GetLineInfo(origin, direction);
            foreach (var mirror in mirrors)
            {
                if (mirror.TryGetIntersection(info, out Vector2f intersection))
                {

                    if (!anyIntersections || (intersection - origin).SquareMagnitude() < (closestIntersection - origin).SquareMagnitude())
                    {
                        anyIntersections = true;
                        closestIntersection = intersection;
                    }
                }
            }

            _beams.Add(new Beam(2, origin, anyIntersections ? (closestIntersection - origin) : direction * 100000, Color.Yellow));

        }


        public void Update()
        {
            
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