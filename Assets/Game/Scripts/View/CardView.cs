using System;
using Data;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class CardViewa : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        
        [Header("Settings")]
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = new Color(0.8f, 0.4f, 0.4f, 0.6f);
        
        private BuildingDefinition  definition;
        private readonly Subject<BuildingDefinition> _onSelected = new Subject<BuildingDefinition>();
        
        public IObservable<BuildingDefinition> OnSelected => _onSelected;

        private CompositeDisposable _disposables;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
            
        }
        
        public void SetData(BuildingDefinition def)
        {
            definition = def;
            
            if (iconImage) iconImage.sprite = def.buildingIcon;
            if (nameText) nameText.text = def.buildingName;


        }
        
        private void OnMouseDown()
        {
            _onSelected.OnNext(definition);

        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
            _onSelected.Dispose();
        }

    }
}
