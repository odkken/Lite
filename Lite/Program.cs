using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using Lite.Lib.Terminal;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public static class Program
    {
        private static Terminal _terminal;
        private static List<CommandData> _commands;

        [Command]
        public static List<string> help()
        {
            return _commands.Select(a => a.Name).ToList();
        }

        [Command]
        public static void SetLogLevel(string category)
        {
            _logCategory = Enum.Parse<Category>(category, true);
        }

        private static Category _logCategory = Category.Debug;

        [Command]
        public static void LoadLevel(int level)
        {
            _inited = false;
            _board = new Board(level, Core.Input, character => _character = character);
            _inited = true;
            _victoryShown = false;
            _board.OnSolved += l =>
            {
                _victoryShown = true;
            };

        }

        public static void Main()
        {
            var window = new RenderWindow(new VideoMode(1280, 720), "Lite");
            window.SetVerticalSyncEnabled(false);
            window.SetActive();
            window.Closed += (sender, eventArgs) => window.Close();

            var timeInfo = new TimeInfo();

            var globalInput = new WindowWrapperInput(window);
            var terminalInput = new BlockableInput(globalInput);
            var gameInput = new BlockableInput(terminalInput);
            gameInput.KeyPressed += eventArgs =>
             {
                 if (eventArgs.Code == Keyboard.Key.Escape)
                     window.Close();
             };
            ILogger logger = null;
            var consoleFont = new Font("fonts/consola.ttf");
            _inited = false;
            Core.Initialize(timeInfo, gameInput, new WindowUtilUtil(() => (Vector2f)window.Size), () => logger, new TextInfo() { DefaultFont = consoleFont }, new World(() => _inited));

            CommandRunner runner = null;
            _terminal = new Terminal(window, consoleFont, terminalInput, () => runner, s => string.IsNullOrWhiteSpace(s) ? new List<string>() : _commands.Where(a => a.Name.ToLower().Contains(s.ToLower())).Select(a => a.Name).OrderBy(a => a.Length).ToList());
            logger = new LambdaLogger((a, b) =>
            {
                if (b >= _logCategory) _terminal.LogMessage(a, b);
            });

            var commandExtractor = new CommandExtractor(logger);
            _commands = commandExtractor.GetAllStaticCommands(Assembly.GetExecutingAssembly());
            runner = new CommandRunner(_commands);

            sc(.5f, .5f, .2f, .5f);

            var victoryButton = new Text("Continue", consoleFont, 55);

            LoadLevel(1);

            while (window.IsOpen)
            {
                timeInfo.Tick();
                window.DispatchEvents();
                _board.Update();
                window.Clear(Color.Black);
                if (_victoryShown)
                    window.Draw(victoryButton);
                window.Draw(_board);
                window.Draw(_character);
                window.Draw(_terminal);
                window.Display();
            }
        }

        [Command]
        public static float GetTime()
        {
            return Core.TimeInfo.CurrentTime;
        }
        [Command]
        public static float dt()
        {
            return Core.TimeInfo.CurrentDt;
        }

        [Command]
        public static List<string> PrintStuff(int thing1, string name)
        {
            return new List<string>
            {
                $"Hi {name}",
                $"I am {thing1}"
            };
        }

        [Command]
        public static void sc(float r, float g, float b, float a)
        {
            _terminal.SetHighlightColor(new Color((byte)(255 * r), (byte)(255 * g), (byte)(255 * b), (byte)(255 * a)));
        }

        private static int gcFrame = 0;
        private static Board _board;
        private static Character _character;
        private static bool _victoryShown;
        private static bool _inited;

    }
}
