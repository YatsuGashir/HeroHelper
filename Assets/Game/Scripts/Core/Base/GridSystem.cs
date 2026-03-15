using Data;

namespace Core.Base
{
    public class GridSystem
    {
        private CellState[,] _grid;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public void Initialize(CellState[,] gridData)
        {
            _grid = gridData;
            Width = gridData.GetLength(0);
            Height = gridData.GetLength(1);

        }

        public CellState[,] GetGridData()
        {
            return _grid;
        }

        public CellState GetCell(int x, int y) => _grid[x, y];
    }
}
