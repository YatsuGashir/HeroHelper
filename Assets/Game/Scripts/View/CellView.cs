using System;
using Data;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CellView : MonoBehaviour
{
   [Header("Components")]
   [SerializeField] private SpriteRenderer _terrainRenderer;
   [SerializeField] private SpriteRenderer _highlightRenderer;
   [SerializeField] private Transform _buildingAnchor;
   [SerializeField] private Collider2D _collider;

   [Header("Settings")]
   [SerializeField] private Color _highlightValidColor = new Color(0.2f, 1f, 0.2f, 0.6f);
   [SerializeField] private Color _highlightInvalidColor = new Color(1f, 0.2f, 0.2f, 0.6f);
   [SerializeField] private float _hoverScale = 1.1f;
   [SerializeField] private float _defaultScale = 1.0f;
   
   public int X { get; private set; }
   public int Y { get; private set; }
   
   //ссылка на постройку
   
   private readonly Subject<CellView> _onCellClick = new Subject<CellView>();
   private readonly Subject<CellView> _onCellHoverEnter = new Subject<CellView>();
   private readonly Subject<CellView> _onCellHoverExit = new Subject<CellView>();
   
   public IObservable<CellView> OnCellClick => _onCellClick;
   public IObservable<CellView> OnCellHoverEnter => _onCellHoverEnter;
   public IObservable<CellView> OnCellHoverExit => _onCellHoverExit;

   private CompositeDisposable _disposables;
   
   private void Awake()
   {
      if (_terrainRenderer == null) _terrainRenderer = GetComponentInChildren<SpriteRenderer>();
      if (_collider == null) _collider = GetComponent<Collider2D>();
        
      _disposables = new CompositeDisposable();
   }
   
   public void Initialize(int x, int y)
   {
      X = x;
      Y = y;
      transform.position = new Vector3(x, y, 0);
      gameObject.name = $"Cell_{x}_{y}";
        
      ResetVisuals();
   }
   
   public void SetTerrain(TerrainType terrain, TerrainSpriteLibrary terrainSpriteLibrary)
   {
      if (_terrainRenderer == null) return;
        
      Sprite sprite = terrainSpriteLibrary.GetTerrainSprite(terrain);
      _terrainRenderer.sprite = sprite;
      _terrainRenderer.color = Color.white;
      
   }
   
   private void ResetVisuals()
   {
      SetHighlight(false, true);
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
      transform.DOScale(Vector3.one * _hoverScale, 0.15f).SetEase(Ease.OutBack);
   }

   private void OnMouseExit()
   {
      _onCellHoverExit.OnNext(this);
      DOTween.Kill(transform);
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
