using UnityEngine;

namespace TapOrDie.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameMusic;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip jumpSound;
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip gameOverSound;
        [SerializeField] private AudioClip buttonSound;
        [SerializeField] private AudioClip collectSound;
        [SerializeField] private AudioClip speedUpSound;

        [Header("Settings")]
        [SerializeField] private float musicVolume = 0.5f;
        [SerializeField] private float sfxVolume = 1f;

        public bool IsMusicEnabled { get; private set; } = true;
        public bool IsSfxEnabled { get; private set; } = true;

        private const string MUSIC_ENABLED_KEY = "TapOrDie_MusicEnabled";
        private const string SFX_ENABLED_KEY = "TapOrDie_SfxEnabled";
        private const string MUSIC_VOLUME_KEY = "TapOrDie_MusicVolume";
        private const string SFX_VOLUME_KEY = "TapOrDie_SfxVolume";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSettings();
                SetupAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Play menu music by default if no specific music is playing
            if (menuMusic != null && !musicSource.isPlaying)
            {
                PlayMusic(menuMusic);
            }
        }

        private void SetupAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SfxSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            ApplyVolumeSettings();
        }

        private void LoadSettings()
        {
            IsMusicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
            IsSfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
            musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.5f);
            sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, IsMusicEnabled ? 1 : 0);
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, IsSfxEnabled ? 1 : 0);
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
            PlayerPrefs.Save();
        }

        private void ApplyVolumeSettings()
        {
            if (musicSource != null)
            {
                musicSource.volume = IsMusicEnabled ? musicVolume : 0f;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = IsSfxEnabled ? sfxVolume : 0f;
            }
        }

        #region Music Control

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource == null) return;

            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.volume = IsMusicEnabled ? musicVolume : 0f;
            musicSource.Play();
        }

        public void PlayMenuMusic()
        {
            PlayMusic(menuMusic);
        }

        public void PlayGameMusic()
        {
            PlayMusic(gameMusic);
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (musicSource != null && !musicSource.isPlaying)
            {
                musicSource.UnPause();
            }
        }

        public void ToggleMusic()
        {
            IsMusicEnabled = !IsMusicEnabled;
            ApplyVolumeSettings();
            SaveSettings();
        }

        public void SetMusicEnabled(bool enabled)
        {
            IsMusicEnabled = enabled;
            ApplyVolumeSettings();
            SaveSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            SaveSettings();
        }

        #endregion

        #region SFX Control

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || sfxSource == null || !IsSfxEnabled) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlayJumpSound()
        {
            PlaySfx(jumpSound);
        }

        public void PlayHitSound()
        {
            PlaySfx(hitSound);
        }

        public void PlayGameOverSound()
        {
            PlaySfx(gameOverSound);
        }

        public void PlayButtonSound()
        {
            PlaySfx(buttonSound);
        }

        public void PlayCollectSound()
        {
            PlaySfx(collectSound);
        }

        public void PlaySpeedUpSound()
        {
            PlaySfx(speedUpSound);
        }

        public void ToggleSfx()
        {
            IsSfxEnabled = !IsSfxEnabled;
            ApplyVolumeSettings();
            SaveSettings();
        }

        public void SetSfxEnabled(bool enabled)
        {
            IsSfxEnabled = enabled;
            ApplyVolumeSettings();
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumeSettings();
            SaveSettings();
        }

        #endregion

        #region Utility

        public void MuteAll()
        {
            if (musicSource != null) musicSource.mute = true;
            if (sfxSource != null) sfxSource.mute = true;
        }

        public void UnmuteAll()
        {
            if (musicSource != null) musicSource.mute = false;
            if (sfxSource != null) sfxSource.mute = false;
        }

        // Called when showing ads - pause audio
        public void OnAdStart()
        {
            MuteAll();
        }

        // Called when ad closes - resume audio
        public void OnAdEnd()
        {
            UnmuteAll();
        }

        #endregion
    }
}
