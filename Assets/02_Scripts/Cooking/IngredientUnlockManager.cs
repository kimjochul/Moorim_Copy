using System.Collections.Generic;
using UnityEngine;

public class IngredientUnlockManager : MonoBehaviour
{
    [SerializeField] private IngredientList allIngredients;

    private HashSet<int> unlockedIngredients = new HashSet<int>();

    private void Start()
    {
        foreach (var ingredient in allIngredients.ingredients)
        {
            if (!ingredient.isLocked)
                Unlock(ingredient.id);
        }
    }

    public bool IsUnlocked(int id)
    {
        return unlockedIngredients.Contains(id);
    }

    public void Unlock(int id)
    {
        if (unlockedIngredients.Contains(id))
            return;

        unlockedIngredients.Add(id);
        Debug.Log($"재료 해금: {id}");
    }
}
