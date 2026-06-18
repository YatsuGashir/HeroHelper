using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultCharSpeed = 0.05f;
    [SerializeField] private float _confirmCooldown = 0.1f;
    [SerializeField] private bool _richTextSupport = true;

    private TextMeshProUGUI _currentTextField;
    private string _fullText;
    private int _visibleCharCount;
    private bool _isWriting;
    private bool _skipRequested;
    private bool _confirmRequested;
    private float _lastConfirmTime;
    
    private CancellationTokenSource _cts;

    private void Awake()
    {
        G.TextController = this;
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void Update()
    {
        if (_currentTextField == null) return;
        
        if (Time.time - _lastConfirmTime < _confirmCooldown)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (_isWriting)
            {
                _skipRequested = true;
            }
            else
            {
                _confirmRequested = true;
            }
        }
    }

    public void Reset()
    {
        _skipRequested = false;
        _confirmRequested = false;
        _isWriting = false;
        _lastConfirmTime = 0f;
        _currentTextField = null;
    }

    public async UniTask PlayTextAsync(TextMeshProUGUI textField, string content, float charSpeed = -1f)
    {
        if (textField == null)
        {
            Debug.LogError("[TextController] Text field is null.");
            return;
        }

        _skipRequested = false;
        _confirmRequested = false;
        _lastConfirmTime = 0f;
        
        _currentTextField = textField;
        _fullText = content ?? string.Empty;
        _visibleCharCount = 0;
        _isWriting = true;
        
        if (_richTextSupport)
        {
            _currentTextField.richText = true;
        }
        
        _currentTextField.maxVisibleCharacters = 0;
        _currentTextField.text = _fullText;
        
        float speed = (charSpeed > 0f) ? charSpeed : _defaultCharSpeed;

        await TypeTextInternal(speed);

        _isWriting = false;
        await WaitForConfirm();

        _lastConfirmTime = Time.time;
        _confirmRequested = false;
        _currentTextField = null;

        await UniTask.Delay(TimeSpan.FromSeconds(_confirmCooldown), cancellationToken: _cts.Token);
    }

    private async UniTask TypeTextInternal(float speed)
    {
        if (string.IsNullOrEmpty(_fullText))
        {
            _visibleCharCount = 0;
            return;
        }
        
        int totalVisibleChars = TMP_Utils.GetVisibleCharacterCount(_fullText);

        while (_visibleCharCount < totalVisibleChars)
        {
            if (_skipRequested)
            {
                _currentTextField.maxVisibleCharacters = int.MaxValue;
                _visibleCharCount = totalVisibleChars;
                _skipRequested = false;
                break;
            }

            _visibleCharCount++;
            
            _currentTextField.maxVisibleCharacters = _visibleCharCount;

            if (ShouldPlaySoundForChar(_visibleCharCount))
            {
                AudioManager.Instance.PlaySFX("bubble");
            }
            
            await UniTask.Delay(TimeSpan.FromSeconds(speed), cancellationToken: _cts.Token);
        }
        
        _currentTextField.maxVisibleCharacters = int.MaxValue;
    }

    private bool ShouldPlaySoundForChar(int visibleIndex)
    {
        return true; 
    }

    private async UniTask WaitForConfirm()
    {
        while (!_confirmRequested)
        {
            if (_currentTextField == null)
                return;
                
            await UniTask.NextFrame(cancellationToken: _cts.Token);
        }
        
        _confirmRequested = false;
    }
}

public static class TMP_Utils
{
    public static int GetVisibleCharacterCount(string richText)
    {
        if (string.IsNullOrEmpty(richText)) return 0;
        
        int count = 0;
        bool inTag = false;
        
        for (int i = 0; i < richText.Length; i++)
        {
            char c = richText[i];
            
            if (c == '<')
            {
                inTag = true;
            }
            else if (c == '>')
            {
                inTag = false;
            }
            else if (!inTag)
            {
                if (char.IsHighSurrogate(c) && i + 1 < richText.Length && char.IsLowSurrogate(richText[i + 1]))
                {
                    count++;
                    i++;
                }
                else
                {
                    count++;
                }
            }
        }
        
        return count;
    }
}