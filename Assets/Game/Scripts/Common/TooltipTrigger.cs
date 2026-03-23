using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriger : MonoBehaviour
{
    [Header("Tooltip Content")]
    [SerializeField] protected string header;
    [SerializeField] protected string content;
    
    [Header("Settings")]
    [Min(0f)] [SerializeField] protected float showDelay = 0.3f;
    
    public string Header 
    { 
        get => header; 
        set => header = value; 
    }
    
    public string Content 
    { 
        get => content; 
        set => content = value; 
    }
    
    private Tween _delayTween;
    private bool _isTooltipVisible = false;

    public void Enter()
    {
        if (_isTooltipVisible) return;
        
        _delayTween?.Kill();

        _delayTween = DOTween.Sequence()
            .AppendInterval(showDelay)
            .AppendCallback(() => {
                TooltipSystem.Show(content, header);
                _isTooltipVisible = true;
            });
    }

    public void Exit()
    {
        _delayTween?.Kill();

        if (_isTooltipVisible)
        {
            TooltipSystem.Hide();
            _isTooltipVisible = false;
        }
    }
}