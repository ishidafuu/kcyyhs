using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    public class SoundManager : SingletonMonoBehaviourDontDestroy<SoundManager>
    {
        [SerializeField] AudioSource m_bgmAudioSource;
        [SerializeField] AudioSource m_seAudioSource;

        [SerializeField, Range(0, 1), Tooltip("マスタ音量")]
        float m_volume = 1;
        [SerializeField, Range(0, 1), Tooltip("BGMの音量")]
        float m_bgmVolume = 1;
        [SerializeField, Range(0, 1), Tooltip("SEの音量")]
        float m_seVolume = 1;

        AudioClip[] m_bgmArray;
        AudioClip[] m_seArray;

        Dictionary<string, int> m_bgmIndexDict = new Dictionary<string, int>();
        Dictionary<string, int> m_seIndexDict = new Dictionary<string, int>();

        readonly string BGMPath = "Audio/BGM";
        readonly string SEPath = "Audio/SE";

        override protected void Awake()
        {
            base.Awake();

            m_bgmArray = Resources.LoadAll<AudioClip>(BGMPath);
            m_seArray = Resources.LoadAll<AudioClip>(SEPath);

            for (int i = 0; i < m_bgmArray.Length; i++)
            {
                m_bgmIndexDict.Add(m_bgmArray[i].name, i);
            }

            for (int i = 0; i < m_seArray.Length; i++)
            {
                m_seIndexDict.Add(m_seArray[i].name, i);
            }
        }

        public int GetBgmIndex(string name)
        {
            if (m_bgmIndexDict.ContainsKey(name))
            {
                return m_bgmIndexDict[name];
            }
            else
            {
                Debug.LogError("指定された名前のBGMファイルが存在しません。");
                return 0;
            }
        }

        public int GetSeIndex(string name)
        {
            if (m_seIndexDict.ContainsKey(name))
            {
                return m_seIndexDict[name];
            }
            else
            {
                Debug.LogError("指定された名前のSEファイルが存在しません。");
                return 0;
            }
        }

        //BGM再生
        public void PlayBgm(int index)
        {
            index = Mathf.Clamp(index, 0, m_bgmArray.Length);

            m_bgmAudioSource.clip = m_bgmArray[index];
            m_bgmAudioSource.loop = true;
            m_bgmAudioSource.volume = m_bgmVolume * m_volume;
            m_bgmAudioSource.Play();
        }

        public void PlayBgmByName(string name)
        {
            PlayBgm(GetBgmIndex(name));
        }

        public void StopBgm()
        {
            m_bgmAudioSource.Stop();
            m_bgmAudioSource.clip = null;
        }

        //SE再生
        public void PlaySe(int index)
        {
            index = Mathf.Clamp(index, 0, m_seArray.Length);

            m_seAudioSource.PlayOneShot(m_seArray[index], m_seVolume * m_volume);
        }

        public void PlaySeByName(string name)
        {
            PlaySe(GetSeIndex(name));
        }

        public void StopSe()
        {
            m_seAudioSource.Stop();
            m_seAudioSource.clip = null;
        }

        public void SetVolume(float value)
        {
            m_volume = Mathf.Clamp01(value);
            m_bgmAudioSource.volume = m_bgmVolume * m_volume;
            m_seAudioSource.volume = m_seVolume * m_volume;
        }
        public float GetVolume() => m_volume;

        public void SetBgmVolume(float value)
        {
            m_bgmVolume = Mathf.Clamp01(value);
            m_bgmAudioSource.volume = m_bgmVolume * m_volume;
        }
        public float GetBgmVolume() => m_bgmVolume;

        public void SetSEVolume(float value)
        {
            m_seVolume = Mathf.Clamp01(value);
            m_seAudioSource.volume = m_seVolume * m_volume;
        }

        public float GetSEVolume() => m_seVolume;
    }

}
