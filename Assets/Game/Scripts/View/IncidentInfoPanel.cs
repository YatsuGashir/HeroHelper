using System.Collections.Generic;
using System.Text;
using Core.Base;
using Core.incidents;
using Cysharp.Threading.Tasks;
using Data;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class IncidentInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject incidentInfoPanel;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text resourceLossText;
        [SerializeField] private Image image;
        [SerializeField] private Button confirmButton;
        
        private bool _isLongIncident = false;
        private ActiveIncident _currentActiveIncident;

        private void Awake()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmEvent);
                
            incidentInfoPanel.SetActive(false);
        }

        public void Init()
        {
            G.Events.LongIncidentStarted.Subscribe(OnLongIncidentStarted).AddTo(this);
            G.Events.ShortIncidentOccurred.Subscribe(OnShortIncidentOccurred).AddTo(this);
        }

        private void OnLongIncidentStarted(ActiveIncident incident)
        { 
            if (incident == null || incident.Data == null) return;

            _isLongIncident = true;
            _currentActiveIncident = incident;

            ShowPanel(incident.Data);
        }
        
        private void OnShortIncidentOccurred(IncidentData incident)
        {
            if (incident == null) return;

            _isLongIncident = false;
            _currentActiveIncident = null;

            ShowPanel(incident);

        }
        
        private void ShowPanel(IncidentData data)
        {
            Time.timeScale = 0;
            incidentInfoPanel.SetActive(true);
            
            if (nameText != null) nameText.text = data.incidentName;
            
            if (image != null && data.incidentSprite != null) 
                image.sprite = data.incidentSprite;
            else if (image != null)
                image.color = Color.clear;
            string lossDescription = FormatResourceLoss(data.resourceLoss);
            
            /*if (resourceLossText != null)
            {
                resourceLossText.text = lossDescription;
                resourceLossText.color = Color.red;
                resourceLossText.gameObject.SetActive(!string.IsNullOrEmpty(lossDescription) && lossDescription != "Нет прямых потерь ресурсов");
            }*/

            if (descriptionText != null)
            {
                string prefix = _isLongIncident ? "ПРЕДУПРЕЖДЕНИЕ:\n" : "<color=red>СОБЫТИЕ!</color>\n";
                descriptionText.text = prefix + data.description;
            }
        }

        private string FormatResourceLoss(List<ResourceAmount> losses)
        {
            if (losses == null || losses.Count == 0)
            {
                return "Нет прямых потерь ресурсов";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < losses.Count; i++)
            {
                var loss = losses[i];
                
                string resourceName = GetResourceName(loss.Type);

                sb.Append($"<color=#ff4444>-{loss.Amount}</color> {resourceName}");

                if (i < losses.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            return sb.ToString();
        }


        private string GetResourceName(ResourceType type)
        {
            switch (type)
            {
                case ResourceType.Spore: return "Споры";
                case ResourceType.Stone: return "Камень";
                case ResourceType.Water: return "Вода";
                case ResourceType.Wood: return "Дерево";
                //case ResourceType.Crystal: return "Кристалл";
                case ResourceType.Food: return "Еда";
                default: return type.ToString();
            }
        }

        private void ConfirmEvent()
        {
            Time.timeScale = 1;
            incidentInfoPanel.SetActive(false);
        }

    }
}
