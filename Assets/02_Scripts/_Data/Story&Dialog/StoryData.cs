using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorySO", menuName = "Story/StorySO")]
public class StorySO : ScriptableObject
{
    public List<Story> stories;
}

