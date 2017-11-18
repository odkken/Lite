using System;
using SFML.Graphics;
using SFML.System;

namespace Lite
{
    public interface IWindowUtil
    {
        FloatRect GetFractionalRect(FloatRect floatRect);
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
    }
}