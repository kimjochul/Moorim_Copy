using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusTab : BaseTab
{
    // 왼쪽 페이지
    [SerializeField] private TextMeshProUGUI txtTotalRevenue;
    [SerializeField] private TextMeshProUGUI txtTotalFame;
    [SerializeField] private TextMeshProUGUI txtTotalHappyGuests;
    [SerializeField] private TextMeshProUGUI txtTotalAngryGuests;

    // 오른쪽 페이지

    // 명월
    [SerializeField] private Image imgMyungwol;
    [SerializeField] private TextMeshProUGUI[] txtMyungwolLikes;

    // 소월
    [SerializeField] private Image imgSowol;
    [SerializeField] private TextMeshProUGUI[] txtSowolLikes;

    public override void RefreshUI()
    {
        this.txtTotalRevenue.text = "";
        this.txtTotalFame.text = "";
        this.txtTotalHappyGuests.text = "";
        this.txtTotalAngryGuests.text = "";

        ProfileMyungwol();
        ProfileSowol();
    }
    public void ProfileMyungwol()
    {
        this.imgMyungwol.sprite = null;
    }
    public void ProfileSowol()
    {
        this.imgSowol.sprite = null;
    }
}
