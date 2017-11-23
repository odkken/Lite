using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lite.Lib.Entities;
using Lite.Lib.GameCore;
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
        private static World _world;

        [Command]
        public static void Load(int level)
        {
            _world.Load(level);
        }


        [Command]
        public static List<string> PrintEntities()
        {
            return _world.Entities;
        }
        [Command]
        public static void ToggleCoords()
        {
            _world.DrawCoordText = !_world.DrawCoordText;
        }

        [Command]
        public static List<string> help()
        {
            return _commands.Select(a => a.Name).ToList();
        }

        [Command]
        public static void SetLogLevel(string category)
        {
            logCategory = Enum.Parse<Category>(category, true);
        }

        private static Category logCategory = Category.Debug;

        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1920, 1080), "Lite", Styles.None, new ContextSettings { AntialiasingLevel = 0 });
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
            _world = new World(i => File.ReadAllLines($"levels/{i}.txt").ToList());
            Entity.SetActions(_world.RegisterEntity, _world.SetPosition);
            var consoleFont = new Font("fonts/consola.ttf");
            Core.Initialize(timeInfo, gameInput, new WindowUtilUtil(() => (Vector2f)window.Size), _world, () => logger, new TextInfo() { DefaultFont = consoleFont });

            CommandRunner runner = null;
            _terminal = new Terminal(window, consoleFont, terminalInput, () => runner, s => string.IsNullOrWhiteSpace(s) ? new List<string>() : _commands.Where(a => a.Name.ToLower().Contains(s.ToLower())).Select(a => a.Name).OrderBy(a => a.Length).ToList());
            logger = new LambdaLogger((a, b) =>
            {
                if (b >= logCategory) _terminal.LogMessage(a, b);
            });

            var commandExtractor = new CommandExtractor(logger);
            _commands = commandExtractor.GetAllStaticCommands(Assembly.GetExecutingAssembly());
            runner = new CommandRunner(_commands);

            sc(.5f, .5f, .2f, .5f);
            var dtText = new Text("", consoleFont) { Position = new Vector2f(500, 600) };
            var dtBuffer = new Queue<float>();

            _world.Load(1);
            ToggleCoords();
            while (window.IsOpen)
            {
                timeInfo.Tick();

                dtBuffer.Enqueue(timeInfo.CurrentDt);
                while (dtBuffer.Count > 100)
                {
                    dtBuffer.Dequeue();
                }
                dtText.DisplayedString = dtBuffer.Average().ToString();
                window.DispatchEvents();
                window.Clear(Color.Black);
                if (Core.TimeInfo.CurrentFrame == gcFrame + 1)
                {
                    logger.Log("GC frame time: " + Core.TimeInfo.CurrentDt, Category.SuperLowDebug);
                }
                _world.Update();
                window.Draw(_world);
                window.Draw(dtText);
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
        public static void LogGcFrame()
        {
            var frame = Core.TimeInfo.CurrentFrame;
            if (frame != gcFrame)
            {
                gcFrame = frame;
                Core.Logger.Log($"gc on frame {Core.TimeInfo.CurrentFrame}.  This frame time = {Core.TimeInfo.CurrentDt}", Category.SuperLowDebug);
            }
        }
    }
}
