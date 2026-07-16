using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private SaveSlotType slotType;

    [Header("UI")]
    [SerializeField] private TMP_Text slotNameText;
    [SerializeField] private TMP_Text infoText;
    [SerializeField] private Button button;

    private SaveLoadMode currentMode;

    public void Init(int index, SaveSlotType type, SaveLoadMode mode)
    {
        slotIndex = index;
        slotType = type;
        currentMode = mode;

        slotNameText.text = type == SaveSlotType.Auto
            ? "자동 저장"
            : $"슬롯 {index}";

        Refresh();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        string path = GetPath();

        if (currentMode == SaveLoadMode.Save)
        {
            if (File.Exists(path))
            {
                UIManager.Instance.CreateOkPopup(
                    "덮어쓰시겠습니까?",
                    () =>
                    {
                        SaveManager.Instance.SaveGame(slotIndex, slotType);
                        Refresh();
                    },
                    () => {}, true
                );
            }
            else
            {
                SaveManager.Instance.SaveGame(slotIndex, slotType);
                Refresh();
            }
        }
        else
        {
            if (File.Exists(path))
                SaveManager.Instance.LoadGame(slotIndex, slotType);
            else
                UIManager.Instance.CreateOkPopup(
                    "해당 위치에는 저장된 파일이 없습니다.",
                    () => { }, () => {},false
                );
        }
    }

    public void Refresh()
    {
        string path = GetPath();

        if (!File.Exists(path))
        {
            infoText.text = "비어 있음";
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        string stateText = data.day.flowState switch
        {
            GameFlowState.Gameplay => "영업 중",
            GameFlowState.Receipt => "영업 종료",
            GameFlowState.Story => "스토리",
            GameFlowState.NPC => "NPC",
            _ => ""
        };

        infoText.text = $"{data.day.day}일차 {data.day.hour:00}:{data.day.minute:00} ({stateText})";
    }

    private string GetPath()
    {
        string fileName = slotType == SaveSlotType.Auto
            ? "auto_save.json"
            : $"save_{slotIndex}.json";

        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
