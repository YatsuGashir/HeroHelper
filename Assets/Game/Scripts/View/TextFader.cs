using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextFader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private float fadeDuration = 0.3f;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        _canvasGroup.alpha = 0f;
        textField.gameObject.SetActive(false);
    }
    

    public void ShowText(string text)
    {
        textField.text = text;
        textField.gameObject.SetActive(true);
        
        _canvasGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutQuad);
    }

    public void HideText()
    {
        _canvasGroup.DOFade(0f, fadeDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => textField.gameObject.SetActive(false));
    }
}