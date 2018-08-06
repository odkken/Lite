namespace Lite.Lib.Interface
{
    public interface IEditableBoard : IBoard
    {
        void ToggleEdit();
        void SetEdit(bool editOn);
        bool IsEditing { get; }
        void SaveLevel(string levelName = null);
    }
}