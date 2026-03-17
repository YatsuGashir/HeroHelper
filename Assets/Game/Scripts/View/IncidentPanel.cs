using System.Collections.Generic;
using Core.incidents;
using GlobalSpace;
using UniRx;
using UnityEngine;

namespace View
{
    public class IncidentPanel : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform _contentParent;
        [SerializeField] private IncidentBar _barPrefab; 

        private Dictionary<IncidentData, IncidentBar> _activeBars = new();

        public void Init()
        {
            G.Events.LongIncidentStarted.Subscribe(OnIncidentStarted).AddTo(this);
            G.Events.LongIncidentUpdated.Subscribe(OnIncidentUpdated).AddTo(this);
            G.Events.LongIncidentResolved.Subscribe(OnIncidentResolved).AddTo(this);
        }

        private void OnIncidentStarted(ActiveIncident incident)
        {
            var bar = Instantiate(_barPrefab, _contentParent);
            bar.Initialize(incident);

            if (!_activeBars.ContainsKey(incident.Data))
            {
                _activeBars[incident.Data] = bar;
            }
        
            Debug.Log($"[UI] Добавлен бар события: {incident.Data.incidentName}");
        }

        private void OnIncidentUpdated(ActiveIncident incident)
        {
            if (_activeBars.TryGetValue(incident.Data, out var bar))
            {
                bar.UpdateProgress();
            }
        }

        private void OnIncidentResolved(ActiveIncident incident)
        {
            if (_activeBars.TryGetValue(incident.Data, out var bar))
            {
                Destroy(bar.gameObject);
                _activeBars.Remove(incident.Data);
                Debug.Log($"[UI] Удален бар события: {incident.Data.incidentName}");
            }
        }
    
        private void OnDestroy()
        {
            foreach (var bar in _activeBars.Values)
            {
                if (bar != null) Destroy(bar.gameObject);
            }
            _activeBars.Clear();
        }
    }
}
