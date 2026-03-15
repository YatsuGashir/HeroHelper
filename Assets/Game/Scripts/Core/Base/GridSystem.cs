using System;
using System.Collections.Generic;
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

        public CellState[] GetNeighbors(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Array.Empty<CellState>();
    
            var neighbors = new List<CellState>();
            
            if (y > 0)
                neighbors.Add(_grid[x, y - 1]);
            
            if (y < Height - 1)
                neighbors.Add(_grid[x, y + 1]);
            
            if (x > 0)
                neighbors.Add(_grid[x - 1, y]);
            
            if (x < Width - 1)
                neighbors.Add(_grid[x + 1, y]);
    
            return neighbors.ToArray();
        }
    }
}
