using System;

namespace Lite.Lib.GameCore
{
    public static class Core
    {
        private static bool _initialized;
        public static ITimeInfo TimeInfo { get; private set; }
        public static IInput Input { get; private set; }
        public static ILogger Logger => _getLogger();
        public static ITextInfo Text { get; private set; }

        public static void Initialize(ITimeInfo timeInfo, IInput input, IWindowUtil windowUtil, Func<ILogger> getLogger, ITextInfo text)
        {
            if (_initialized)
                return;
            _initialized = true;
            TimeInfo = timeInfo;
            Input = input;
            WindowUtil = windowUtil;
            _getLogger = getLogger;
            Text = text;
        }

        public static IWindowUtil WindowUtil;   
        private static Func<ILogger> _getLogger;
    }
}