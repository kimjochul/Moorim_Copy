using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Story
{
    public int id;
    
    public string storyName;
    public string speaker;
    [TextArea(3, 10)]
    public List<string> story;
}