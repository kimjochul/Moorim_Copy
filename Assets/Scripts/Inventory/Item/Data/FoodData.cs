using UnityEngine;

[CreateAssetMenu(fileName = "FoodData", menuName = "Scriptable Objects/FoodData")]
public class FoodData : ItemData
{
    public bool isLocked;
    public Constants.FoodType type;
}
