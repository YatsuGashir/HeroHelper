using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;
using View;

namespace View
{
    public class BuildingViewSystem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer buildingPrefab;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public void Init()
        {
            G.Events.CellChanged
                .Subscribe(OnCellChanged)
                .AddTo(_disposables);
        }

        private void OnCellChanged(CellUpdateEventData data)
        {
            var cellView = G.GridView.GetCellView(data.X, data.Y);
            if (cellView == null) return;

            if (data.State.building != null)
            {
                SpawnBuilding(cellView, data.State.building);
            }
            else
            {
                ClearBuilding(cellView);
            }
        }

        private void SpawnBuilding(CellView cellView, BuildingInstance instance)
        {
            buildingPrefab.sprite = instance.GetDefinition().buildingIcon;
            UpdateSortingOrder(cellView);
            cellView.SetVisibleOverlay(false);
            var go = Instantiate(buildingPrefab, cellView.transform);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<BuildingStatusView>().Init(instance);
        }

        private void ClearBuilding(CellView cellView)
        {
            var buildingViews = cellView.GetComponentsInChildren<IBuildingView>(true);

            foreach (var buildingView in buildingViews)
            {
                if (buildingView is Component component)
                {
                    if (component.gameObject != cellView.gameObject)
                    {
                        Destroy(component.gameObject);
                    }
                }
            }
        }
        
        private int _baseSortingOrder = 3; 
        private void UpdateSortingOrder(CellView  cellView)
        {
       
            int calculatedOrder = _baseSortingOrder - cellView.Y;
            
            buildingPrefab.sortingOrder = calculatedOrder;
                

        }
    }
}
