using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RadioManager : MonoBehaviour
{
    public static RadioManager Instance { get; private set; }

    [Header("=== Playlist ===")]
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private bool shufflePlaylist = false;
    
    [Header("=== Timing ===")]
    [SerializeField] private float delayBetweenTracks = 3f;
    [SerializeField] private float fadeDuration = 1f;
    
    [Header("=== Audio Settings ===")]
    [SerializeField] private bool useAudioManager = true;  // 🔥 Интеграция с AudioManager
    [SerializeField] private AudioMixerGroup fallbackOutputGroup;  // 🔥 Резервный выход, если нет AudioManager
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.8f;
    
    [Header("=== Events ===")]
    [SerializeField] private UnityEngine.Events.UnityEvent onTrackStart;
    [SerializeField] private UnityEngine.Events.UnityEvent onTrackEnd;

    private AudioSource _radioSource;
    private int _currentTrackIndex = -1;
    private bool _isPlaying = false;
    private bool _isPaused = false;
    private Coroutine _playbackCoroutine;
    private List<int> _shuffledIndices;
    
    // 🔥 Ссылка на AudioManager (не обязательная)
    private AudioManager _audioManager;

    // 🔥 События для внешней подписки
    public event Action<int, AudioClip> OnTrackChanged;
    public event Action OnRadioStarted;
    public event Action OnRadioStopped;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        
        // 🔥 Попытка найти AudioManager
        if (useAudioManager)
        {
            _audioManager = FindObjectOfType<AudioManager>();
            if (_audioManager == null)
                Debug.LogWarning("📻 RadioManager: AudioManager not found! Using fallback settings.");
        }
        
        InitializeRadio();
    }

    private void InitializeRadio()
    {
        _radioSource = GetComponent<AudioSource>();
        if (_radioSource == null)
            _radioSource = gameObject.AddComponent<AudioSource>();
        
        // 🔥 Настройка выхода: приоритет AudioManager, затем fallback, затем ничего
        if (useAudioManager && _audioManager != null)
        {
            // Используем тот же выход, что и музыка в AudioManager
            // (предполагаем, что у него есть публичный доступ или мы знаем название группы)
            _radioSource.outputAudioMixerGroup = GetMusicOutputGroup();
        }
        else if (fallbackOutputGroup != null)
        {
            _radioSource.outputAudioMixerGroup = fallbackOutputGroup;
        }
        
        _radioSource.playOnAwake = false;
        _radioSource.loop = false;
        _radioSource.volume = 0f;
    }

    // 🔥 Получение AudioMixerGroup из AudioManager (через рефлексию или публичный метод)
    private AudioMixerGroup GetMusicOutputGroup()
    {
        if (_audioManager == null) return null;
        
        // Вариант 1: Если AudioManager имеет публичное свойство/метод
        // return _audioManager.MusicOutputGroup;
        
        // Вариант 2: Через рефлексию (если поле приватное)
        var field = typeof(AudioManager).GetField("_musicOutput", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
            return field.GetValue(_audioManager) as AudioMixerGroup;
        
        // Вариант 3: Fallback
        return fallbackOutputGroup;
    }

    // 🔥 Получение текущей громкости музыки (с учётом AudioManager)
    private float GetCurrentMusicVolume()
    {
        if (useAudioManager && _audioManager != null)
        {
            // AudioManager сам вернёт значение из PlayerPrefs
            return _audioManager.GetMusicVolume();
        }
        return defaultVolume;
    }

    // ======================
    // === УПРАВЛЕНИЕ =======
    // ======================

    public void StartRadio()
    {
        if (playlist == null || playlist.Length == 0)
        {
            Debug.LogWarning("RadioManager: Плейлист пуст!");
            return;
        }

        // 🔥 Проверка: если AudioManager заглушен — не запускаем радио
        if (useAudioManager && _audioManager != null)
        {
            // Опционально: можно проверять мастер-громкость
            // if (_audioManager.GetMasterVolume() < 0.01f) return;
        }

        if (_isPlaying) return;
        
        _isPlaying = true;
        _isPaused = false;
        _currentTrackIndex = -1;
        
        if (shufflePlaylist)
        {
            _shuffledIndices = new List<int>();
            for (int i = 0; i < playlist.Length; i++)
                _shuffledIndices.Add(i);
            
            for (int i = _shuffledIndices.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                int temp = _shuffledIndices[i];
                _shuffledIndices[i] = _shuffledIndices[j];
                _shuffledIndices[j] = temp;
            }
        }
        
        OnRadioStarted?.Invoke();
        _playbackCoroutine = StartCoroutine(PlaybackLoop());
    }

    public void StopRadio()
    {
        if (!_isPlaying) return;
        
        _isPlaying = false;
        _isPaused = false;
        
        if (_playbackCoroutine != null)
            StopCoroutine(_playbackCoroutine);
        
        StartCoroutine(FadeOutAndStop());
        OnRadioStopped?.Invoke();
    }

    public void TogglePause()
    {
        if (!_isPlaying) return;
        
        _isPaused = !_isPaused;
        
        if (_isPaused)
        {
            _radioSource.Pause();
        }
        else
        {
            _radioSource.UnPause();
            if (_playbackCoroutine == null && _isPlaying)
                _playbackCoroutine = StartCoroutine(PlaybackLoop());
        }
    }

    public void SkipToNext()
    {
        if (!_isPlaying || _isPaused) return;
        
        if (_playbackCoroutine != null)
            StopCoroutine(_playbackCoroutine);
        
        _playbackCoroutine = StartCoroutine(PlaybackLoop());
    }

    public void PlayTrack(int index)
    {
        if (index < 0 || index >= playlist.Length) return;
        _currentTrackIndex = index - 1;
        SkipToNext();
    }

    // ======================
    // === ОСНОВНОЙ ЦИКЛ ====
    // ======================

    private IEnumerator PlaybackLoop()
    {
        while (_isPlaying && !_isPaused)
        {
            // 🔥 Проверка глобальной паузы AudioManager
            if (useAudioManager && _audioManager != null)
            {
                // Если нужно, можно реагировать на паузу аудиоменеджера
                // yield return new WaitUntil(() => !_audioManagerIsPaused);
            }

            int nextIndex = GetNextTrackIndex();
            if (nextIndex < 0) break;
            
            AudioClip nextTrack = playlist[nextIndex];
            if (nextTrack == null) continue;
            
            yield return PlayTrackWithFade(nextTrack, nextIndex);
            onTrackEnd?.Invoke();
            
            if (_isPlaying && !_isPaused && delayBetweenTracks > 0)
            {
                yield return new WaitForSeconds(delayBetweenTracks);
            }
        }
    }

    private int GetNextTrackIndex()
    {
        if (playlist == null || playlist.Length == 0) return -1;
        
        if (shufflePlaylist && _shuffledIndices != null)
        {
            _currentTrackIndex = (_currentTrackIndex + 1) % _shuffledIndices.Count;
            return _shuffledIndices[_currentTrackIndex];
        }
        else
        {
            _currentTrackIndex = (_currentTrackIndex + 1) % playlist.Length;
            return _currentTrackIndex;
        }
    }

    private IEnumerator PlayTrackWithFade(AudioClip clip, int trackIndex)
    {
        _radioSource.clip = clip;
        _radioSource.volume = 0f;
        _radioSource.Play();
        
        // 🔥 Фейд-ин с учётом громкости из AudioManager
        float targetVolume = GetCurrentMusicVolume() * defaultVolume;
        yield return FadeVolume(0f, targetVolume, fadeDuration);
        
        _currentTrackIndex = trackIndex;
        onTrackStart?.Invoke();
        OnTrackChanged?.Invoke(trackIndex, clip);
        Debug.Log($"🎵 Radio: Playing '{clip.name}' ({trackIndex + 1}/{playlist.Length})");
        
        while (_radioSource.isPlaying && _isPlaying)
        {
            // 🔥 Динамическая подстройка громкости (если игрок меняет настройки)
            if (useAudioManager && _audioManager != null)
            {
                float currentVol = GetCurrentMusicVolume() * defaultVolume;
                if (Mathf.Abs(_radioSource.volume - currentVol) > 0.01f)
                {
                    _radioSource.volume = currentVol;
                }
            }
            
            if (_isPaused)
            {
                yield return new WaitUntil(() => !_isPaused || !_isPlaying);
                if (!_isPlaying) yield break;
            }
            yield return null;
        }
        
        yield return FadeVolume(_radioSource.volume, 0f, fadeDuration);
        _radioSource.Stop();
    }

    private IEnumerator FadeOutAndStop()
    {
        yield return FadeVolume(_radioSource.volume, 0f, fadeDuration);
        _radioSource.Stop();
    }

    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            _radioSource.volume = to;
            yield break;
        }
        
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            t = Mathf.SmoothStep(0f, 1f, t);
            _radioSource.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }
        _radioSource.volume = to;
    }

    // ======================
    // === УТИЛИТЫ ==========
    // ======================

    /// <summary>Установить громкость радио (множитель к громкости музыки)</summary>
    public void SetVolume(float volume)
    {
        defaultVolume = Mathf.Clamp01(volume);
        
        // Если радио играет — сразу применяем
        if (_isPlaying && !_isPaused)
        {
            float baseVol = GetCurrentMusicVolume();
            _radioSource.volume = baseVol * defaultVolume;
        }
    }

    /// <summary>Принудительно обновить громкость из AudioManager</summary>
    public void RefreshVolumeFromAudioManager()
    {
        if (_isPlaying && !_isPaused && useAudioManager && _audioManager != null)
        {
            float targetVol = GetCurrentMusicVolume() * defaultVolume;
            _radioSource.volume = targetVol;
        }
    }

    public (int index, AudioClip clip, bool isPlaying) GetCurrentTrackInfo()
    {
        if (_currentTrackIndex < 0 || _currentTrackIndex >= playlist.Length)
            return (-1, null, _isPlaying && !_isPaused);
        return (_currentTrackIndex, playlist[_currentTrackIndex], _isPlaying && !_isPaused);
    }

    public void AddToPlaylist(AudioClip clip)
    {
        if (clip == null) return;
        var newList = new AudioClip[playlist.Length + 1];
        playlist.CopyTo(newList, 0);
        newList[newList.Length - 1] = clip;
        playlist = newList;
    }

    public void ClearPlaylist()
    {
        playlist = Array.Empty<AudioClip>();
        if (_isPlaying) StopRadio();
    }

    // ======================
    // === ОТЛАДКА ==========
    // ======================

    private void OnValidate()
    {
        fadeDuration = Mathf.Max(0f, fadeDuration);
        delayBetweenTracks = Mathf.Max(0f, delayBetweenTracks);
    }

#if UNITY_EDITOR
    [ContextMenu("Print Playlist")]
    private void PrintPlaylist()
    {
        if (playlist == null || playlist.Length == 0)
        {
            Debug.Log("📻 Radio: Playlist is empty");
            return;
        }
        
        Debug.Log($"📻 Radio Playlist ({playlist.Length} tracks):");
        for (int i = 0; i < playlist.Length; i++)
        {
            string marker = (i == _currentTrackIndex && _isPlaying) ? " ▶" : "";
            Debug.Log($"  {i + 1}. {playlist[i]?.name ?? "NULL"}{marker}");
        }
        
        if (useAudioManager && _audioManager != null)
            Debug.Log($"🔗 Integrated with AudioManager | Volume: {GetCurrentMusicVolume():F2}");
    }
#endif

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}