using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorySO", menuName = "Scriptable Objects/StorySO")]
public class StorySO : ScriptableObject
{
    public int id;
    public string storyName;

    public List<Story> stories;
}

