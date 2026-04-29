using UnityEngine;

public enum SaveLoadMode
{
    Save,
    Load
}

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private SaveSlotUI[] slots;

    public void OpenSaveUI()
    {
        gameObject.SetActive(true);

        // 1번 = 자동
        slots[0].Init(0, SaveSlotType.Auto, SaveLoadMode.Save);

        // 2~4번 = 수동
        for (int i = 1; i < slots.Length; i++)
        {
            slots[i].Init(i, SaveSlotType.Manual, SaveLoadMode.Save);
        }
    }

    public void OpenLoadUI()
    {
        gameObject.SetActive(true);

        slots[0].Init(0, SaveSlotType.Auto, SaveLoadMode.Load);

        for (int i = 1; i < slots.Length; i++)
        {
            slots[i].Init(i, SaveSlotType.Manual, SaveLoadMode.Load);
        }
    }
}
