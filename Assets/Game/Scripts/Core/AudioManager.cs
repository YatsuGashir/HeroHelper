using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundType
{
    PlacePart,
    BrainGame,
    CorrectPart,
    WrongPart
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    // 🔑 Ключи для PlayerPrefs
    private const string KEY_MASTER_VOLUME = "Audio_MasterVol";
    private const string KEY_MUSIC_VOLUME = "Audio_MusicVol";
    private const string KEY_SFX_VOLUME = "Audio_SFXVol";
    
    // Значения по умолчанию
    private const float DEFAULT_VOLUME = 1f;
    private const float DEFAULT_MUSIC_VOLUME = 1f;

    [Header("=== Audio Mixer Groups ===")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicOutput;
    [SerializeField] private AudioMixerGroup sfxOutput;

    [Header("=== Music Settings ===")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float defaultMusicVolume = DEFAULT_MUSIC_VOLUME;
    [SerializeField] private float musicFadeDuration = 1f;

    [Header("=== SFX Settings ===")]
    [SerializeField] private int maxSimultaneousSFX = 10;

    [Header("=== Sound Library ===")]
    [SerializeField] private SoundList[] soundList;

    private Dictionary<string, SoundList> soundDictionary;
    private Coroutine currentFadeRoutine;
    private bool isPaused = false;
    
    private Coroutine _currentDuckRoutine;
    private float _preDuckMusicVolume = -1f; // Запоминаем громкость до приглушения

    // Ключи параметров микшера (должны совпадать с Exposed Parameters в Audio Mixer)
    private const string MASTER_VOLUME_PARAM = "MasterVol";
    private const string MUSIC_VOLUME_PARAM = "MusicVol";
    private const string SFX_VOLUME_PARAM = "SFXVol";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeSoundDictionary();
        InitializeAudioSources();
        
        // 🔥 Загружаем настройки из PlayerPrefs при старте
        LoadVolumeSettings();
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, SoundList>();
        foreach (var sound in soundList)
        {
            if (!string.IsNullOrEmpty(sound.name))
            {
                soundDictionary[sound.name] = sound;
            }
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        _activeMusicSource = musicSource;
        _secondaryMusicSource = gameObject.AddComponent<AudioSource>();

        _activeMusicSource.outputAudioMixerGroup = musicOutput;
        _secondaryMusicSource.outputAudioMixerGroup = musicOutput;

        _activeMusicSource.loop = true;
        _secondaryMusicSource.loop = true;

        _activeMusicSource.playOnAwake = false;
        _secondaryMusicSource.playOnAwake = false;
    }

// ======================
// ====== МУЗЫКА =======
// ======================

    private AudioSource _activeMusicSource;
    private AudioSource _secondaryMusicSource;

    public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = -1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Пустой AudioClip!");
            return;
        }

        if (fadeDuration < 0)
            fadeDuration = musicFadeDuration;

        StartCoroutine(CrossfadeMusic(clip, loop, fadeDuration));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip, bool loop, float duration)
    {
        AudioSource oldSource = _activeMusicSource;
        AudioSource newSource = (_activeMusicSource == musicSource) ? _secondaryMusicSource : musicSource;

        newSource.clip = newClip;
        newSource.loop = loop;
        newSource.volume = 0f;
        newSource.Play();

        float timer = 0f;
        float targetVol = GetMusicVolume();

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;

            oldSource.volume = Mathf.Lerp(targetVol, 0f, t);
            newSource.volume = Mathf.Lerp(0f, targetVol, t);

            yield return null;
        }

        oldSource.Stop();
        newSource.volume = targetVol;

        _activeMusicSource = newSource;
    }

