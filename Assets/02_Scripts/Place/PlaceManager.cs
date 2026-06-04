using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlaceManager : MonoBehaviour
{
    private GuestManager guestManager;
    
    public TMP_InputField openTableCount;
    public Button tableAdd;
    public GameObject[] tableSets;

    private void Awake()
    {
        guestManager = FindFirstObjectByType<GuestManager>();
        
        tableAdd.onClick.AddListener(OpenTable);
    }
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("Press R");
            guestManager.SpawnGuset();
        }
    }

    private void OpenTable()
    {
        int targetCount = int.Parse(openTableCount.text);
        int activatedCount = 0;

        // tableSets 배열을 돌면서 비활성화된 테이블을 찾아 활성화
        foreach (GameObject table in tableSets)
        {
            if (!table.activeSelf)
            {
                table.SetActive(true); 
                activatedCount++;

                if (activatedCount >= targetCount)
                {
                    break;
                }
            }
        }
    }
}