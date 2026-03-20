using System.Collections.Generic;
using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace View
{
    public class GridView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private TerrainSpriteLibrary _spriteLibrary;

        [Header("Grid Settings")]
        [SerializeField] private float _cellSize = 1.0f;

        private CellView[,] _cellPool;
        private CellState[,] _currentGridData;
        private int _width;
        private int _height;
    
        private CompositeDisposable _disposables;

        private void Awake() => _disposables = new CompositeDisposable();

        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
            ClearPool();
            _cellPool = new CellView[width, height];
        
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    CellView cell = Instantiate(_cellPrefab, transform);
                    cell.Initialize(x, y);
                    cell.transform.localScale = Vector3.one * _cellSize;
                    _cellPool[x, y] = cell;
                }
            }
            
            G.Events.CellChanged.Subscribe(UpdateCell).AddTo(_disposables);
        }

        public void SyncWithGrid(CellState[,] gridData)
        {
            if (gridData == null) return;
            _currentGridData = gridData;

            // Первый проход: устанавливаем все тайлы
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    ApplyCellVisuals(x, y, gridData[x, y]);
                    G.PlacementManager.RegisterCell(_cellPool[x, y]);
                }
            }

            // Второй проход: обновляем autotile у всех Meadow (когда соседи уже известны)
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (gridData[x, y].terrainType != TerrainType.Water)
                    {
                        _cellPool[x, y].SetBaseTerrain(TerrainType.Meadow, _spriteLibrary, GetNeighborTerrain);
                    }
                }
            }
        }

        // Применение визуалов для одной клетки
        private void ApplyCellVisuals(int x, int y, CellState data)
        {
            CellView view = _cellPool[x, y];

            // Всегда базовый тайл Meadow или Water для автотайла
            //TerrainType baseType = (data.terrainType == TerrainType.Water) ? TerrainType.Water : TerrainType.Meadow;
            if(data.terrainType != TerrainType.Water)
                view.SetBaseTerrain(TerrainType.Meadow, _spriteLibrary, GetNeighborTerrain);

            // Overlay поверх базы
            if (_spriteLibrary.IsOverlayTerrain(data.terrainType))
                view.SetOverlay(data.terrainType, _spriteLibrary);
            else
                view.SetOverlay(TerrainType.None, _spriteLibrary);
        }

        // Коллбек для получения типа соседа
        private TerrainType GetNeighborTerrain(int x, int y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height) return TerrainType.None;
    
            var type = _currentGridData?[x, y].terrainType ?? TerrainType.None;
    
            // 🔑 Оверлеи считаем "прозрачными" для autotile луга
            if (_spriteLibrary.IsOverlayTerrain(type))
                return TerrainType.Meadow;
    
            return type;
        }

        // Обновление одной клетки (для реактивных обновлений)
        public void UpdateCell(CellUpdateEventData eventData)
        {
            int x = eventData.X, y = eventData.Y;
            if (x < 0 || x >= _width || y < 0 || y >= _height) return;
            
            _currentGridData[x, y] = eventData.State;
            ApplyCellVisuals(x, y, eventData.State);
            
            // Если обновили Meadow — пересчитываем autotile у соседей
            if (eventData.State.terrainType == TerrainType.Meadow)
            {
                UpdateMeadowNeighborsAutotile(x, y);
            }
            // Если обновили соседнюю клетку — возможно, нужно обновить autotile у этой, если она Meadow
            else if (_currentGridData[x, y].terrainType == TerrainType.Meadow)
            {
                _cellPool[x, y].SetBaseTerrain(TerrainType.Meadow, _spriteLibrary, GetNeighborTerrain);
            }
        }

        // Пересчёт autotile у 4 соседей при изменении клетки
        private void UpdateMeadowNeighborsAutotile(int centerX, int centerY)
        {
            //int[] dx = { 0, 1, 0, -1 };
            //int[] dy = { 1, 0, -1, 0 };
            
            int[] dx = { -1 , 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
            
            for (int i = 0; i < 8; i++)
            {
                int nx = centerX + dx[i], ny = centerY + dy[i];
                if (nx >= 0 && nx < _width && ny >= 0 && ny < _height && 
                    _currentGridData[nx, ny].terrainType == TerrainType.Meadow)
                {
                    _cellPool[nx, ny].SetBaseTerrain(TerrainType.Meadow, _spriteLibrary, GetNeighborTerrain);
                }
            }
        }

        public CellView GetCellView(int x, int y) => _cellPool[x, y];

        public IEnumerable<CellView> GetAllCells()
        {
            for (int x = 0; x < _width; x++)
                for (int y = 0; y < _height; y++)
                    yield return _cellPool[x, y];
        }

        private void ClearPool()
        {
            if (_cellPool == null) return;
            foreach (var cell in _cellPool)
                if (cell != null) Destroy(cell.gameObject);
            _cellPool = null;
            _currentGridData = null;
        }

        public void ClearAllHighlights()
        {
            if (_cellPool == null) return;
            foreach (var cell in GetAllCells())
                cell.SetHighlight(false, true);
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
            ClearPool();
        }
    }
    
    public class CellUpdateEventData
    {
        public int X, Y;
        public CellState State;
    }
}