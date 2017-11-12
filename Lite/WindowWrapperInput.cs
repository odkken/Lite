using System;
using SFML.Window;

namespace Lite
{
    public class WindowWrapperInput : IInput
    {
        public event Action<TextEventArgs> TextEntered;
        public void Consume()
        {
            
        }

        public void Release()
        {
            
        }

        public event Action<KeyEventArgs> KeyPressed;

        public WindowWrapperInput(Window window)
        {
            window.TextEntered += (sender, args) => TextEntered?.Invoke(args);
            window.KeyPressed += (sender, args) => KeyPressed?.Invoke(args);
        }
    }
}