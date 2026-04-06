using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private Transform poolParent;

    // 타입별 비활성 오브젝트 큐
    private Dictionary<string, Queue<GameObject>> pools
        = new Dictionary<string, Queue<GameObject>>();

    // 타입 → 프리팹 매핑
    private Dictionary<string, GameObject> prefabMap
        = new Dictionary<string, GameObject>();

    
    // 프리팹 정보를 여기에 등록하는 메소드
    // 이 메소드를 호출해야 해당 타입의 풀이 생성됨.
    public void Register(string key, GameObject prefab, int preloadCount = 3)
    {
        // 이미 등록된 타입이면 무시
        if (pools.ContainsKey(key))
            return;
        
        prefabMap[key] = prefab;
        var queue = new Queue<GameObject>();

        // 미리 만들어두기
        for (int i = 0; i < preloadCount; i++)
        {
            GameObject obj = CreateObject(key);
            queue.Enqueue(obj);
        }

        pools[key] = queue;
        Debug.Log($"[풀] {key} 등록 완료 {preloadCount}개");
    }

    // 풀에서 꺼내기, 큐가 비면 자동으로 새로 만듦
    public GameObject Get(string key)
    {
        // 등록 안 된 타입이면 에러
        if (!prefabMap.ContainsKey(key))
            return null;

        GameObject obj;

        if (pools.ContainsKey(key) && pools[key].Count > 0)
            obj = pools[key].Dequeue(); // 재활용
        else
            obj = CreateObject(key);

        return obj;
    }

    // Destroy 대신 이 메소드를 호출, 배활성화 후 큐에 반환
    public void Return(string key, GameObject obj)
    {
        if (obj == null)
            return;
        
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);

        if (!pools.ContainsKey(key))
            pools[key] = new Queue<GameObject>();

        pools[key].Enqueue(obj);
    }

    private GameObject CreateObject(string key)
    {
        GameObject obj = Instantiate(prefabMap[key], poolParent);

        // NavMeshAgent가 있으면 끄기 (손님용)
        NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        obj.SetActive(false);
        return obj;
    }
}
