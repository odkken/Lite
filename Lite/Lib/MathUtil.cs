using System;
using System.Security.Cryptography;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib
{
    public static class MathUtil
    {

        static readonly RandomNumberGenerator Rng = new RNGCryptoServiceProvider();
        static readonly byte[] Bytes = new byte[4];

        public static float Bottom(this FloatRect rect)
        {
            return rect.Top + rect.Height;
        }


        public static FloatRect GetTargetRegion(this View view)
        {
            var center = view.Center;
            var size = view.Size;
            return new FloatRect(new Vector2f(center.X - size.X / 2, center.Y - size.Y / 2), size);
        }

        public static float Right(this FloatRect rect)
        {
            return rect.Left + rect.Width;
        }

        public static Random RNG = new Random();
        //public static float GetRandom()
        //{
        //    //Rng.GetBytes(Bytes);
        //    //return BitConverter.ToSingle(Bytes, 0);
        //}

        public static float Magnitude(this Vector2f vector)
        {
            return MathF.Sqrt(vector.SquareMagnitude());
        }

        public static float SquareMagnitude(this Vector2f vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y;
        }
        public static Vector2f Normalize(this Vector2f vector)
        {
            return vector / vector.Magnitude();
        }

        public static float Lerp(this float from, float to, float fraction)
        {
            return from + (to - from) * fraction;
        }

        public static byte Lerp(this byte from, byte to, float fraction)
        {
            return (byte)(from + (to - from) * fraction);
        }

        public static Color Lerp(this Color from, Color to, float fraction)
        {
            return new Color(from.R.Lerp(to.R, fraction), from.G.Lerp(to.G, fraction), from.B.Lerp(to.B, fraction), from.A.Lerp(to.A, fraction));
        }


        public static float Dot(this Vector2f me, Vector2f other)
        {
            return me.X * other.X + me.Y * other.Y;
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


        public static Vector2f CalculateReflection(Vector2f incedent, Vector2f surface)
        {
            var incidentAlongSurface = surface * incedent.Dot(surface) / surface.SquareMagnitude();

            return -incedent + 2 * incidentAlongSurface;
        }
    }
}