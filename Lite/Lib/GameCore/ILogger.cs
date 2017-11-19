namespace Lite.Lib.GameCore
{
    public enum Category
    {
        Debug,
        Warning,
        Error
    }
    public interface ILogger
    {
        void Log(string msg, Category category = Category.Debug);
    }
}