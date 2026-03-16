using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Настройки движения")]
    public float panSpeed = 5f;
    public float zoomSpeed = 2f;
    
    [Header("Мертвые зоны (для защиты от случайных действий)")]
    [Tooltip("Минимальное движение мыши для начала панорамирования (0-1)")]
    public float panDeadZone = 0.03f;
    
    [Tooltip("Минимальный скролл для начала зума")]
    public float zoomDeadZone = 0.02f;
    
    [Tooltip("Плавность нарастания скорости панорамирования")]
    public float panSmoothing = 8f;

    [Header("Ограничения Зума")]
    public float minZoom = 1f;
    public float maxZoom = 10f;

    [Header("Границы карты (Ограничения движения)")]
    public Vector2 mapBoundsMin;
    public Vector2 mapBoundsMax;

    private Camera cam;
    private Vector2 _panAccumulator; // Накопитель движения для плавности

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void LateUpdate()
    {
        HandlePanning();
        HandleZooming();
    }

    void HandlePanning()
    {
        // Проверяем зажатие Правой Кнопки Мыши
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            // === DEAD ZONE: Игнорируем микро-движения ===
            if (Mathf.Abs(xAxis) < panDeadZone) xAxis = 0;
            if (Mathf.Abs(yAxis) < panDeadZone) yAxis = 0;

            // Если после отсечения движение стало нулевым — выходим
            if (xAxis == 0 && yAxis == 0)
            {
                // Плавно сбрасываем накопитель
                _panAccumulator = Vector2.Lerp(_panAccumulator, Vector2.zero, Time.deltaTime * panSmoothing * 2);
                return;
            }

            // === SMOOTHING: Плавное нарастание скорости ===
            Vector2 input = new Vector2(xAxis, yAxis);
            _panAccumulator = Vector2.Lerp(_panAccumulator, input, Time.deltaTime * panSmoothing);

            // Двигаем камеру с учетом накопленного ввода
            Vector3 move = new Vector3(-_panAccumulator.x, -_panAccumulator.y, 0) * panSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);

            // Ограничиваем позицию камеры по границам
            Vector3 newPos = transform.position;
            newPos.x = Mathf.Clamp(newPos.x, mapBoundsMin.x, mapBoundsMax.x);
            newPos.y = Mathf.Clamp(newPos.y, mapBoundsMin.y, mapBoundsMax.y);
            
            transform.position = newPos;
        }
        else
        {
            // Если кнопка отпущена — сбрасываем накопитель
            _panAccumulator = Vector2.Lerp(_panAccumulator, Vector2.zero, Time.deltaTime * panSmoothing);
        }
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // === DEAD ZONE: Игнорируем микро-скролл ===
        if (Mathf.Abs(scroll) < zoomDeadZone)
        {
            return;
        }

        // Применяем зум только если скролл значимый
        cam.orthographicSize -= scroll * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
    
    #if UNITY_EDITOR
    public Color gizmoColor = Color.green;  
    public float gizmoLineWidth = 0.1f;
    
    void OnDrawGizmos()
    {
        Vector2 min = mapBoundsMin;
        Vector2 max = mapBoundsMax;

        Gizmos.color = gizmoColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        // Рисуем рамку границ
        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0));
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0));

        // Полупрозрачная заливка
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Vector3 center = new Vector3((min.x + max.x) / 2, (min.y + max.y) / 2, 0);
        Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.01f);
        
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawCube(center, size);
    }
    #endif
}