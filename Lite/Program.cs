using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public static class Program
    {
        private static Terminal terminal;
        private static List<CommandData> _commands;

        [Command]
        public static List<string> help()
        {
            return _commands.Select(a => a.Name).ToList();
        }

        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1920,1080), "Lite", Styles.None, new ContextSettings { AntialiasingLevel = 0 });
            window.SetVerticalSyncEnabled(false);
            window.SetActive();
            window.Closed += (sender, eventArgs) => window.Close();
            window.KeyPressed += (sender, eventArgs) =>
            {
                if (eventArgs.Code == Keyboard.Key.Escape)
                    window.Close();
            };
            var timeInfo = new TimeInfo();

            var globalInput = new WindowWrapperInput(window);
            var terminalInput = new BlockableInput(globalInput);
            var gameInput = new BlockableInput(terminalInput);
            Core.Initialize(timeInfo, gameInput);

            var consoleFont = new Font("fonts/consola.ttf");
            var commandExtractor = new CommandExtractor();
            _commands = commandExtractor.GetAllStaticCommands(Assembly.GetExecutingAssembly());
            terminal = new Terminal(window, consoleFont, terminalInput, new CommandRunner(_commands));
            sc(.5f, .5f, .2f, .5f);
            var dtText = new Text("", consoleFont) { Position = new Vector2f(500, 600) };
            var dtBuffer = new Queue<float>();
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
                window.Draw(terminal);
                window.Draw(dtText);
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
            terminal.SetHighlightColor(new Color((byte)(255 * r), (byte)(255 * g), (byte)(255 * b), (byte)(255 * a)));
        }
    }


}
