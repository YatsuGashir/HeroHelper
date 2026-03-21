using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace View
{
    public class SuccessorDiedAnimation : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform parentRect;
        [SerializeField] private RectTransform childRect; 
        [SerializeField] private TMP_Text text;
        
        [Header("Settings")]
        [SerializeField] private float moveToCenterDuration = 0.5f;
        [SerializeField] private float fallDuration = 0.4f;
        [SerializeField] private float fallDistance = 150f;
        [SerializeField] private Vector3 fallRotation = new Vector3(0, 0, 45);
        [SerializeField] private Ease fallEase = Ease.InBack;
        [SerializeField] private float textDisplayDelay = 1.5f;
        
        [Header("Optional")]
        [SerializeField] private bool addBounce = true;
        [SerializeField] private float bounceStrength = 30f;
        
        private TextFader textFader;

        private Sequence _animationSequence;
        
        public async UniTask PlayAnimation(RectTransform targetChild, TextFader fader)
        {
            childRect = targetChild;
            textFader = fader;
            
            _animationSequence?.Kill();
            
            var completionSource = new UniTaskCompletionSource();
            
            
            _animationSequence = DOTween.Sequence();
            
            if (parentRect != null)
            {
                Vector2 screenCenter = Vector2.zero; 
                
                _animationSequence.Append(
                    parentRect.DOAnchorPos(screenCenter, moveToCenterDuration)
                        .SetEase(Ease.OutQuad)
                );
            }


            _animationSequence.AppendCallback(() => 
            {
                childRect.anchoredPosition = Vector2.zero;
                childRect.localRotation = Quaternion.identity;
            });

                _animationSequence.Join(
                    childRect.DOAnchorPosY(-fallDistance, fallDuration)
                        .SetEase(fallEase)
                );
                _animationSequence.Join(
                    childRect.DOLocalRotate(fallRotation, fallDuration)
                        .SetEase(fallEase)
                );
                
                if (addBounce)
                {
                    _animationSequence.Append(
                        childRect.DOAnchorPosY(-fallDistance + bounceStrength, 0.15f)
                            .SetEase(Ease.OutQuad)
                    );
                    _animationSequence.Append(
                        childRect.DOAnchorPosY(-fallDistance, 0.1f)
                            .SetEase(Ease.InQuad)
                    );
                }

                _animationSequence.AppendCallback(() =>
                {
                    fader?.ShowText("Король умер");
                });
                
                _animationSequence.AppendInterval(textDisplayDelay);
                
                _animationSequence.AppendCallback(() =>
                {
                    fader?.HideText();
                });
                
                _animationSequence.AppendCallback(() =>
                {
                    completionSource.TrySetResult();
                });
            
                _animationSequence.Play();
            
                await completionSource.Task;
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }
    }
}