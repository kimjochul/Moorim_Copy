using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [Header("볼륨 슬라이더 UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        Init();
        InitEvent();
    }
    private void Init()
    {
        if (SoundManager.Instance != null)
        {
            this.masterSlider.value = SoundManager.Instance.MasterVolume;
            this.bgmSlider.value = SoundManager.Instance.BgmVolume;
            this.sfxSlider.value = SoundManager.Instance.SfxVolume;
        }
    }
    private void InitEvent()
    {
        this.masterSlider.onValueChanged.AddListener(SetMasterVolume);
        this.bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        this.sfxSlider.onValueChanged.AddListener(SetSfxVolume);
    }
    // 슬라이더를 움직일 때마다 실행되는 함수들

    public void SetMasterVolume(float volume)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMasterVolume(volume);
    }

    public void SetBgmVolume(float volume)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetBGMVolume(volume);
    }

    public void SetSfxVolume(float volume)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetSFXVolume(volume);
    }
}
