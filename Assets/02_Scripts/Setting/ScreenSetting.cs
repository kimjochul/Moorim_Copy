using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenSetting : MonoBehaviour
{
    [Header("해상도")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private struct ResData { public int w, h; }
    private List<ResData> resList = new List<ResData>
    {
        new ResData { w = 1280, h = 720 },
        new ResData { w = 1600, h = 900 },
        new ResData { w = 1920, h = 1080 },
        new ResData { w = 2560, h = 1440 }
    };

    [Header("글자크기")]
    [SerializeField] private Slider fontSizeSlider;

    public UnityAction<float> OnFontSizeChanged;
    public float CurrentTextScale { get; private set; } = 1f;
    void Awake()
    {
        Init();
        InitEvent();
    }

    /// <summary>
    /// 모니터가 지원하는 해상도를 읽어와서 드롭다운 옵션에 추가
    /// </summary>
    private void Init()
    {
        // 해상도
        this.resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < this.resList.Count; i++)
        {
            options.Add($"{this.resList[i].w}x{this.resList[i].h}");
        }

        this.resolutionDropdown.AddOptions(options);

        string currentRes = $"{Screen.width}x{Screen.height}";
        int index = options.FindIndex(s => s == currentRes);
        if (index != -1)
            this.resolutionDropdown.value = index;

        this.resolutionDropdown.RefreshShownValue();

        // 글자 크기
        if (this.fontSizeSlider != null)
        {
            this.fontSizeSlider.value = this.CurrentTextScale;
        }
    }
    private void InitEvent()
    {
        this.resolutionDropdown.onValueChanged.AddListener(SetResolution);
        this.fontSizeSlider.onValueChanged.AddListener(SetTextScale);
    }

    /// <summary>
    /// 드롭다운에서 값을 선택했을 때 실행될 함수 (실제 해상도 변경)
    /// </summary>
    public void SetResolution(int index)
    {
        ResData res = this.resList[index];

        Screen.SetResolution(res.w, res.h, Screen.fullScreenMode);

        Debug.Log($"현재 해상도: {res.w} x {res.h}");
    }

    /// <summary>
    /// 슬라이더를 움직일 때마다 호출되는 함수 (글자 크기 변경)
    /// </summary>
    public void SetTextScale(float scale)
    {
        this.CurrentTextScale = scale;

        // 이벤트에 가입된 모든 텍스트들에게 변경된 배율(scale)을 방송합니다!
        this.OnFontSizeChanged?.Invoke(this.CurrentTextScale);
    }
}
