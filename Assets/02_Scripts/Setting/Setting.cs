using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] private GameObject objSetting;
    [SerializeField] private Button btnClose;

    private UnityAction onShow;
    private UnityAction onHide;

    [Header("해상도 UI 연결")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private struct ResData { public int w, h; }
    private List<ResData> resList = new List<ResData>
    {
        new ResData { w = 1280, h = 720 },
        new ResData { w = 1600, h = 900 },
        new ResData { w = 1920, h = 1080 },
        new ResData { w = 2560, h = 1440 }
    };

    void Start()
    {
        if (this.objSetting != null)
        {
            this.objSetting.SetActive(false);
        }
        Init();
        EventInit();
    }

    private void Init()
    {
        InitDropdown();
    }
    private void InitDropdown()
    {
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
    }


    /// <summary>
    /// 드롭다운에서 값을 선택했을 때 실행될 함수
    /// </summary>
    public void SetResolution(int index)
    {
        ResData res = this.resList[index];
        Screen.SetResolution(res.w, res.h, Screen.fullScreenMode);
    }
    private void EventInit()
    {
        this.btnClose.onClick.AddListener(Hide);
    }
    public void Show()
    {
        this.onShow?.Invoke();
        this.objSetting.SetActive(true);
    }
    public void Hide()
    {
        this.onHide?.Invoke();
        this.objSetting.SetActive(false);
    }
}
