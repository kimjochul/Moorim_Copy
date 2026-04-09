using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCTalkPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image npcImage;
    [SerializeField] private float textSpeed = 0.02f;
    
    public void ChangeText(string text)
    {
        if (!this.isActiveAndEnabled)
            return;
        
        this.text.text = "";
        StopAllCoroutines();
        StartCoroutine(TempTextCoroutine(text));
    }

    private IEnumerator TempTextCoroutine(string message)
    {
        foreach (char c in message)
        {
            this.text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
