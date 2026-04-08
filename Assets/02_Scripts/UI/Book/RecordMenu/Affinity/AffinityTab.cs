using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using System.Collections.Generic;

public class AffinityTab : BaseTab
{
    private List<NPCProfile> listNPCProfiles = new List<NPCProfile>();
    // 왼쪽 페이지
    [SerializeField]
    private GameObject objNPCProfile;
    [SerializeField]
    private Transform tsParent;

    // 오른쪽 페이지
    [SerializeField]
    private GameObject objRightPage;
    [SerializeField]
    private Image imgPortrait;                  // 초상화 이미지
    [SerializeField]
    private TextMeshProUGUI txtName;            // 이름
    [SerializeField]
    private TextMeshProUGUI txtDescription;     // 설명글
    [SerializeField]
    private TextMeshProUGUI txtLikes;           // 좋아하는 것
    [SerializeField]
    private TextMeshProUGUI txtDislikes;        // 싫어하는 것

    [SerializeField]
    private GameObject progressPrefab;
    [SerializeField]
    private Transform progressParent;

    public override void SetupData()
    {
        List<AffinityNPCData> affinity = DataManager.Instance.GetAffinityNPCList();
        UpdateNPCList(affinity);
    }

    private void UpdateNPCList(List<AffinityNPCData> affinityList)
    {
        for (int i = 0; i < affinityList.Count; i++)
        {
            if (i >= this.listNPCProfiles.Count)
            {
                NPCProfile profile = Instantiate(this.objNPCProfile, this.tsParent).GetComponent<NPCProfile>();
                this.listNPCProfiles.Add(profile);
            }
            this.listNPCProfiles[i].gameObject.SetActive(true);
            this.listNPCProfiles[i].Init(affinityList[i], ShowGuestProfile);
        }

        for (int i = affinityList.Count; i < this.listNPCProfiles.Count; i++)
        {
            this.listNPCProfiles[i].gameObject.SetActive(false);
        }
    }

    private void ShowGuestProfile(AffinityNPCData data)
    {
        this.objRightPage.SetActive(true);

        this.imgPortrait.sprite = data.profile;
        this.txtName.text = data.myName;
        this.txtDescription.text = data.description;

        this.txtLikes.text = data.likes != null ? string.Join(", ", data.likes) : "없음";

        this.txtDislikes.text = data.dislikes != null ? string.Join(", ", data.dislikes) : "없음";
    }

    public override void ResetSetting() 
    {
        this.objRightPage.SetActive(false);

        this.imgPortrait.sprite = null;
        this.txtName.text = "";
        this.txtDescription.text = "";
        this.txtLikes.text = "";
        this.txtDislikes.text = "";
    }
}
