using UnityEngine;
using UnityEngine.UI;

public class TempUI : MonoBehaviour
{
    Button button;   
    private void Awake()
    {
        button =  GetComponent<Button>();
        button.onClick.AddListener(() => 
            UIManager.Instance.OpenDialogPopup("ExampleName", "This is where dialog is shown. If you click once full dialog will be shown")
            );
    }
}
