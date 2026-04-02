using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
public class AffinityTab : BaseTab
{
    [SerializeField]
    private GameObject objNPCProfile;
    [SerializeField]
    private Transform tsParent;
    // 왼쪽 페이지



    // 오른쪽 페이지
    private Image imgPortrait;                  // 초상화 이미지
    private TextMeshProUGUI txtName;            // 이름
    private TextMeshProUGUI txtDescription;     // 설명글
    private TextMeshProUGUI txtLikes;           // 좋아하는 것
    private TextMeshProUGUI txtDislikes;        // 싫어하는 것

    public override void RefreshUI()
    {
        // 
    }
    public void CreateNPCProfile(GuestData data)
    {
        GuestProfile profile = Instantiate(this.objNPCProfile, this.tsParent).GetComponent<GuestProfile>();

        profile.Init(data, ShowGuestProfile);
    }
    private void ShowGuestProfile(GuestData data)
    {
        this.imgPortrait.sprite = null;
        this.txtName.text = "";
        this.txtDescription.text = "";
        this.txtLikes.text = "";
        this.txtDislikes.text = "";
    }
}
