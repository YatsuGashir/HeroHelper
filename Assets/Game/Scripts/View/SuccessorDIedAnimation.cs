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
        [SerializeField] private RectTransform crownRect;  // 🔥 Новая ссылка на корону
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
        
        [Header("Crown Settings")]  // 🔥 Настройки короны
        [SerializeField] private Vector2 crownOffset = new Vector2(0, 60);  // Смещение короны относительно ребёнка
        [SerializeField] private Vector3 crownFallRotation = new Vector3(0, 0, -30);  // Вращение короны при падении
        [SerializeField] private float crownFallDelay = 0.05f;  // Небольшая задержка для естественности
        
        private TextFader textFader;
        private Sequence _animationSequence;
        
        public async UniTask PlayAnimation(RectTransform targetChild, TextFader fader)
        {
            childRect = targetChild;
            textFader = fader;
            
            _animationSequence?.Kill();
            
            var completionSource = new UniTaskCompletionSource();
            
            _animationSequence = DOTween.Sequence();
            
            // === ЭТАП 1: Родитель двигается в центр ===
            if (parentRect != null)
            {
                Vector2 screenCenter = Vector2.zero; 
                
                _animationSequence.Append(
                    parentRect.DOAnchorPos(screenCenter, moveToCenterDuration)
                        .SetEase(Ease.OutQuad)
                );
            }

            // === ЭТАП 2: Подготовка ребёнка и короны ===
            _animationSequence.AppendCallback(() => 
            {
                // Сброс позиции ребёнка
                childRect.anchoredPosition = Vector2.zero;
                childRect.localRotation = Quaternion.identity;
                
                // 🔥 Подготовка короны
                if (crownRect != null)
                {
                    crownRect.gameObject.SetActive(true);
                    crownRect.anchoredPosition = crownOffset;  // Позиция на голове
                    crownRect.localRotation = Quaternion.identity;
                }
                
                AudioManager.Instance.PlaySFX("death");
            });

            // === ЭТАП 3: Падение ребёнка и короны (параллельно) ===
            
            // Падение ребёнка
            _animationSequence.Join(
                childRect.DOAnchorPosY(-fallDistance, fallDuration)
                    .SetEase(fallEase)
            );
            _animationSequence.Join(
                childRect.DOLocalRotate(fallRotation, fallDuration)
                    .SetEase(fallEase)
            );
            
            // 🔥 Падение короны (с небольшой задержкой для естественности)
            if (crownRect != null)
            {
                _animationSequence.Join(
                    crownRect.DOAnchorPosY(crownOffset.y - fallDistance - 20, fallDuration)
                        .SetEase(fallEase)
                        .SetDelay(crownFallDelay)  // 🔥 Задержка внутри твина
                );
                _animationSequence.Join(
                    crownRect.DOLocalRotate(crownFallRotation, fallDuration)
                        .SetEase(fallEase)
                        .SetDelay(crownFallDelay)  // 🔥 Та же задержка для вращения
                );
                
                // 🔥 Отскок короны (если включён)
                if (addBounce)
                {
                    _animationSequence.Append(
                        crownRect.DOAnchorPosY(crownOffset.y - fallDistance - 20 + bounceStrength * 0.7f, 0.15f)
                            .SetEase(Ease.OutQuad)
                    );
                    _animationSequence.Append(
                        crownRect.DOAnchorPosY(crownOffset.y - fallDistance - 20, 0.1f)
                            .SetEase(Ease.InQuad)
                    );
                }
            }
            crownRect.rotation = Quaternion.identity;
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
            
            if (crownRect != null)
            {
                _animationSequence.AppendCallback(() =>
                {
                    crownRect.localRotation = Quaternion.identity;
                });
            }
            
            // === ЭТАП 4: Показ текста ===
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

        // 🔥 Метод для сброса короны перед следующей анимацией
        public void ResetCrown()
        {
            if (crownRect != null)
            {
                crownRect.anchoredPosition = crownOffset;
                crownRect.localRotation = Quaternion.identity;
                crownRect.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
        }
    }
}