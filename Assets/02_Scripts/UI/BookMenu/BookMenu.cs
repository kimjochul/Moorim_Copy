using System;
using UnityEngine;
using UnityEngine.UI;
using static Constants;


public class BookMenu : MonoBehaviour
{
    [Serializable]
    public class TabPair
    {
        public BookTabType tabType;
        public Toggle toggleTab;
        public GameObject objTab;
    }

    [Header("ÅÇ ¿¬°á ¼³Á¤")]
    [SerializeField]
    private TabPair[] tabs;

    private void OnEnable()
    {
        OpenTab(BookTabType.Status);
    }


    void Start()
    {
        ToggleTabInit();
    }

    public void ToggleTabInit()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            int index = i;

            this.tabs[i].toggleTab.onValueChanged.AddListener((isOn) =>
            {
                this.tabs[index].objTab.SetActive(isOn);

                if (isOn)
                {

                }
            });

            this.tabs[i].objTab.SetActive(tabs[i].toggleTab.isOn);
        }
    }

    public void OpenTab(BookTabType targetType)
    {
        foreach (var tab in tabs)
        {
            if (tab.tabType == targetType)
            {
                tab.toggleTab.isOn = true; 
                break;
            }
        }
    }
}