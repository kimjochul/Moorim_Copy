using UnityEngine;

public class IngredientItem : MonoBehaviour, IItem
{
    public int ID { get; }
    public Inventory Inven { get; }
    public GameObject Obj { get; }
    public string ItemName { get; }
    public Sprite Icon { get; }
    public int InventoryCapacity { get; }
    
    [SerializeField] IngredientList ingredientList;
    
    public void Get()
    {
        
    }

    public void Use()
    {
        
    }
}
