using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "SoundSO/SoundList")]
public class SoundList : ScriptableObject
{
    [Header("배경 리스트")]
    public List<SoundData> bgmList = new List<SoundData>();

    [Header("효과음 리스트")]
    public List<SoundData> sfxList = new List<SoundData>();
}
