using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    
    public string content;
    
    
    [Min(0f)] public float showDelay = 0.3f;
    
    private Tween _delayTween;
    private bool _isTooltipVisible = false;

    public void OnPointerEnter(PointerEventData eventData)
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

    public void OnPointerExit(PointerEventData eventData)
    {
        _delayTween?.Kill();

        if (_isTooltipVisible)
        {
            TooltipSystem.Hide();
            _isTooltipVisible = false;
        }
    }
}