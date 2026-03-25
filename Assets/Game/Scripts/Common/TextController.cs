using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using TMPro;
using UnityEngine;

public class TextController: MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _defaultCharSpeed = 0.05f;
    [SerializeField] private float _confirmCooldown = 0.1f; // ← НОВОЕ: задержка после подтверждения

    private TextMeshProUGUI _currentTextField;
    private string _fullText;
    private int _visibleCharCount;
    private bool _isWriting;
    private bool _skipRequested;
    private bool _confirmRequested;
    private float _lastConfirmTime; // ← НОВОЕ: время последнего подтверждения
    
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

        // ← НОВОЕ: игнорируем ввод, если недавно было подтверждение
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
        _lastConfirmTime = 0f; // ← НОВОЕ: сбрасываем кулдаун
        _currentTextField = null;
    }

    public async UniTask PlayTextAsync(TextMeshProUGUI textField, string content, float charSpeed = -1f)
    {
        if (textField == null)
        {
            Debug.LogError("[TextController] Text field is null.");
            return;
        }

        // ← НОВОЕ: сбрасываем ввод перед началом нового текста
        _skipRequested = false;
        _confirmRequested = false;
        _lastConfirmTime = 0f;
        
        _currentTextField = textField;
        _fullText = content ?? string.Empty;
        _visibleCharCount = 0;
        _isWriting = true;
        
        float speed = (charSpeed > 0f) ? charSpeed : _defaultCharSpeed;

        await TypeTextInternal(speed);

        _isWriting = false;
        await WaitForConfirm();

        // ← НОВОЕ: записываем время подтверждения и добавляем микро-задержку
        _lastConfirmTime = Time.time;
        _confirmRequested = false;
        _currentTextField = null;
        
        // Микро-пауза, чтобы «остыл» ввод
        await UniTask.Delay(TimeSpan.FromSeconds(_confirmCooldown), cancellationToken: _cts.Token);
    }

    private async UniTask TypeTextInternal(float speed)
    {
        _currentTextField.text = string.Empty;
        int totalChars = _fullText.Length;

        while (_visibleCharCount < totalChars)
        {
            if (_skipRequested)
            {
                _currentTextField.text = _fullText;
                _visibleCharCount = totalChars;
                _skipRequested = false;
                break;
            }

            _visibleCharCount++;
            _currentTextField.text = _fullText.Substring(0, _visibleCharCount);

            await UniTask.Delay(TimeSpan.FromSeconds(speed), cancellationToken: _cts.Token);
        }
    }

    private async UniTask WaitForConfirm()
    {
        // ← НОВОЕ: ждём подтверждения, но с защитой от «залипания»
        while (!_confirmRequested)
        {
            // Если поле стало null (например, сцена сменилась) — выходим
            if (_currentTextField == null)
                return;
                
            await UniTask.NextFrame(cancellationToken: _cts.Token);
        }
        
        _confirmRequested = false;
    }
}