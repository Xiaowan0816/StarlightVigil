using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using DG.Tweening;
using StarlightVigil;

namespace StarlightVigil.Managers
{
    /// <summary>
    /// ��Ƶ������ - ����������Ч������
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("��Ƶ�����")]
        public AudioMixer mainMixer;

        [Header("��Դ")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource ambientSource;

        [Header("��������")]
        [Range(0, 1)] public float masterVolume = 1f;
        [Range(0, 1)] public float musicVolume = 0.7f;
        [Range(0, 1)] public float sfxVolume = 1f;
        [Range(0, 1)] public float ambientVolume = 0.5f;

        [Header("���ֲ㼶")]
        public AudioClip[] musicLayers; // 4������
        private int currentMusicLayer = 0;

        [Header("��Ч��")]
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
        /// ��ʼ����Ч��
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
        /// ������Ч
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

                // ������Ϻ󷵻س���
                DOVirtual.DelayedCall(clip.length, () =>
                {
                    sfxPool.Enqueue(source);
                });
            }
        }

        /// <summary>
        /// ����3D��Ч
        /// </summary>
        public void PlaySFX3D(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            GameObject tempAudio = new GameObject("TempAudio");
            tempAudio.transform.position = position;
            AudioSource source = tempAudio.AddComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume * sfxVolume;
            source.spatialBlend = 1f; // 3D��Ч
            source.maxDistance = 20f;
            source.Play();

            Destroy(tempAudio, clip.length);
        }

        /// <summary>
        /// �л����ֲ㼶
        /// </summary>
        public void SwitchMusicLayer(int layer, float fadeTime = 1f)
        {
            if (layer < 0 || layer >= musicLayers.Length) return;
            if (musicLayers[layer] == null) return;

            currentMusicLayer = layer;

            // ������ǰ����
            musicSource.DOFade(0, fadeTime * 0.5f).OnComplete(() =>
            {
                musicSource.clip = musicLayers[layer];
                musicSource.Play();
                musicSource.DOFade(musicVolume, fadeTime * 0.5f);
            });

            GameDebug.Log($"Switched to music layer {layer}");
        }

        /// <summary>
        /// ���Ż�����
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
        /// ����������
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// ������������
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// ������Ч����
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            mainMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            SaveVolumeSettings();
        }

        /// <summary>
        /// ������������
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
        /// ������������
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