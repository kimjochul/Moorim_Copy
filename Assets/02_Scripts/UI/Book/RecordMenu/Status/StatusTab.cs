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
    [SerializeField] private TextMeshProUGUI txtMyungwolLike;
    [SerializeField] private TextMeshProUGUI txtMyungwolDislike;
    [SerializeField] private TextMeshProUGUI txtMyungwolSkill;

    // 소월
    [SerializeField] private Image imgSowol;
    [SerializeField] private TextMeshProUGUI txtSowolLike;
    [SerializeField] private TextMeshProUGUI txtSowolDislike;
    [SerializeField] private TextMeshProUGUI txtSowolSkill;

    public override void SetupData()
    {
        ProfileSetting();
    }
    private void ProfileSetting()
    {
        this.txtTotalRevenue.text = "";
        this.txtTotalFame.text = "";
        this.txtTotalHappyGuests.text = "";
        this.txtTotalAngryGuests.text = "";


        PlayerData data = new PlayerData();


        ProfileMyungwol(data);
        ProfileSowol(data);
    }
    public void ProfileMyungwol(PlayerData data)
    {
        this.imgMyungwol.sprite = data.icon;
        this.txtMyungwolLike.text = data.like;
        this.txtMyungwolDislike.text = data.dislike;
        this.txtMyungwolSkill.text = data.skill;
    }
    public void ProfileSowol(PlayerData data)
    {
        this.imgSowol.sprite = data.icon;
        this.txtSowolLike.text = data.like;
        this.txtSowolDislike.text = data.dislike;
        this.txtSowolSkill.text = data.skill;
    }
}
