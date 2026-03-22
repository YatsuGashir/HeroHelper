using System;
using Data;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class CellView : MonoBehaviour
{
   [Header("Components")]
   [SerializeField] private SpriteRenderer _baseRenderer;      // Meadow/Water
   [SerializeField] private SpriteRenderer _overlayRenderer;   // Forest/Stone/Crystal/Grass
   [SerializeField] private SpriteRenderer _highlightRenderer;
   [SerializeField] private Transform _buildingAnchor;
   [SerializeField] private Collider2D _collider;

   [Header("Settings")]
   [SerializeField] private Color _highlightValidColor = new Color(0.2f, 1f, 0.2f, 0.6f);
   [SerializeField] private Color _highlightInvalidColor = new Color(1f, 0.2f, 0.2f, 0.6f);
   [SerializeField] private float _hoverScale = 1.1f;
   [SerializeField] private float _defaultScale = 1.0f;

   [Header("References")]
   [SerializeField] private Material forestMaterial;
   
   public int X { get; private set; }
   public int Y { get; private set; }
   
   private readonly Subject<CellView> _onCellClick = new Subject<CellView>();
   private readonly Subject<CellView> _onCellHoverEnter = new Subject<CellView>();
   private readonly Subject<CellView> _onCellHoverExit = new Subject<CellView>();
   
   public IObservable<CellView> OnCellClick => _onCellClick;
   public IObservable<CellView> OnCellHoverEnter => _onCellHoverEnter;
   public IObservable<CellView> OnCellHoverExit => _onCellHoverExit;

   private CompositeDisposable _disposables;
   
   private void Awake()
   {
      if (_baseRenderer == null) _baseRenderer = GetComponentInChildren<SpriteRenderer>();
      if (_collider == null) _collider = GetComponent<Collider2D>();
        
      _disposables = new CompositeDisposable();
      
      // Настройка оверлей-рендерера: выше базы, но ниже хайлайта
      if (_overlayRenderer != null)
      {
          _overlayRenderer.sortingOrder = (_baseRenderer?.sortingOrder ?? 0) + 1;
          _overlayRenderer.gameObject.SetActive(false); // по умолчанию скрыт
      }
   }
   
   public void Initialize(int x, int y)
   {
      X = x;
      Y = y;
      transform.position = new Vector3(x, y, 0);
      gameObject.name = $"Cell_{x}_{y}";
      ResetVisuals();
      UpdateSortingOrder();
   }
   
   private int _baseSortingOrder = 2; 
   private void UpdateSortingOrder()
   {
       
       int calculatedOrder = _baseSortingOrder - Y;

       if (_baseRenderer != null)
           _baseRenderer.sortingOrder = calculatedOrder;

       if (_overlayRenderer != null)
       {
           _overlayRenderer.sortingOrder = calculatedOrder + 1;
       }

       if (_highlightRenderer != null)
       {
           _highlightRenderer.sortingOrder = calculatedOrder + 2;
       }

   }

   // === Установка базового тайла (Meadow с autotile или Water) ===
   public void SetBaseTerrain(TerrainType terrain, TerrainSpriteLibrary library, 
       Func<int, int, TerrainType> getNeighborTerrain)
   {
       if (_baseRenderer == null || library == null) return;
    
       Sprite sprite = null;

       // Autotile только для Meadow
       if (terrain != TerrainType.Water)
       {
           int autotileIndex = CalculateMeadowAutotileIndex(getNeighborTerrain);
           sprite = library.GetAutotileSprite(TerrainType.Meadow, autotileIndex);
       }
    
       // Фоллбэк на обычный спрайт
       if (sprite == null)
       {
           sprite = library.GetTerrainSprite(terrain);
       }
    
       if (sprite != null)
       {
           _baseRenderer.sprite = sprite;
           _baseRenderer.color = Color.white;
           _baseRenderer.gameObject.SetActive(true);
       }
   }

   // Расчёт индекса autotile для Meadow
   // Расчёт индекса autotile для Meadow (с поддержкой 20 спрайтов)
   private int CalculateMeadowAutotileIndex(Func<int, int, TerrainType> getNeighbor)
   {
       if (getNeighbor == null) return 0;
    
       // Ортогональные соседи
       bool top = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X, Y + 1));
       bool right = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X + 1, Y));
       bool bottom = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X, Y - 1));
       bool left = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X - 1, Y));
    
       // Диагональные соседи (для углов)
       bool topLeft = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X - 1, Y + 1));
       bool topRight = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X + 1, Y + 1));
       bool bottomRight = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X + 1, Y - 1));
       bool bottomLeft = !AutotileHelper.IsBlockingTerrain(getNeighbor.Invoke(X - 1, Y - 1));
    
       return AutotileHelper.CalculateAutotileIndexWithCorners(
           top, right, bottom, left,
           topLeft, topRight, bottomRight, bottomLeft);
   }

   // === Установка оверлея (Forest/Stone/Crystal/Grass) ===
   public void SetOverlay(TerrainType overlayType, TerrainSpriteLibrary library)
   {
       if (_overlayRenderer == null || library == null) return;

       // Если тип не является оверлеем — скрываем рендерер
       if (!library.IsOverlayTerrain(overlayType))
       {
           _overlayRenderer.gameObject.SetActive(false);
           _overlayRenderer.sprite = null;
           return;
       }
       
       // Берём спрайт и показываем оверлей
       Sprite sprite = library.GetTerrainSprite(overlayType);
       if (sprite != null)
       {
           _overlayRenderer.sprite = sprite;
           _overlayRenderer.color = Color.white;
           _overlayRenderer.gameObject.SetActive(true);
           if (overlayType == TerrainType.Threes)
           {
                _overlayRenderer.sharedMaterial = forestMaterial;
           }
       }
   }
   
   private void ResetVisuals()
   {
      SetHighlight(false, true);
      if (_overlayRenderer != null)
      {
          _overlayRenderer.gameObject.SetActive(false);
          _overlayRenderer.sprite = null;
      }
   }
   
   public void SetHighlight(bool isActive, bool isValid)
   {
      if (_highlightRenderer == null) return;

      _highlightRenderer.gameObject.SetActive(isActive);
        
      if (isActive)
      {
         _highlightRenderer.color = isValid ? _highlightValidColor : _highlightInvalidColor;
         _highlightRenderer.transform.DOScale(Vector3.one * 1.05f, 0.1f).SetEase(Ease.OutQuad);
      }
      else
      {
         DOTween.Kill(_highlightRenderer.transform);
         _highlightRenderer.transform.localScale = Vector3.one;
      }
   }
   
   private void OnMouseEnter()
   {
      _onCellHoverEnter.OnNext(this);
      //transform.DOScale(Vector3.one * _hoverScale, 0.15f).SetEase(Ease.OutBack);
   }

   private void OnMouseExit()
   {
      _onCellHoverExit.OnNext(this);
      //DOTween.Kill(transform);
      transform.localScale = Vector3.one;
   }

   private void OnMouseDown()
   {
      _onCellClick.OnNext(this);
   }

   private void OnDestroy()
   {
      _disposables?.Dispose();
      _onCellClick.Dispose();
      _onCellHoverEnter.Dispose();
      _onCellHoverExit.Dispose();
   }
}