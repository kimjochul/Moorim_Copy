using System;
using UnityEngine;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class GuestManager : MonoBehaviour
{
    [Header("연결해야할 외부 정보")]
    [SerializeField] private DayManager dayManager;

    [Header("Target Point")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Spawn Info")]
    [SerializeField] private float startSpawnTime = 1f;
    [SerializeField] private float spawnInterval = 5f;

    [Serializable]
    public struct GuestPrefabEntry
    {
       public GameObject prefab;
       public Constants.GuestAppearance appearance;
    }

    [SerializeField] private GuestPrefabEntry[] normalGuestPrefabs;
    public GameObject richGuestPrefab;
    public GameObject thiefGuestPrefab;

    public static event Action OnCustomerServed;
    private int customerCount = 0;

    // 손님 런타임 정보
    private int globalIDCounter = 0;

    void Awake()
    {
        dayManager.OnDayStart += StartSpawn;
        dayManager.OnDayEnd += StopSpawn;

        // 풀 매니저에 프리팹 등록
        foreach (var entry in normalGuestPrefabs)
        {
            GuestPoolManager.Instance.RegisterPrefab(entry.appearance, entry.prefab, 3);
        }

        if (richGuestPrefab != null)
            GuestPoolManager.Instance.RegisterPrefab(Constants.GuestAppearance.RichMan, richGuestPrefab, 1);

        if (thiefGuestPrefab != null)
            GuestPoolManager.Instance.RegisterPrefab(Constants.GuestAppearance.ThiefGuy, thiefGuestPrefab, 1);
    }

    void OnDisable()
    {
        dayManager.OnDayStart -= StartSpawn;
        dayManager.OnDayEnd -= StopSpawn;
    }

    private void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnGuset), startSpawnTime, spawnInterval);
    }

    private void SpawnGuset()
    {
        globalIDCounter++; // 스폰될 때마다 고유 id 1씩 증가

        Constants.GuestAppearance appearance = Constants.GuestAppearance.NormalMaleA;

        // 확률 돌려서 손님 타입과 외형 결정 (0~1사이 랜덤)
        float rand = UnityEngine.Random.value;
        if (rand < 0.05f)
            appearance = Constants.GuestAppearance.ThiefGuy;
        else if (rand < 0.1f)
            appearance = Constants.GuestAppearance.RichMan;
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, normalGuestPrefabs.Length);
            appearance  = normalGuestPrefabs[randomIndex].appearance;
        }

        // 풀에서 타입에 맞는 오브젝트 꺼냄
        GameObject guestObj = GuestPoolManager.Instance.Get(appearance);

        if (guestObj != null)
        {
            guestObj.transform.position = spawnPoint.position;
            guestObj.transform.rotation = Quaternion.identity;

            GuestBase guest = guestObj.GetComponent<GuestBase>();

            if (guest != null)
            {
                guest.ResetState();
                guest.Init(globalIDCounter, appearance, exitPoint);

                guest.OnGuestServedAndPaid += () =>
                {
                    customerCount++;
                    OnCustomerServed?.Invoke();
                };
            }
        }
    }

    public void ServeCustomer()
    {
        customerCount++;
        OnCustomerServed?.Invoke();
    }

    public void StopSpawn()
    {
        CancelInvoke();
    }
}


