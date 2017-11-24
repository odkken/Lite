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
        public static IWorld World { get; private set; }

        public static void Initialize(ITimeInfo timeInfo, IInput input, IWindowUtil windowUtil, Func<ILogger> getLogger, ITextInfo text, World world)
        {
            if (_initialized)
                return;
            _initialized = true;
            TimeInfo = timeInfo;
            Input = input;
            WindowUtil = windowUtil;
            _getLogger = getLogger;
            Text = text;
            World = world;
        }

        public static IWindowUtil WindowUtil;   
        private static Func<ILogger> _getLogger;
    }

    public interface IWorld
    {
        bool Initialized { get; }
    }

    public class World : IWorld
    {
        private readonly Func<bool> _isInitialized;

        public World(Func<bool> isInitialized)
        {
            _isInitialized = isInitialized;
        }
        public bool Initialized => _isInitialized();
    }
}