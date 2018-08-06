using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lite.Lib;
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
        private static string _lastLevel;

        [Command]
        public static void LoadLevel(string name)
        {
            _board.LoadLevel(name);
            _lastLevel = name;
        }

        [Command]
        public static Vector2f GetMousePos()
        {
            return editInput.GetMousePos();
        }

        [Command]
        public static void ToggleEdit()
        {
            _board.ToggleEdit();
            if (_board.IsEditing)
                editInput.Consume();
            else
                editInput.Release();
        }

        public static void Main()
        {
            var window = new RenderWindow(new VideoMode(1280, 720), "Lite");
            window.SetVerticalSyncEnabled(false);
            window.SetActive();
            window.Closed += (sender, eventArgs) => window.Close();

            var timeInfo = new TimeInfo();

            var globalInput = new WindowWrapperInput(window);
            globalInput.KeyPressed += args =>
            {
                if (args.Code == Keyboard.Key.Escape)
                    window.Close();
            };

            terminalInput = new BlockableInput(globalInput);
            editInput = new BlockableInput(terminalInput);
            gameInput = new BlockableInput(editInput);
            gameInput.KeyPressed += eventArgs =>
            {
                switch (eventArgs.Code)
                {
                    case Keyboard.Key.R when gameInput.IsControlDown && _lastLevel != null:
                        LoadLevel(_lastLevel);
                        break;
                }
            };
            var getScreenPos = new Func<int, Vector2i, Vector2i, Vector2f>((tileSize, pos, boardSize) =>
            {
                var totalWidth = boardSize.X * tileSize;
                var totalHeight = boardSize.Y * tileSize;
                var halfWindowSize = (Vector2i)Core.WindowUtil.GetPixelSize(new Vector2f(.5f, .5f));
                var origin = new Vector2i(halfWindowSize.X - totalWidth / 2, halfWindowSize.Y - totalHeight / 2);
                return new Vector2f(origin.X + tileSize * pos.X, origin.Y + tileSize * pos.Y);
            });
            var getTileSize = new Func<Vector2i, int>(u =>
            {
                var ratio = Core.WindowUtil.GetPixelSize(new Vector2f(1.0f / u.X, 1.0f / u.Y));
                return (int)Math.Min(ratio.X, ratio.Y);
            });
            var tileFactory = new TileFactory(getTileSize, getScreenPos);

            var coreBoard = new Board(new TextLevelParser(gameInput, tileFactory, getScreenPos));
            _board = new EditableBoard(coreBoard, editInput,
                tileFactory, () => coreBoard.PixelSize, () => coreBoard.ScreenOffset, getTileSize, type => Tile.ColorLookup[type].Item1, coreBoard.SetTile);
            ILogger logger = null;
            var consoleFont = new Font("fonts/consola.ttf");
            Core.Initialize(timeInfo, gameInput, new WindowUtilUtil(() => window.Size), () => logger, new TextInfo() { DefaultFont = consoleFont });

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

            LoadLevel("1");

            while (window.IsOpen)
            {
                timeInfo.Tick();
                window.DispatchEvents();
                _board.Update(timeInfo.CurrentDt);
                globalInput.Update(timeInfo.CurrentDt);
                window.Clear(Color.Black);
                window.Draw(_board);
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
        private static IEditableBoard _board;
        private static bool _inited;
        private static BlockableInput terminalInput;
        private static BlockableInput gameInput;
        private static BlockableInput editInput;
    }
}
