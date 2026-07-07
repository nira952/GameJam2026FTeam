using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class AudioManager : MonoBehaviour
{
    #region Singleton
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }
    #endregion

    [SerializeField] private AudioSetting audioData;
    [SerializeField] private AudioMixer audioMixer; 

    [Header("Settings")]
    public int maxSeSources = 10;

    private AudioSource bgmSource;
    private List<AudioSource> seSources = new List<AudioSource>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (audioData == null || audioMixer == null)
        {
            Debug.LogWarning("オーディオデータ、またはAudioMixerが参照できません");
            return;
        }

        InitAudioSources();
    }

    private void InitAudioSources()
    {
        // MixerからBGMとSEのグループを探す
        AudioMixerGroup[] bgmGroups = audioMixer.FindMatchingGroups("Master/BGM");
        AudioMixerGroup[] seGroups = audioMixer.FindMatchingGroups("Master/SE");

        // BGM用のAudioSourceを作成してMixerグループを割り当て
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        if (bgmGroups.Length > 0) bgmSource.outputAudioMixerGroup = bgmGroups[0];

        // SE用のAudioSourceを事前生成してMixerグループを割り当て
        for (int i = 0; i < maxSeSources; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            if (seGroups.Length > 0) source.outputAudioMixerGroup = seGroups[0];
            seSources.Add(source);
        }
    }

    // 外部（設定画面UIなど）からマスター音量を変更する関数 (volumeは 0.0f ～ 1.0f で受け取る)
    public void SetBgmMasterVolume(float volume)
    {
        float db = LinearToDecibel(volume);
        audioMixer.SetFloat("BGMVolume", db); // 露出させたパラメータ名を指定
    }

    public void SetSeMasterVolume(float volume)
    {
        float db = LinearToDecibel(volume);
        audioMixer.SetFloat("SEVolume", db); // 露出させたパラメータ名を指定
    }

    // 0～1の値を、AudioMixer用の-80dB～0dBに変換する計算
    private float LinearToDecibel(float linear)
    {
        if (linear <= 0f) return -80f; // 0以下なら完全にミュート
        return Mathf.Log10(linear) * 20f;
    }

    public void Play(BgmName name)
    {
        int kind = (int)name;
        bgmSource.clip = audioData.bgmList[kind].clip;
        // 個別の初期音量設定だけを反映（マスター音量はMixer側で自動合成されるため計算不要）
        bgmSource.volume = audioData.bgmList[kind].volume;
        bgmSource.Play();
    }

    public void Play(SeName name)
    {
        int kind = (int)name;
        AudioSource freeSource = null;
        for (int i = 0; i < seSources.Count; i++)
        {
            if (!seSources[i].isPlaying)
            {
                freeSource = seSources[i];
                break;
            }
        }

        if (freeSource == null)
        {
            Debug.LogWarning("SEの最大同時再生数を超えているため再生をスキップしました。");
            return;
        }

        freeSource.clip = audioData.seList[kind].clip;
        freeSource.volume = audioData.seList[kind].volume; // 個別音量のみ設定
        freeSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying) bgmSource.Stop();
    }

    public void StopAllSE()
    {
        for (int i = 0; i < seSources.Count; i++)
        {
            if (seSources[i].isPlaying) seSources[i].Stop();
        }
    }


    private void OnDestroy()
    {
        // インスタンスを破棄する際に、参照をクリアする
        if (instance == this)
        {
            instance = null;
        }
    }
}