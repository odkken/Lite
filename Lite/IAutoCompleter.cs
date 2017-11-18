using System.Collections.Generic;
using SFML.Graphics;

namespace Lite
{
    public interface IAutoCompleter : Drawable
    {
        void UpdateInputString(string str);
        void Escape();
        bool IsActive { get; }
        void IncrementSelection();
        void DecrementSelection();
        string ChooseSelectedItem();
    }
}