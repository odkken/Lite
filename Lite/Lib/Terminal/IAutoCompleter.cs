using SFML.Graphics;

namespace Lite.Lib.Terminal
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