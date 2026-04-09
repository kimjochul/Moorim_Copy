using UnityEngine;

public class Door : MonoBehaviour
{
    void Start()
    {
        UIManager.Instance.CreateAmountPopup("¾È³ç", null, (amount) =>
        {
            Debug.Log($"{amount}");
        },() =>
        {
            Debug.Log("²¨Áü");
        });
    }
}
