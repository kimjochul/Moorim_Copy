using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialog
{
    public int id;
    // 케릭터 정보 추가
    
    [TextArea(3, 10)]
    public List<string> dialogs;
}