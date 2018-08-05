using System;
using System.Numerics;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib
{
    class EditableBoard : IEditableBoard
    {
        private IBoard _board;
        private readonly Action<int, int, ITile> _editTileAction;
        private readonly IInput _input;
        private readonly Func<Vector2f> _getTileSize;
        
        public EditableBoard(IBoard board, Action<int, int, ITile> editTileAction, IInput input, Func<Vector2f> getTileSize, Func<Vector2i, Vector2f> getScreenPos, Func<Vector2f> getBoardSize, Func<Vector2i> getBoardOffset)
        {
            _board = board;
            _editTileAction = editTileAction;
            _input = input;
            _getTileSize = getTileSize;
            _getScreenPos = getScreenPos;
            _getBoardSize = getBoardSize;
            _getBoardOffset = getBoardOffset;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _board.Draw(target, states);
            if (!IsEditing)
                return;
            _highlightTile?.Draw(target, states);
        }

        public (int, int, ITile) GetTileFromScreenCoord(Vector2f fractionalScreenCoord)
        {
            return _board.GetTileFromScreenCoord(fractionalScreenCoord);
        }

        private ITile _highlightTile;
        private readonly Func<Vector2i, Vector2f> _getScreenPos;
        private readonly Func<Vector2f> _getBoardSize;
        private readonly Func<Vector2i> _getBoardOffset;

        public void Update(float dt)
        {
            _board.Update(dt);
            if (!IsEditing) return;

            _highlightTile = null;
            var mousePos = _input.GetMousePosAbsolute() - _getBoardOffset();
            var boardSize = _getBoardSize();

            var fractionalMousePos = new Vector2f((float)((mousePos.X) * 1.0 / boardSize.X), (float)((mousePos.Y) * 1.0 / boardSize.Y));

            var (x, y, highlightedTile) = _board.GetTileFromScreenCoord(fractionalMousePos);
            if (highlightedTile != null)
            {
                _highlightTile = new KeyTile(new Vector2i(x, y), _getTileSize(), _getScreenPos);
            }
        }

        public void LoadLevel(string name)
        {
            _board.LoadLevel(name);
        }

        public void ToggleEdit()
        {
            IsEditing = !IsEditing;
        }

        public void SetEdit(bool editOn)
        {
            IsEditing = editOn;
        }

        public bool IsEditing { get; set; }
    }
}