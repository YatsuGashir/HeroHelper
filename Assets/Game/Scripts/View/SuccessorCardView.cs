using System;
using Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SuccessorCardView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _portrait;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _lifespanText;
    [SerializeField] private Button _selectButton;
        
    private readonly Subject<SuccessorCardView> _onSelected = new();
    public IObservable<SuccessorCardView> OnSelected => _onSelected;
        
    private SuccessorProfile _profile;

    public void Setup(SuccessorProfile profile)
    {
        _profile = profile;
        _nameText.text = profile.successorName;
        _lifespanText.text = $"Жизнь: {profile.timeToDeath} ходов";
        _portrait.sprite = profile.portrait;
    }

    private void Start()
    {
        _selectButton.onClick.AddListener(() => _onSelected.OnNext(this));
    }

    private void OnDestroy() => _onSelected.Dispose();
}
