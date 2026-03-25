using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuestList", menuName = "Scriptable Objects/GuestList")]
public class GuestList : ScriptableObject
{
    public List<GuestData> guests;
}
