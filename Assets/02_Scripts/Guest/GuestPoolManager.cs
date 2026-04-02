using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

public class GuestPoolManager : Singleton<GuestPoolManager>
{
    [SerializeField] private Transform poolParent;

    // 타입별 비활성 오브젝트 큐
    private Dictionary<Constants.GuestAppearance, Queue<GameObject>> pools
        = new Dictionary<Constants.GuestAppearance, Queue<GameObject>>();

    // 타입 → 프리팹 매핑
    private Dictionary<Constants.GuestAppearance, GameObject> prefabMap
        = new Dictionary<Constants.GuestAppearance, GameObject>();

    
    // GuestManager에서 가지고 있는 프리팹 정보를 여기에 등록하는 메소드
    // 이 메소드를 호출해야 해당 타입의 풀이 생성됨.
    public void RegisterPrefab(Constants.GuestAppearance type, GameObject prefab, int preloadCount = 3)
    {
        // 이미 등록된 타입이면 무시
        if (pools.ContainsKey(type))
            return;
        
        prefabMap[type] = prefab;
        var queue = new Queue<GameObject>();

        // 미리 만들어두기
        for (int i = 0; i < preloadCount; i++)
        {
            GameObject obj = Instantiate(prefab, poolParent);
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.enabled = false;

            obj.SetActive(false);
            queue.Enqueue(obj);
        }

        pools[type] = queue;
        Debug.Log($"[풀] {type} 등록 완료 {preloadCount}개");
    }

    // 풀에서 꺼내기, 큐가 비면 자동으로 새로 만듦
    public GameObject Get(Constants.GuestAppearance type)
    {
        // 등록 안 된 타입이면 에러
        if (!prefabMap.ContainsKey(type))
            return null;

        GameObject obj;

        if (pools.ContainsKey(type) && pools[type].Count > 0)
            obj = pools[type].Dequeue(); // 재활용
        else
        {
            obj = Instantiate(prefabMap[type], poolParent); // 풀이 비었으면 새로 만들기
            
            NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
            if (agent != null)
                agent.enabled = false;

            obj.SetActive(false);
        }
        return obj;
    }

    // Destroy 대신 이 메소드를 호출, 배활성화 후 큐에 반환
    public void Return(Constants.GuestAppearance type, GameObject obj)
    {
        if (obj == null)
            return;
        
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);

        if (!pools.ContainsKey(type))
            pools[type] = new Queue<GameObject>();

        pools[type].Enqueue(obj);
    }
}
