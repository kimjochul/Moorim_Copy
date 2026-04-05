using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Constants;


public class Book : MonoBehaviour
{
    [Serializable]
    public class MenuPair
    {
        public Toggle toggle; 
        public BaseMenu menuScript;  
    }

    [Header("상단 메뉴 설정")]
    [SerializeField] private MenuPair[] menus;

    private void Awake()
    {
        for (int i = 0; i < this.menus.Length; i++)
        {
            int index = i; 

            this.menus[i].toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    this.menus[index].menuScript.OpenMenu();
                }
                else
                {
                    this.menus[index].menuScript.CloseMenu();
                }
            });
        }
    }

    private void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        if (this.menus != null && this.menus.Length > 0)
        {
            this.menus[0].toggle.isOn = true;
        }
    }
}