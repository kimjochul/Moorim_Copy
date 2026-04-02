using UnityEngine;

public abstract class BaseTab : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        RefreshUI();
    }

    public abstract void RefreshUI();
}
