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
        
         

        private void Awake()
        {
            if (confirmButton != null)
                confirmButton.onClick.AddListener(ConfirmEvent);
                
            incidentInfoPanel.SetActive(false);
        }

        public void Init()
        {
            G.Events.LongIncidentStarted.Subscribe(OnIncidentStarted).AddTo(this);
        }

        private void OnIncidentStarted(ActiveIncident incident)
        { 
            if (incident == null || incident.Data == null) return;

            incidentInfoPanel.SetActive(true);
            
            if (nameText != null) nameText.text = incident.Data.incidentName;
            if (image != null && incident.Data.incidentSprite != null) image.sprite = incident.Data.incidentSprite;
            
            string lossDescription = FormatResourceLoss(incident.Data.resourceLoss);
            
            if (resourceLossText != null)
            {
                resourceLossText.text = lossDescription;
                resourceLossText.color = Color.red;
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
                case ResourceType.Crystal: return "Кристаллы";
                case ResourceType.Food: return "Еда";
                default: return type.ToString();
            }
        }

        private void ConfirmEvent()
        {
            incidentInfoPanel.SetActive(false);
        }

    }
}
