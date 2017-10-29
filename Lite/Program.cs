using System;
using System.Collections.Generic;
using System.Numerics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Lite
{

    public static class Program
    {

        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1024, 768), "Lite");
            window.SetVerticalSyncEnabled(true);

            window.SetActive();

            window.Closed += (sender, eventArgs) => window.Close();
            window.KeyPressed += (sender, eventArgs) =>
            {
                if (eventArgs.Code == Keyboard.Key.Escape)
                    window.Close();
            };

            var mirror = new Mirror(new Vector2f(200, 0));

            var beam = new LightRay(new Vector2f(500, 100), new Vector2f(-50, 10), () => new List<Mirror>
            {
                mirror
            });
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);
                window.Draw(beam);
                window.Draw(mirror);
                window.Display();
            }
        }
    }
}
