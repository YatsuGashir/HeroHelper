using Data;
using GlobalSpace;
using UniRx;
using UnityEngine;
using View;

namespace View
{
    public class BuildingViewSystem : MonoBehaviour
    {
        [SerializeField] private GameObject buildingPrefab;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private SpriteRenderer _spriteRenderer;

        public void Init()
        {
            G.Events.CellChanged
                .Subscribe(OnCellChanged)
                .AddTo(_disposables);
            
            _spriteRenderer = buildingPrefab.GetComponent<SpriteRenderer>();
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
            AudioManager.Instance.PlaySFX("SoundDust");

            var go = Instantiate(buildingPrefab, cellView.transform);
            go.transform.localPosition = Vector3.zero;

            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = instance.GetDefinition().buildingIcon;

            UpdateSortingOrder(cellView, sr);

            cellView.SetVisibleOverlay(false);

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
        private void UpdateSortingOrder(CellView cellView, SpriteRenderer sr)
        {
            int calculatedOrder = _baseSortingOrder - cellView.Y;
            sr.sortingOrder = calculatedOrder + 3;
        }
    }
}