// ======================
// ======= SFX =========
// ======================

    public void PlaySFX(SoundType type, float volumeScale = 1f)
    {
        PlaySFX(type.ToString(), volumeScale);
    }

    public void PlaySFX(string soundName, float volumeScale = 1f)
    {
        if (!soundDictionary.TryGetValue(soundName, out SoundList sound))
        {
            Debug.LogWarning($"Sound '{soundName}' not found in AudioManager!");
            return;
        }

        if (sound.Sounds == null || sound.Sounds.Length == 0)
        {
            Debug.LogWarning($"Sound '{soundName}' has no clips assigned!");
            return;
        }

        // Ограничение количества одновременных SFX
        int activeSources = 0;
        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (source != musicSource && source.isPlaying && source.outputAudioMixerGroup == sfxOutput)
                activeSources++;
        }

        if (activeSources >= maxSimultaneousSFX)
        {
            return;
        }

        AudioClip clip = sound.Sounds[UnityEngine.Random.Range(0, sound.Sounds.Length)];
        AudioSource sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = clip;
        sfxSource.outputAudioMixerGroup = sfxOutput;
        sfxSource.volume = volumeScale;
        sfxSource.Play();

        Destroy(sfxSource, clip.length);
    }

// ======================
// ===== ГРОМКОСТЬ =====
// ======================

    private float VolumeToDecibel(float linearVolume)
    {
        if (linearVolume < 0.0001f) 
            return -80f;
        return Mathf.Log10(linearVolume) * 20f;
    }

    // 🔹 Master Volume
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float dB = VolumeToDecibel(volume);
        masterMixer.SetFloat(MASTER_VOLUME_PARAM, dB);
        
        // 🔥 Сохраняем в PlayerPrefs
        PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, DEFAULT_VOLUME);
    }

    // 🔹 Music Volume
    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float dB = VolumeToDecibel(volume);
        masterMixer.SetFloat(MUSIC_VOLUME_PARAM, dB);
        
        // 🔥 Сохраняем в PlayerPrefs
        PlayerPrefs.SetFloat(KEY_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, defaultMusicVolume);
    }

    // 🔹 SFX Volume
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        float dB = VolumeToDecibel(volume);
        masterMixer.SetFloat(SFX_VOLUME_PARAM, dB);
        
        // 🔥 Сохраняем в PlayerPrefs
        PlayerPrefs.SetFloat(KEY_SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(KEY_SFX_VOLUME, DEFAULT_VOLUME);
    }

    // 🔥 Загрузка всех настроек при старте
    private void LoadVolumeSettings()
    {
        // Получаем значения из PlayerPrefs или используем дефолтные
        float masterVol = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, DEFAULT_VOLUME);
        float musicVol = PlayerPrefs.GetFloat(KEY_MUSIC_VOLUME, defaultMusicVolume);
        float sfxVol = PlayerPrefs.GetFloat(KEY_SFX_VOLUME, DEFAULT_VOLUME);

        // Применяем к микшеру (без повторного сохранения!)
        masterMixer.SetFloat(MASTER_VOLUME_PARAM, VolumeToDecibel(masterVol));
        masterMixer.SetFloat(MUSIC_VOLUME_PARAM, VolumeToDecibel(musicVol));
        masterMixer.SetFloat(SFX_VOLUME_PARAM, VolumeToDecibel(sfxVol));
    }

    // 🔥 Удаление всех сохранённых настроек (для сброса или тестов)
    public void ResetVolumeSettings()
    {
        PlayerPrefs.DeleteKey(KEY_MASTER_VOLUME);
        PlayerPrefs.DeleteKey(KEY_MUSIC_VOLUME);
        PlayerPrefs.DeleteKey(KEY_SFX_VOLUME);
        PlayerPrefs.Save();
        
        // Применяем дефолтные значения
        LoadVolumeSettings();
    }

