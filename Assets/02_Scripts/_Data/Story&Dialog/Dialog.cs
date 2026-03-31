using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog
{
    public int id;
    
    [TextArea(3, 10)]
    public List<string> dialogs;
}