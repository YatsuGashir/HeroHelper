using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem _current;
    
    public ToolTip tooltip;

    private void Awake()
    {
        _current = this;
    }

    public static void Show(string content, string header = "")
    {
        if (_current?.tooltip == null) return;
        
        _current.tooltip.SetText(content, header);
        _current.tooltip.ShowAnimated(); // ← Анимированное появление
    }

    public static void Hide()
    {
        if (_current?.tooltip == null) return;
        
        _current.tooltip.HideAnimated(); // ← Анимированное исчезновение
    }
}