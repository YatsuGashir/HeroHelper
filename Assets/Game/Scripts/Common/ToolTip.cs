using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ToolTip : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    public int characterWrapLimit = 50;
    public float fadeInDuration = 0.15f;
    public float fadeOutDuration = 0.1f;
    public float scaleDuration = 0.12f;
    public Vector2 targetScale = Vector2.one;
    public Vector2 startScale = new Vector2(0.95f, 0.95f);

    [Header("Offset")]
    public Vector2 positionOffset = new Vector2(15, -25);

    private RectTransform _rectTransform;
    private Sequence _appearSequence;
    private Sequence _disappearSequence;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        _rectTransform.localScale = startScale;
        gameObject.SetActive(false);
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled =
            (headerLength > characterWrapLimit || contentLength > characterWrapLimit);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
    }

    public void ShowAnimated()
    {
        _disappearSequence?.Kill();

        gameObject.SetActive(true);

        canvasGroup.alpha = 0f;
        _rectTransform.localScale = startScale;

        _appearSequence = DOTween.Sequence()
            .Join(_rectTransform.DOScale(targetScale, scaleDuration)
                .SetEase(Ease.OutBack))
            .Join(canvasGroup.DOFade(1f, fadeInDuration)
                .SetEase(Ease.OutQuad))
            .Play();
    }

    public void HideAnimated()
    {
        _appearSequence?.Kill();

        _disappearSequence = DOTween.Sequence()
            .Join(_rectTransform.DOScale(startScale, fadeOutDuration)
                .SetEase(Ease.InQuad))
            .Join(canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(Ease.InQuad))
            .OnComplete(() => gameObject.SetActive(false))
            .Play();
    }

    private void LateUpdate()
    {
        if (!gameObject.activeSelf) return;

        Vector2 position = Input.mousePosition;
        position += positionOffset;
        Vector2 size = _rectTransform.rect.size;

        float pivotX = _rectTransform.pivot.x;
        float pivotY = _rectTransform.pivot.y;

        // Clamp X
        position.x = Mathf.Clamp(
            position.x,
            size.x * pivotX,
            Screen.width - size.x * (1 - pivotX)
        );

        // Clamp Y
        position.y = Mathf.Clamp(
            position.y,
            size.y * pivotY,
            Screen.height - size.y * (1 - pivotY)
        );

        transform.position = position;
    }

    private void OnDestroy()
    {
        _appearSequence?.Kill();
        _disappearSequence?.Kill();
    }
}