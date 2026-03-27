// File: View/AudioSettingsUI.cs
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class AudioSettingsUI : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;
        

        private AudioManager _audioManager;

        public void Init(AudioManager manager = null)
        {
            _audioManager = manager ?? AudioManager.Instance;
    
            if (_audioManager == null)
            {
                Debug.LogError("[AudioSettingsUI] AudioManager is null in Init()!");
                enabled = false;
                return;
            }
    
            // 🔥 Сразу инициализируем слайдеры, если они уже готовы
            if (masterSlider != null || musicSlider != null || sfxSlider != null)
            {
                InitializeSliders();
                SubscribeToSliders();
            }
        }

        /*private void Start()
        {
            InitializeSliders();
            SubscribeToSliders();
        }*/

        private void InitializeSliders()
        {
            // 🔥 Устанавливаем значения слайдеров из сохранённых настроек
            if (masterSlider != null)
            {
                masterSlider.value = _audioManager.GetMasterVolume();
            }
            
            if (musicSlider != null)
            {
                musicSlider.value = _audioManager.GetMusicVolume();
            }
            
            if (sfxSlider != null)
            {
                sfxSlider.value = _audioManager.GetSFXVolume();
            }
        }

        private void SubscribeToSliders()
        {
            // 🔥 Подписываемся на изменение значений
            if (masterSlider != null)
                masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
            if (musicSlider != null)
                musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
            if (sfxSlider != null)
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // 🔥 Обработчики изменений
        private void OnMasterVolumeChanged(float value)
        {
            _audioManager.SetMasterVolume(value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            _audioManager.SetMusicVolume(value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            _audioManager.SetSFXVolume(value);
        }

        private void UpdateValueText(Text text, float value)
        {
            if (text != null)
                text.text = Mathf.RoundToInt(value * 100) + "%";
        }

        // 🔥 Отписка при уничтожении (важно!)
        private void OnDestroy()
        {
            if (masterSlider != null)
                masterSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            
            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            
            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
        }
    }
}