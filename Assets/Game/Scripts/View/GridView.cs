using System.Collections.Generic;
using System.Linq;
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
        private int _width;
        private int _height;
    
        private CompositeDisposable _disposables;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
        }

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
                    cell.gameObject.SetActive(true); 
                    cell.transform.localScale = Vector3.one * _cellSize;
                
                    _cellPool[x, y] = cell;
                }
            }

        }

        public void SyncWithGrid(CellState[,] gridData)
        {
            if (gridData == null) return;

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CellState data = gridData[x, y];
                    CellView view = _cellPool[x, y];

                    view.SetTerrain(data.terrainType, _spriteLibrary);

                }
            }
        }

        
        private void UpdateCellVisuals(CellUpdateEventData eventData)
        {
            int x = eventData.X;
            int y = eventData.Y;
        
            if (x < 0 || x >= _width || y < 0 || y >= _height) return;

            CellView view = _cellPool[x, y];
            CellState data = eventData.State;
            
            view.SetTerrain(data.terrainType, _spriteLibrary);

        }

        private IEnumerable<CellView> GetAllCells()
        {
            for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                yield return _cellPool[x, y];
        }

        private void ClearPool()
        {
            if (_cellPool == null) return;
        
            foreach (var cell in _cellPool)
            {
                if (cell != null) Destroy(cell.gameObject);
            }
            _cellPool = null;
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
            ClearPool();
        }
    }
    
    public class CellUpdateEventData
    {
        public int X;
        public int Y;
        public CellState State;
    }
}