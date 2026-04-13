using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Story
{
    public int id;
    public string storyName;

    [TextArea(3, 10)]
    public List<StoryLine> lines;
}