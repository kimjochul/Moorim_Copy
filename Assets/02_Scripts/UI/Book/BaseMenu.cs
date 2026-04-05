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

    protected virtual void Awake()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;
            this.tabs[i].toggleTab.onValueChanged.AddListener((isOn) =>
            {
                this.tabs[index].tabScript.gameObject.SetActive(isOn);

                if (isOn)
                {
                    this.tabs[index].tabScript.SetupData();
                }
            });
        }
    }

    // 책이 이 메뉴를 열 때 호출하는 함수
    public virtual void OpenMenu()
    {
        this.gameObject.SetActive(true);
        // 첫번째 탭이 켜지게
        if (this.tabs != null && this.tabs.Length > 0)
            this.tabs[0].toggleTab.isOn = true;
    }

    public virtual void CloseMenu()
    {
        this.gameObject.SetActive(false);
    }
}
