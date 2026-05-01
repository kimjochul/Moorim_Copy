using UnityEngine;
using UnityEngine.UI;

public enum SaveLoadMode
{
    Save,
    Load
}

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] private SaveSlotUI[] slots;
    [SerializeField] private Button closeButton;

    private SaveLoadMode _currentMode;
    
    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }
    
    public void OpenSaveUI(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);
            
            _currentMode = SaveLoadMode.Save;
            
            // 1번 = 자동
            slots[0].Init(0, SaveSlotType.Auto, SaveLoadMode.Save);

            // 2~4번 = 수동
            for (int i = 1; i < slots.Length; i++)
            {
                slots[i].Init(i, SaveSlotType.Manual, SaveLoadMode.Save);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OpenLoadUI(bool isShow)
    {
        if (isShow)
        {
            gameObject.SetActive(true);

            _currentMode = SaveLoadMode.Load;
            
            slots[0].Init(0, SaveSlotType.Auto, SaveLoadMode.Load);

            for (int i = 1; i < slots.Length; i++)
            {
                slots[i].Init(i, SaveSlotType.Manual, SaveLoadMode.Load);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Close()
    {
        if(_currentMode == SaveLoadMode.Save)
            UIManager.Instance.ActiveSaveUI(false);
        else
            UIManager.Instance.ActiveLoadUI(false);
    }
}
