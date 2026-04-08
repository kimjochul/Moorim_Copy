using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using System.Collections.Generic;

public class AffinityTab : BaseTab
{
    [SerializeField]
    private GameObject objNPCProfile;
    [SerializeField]
    private Transform tsParent;

    private List<NPCProfile> listNPCProfiles = new List<NPCProfile>();
    // 왼쪽 페이지


    // 오른쪽 페이지
    private Image imgPortrait;                  // 초상화 이미지
    private TextMeshProUGUI txtName;            // 이름
    private TextMeshProUGUI txtDescription;     // 설명글
    private TextMeshProUGUI txtLikes;           // 좋아하는 것
    private TextMeshProUGUI txtDislikes;        // 싫어하는 것

    public override void SetupData()
    {
        //UpdateNPCList();
    }
    // GuestData는 임시
    private void UpdateNPCList(List<GuestData> guestList)
    {
        for (int i = 0; i < guestList.Count; i++)
        {
            if (i >= this.listNPCProfiles.Count)
            {
                NPCProfile profile = Instantiate(this.objNPCProfile, this.tsParent).GetComponent<NPCProfile>();
                this.listNPCProfiles.Add(profile);
            }

            this.listNPCProfiles[i].gameObject.SetActive(true);
            this.listNPCProfiles[i].Init(guestList[i], ShowGuestProfile);
        }

        for (int i = guestList.Count; i < this.listNPCProfiles.Count; i++)
        {
            this.listNPCProfiles[i].gameObject.SetActive(false);
        }
    }

    // GuestData 임시
    private void ShowGuestProfile(GuestData data)
    {
        this.imgPortrait.sprite = null;
        this.txtName.text = "";
        this.txtDescription.text = "";
        this.txtLikes.text = "";
        this.txtDislikes.text = "";
    }

    public override void ResetSetting() { }
}
