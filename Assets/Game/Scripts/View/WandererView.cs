using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace View
{
    public class WandererView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _root;
        
        [Header("Settings")]
        [SerializeField] private Color _idleTint = Color.white;
        [SerializeField] private Color _movingTint = new Color(1f, 1f, 0.8f);
        [SerializeField] private float _bounceScale = 1.15f;
        
        private Core.Base.Wanderer _model;
        private CompositeDisposable _disposables;
        private Sequence _idleSequence;

        private void Awake()
        {
            _disposables = new CompositeDisposable();
            if (_spriteRenderer == null) 
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Bind(Core.Base.Wanderer model)
        {
            _model = model;
            
            model.Data.IsMoving
                .Subscribe(isMoving =>
                {
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.DOColor(
                            isMoving ? _movingTint : _idleTint, 
                            0.1f
                        );
                    }
                    
                    if (isMoving)
                    {
                        if (_root != null)
                        {
                            DOTween.Kill(_root);
                            _root.DOScale(Vector3.one * _bounceScale, 0.15f)
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() => 
                                {
                                    if (_root != null)
                                        _root.DOScale(Vector3.one, 0.15f).SetEase(Ease.InQuad);
                                });
                        }
                    }
                    else
                    {
                        if (_root != null)
                        {
                            DOTween.Kill(_root);
                            _root.localScale = Vector3.one;
                        }
                    }
                })
                .AddTo(_disposables);
                
            StartIdleAnimation();
        }

        private void StartIdleAnimation()
        {
            if (_root == null) return;
            
            _idleSequence = DOTween.Sequence()
                .Append(_root.DOScale(Vector3.one * 0.95f, 1f).SetEase(Ease.InOutSine))
                .Append(_root.DOScale(Vector3.one, 1f).SetEase(Ease.InOutSine))
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void MoveTo(int targetX, int targetY, float duration)
        {
            if (_root == null) return;
            
            float currentZ = _root.position.z;
            var targetPos = new Vector3(targetX, targetY, currentZ);
            _root.DOMove(targetPos, duration).SetEase(Ease.Linear);
        }

        public void MoveToImmediate(int x, int y)
        {
            if (_root == null) return;
            
            float currentZ = _root.position.z;
            _root.position = new Vector3(x, y, currentZ);
        }

        public void Unbind()
        {
            _disposables?.Clear();
            
            if (_root != null)
            {
                DOTween.Kill(_root);
            }
            _idleSequence?.Kill();
            _model = null;
        }

        private void OnDestroy()
        {
            _disposables?.Dispose();
            if (_root != null)
                DOTween.Kill(_root);
        }
    }
}