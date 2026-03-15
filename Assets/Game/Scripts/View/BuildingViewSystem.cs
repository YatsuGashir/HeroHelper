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
            var go = Instantiate(buildingPrefab, cellView.transform);
            go.transform.localPosition = Vector3.zero;
        }

        private void ClearBuilding(CellView cellView)
        {
            foreach (Transform child in cellView.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
