using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;
using View;

namespace Data
{
    public class PlacementManager
    {

        [Header("Cursor")]
        [SerializeField] private Texture2D buildingCursorTexture;
        [SerializeField] private Vector2 cursorHotspot = Vector2.zero;

        private BuildingDefinition _selectedBuilding;
        private bool _isPlacing;
        
        private CompositeDisposable _disposables = new CompositeDisposable();
        

        public void RegisterCell(CellView cell)
        {
            cell.OnCellClick
                .Subscribe(TryPlaceBuilding)
                .AddTo(_disposables);

            cell.OnCellHoverEnter
                .Subscribe(OnCellHoverEnter)
                .AddTo(_disposables);

            cell.OnCellHoverExit
                .Subscribe(OnCellHoverExit)
                .AddTo(_disposables);
        }
        public void RegisterCard(CardView card)
        {
            card.OnCellClick
                .Subscribe(OnCardClicked)
                .AddTo(_disposables);
        }
        private void OnCardClicked(BuildingDefinition building)
        {
            if (building == null) return;

            // Если кликнули по уже выбранному зданию — отменяем размещение
            if (_isPlacing && _selectedBuilding == building)
            {
                ClearSelection();
            }
            else
            {
                // Иначе выбираем новое здание
                SelectBuilding(building);
            }
        }

        private void OnCellHoverEnter(CellView cell)
        {
            if (!_isPlacing || _selectedBuilding == null)
                return;

            bool canPlace = CanPlaceBuilding(_selectedBuilding, cell);
            cell.SetHighlight(true, canPlace);
        }

        private void OnCellHoverExit(CellView cell)
        {
            cell.SetHighlight(false, true);
        }
    
        public void SelectBuilding(BuildingDefinition building)
        {
            _selectedBuilding = building;
            _isPlacing = true;

            if (buildingCursorTexture != null)
                Cursor.SetCursor(buildingCursorTexture, cursorHotspot, CursorMode.Auto);
        }
        

        public void ClearSelection()
        {
            _selectedBuilding = null;
            _isPlacing = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        public void TryPlaceBuilding(CellView cell)
        {
            if (!_isPlacing || _selectedBuilding == null) return;

            if (CanPlaceBuilding(_selectedBuilding, cell))
            {
                G.BuildingFactory.CreateBuilding(_selectedBuilding, cell.X, cell.Y);
                
                G.DeckManager.DiscardCard(_selectedBuilding);
                G.HandManager.RemoveCard(_selectedBuilding);
        
                Debug.Log($"[Placement] Здание { _selectedBuilding.name} размещено, карта удалена из руки");
        
                // 3. Сбрасываем выбор
                ClearSelection();
            }
            else
            {
                Debug.Log("Нельзя разместить здесь здание");
            }
        }

        private bool CanPlaceBuilding(BuildingDefinition def, CellView cell)
        {
            var gridCell = G.GridSystem.GetCell(cell.X, cell.Y);
            if (gridCell == null || gridCell.building != null) return false;


            bool terrainOk = def.allowedTerrainTypes.Contains(gridCell.terrainType);

            if (!terrainOk) return false;


            if (def.requiredNeighborTag != 0)
            {
                bool hasNeighbor = false;
                foreach (var neighbor in G.GridSystem.GetNeighbors(cell.X, cell.Y))
                {
                    if ((neighbor.building?.GetDefinition().buildingTags & def.requiredNeighborTag) != 0)
                        hasNeighbor = true;
                }

                if (!hasNeighbor) return false;
            }

            return true;
        }

    }
}
