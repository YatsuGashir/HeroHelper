using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigerForUi : TooltipTriger, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Enter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Exit();
    }
}
