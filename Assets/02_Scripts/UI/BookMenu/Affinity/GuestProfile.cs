using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GuestProfile : MonoBehaviour
{
    [SerializeField] Image imgIcon;
    [SerializeField] TextMeshProUGUI txtName;
    [SerializeField] Button btnSlot;

    public void Init(GuestData data, Action<GuestData> onClickAction)
    {
        this.imgIcon.sprite = null;
        this.txtName.text = "";

        this.btnSlot.onClick.RemoveAllListeners();

        this.btnSlot.onClick.AddListener(() =>
        {
            onClickAction?.Invoke(data);
        });
    }
}
