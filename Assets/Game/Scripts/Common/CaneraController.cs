using UnityEngine;

public class CaneraController : MonoBehaviour
{
 [Header("Настройки движения")]
    public float panSpeed = 5f;
    public float zoomSpeed = 2f;

    [Header("Ограничения Зума")]
    public float minZoom = 1f;
    public float maxZoom = 10f;

    [Header("Границы карты (Ограничения движения)")]
    public Vector2 mapBoundsMin;
    public Vector2 mapBoundsMax;

    private Camera cam;

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
        if (Input.GetMouseButton(1))
        {
            float xAxis = Input.GetAxis("Mouse X");
            float yAxis = Input.GetAxis("Mouse Y");

            Vector3 move = new Vector3(-xAxis, -yAxis, 0) * panSpeed * Time.deltaTime;
            
            transform.Translate(move);

            Vector3 newPos = transform.position;
            newPos.x = Mathf.Clamp(newPos.x, mapBoundsMin.x, mapBoundsMax.x);
            newPos.y = Mathf.Clamp(newPos.y, mapBoundsMin.y, mapBoundsMax.y);

            transform.position = newPos;
        }
    }

    void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
    
    #if UNITY_EDITOR
    public Color gizmoColor = Color.green;  
    public float gizmoLineWidth = 0.1f;  
    void OnDrawGizmos()
    {

        // Получаем значения границ из контроллера
        // Используем публичные поля, убедитесь, что они доступны
        Vector2 min = mapBoundsMin;
        Vector2 max = mapBoundsMax;

        // Настраиваем цвет и матрицу трансформации
        Gizmos.color = gizmoColor;
        
        // Сохраняем текущую матрицу, чтобы вернуть её в конце
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

        // Рисуем квадрат/прямоугольник
        // Мы рисуем его на плоскости Z=0 относительно позиции объекта
        Vector3 bottomLeft = new Vector3(min.x, min.y, 0);
        Vector3 topRight = new Vector3(max.x, max.y, 0);
        
        // Рисуем 4 линии, образующие прямоугольник
        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x, min.y, 0)); // Низ
        Gizmos.DrawLine(new Vector3(max.x, min.y, 0), new Vector3(max.x, max.y, 0)); // Право
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x, max.y, 0)); // Верх
        Gizmos.DrawLine(new Vector3(min.x, max.y, 0), new Vector3(min.x, min.y, 0)); // Лево

        // Опционально: рисуем полупрозрачную плоскость внутри для наглядности
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.1f);
        Vector3 center = new Vector3((min.x + max.x) / 2, (min.y + max.y) / 2, 0);
        Vector3 size = new Vector3(max.x - min.x, max.y - min.y, 0.01f);
        
        // Восстанавливаем матрицу перед рисованием куба, чтобы он не зависел от вращения камеры
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawCube(center, size);
    }
    #endif
}
