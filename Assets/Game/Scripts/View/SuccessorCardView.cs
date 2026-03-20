using System;
using Data;
using GlobalSpace;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SuccessorCardView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform _portraitAnchor;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _lifespanText;
    [SerializeField] private Button _selectButton;
        
    private readonly Subject<SuccessorCardView> _onSelected = new();
    public IObservable<SuccessorCardView> OnSelected => _onSelected;
        
    private SuccessorProfile _profile;
    private GameObject _currentPortrait = null;

    public void Setup(SuccessorProfile profile)
    {
        _profile = profile;
        _nameText.text = profile.successorName;
        _lifespanText.text = $"Жизнь: {profile.timeToDeath} ходов";
        Destroy(_currentPortrait);
        _currentPortrait = G.SuccessorFaceBuilder.BuildSuccessor(_profile);
        _currentPortrait.transform.SetParent(_portraitAnchor, false);
        _currentPortrait.transform.localPosition = Vector3.zero;
    }

    private void Start()
    {
        _selectButton.onClick.AddListener(() => _onSelected.OnNext(this));
    }

    private void OnDestroy() => _onSelected.Dispose();
}
