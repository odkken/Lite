using SFML.Graphics;

namespace Lite
{
    public enum Tag
    {
        Input,
        Response
    }
    public interface IWrappedTextRenderer : Drawable
    {
        void AddLine(string line, Tag tag);
    }
}