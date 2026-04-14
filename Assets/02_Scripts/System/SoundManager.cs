using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("사운드 데이터 에셋")]
    [SerializeField] private SoundList soundList;

    [Header("볼륨 세팅")]
    [Range(0f, 1f)][SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float bgmVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 1f;

    public float MasterVolume => this.masterVolume;
    public float BgmVolume => this.bgmVolume;
    public float SfxVolume => this.sfxVolume;

    private Dictionary<string, SoundData> dicBGM = new Dictionary<string, SoundData>();
    private Dictionary<string, SoundData> dicSFX = new Dictionary<string, SoundData>();
    public Button BGMBtn;
    public Button SFXBtn;

    protected override void Awake()
    {
        base.Awake();
        Setting();
        Init();

        this.BGMBtn.onClick.AddListener(() =>
        {
            PlayBGM("테스트배경음");
        });
        this.SFXBtn.onClick.AddListener(() =>
        {
            PlaySFX("테스트효과음");
        });
    }

    private void Init()
    {
        if (this.soundList == null) return;

        foreach (var data in this.soundList.bgmList)
        {
            if (data != null && !this.dicBGM.ContainsKey(data.soundName))
                this.dicBGM.Add(data.soundName, data);
        }

        foreach (var data in this.soundList.sfxList)
        {
            if (data != null && !this.dicSFX.ContainsKey(data.soundName))
                this.dicSFX.Add(data.soundName, data);
        }
    }

    private void Setting()
    {
        if (this.bgmSource == null)
            this.bgmSource = this.gameObject.AddComponent<AudioSource>();
        if (this.sfxSource == null)
            this.sfxSource = this.gameObject.AddComponent<AudioSource>();

        this.bgmSource.loop = true;
        this.sfxSource.loop = false;
    }

    /// <summary>
    /// 지정한 이름의 BGM을 재생
    /// </summary>
    public void PlayBGM(string soundName)
    {
        if (!this.dicBGM.TryGetValue(soundName, out SoundData data))
            return;

        if (this.bgmSource.clip == data.clip && this.bgmSource.isPlaying)
            return;

        this.bgmSource.clip = data.clip;
        this.bgmSource.volume = data.volume * this.bgmVolume * this.masterVolume;
        this.bgmSource.Play();
    }

    /// <summary>
    /// 지정한 이름의 SFX을 재생
    /// </summary>
    public void PlaySFX(string soundName)
    {
        if (!this.dicSFX.TryGetValue(soundName, out SoundData data))
            return;

        this.sfxSource.PlayOneShot(data.clip, data.volume * this.sfxVolume * this.masterVolume);
    }

    /// <summary>
    /// 특정 오브젝트의 위치에서 효과음을 재생
    /// </summary>
    public void PlaySoundOnObj(string soundName, GameObject obj)
    {
        if (!this.dicSFX.TryGetValue(soundName, out SoundData data))
            return;

        AudioSource source = obj.GetComponent<AudioSource>();

        if (source == null)
        {
            source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        source.PlayOneShot(data.clip, data.volume * this.sfxVolume * this.masterVolume);
    }

    /// <summary>
    /// BGM 오디오 클립을 검색하여 반환
    /// </summary>
    public AudioClip GetBGMClip(string soundName)
    {
        if (this.dicBGM.TryGetValue(soundName, out SoundData data))
        {
            return data.clip;
        }
        return null;
    }

    /// <summary>
    /// SFX 오디오 클립을 검색하여 반환
    /// </summary>
    public AudioClip GetSFXClip(string soundName)
    {
        if (this.dicSFX.TryGetValue(soundName, out SoundData data))
        {
            return data.clip;
        }
        return null;
    }
    /// <summary>
    /// 마스터 볼륨을 조절 (0.0 ~ 1.0)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        this.masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    /// <summary>
    /// BGM 볼륨을 조절 (0.0 ~ 1.0)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        this.bgmVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    /// <summary>
    /// SFX 볼륨을 조절 (0.0 ~ 1.0)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        this.sfxVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    /// <summary>
    /// 변경된 볼륨 수치를 AudioSource에 즉시 반영
    /// </summary>
    private void UpdateVolumes()
    {
        if (this.bgmSource != null)
            this.bgmSource.volume = this.bgmVolume * this.masterVolume;

        if (this.sfxSource != null)
            this.sfxSource.volume = this.sfxVolume * this.masterVolume;
    }

    /// <summary>
    /// 현재 재생 중인 배경음악을 정지
    /// </summary>
    public void StopBGM() => this.bgmSource.Stop();

    /// <summary>
    /// 현재 재생 중인 효과음을 정지
    /// </summary>
    public void StopSFX() => this.sfxSource.Stop();

    /// <summary>
    /// 현재 재생 중인 배경음악을 일시정지
    /// </summary>
    public void PauseBGM() => this.bgmSource.Pause();

    /// <summary>
    /// 일시정지된 배경음악을 다시 재생
    /// </summary>
    public void ResumeBGM() => this.bgmSource.UnPause();

    /// <summary>
    /// 현재 재생 중인 모든 효과음을 일시정지
    /// </summary>
    public void PauseSFX() => this.sfxSource.Pause();

    /// <summary>
    /// 일시정지된 효과음들을 다시 재생
    /// </summary>
    public void ResumeSFX() => this.sfxSource.UnPause();

    /// <summary>
    /// 배경음악의 음소거 설정
    /// </summary>
    public void SetMuteBGM(bool isMute) => this.bgmSource.mute = isMute;

    /// <summary>
    /// 효과음의 음소거 설정
    /// </summary>
    public void SetMuteSFX(bool isMute) => this.sfxSource.mute = isMute;
}