using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenu : MonoBehaviour
{
    [Serializable]
    public class TabInfo
    {
        public Toggle toggleTab;
        public BaseTab tabScript;
    }

    [Header("이 메뉴에 속한 탭들")]
    [SerializeField] protected TabInfo[] tabs;

    void Awake()
    {
        for (int i = 0; i < this.tabs.Length; i++)
        {
            int index = i; 

            this.tabs[i].toggleTab.onValueChanged.AddListener((isOn) =>
            {
                if (this.tabs[index].tabScript != null)
                {
                    this.tabs[index].tabScript.gameObject.SetActive(isOn);
                    if(isOn)
                        this.tabs[index].tabScript.ResetSetting();
                }
            });
        }
    }
    public virtual void SetupMenuData()
    {
        for (int i = 0; i < this.tabs.Length; i++)
        {
            if (this.tabs[i].tabScript != null)
            {
                this.tabs[i].tabScript. SetupData();
            }
        }
    }

    // 메뉴를 열 때 호출하는 함수
    public virtual void OpenMenu()
    {
        if (this.tabs != null && this.tabs.Length > 0)
        {
            this.tabs[0].toggleTab.isOn = true;
        }

        this.gameObject.SetActive(true);
    }
    
    public virtual void CloseMenu()
    {
        if (this.tabs != null && this.tabs.Length > 0)
        {
            this.tabs[0].toggleTab.isOn = true;
        }

        this.gameObject.SetActive(false);
    }
}
