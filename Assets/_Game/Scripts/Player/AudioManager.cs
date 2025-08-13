using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using DG.Tweening;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    /// <summary>
    /// 音频管理器 - 管理所有音效和音乐
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("音频混合器")]
        public AudioMixer mainMixer;

        [Header("音源")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambientSource;

        [Header("音量设置")]
        [Range(0, 1)] public float masterVolume = 1f;
        [Range(0, 1)] public float musicVolume = 0.7f;
        [Range(0, 1)] public float sfxVolume = 1f;
        [Range(0, 1)] public float ambientVolume = 0.5f;

        [Header("音乐层级")]
        public AudioClip[] musicLayers; // 4层音乐
        private int currentMusicLayer = 0;

        [Header("音效池")]
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
        private int poolSize = 10;

        protected override void Awake()
        {
            base.Awake();
            InitializeAudioPool();
            LoadVolumeSettings();
            GameDebug.Log("AudioManager Initialized");
        }

        /// <summary>
        /// 初始化音效池
        /// </summary>
        void InitializeAudioPool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject sfxObject = new GameObject($"SFX_Source_{i}");
                sfxObject.transform.parent = transform;
                AudioSource source = sfxObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Enqueue(source);
            }
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            if (sfxPool.Count > 0)
            {
                AudioSource source = sfxPool.Dequeue();
                source.clip = clip;
                source.volume = volume * sfxVolume;
                source.Play();

                // 播放完毕后返回池中
                DOVirtual.DelayedCall(clip.length, () =>
                {
                    sfxPool.Enqueue(source);
                });
            }
        }

        /// <summary>
        /// 播放3D音效
        /// </summary>
        public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = position;
            AudioSource source = tempAudio.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume * sfxVolume;
            source.spatialBlend = 1f; // 3D音效
            source.maxDistance = 20f;
            source.Play();

            Destroy(tempAudio, clip.length);
        }

        /// <summary>
        /// 切换音乐层级
        /// </summary>
        public void SwitchMusicLayer(int layer, float fadeTime = 1f)
        {
            if (layer < 0 || layer >= musicLayers.Length) return;
            if (musicLayers[layer] == null) return;

            currentMusicLayer = layer;

            // 淡出当前音乐
            musicSource.DOFade(0, fadeTime * 0.5f).OnComplete(() =>
            {
                musicSource.clip = musicLayers[layer];
                musicSource.Play();
                musicSource.DOFade(musicVolume, fadeTime * 0.5f);
            });

            GameDebug.Log($"Switched to music layer {layer}");
        }

        /// <summary>
        /// 播放环境音
        /// </summary>
        public void PlayAmbient(AudioClip clip, bool loop = true)
        {
            if (clip == null) return;

            ambientSource.clip = clip;
            ambientSource.loop = loop;
            ambientSource.volume = ambientVolume;
            ambientSource.Play();
        }

        /// <summary>
        /// 设置主音量
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// 保存音量设置
        /// </summary>
        void SaveVolumeSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 加载音量设置
        /// </summary>
        void LoadVolumeSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);

            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }
    }
}