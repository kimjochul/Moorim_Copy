using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour // where T: monobehavior T에 대한 조건 monobehavior여야함
{
    // 다른 클래스에서 상속받아서 사용하면됨
    private static T instance;
    private static bool isQuitting = false;
    public static T Instance
    {
        get
        {
            if (isQuitting)
                return null;
            
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();
            }
            return instance;
        }
    }
    
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T; // 안전한 형변환
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    
    protected virtual void OnApplicationQuit()
    {
        isQuitting = true;
    }

    protected virtual void OnDestroy()
    {
        isQuitting = true;
    }
}
