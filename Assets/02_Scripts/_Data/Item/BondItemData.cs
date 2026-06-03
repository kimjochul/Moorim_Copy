using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BondItemData", menuName = "Scriptable Objects/BondItemData")]
public class BondItemData : ItemData
{
    public int bondWithWhom;
    public bool haveThisItem = false;
    public List<StatModifier> modifiers;
}

[Serializable]
public class StatModifier
{
    public BondEffectType type;
    public float value;
}