namespace Lite
{
    public static class Core
    {
        private static bool _initialized;
        public static ITimeInfo TimeInfo { get; private set; }
        public static IInput Input { get; private set; }
        public static void Initialize(ITimeInfo timeInfo, IInput input, IWindowUtil windowUtil)
        {
            if (_initialized)
                return;
            _initialized = true;
            TimeInfo = timeInfo;
            Input = input;
            WindowUtil = windowUtil;
        }

        public static IWindowUtil WindowUtil;
    }
}