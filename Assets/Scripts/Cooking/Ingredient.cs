using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ingredient
{
    public int id;
    public string name;
    public int price;
    public int maxCapacity;
}

// IItem 상속받은 ingredient 아이템을 생성해야할 듯