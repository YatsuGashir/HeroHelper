using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _clickCursor;      // Курсор при нажатии ЛКМ
    [SerializeField] private Texture2D _hoverCursor;      // Опционально: при наведении
    
    [Header("Cursor Settings")]
    [SerializeField] private Vector2 _cursorHotspot = Vector2.zero; // Точка привязки (обычно центр или верх-лево)
    [SerializeField] private CursorMode _cursorMode = CursorMode.Auto;
    
    [Header("Interaction")]
    [SerializeField] private LayerMask _clickableLayer;   // Слой объектов, на которых меняется курсор
    [SerializeField] private float _clickCursorDelay = 0.05f; // Задержка перед сменой (для предотвращения мерцания)

    private bool _isClicking;
    private float _clickStartTime;
    private Texture2D _currentCursor;

    private void Start()
    {
        // Применяем курсор по умолчанию
        SetCursor(_defaultCursor);
    }

    private void Update()
    {
        HandleCursorChange();
    }

    private void HandleCursorChange()
    {
        // === ЛКМ нажато ===
        if (Input.GetMouseButton(0))
        {
            if (!_isClicking)
            {
                _isClicking = true;
                _clickStartTime = Time.time;
            }
            
            // Меняем курсор только после небольшой задержки (чтобы не мерцал)
            if (Time.time - _clickStartTime >= _clickCursorDelay && _clickCursor != null)
            {
                SetCursor(_clickCursor);
            }
        }
        // === ЛКМ отпущено ===
        else if (_isClicking)
        {
            _isClicking = false;
            RestoreDefaultCursor();
        }
        
        // === Опционально: курсор при наведении на интерактивные объекты ===
        if (!_isClicking && _hoverCursor != null)
        {
            if (IsPointerOverClickableObject())
            {
                SetCursor(_hoverCursor);
            }
            else if (_currentCursor != _defaultCursor)
            {
                RestoreDefaultCursor();
            }
        }
    }

    /// <summary>
    /// Проверка: наведён ли курсор на объект с интерактивным слоем
    /// </summary>
    private bool IsPointerOverClickableObject()
    {
        // Для UI (EventSystem)
        if (UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        
        // Для 2D/3D объектов
        Vector2 mousePos = Camera.main?.ScreenToWorldPoint(Input.mousePosition) ?? (Vector2)Input.mousePosition;
        var hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, _clickableLayer);
        return hit.collider != null;
        
        // Для 3D:
        // var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // return Physics.Raycast(ray, out var hit, Mathf.Infinity, _clickableLayer);
    }

    /// <summary>
    /// Установить кастомный курсор
    /// </summary>
    private void SetCursor(Texture2D texture)
    {
        if (texture == _currentCursor) return; // Не меняем, если уже установлен
        
        Cursor.SetCursor(texture, _cursorHotspot, _cursorMode);
        _currentCursor = texture;
    }

    /// <summary>
    /// Вернуть курсор по умолчанию
    /// </summary>
    private void RestoreDefaultCursor()
    {
        SetCursor(_defaultCursor);
    }

    /// <summary>
    /// Публичный метод для принудительной смены курсора (из других скриптов)
    /// </summary>
    public void ForceSetCursor(Texture2D texture, Vector2? hotspot = null)
    {
        var hot = hotspot ?? _cursorHotspot;
        Cursor.SetCursor(texture, hot, _cursorMode);
        _currentCursor = texture;
    }

    /// <summary>
    /// Скрыть курсор полностью
    /// </summary>
    public void HideCursor()
    {
        Cursor.visible = false;
    }

    /// <summary>
    /// Показать курсор
    /// </summary>
    public void ShowCursor()
    {
        Cursor.visible = true;
        RestoreDefaultCursor();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Восстанавливаем курсор при возврате фокуса в окно
        if (hasFocus)
            RestoreDefaultCursor();
    }

    private void OnDestroy()
    {
        // Сбрасываем курсор при уничтожении менеджера
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Cursor.visible = true;
    }
}