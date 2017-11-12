using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Text;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1024, 768), "Lite", Styles.Default, new ContextSettings { AntialiasingLevel = 8 });
            window.SetVerticalSyncEnabled(true);
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
            var commands = commandExtractor.GetAllStaticCommands(Assembly.GetExecutingAssembly());
            var terminal = new Terminal(window, consoleFont, terminalInput, new CommandRunner(commands));

            while (window.IsOpen)
            {
                timeInfo.Tick();
                window.DispatchEvents();
                window.Clear(Color.Black);
                window.Draw(terminal);
                window.Display();
            }
        }

        [Command]
        public static float GetTime()
        {
            return Core.TimeInfo.CurrentTime;
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
    }


}
