using Data;
using DG.Tweening;
using GlobalSpace;
using UniRx;
using UnityEngine;
using View;

namespace View
{
    public class BuildingViewSystem : MonoBehaviour
    {
        [SerializeField] private GameObject buildingPrefab;
        
        [SerializeField] private Transform cameraTransform;

        private CompositeDisposable _disposables = new CompositeDisposable();
        private SpriteRenderer _spriteRenderer;
        private ParticleSystem _particleSystem;

        public void Init()
        {
            G.Events.CellChanged
                .Subscribe(OnCellChanged)
                .AddTo(_disposables);
            
            _spriteRenderer = buildingPrefab.GetComponent<SpriteRenderer>();
            _particleSystem = buildingPrefab.GetComponent<ParticleSystem>();
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
            
            if (_particleSystem != null)
            {
                _particleSystem.Play(); // Вызываем только если компонент есть
            }
            ShakeCamera();
        }
        
        private void ShakeCamera()
        {
            if (cameraTransform == null) return;

            cameraTransform
                .DOShakePosition(
                    duration: 0.15f,
                    strength: 0.03f,
                    vibrato: 3,
                    randomness: 50,
                    snapping: false,
                    fadeOut: true
                );
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
        
        private int _baseSortingOrder = 2; 
        private void UpdateSortingOrder(CellView cellView, SpriteRenderer sr)
        {
            int calculatedOrder = _baseSortingOrder - cellView.Y;
            sr.sortingOrder = calculatedOrder +2;
        }
    }
}
