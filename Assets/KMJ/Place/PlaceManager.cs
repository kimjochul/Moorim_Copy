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
    private int currentTableIndex;

    public Button stoveAdd;
    public GameObject[] cookingStations;
    public GameObject[] kitchenTables;
    private int currentStoveIndex;

    private void Awake()
    {
        guestManager = FindFirstObjectByType<GuestManager>();
        
        tableAdd.onClick.AddListener(OpenTable);
        stoveAdd.onClick.AddListener(OpenStove);
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
        // 입력값이 비어있거나 파싱에 실패할 경우를 대비한 안전 장치 (선택 사항)
        if (!int.TryParse(openTableCount.text, out int targetCount)) return;

        // 현재 인덱스부터 targetCount만큼 추가로 활성화 (O(K), K는 추가할 테이블 수)
        int limit = Mathf.Min(currentTableIndex + targetCount, tableSets.Length);
        
        for (int i = currentTableIndex; i < limit; i++)
        {
            tableSets[i].SetActive(true);
            currentTableIndex++; // 다음번에 열기 시작할 위치 저장
        }
    }

    private void OpenStove()
    {
        // 시간복잡도 O(1): 현재 인덱스가 배열 길이보다 작을 때만 바로 활성화하고 끝
        if (currentStoveIndex < cookingStations.Length)
        {
            cookingStations[currentStoveIndex].SetActive(true);
            kitchenTables[currentStoveIndex].SetActive(false);
            
            currentStoveIndex++; // 다음번에 켤 스토브 인덱스로 이동
        }
    }
}