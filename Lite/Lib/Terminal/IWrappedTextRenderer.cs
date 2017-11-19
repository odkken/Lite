using SFML.Graphics;

namespace Lite.Lib.Terminal
{
    public enum Tag
    {
        Input,
        Response,
        Error,
        Warning,
        Debug,
        SuperLowDebug
    }
    public interface IWrappedTextRenderer : Drawable
    {
        void AddLine(string line, Tag tag);
        void ScrollUp();
        void ScrollDown();
    }
}