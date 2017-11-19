namespace Lite.Lib.GameCore
{
    public interface ITimeInfo
    {
        float CurrentDt { get; }
        float CurrentTime { get; }
        int CurrentFrame { get; }
    }
}