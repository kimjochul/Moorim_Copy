using System;
using UnityEngine;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class GuestManager : MonoBehaviour
{
    [SerializeField] private DayManager dayManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;

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
    }

    void OnDisable()
    {
        dayManager.OnDayStart -= StartSpawn;
        dayManager.OnDayEnd -= StopSpawn;
    }

    private void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnGuset), 1f, 5f); // 고객 생성, 랜덤 요소 추가 필요
    }

    private void SpawnGuset()
    {
        globalIDCounter++; // 스폰될 때마다 고유 id 1씩 증가

        GameObject selectedPrefab = null;
        Constants.GuestAppearance appearance = Constants.GuestAppearance.NormalMaleA;

        // 확률 돌려서 손님 타입과 외형 결정 (0~1사이 랜덤)
        float rand = UnityEngine.Random.value;
        if (rand < 0.05f)
        {
            selectedPrefab = thiefGuestPrefab;
            appearance = Constants.GuestAppearance.ThiefGuy;
        }
        else if (rand < 0.1f)
        {
            selectedPrefab = richGuestPrefab;
            appearance = Constants.GuestAppearance.RichMan;
        }
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, normalGuestPrefabs.Length);
            selectedPrefab = normalGuestPrefabs[randomIndex].prefab;
            appearance  = normalGuestPrefabs[randomIndex].appearance;

            // 뽑힌 인덱스에 맞춰서 Appearance 열거형도 알맞게 짝지어줌
            // (배열 0번은 NormalA, 1번은 NormalB, 2번은 NormalC)
            if (randomIndex == 0) appearance = Constants.GuestAppearance.NormalMaleA;
            else if (randomIndex == 1) appearance = Constants.GuestAppearance.NormalMaleB;
            else if (randomIndex == 2) appearance = Constants.GuestAppearance.NormalFemaleA;
            else appearance = Constants.GuestAppearance.NormalFemaleB;
        }

        // 프리팹 생성 및 정보 주입
        if (selectedPrefab != null)
        {
            GameObject guestObj = Instantiate(selectedPrefab, spawnPoint.position, Quaternion.identity);
            GuestBase guest = guestObj.GetComponent<GuestBase>();

            if (guest != null)
            {
                guest.Init(globalIDCounter, appearance, exitPoint);
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


