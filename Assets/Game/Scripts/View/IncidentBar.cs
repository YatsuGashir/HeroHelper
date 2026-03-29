using Core.incidents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncidentBar : MonoBehaviour
{
        [Header("Components")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _timerText;
        //[SerializeField] private Slider _progressSlider;

        private ActiveIncident _currentIncident;


        public void Initialize(ActiveIncident incident)
        {
            _currentIncident = incident;
            
            if (_titleText != null)
                _titleText.text = incident.Data.incidentName;
            
            UpdateVisuals();
        }


        public void UpdateProgress()
        {
            if (_currentIncident == null) return;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            int total = _currentIncident.Data.turnsUntilTrigger;
            int current = total - _currentIncident.TurnsRemaining; 
            
            int remaining = _currentIncident.TurnsRemaining;

            if (_timerText != null)
                _timerText.text = $"{remaining}";
            
            float progress = 1f - ((float)remaining / total);

            if (total <= 0) progress = 1f;
            progress = Mathf.Clamp01(progress);

            //if (_progressSlider != null)
                //_progressSlider.value = progress;

        }
        
}
