using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GlobalSpace;
using TMPro;
using UnityEngine;

public class TextController:  MonoBehaviour
{
   [Header("Settings")]
        [SerializeField] private float _defaultCharSpeed = 0.05f;

        private TextMeshProUGUI _currentTextField;
        private string _fullText;
        private int _visibleCharCount;
        private bool _isWriting;
        private bool _skipRequested;
        private bool _confirmRequested;
        
        // Токен для отмены задач при уничтожении объекта
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
            // Обработка ввода только если есть активное поле
            if (_currentTextField == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (_isWriting)
                {
                    // Если текст еще печатается - запрос на пропуск анимации
                    _skipRequested = true;
                }
                else
                {
                    // Если текст полностью напечатан - запрос на подтверждение (следующий текст)
                    _confirmRequested = true;
                }
            }
        }

        /// <summary>
        /// Сбрасывает флаги состояния (вызывать перед началом новой диалогой сцены)
        /// </summary>
        public void Reset()
        {
            _skipRequested = false;
            _confirmRequested = false;
            _isWriting = false;
            _currentTextField = null;
        }

        /// <summary>
        /// Запускает эффект печати текста и ждет, пока пользователь не нажмет кнопку для продолжения.
        /// </summary>
        /// <param name="textField">Компонент текста</param>
        /// <param name="content">Полный текст сообщения</param>
        /// <param name="charSpeed">Скорость печати одного символа</param>
        public async UniTask PlayTextAsync(TextMeshProUGUI textField, string content, float charSpeed = -1f)
        {
            if (textField == null)
            {
                Debug.LogError("[TextController] Text field is null.");
                return;
            }

            // Инициализация состояния
            _currentTextField = textField;
            _fullText = content ?? string.Empty;
            _visibleCharCount = 0;
            _skipRequested = false;
            _confirmRequested = false;
            _isWriting = true;
            
            // Если скорость не задана, используем дефолтную
            float speed = (charSpeed > 0f) ? charSpeed : _defaultCharSpeed;

            // Запускаем процесс печати
            await TypeTextInternal(speed);

            // После завершения печати ждем нажатия для продолжения
            await WaitForConfirm();

            // Очистка после завершения
            _isWriting = false;
            _currentTextField = null;
        }

        /// <summary>
        /// Внутренняя логика посимвольного вывода
        /// </summary>
        private async UniTask TypeTextInternal(float speed)
        {
            _currentTextField.text = string.Empty;
            int totalChars = _fullText.Length;

            while (_visibleCharCount < totalChars)
            {
                // Если запрошен скип - моментально показываем весь текст и выходим
                if (_skipRequested)
                {
                    _currentTextField.text = _fullText;
                    _visibleCharCount = totalChars;
                    _skipRequested = false; // Сбрасываем, чтобы не сработало в WaitForConfirm
                    break;
                }

                // Выводим следующий символ
                _visibleCharCount++;
                _currentTextField.text = _fullText.Substring(0, _visibleCharCount);

                // Ждем перед следующим символом
                await UniTask.Delay(TimeSpan.FromSeconds(speed), cancellationToken: _cts.Token);
            }
        }

        /// <summary>
        /// Ждет нажатия кнопки после завершения печати текста
        /// </summary>
        private async UniTask WaitForConfirm()
        {
            // Ждем, пока пользователь не нажмет кнопку (флаг _confirmRequested)
            await UniTask.WaitUntil(() => _confirmRequested, cancellationToken: _cts.Token);
            
            // Сбрасываем флаг подтверждения для следующего раза
            _confirmRequested = false;
        }
    }
