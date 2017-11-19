using System;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib.GameCore
{
    public interface IWindowUtil
    {
        FloatRect GetFractionalRect(FloatRect floatRect);
        Vector2f GetPixelSize(Vector2f fractionalWindowSize);
    }

    class WindowUtilUtil : IWindowUtil
    {
        private readonly Func<Vector2f> _getWindowSize;

        public WindowUtilUtil(Func<Vector2f> getWindowSize)
        {
            _getWindowSize = getWindowSize;
        }

        public FloatRect GetFractionalRect(FloatRect floatRect)
        {
            var windowSize = _getWindowSize();
            return new FloatRect(floatRect.Left / windowSize.X, floatRect.Top / windowSize.Y, floatRect.Width / windowSize.X, floatRect.Height / windowSize.Y);
        }

        public Vector2f GetPixelSize(Vector2f fractionalWindowSize)
        {
            var ws = _getWindowSize();
            return new Vector2f(fractionalWindowSize.X * ws.X, fractionalWindowSize.Y * ws.Y);
        }
    }
}