// ======================
// ==== СОСТОЯНИЯ ======
// ======================

    public void PauseAudio()
    {
        if (isPaused) return;
        isPaused = true;
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void UnpauseAudio()
    {
        if (!isPaused) return;
        isPaused = false;
        if (!musicSource.isPlaying && musicSource.clip != null)
        {
            musicSource.UnPause();
        }
    }

    public void MuteAll(bool mute)
    {
        float vol = mute ? 0f : 1f;
        SetMasterVolume(vol);
    }
    
        /// <summary>
    /// Плавно приглушает музыку на указанное время, затем возвращает громкость.
    /// </summary>
    /// <param name="duckAmount">Насколько приглушить (0...1). 0.3 = 30% от текущей громкости.</param>
    /// <param name="duration">Сколько секунд держать приглушённой.</param>
    /// <param name="fadeTime">Время плавного перехода (в обе стороны).</param>
    public void DuckMusic(float duckAmount, float duration, float fadeTime = 0.3f)
    {
        // Если уже идёт приглушение — перезапускаем таймер (опционально)
        if (_currentDuckRoutine != null)
            StopCoroutine(_currentDuckRoutine);
        
        _currentDuckRoutine = StartCoroutine(DuckMusicRoutine(duckAmount, duration, fadeTime));
    }

    private IEnumerator DuckMusicRoutine(float duckAmount, float duration, float fadeTime)
    {
        duckAmount = Mathf.Clamp01(duckAmount);
        fadeTime = Mathf.Max(0f, fadeTime);
        
        // Запоминаем текущую целевую громкость музыки (из PlayerPrefs)
        float targetVolume = GetMusicVolume();
        _preDuckMusicVolume = targetVolume;
        
        // === FADE OUT: приглушаем ===
        float timer = 0f;
        float startVol = _activeMusicSource.volume;
        float endVol = targetVolume * duckAmount;
        
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeTime;
            _activeMusicSource.volume = Mathf.Lerp(startVol, endVol, t);
            _secondaryMusicSource.volume = Mathf.Lerp(startVol, endVol, t);
            yield return null;
        }
        
        // === HOLD: держим приглушённой ===
        yield return new WaitForSecondsRealtime(duration);
        
        // === FADE IN: возвращаем громкость ===
        timer = 0f;
        startVol = _activeMusicSource.volume;
        endVol = _preDuckMusicVolume;
        
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeTime;
            _activeMusicSource.volume = Mathf.Lerp(startVol, endVol, t);
            _secondaryMusicSource.volume = Mathf.Lerp(startVol, endVol, t);
            yield return null;
        }
        
        // Фиксируем финальную громкость
        _activeMusicSource.volume = endVol;
        _secondaryMusicSource.volume = endVol;
        _preDuckMusicVolume = -1f;
        _currentDuckRoutine = null;
    }

    /// <summary>
    /// Экстренно отменяет приглушение и возвращает музыку к нормальной громкости.
    /// </summary>
    public void StopDuckingMusic(float fadeTime = 0.3f)
    {
        if (_currentDuckRoutine != null)
            StopCoroutine(_currentDuckRoutine);
        
        if (_preDuckMusicVolume >= 0f)
        {
            StartCoroutine(RestoreMusicVolumeRoutine(_preDuckMusicVolume, fadeTime));
            _preDuckMusicVolume = -1f;
            _currentDuckRoutine = null;
        }
    }

    private IEnumerator RestoreMusicVolumeRoutine(float targetVolume, float fadeTime)
    {
        float timer = 0f;
        float startVol = _activeMusicSource.volume;
        
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeTime;
            _activeMusicSource.volume = Mathf.Lerp(startVol, targetVolume, t);
            _secondaryMusicSource.volume = Mathf.Lerp(startVol, targetVolume, t);
            yield return null;
        }
        
        _activeMusicSource.volume = targetVolume;
        _secondaryMusicSource.volume = targetVolume;
    }

// ======================
// ==== ОТЛАДКА ========
// ======================

#if UNITY_EDITOR
    private void OnEnable()
    {
        // Авто-заполнение массива звуков по enum (только в редакторе)
        string[] names = Enum.GetNames(typeof(SoundType));
        if (soundList == null || soundList.Length != names.Length)
        {
            Array.Resize(ref soundList, names.Length);
            for (int i = 0; i < soundList.Length; i++)
            {
                if (string.IsNullOrEmpty(soundList[i].name))
                    soundList[i].name = names[i];
            }
        }
    }

    // 🔥 Отладочная кнопка в Inspector
    [ContextMenu("Print Current Volume Settings")]
    private void PrintVolumeSettings()
    {
        Debug.Log($"=== Audio Settings ===");
        Debug.Log($"Master: {GetMasterVolume():F2}");
        Debug.Log($"Music: {GetMusicVolume():F2}");
        Debug.Log($"SFX: {GetSFXVolume():F2}");
    }
#endif

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}

[Serializable]
public struct SoundList
{
    public string name;
    public AudioClip[] Sounds;
}