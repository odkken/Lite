using System;
using Lite.Lib.GameCore;
using Lite.Lib.Interface;
using SFML.Graphics;
using SFML.System;

namespace Lite.Lib
{
    class EditableBoard : IEditableBoard
    {
        private IBoard _board;
        private readonly IInput _input;
        private readonly ITileFactory _tileFactory;
        private readonly Func<Vector2i, int> _getTileSize;

        public EditPanel EditPanel { get; }

        public EditableBoard(IBoard board, IInput input,
            ITileFactory tileFactory, Func<Vector2i> getBoardSize, Func<Vector2i> getBoardOffset,
            Func<Vector2i, int> getTileSize, Func<TileType, Color> colorGet, Action<int, int, ITile> setTile)
        {
            _board = board;
            _input = input;
            _input.MouseButtonDown += args =>
            {
                if (_highlightTile != null)
                {
                    setTile(_highlightTile.X, _highlightTile.Y, _highlightTile);
                    _highlightTile = null;
                }
            };
            _tileFactory = tileFactory;
            _getBoardSize = getBoardSize;
            _getBoardOffset = getBoardOffset;
            _getTileSize = getTileSize;
            EditPanel = new EditPanel(input, () => IsEditing, colorGet);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _board.Draw(target, states);
            if (!IsEditing)
                return;
            _highlightTile?.Draw(target, states);
            EditPanel.Draw(target, states);
        }

        public (int, int, ITile) GetTileFromScreenCoord(Vector2f fractionalScreenCoord)
        {
            return _board.GetTileFromScreenCoord(fractionalScreenCoord);
        }

        private ITile _highlightTile;
        private readonly Func<Vector2i> _getBoardSize;
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
            var tileSize = _getTileSize(Size);
            if (highlightedTile != null)
            {
                _highlightTile = _tileFactory.CreateTile(x, y, tileSize, Size, EditPanel.SelectedTileType);
            }
        }

        public void LoadLevel(string name)
        {
            _board.LoadLevel(name);
        }

        public Vector2i Size => _board.Size;

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