using System;
using SFML.System;

namespace Lite
{
    public static class MathUtil
    {
        public static float Magnitude(this Vector2f vector)
        {
            return MathF.Sqrt(vector.SquareMagnitude());
        }

        public static float SquareMagnitude(this Vector2f vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y;
        }

        public class LineInfo
        {
            public float A { get; set; }
            public float B { get; set; }
            public float C { get; set; }
        }

        public static LineInfo GetLineInfo(Vector2f origin, Vector2f direction)
        {
            var point2 = origin + direction;
            return GetLineInfo(origin.X, point2.X, origin.Y, point2.Y);
        }

        public static LineInfo GetLineInfo(float x1, float x2, float y1, float y2)
        {
            var a = -(y2 - y1);
            var b = x2 - x1;
            return new LineInfo
            {
                A = a,
                B = b,
                C = a * x1 + b * y1
            };
        }
    }
}