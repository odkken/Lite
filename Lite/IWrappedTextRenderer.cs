using SFML.Graphics;

namespace Lite
{
    public enum Tag
    {
        Input,
        Response,
        Error,
        Warning,
        Debug
    }
    public interface IWrappedTextRenderer : Drawable
    {
        void AddLine(string line, Tag tag);
        void ScrollUp();
        void ScrollDown();
    }
}