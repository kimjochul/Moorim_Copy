using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour // where T: monobehavior T에 대한 조건 monobehavior여야함
{
    // 다른 클래스에서 상속받아서 사용하면됨
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();
                if (instance == null)
                {
                    var newObj = new GameObject(typeof(T).ToString());
                    instance = newObj.AddComponent<T>();
                }
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
}
