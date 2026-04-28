using System;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class GuestManager : MonoBehaviour
{
    [Header("연결해야할 외부 정보")]
    [SerializeField] private GuestResultTracker resultTracker;
    [SerializeField] private OrdersPopupController orderPopup;

    [Header("Target Point")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;

    [Header("Spawn Info")]
    [SerializeField] private float startSpawnTime = 1f;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Watitng Point")]
    [SerializeField] private int maxWaitingCount = 4;
    [SerializeField] private Transform[] waitingPoint;

    [Serializable]
    public struct GuestPrefabEntry
    {
       public GameObject prefab;
       public GuestAppearance appearance;
    }

    [SerializeField] private GuestPrefabEntry[] normalGuestPrefabs;
    public GameObject richGuestPrefab;
    public GameObject thiefGuestPrefab;

    public static event Action OnGuestServed;
    private int GuestCount = 0;

    // 손님 런타임 정보
    private int globalIDCounter = 0;

    // 현재 소환된 손님들 리스트
    private List<GuestBase> activeGuests = new List<GuestBase>();
    
    // 웨이팅 목록
    private List<GuestBase> waitingGuests = new List<GuestBase>();

    void Awake()
    {
        
        DayManager.Instance.OnDayStart += StartSpawn;
        DayManager.Instance.OnDayEnd += StopSpawn;

        // 풀 매니저에 프리팹 등록
        foreach (var entry in normalGuestPrefabs)
        {
            PoolManager.Instance.Register(entry.appearance.ToString(), entry.prefab, 3);
        }

        if (richGuestPrefab != null)
            PoolManager.Instance.Register(GuestAppearance.RichMan.ToString(), richGuestPrefab, 1);

        if (thiefGuestPrefab != null)
            PoolManager.Instance.Register(GuestAppearance.ThiefGuy.ToString(), thiefGuestPrefab, 1);
    }

    void OnDisable()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart -= StartSpawn;
            DayManager.Instance.OnDayEnd -= StopSpawn;
        }
    }

    private void StartSpawn()
    {
        InvokeRepeating(nameof(SpawnGuset), startSpawnTime, spawnInterval);
    }

    private void SpawnGuset()
    {
        // 빈자리도 없고 웨이팅도 꽉 차면 스폰 중지
        bool hasEmptyChair = SeatManager.Instance.FindEmptyChair() != null;

        if (!hasEmptyChair && waitingGuests.Count >= maxWaitingCount) return;
        
        globalIDCounter++; // 스폰될 때마다 고유 id 1씩 증가

        GuestAppearance appearance = GuestAppearance.NormalMaleA;

        // 확률 돌려서 손님 타입과 외형 결정 (0~1사이 랜덤)
        float rand = UnityEngine.Random.value;
        if (rand < 0.05f)
            appearance = GuestAppearance.ThiefGuy;
        else if (rand < 0.1f)
            appearance = GuestAppearance.RichMan;
        else
        {
            int randomIndex = UnityEngine.Random.Range(0, normalGuestPrefabs.Length);
            appearance  = normalGuestPrefabs[randomIndex].appearance;
        }

        // 풀에서 타입에 맞는 오브젝트 꺼냄
        GameObject guestObj = PoolManager.Instance.Get(appearance.ToString());
        
        if (guestObj != null)
        {
            guestObj.transform.position = spawnPoint.position;
            guestObj.transform.rotation = Quaternion.identity;

            GuestBase guest = guestObj.GetComponent<GuestBase>();

            if (guest != null)
            {
                activeGuests.Add(guest);
                
                guest.ResetState();
                guest.Init(globalIDCounter, appearance, exitPoint, this);

                guest.OnGuestPaid += () =>
                {
                    GuestCount++; // 서빙 완료된 손님 수 +1
                    OnGuestServed?.Invoke(); // 외부에 손님 한 명 끝났다고 알림
                };

                guest.OnGuestExited += () =>
                {
                    resultTracker.RecordGuest(guest.CurrentMood); // 기분별 집계
                    activeGuests.Remove(guest);
                };

                guest.OnOrderPlaced += (g, order) =>
                {
                    orderPopup.AddOrder(g, order);
                };

                guest.OnOrderServed += (g) =>
                {
                    orderPopup.RemoveOrder(g);  // 서빙 완료 시 제거
                };

                guest.OnGuestAngryExited += (g) =>
                {
                    orderPopup.RemoveOrder(g);
                };
            }
        }
    }

    public bool JoinWaiting(GuestBase guest, out Vector3 waitPosition)
    {
        waitPosition = Vector3.zero;

        if (waitingGuests.Count >= maxWaitingCount)
            return false;
        
        int pointIndex = waitingGuests.Count;
        waitingGuests.Add(guest);

        if (waitingPoint != null && pointIndex < waitingPoint.Length)
            waitPosition = waitingPoint[pointIndex].position;
        else
            waitPosition = spawnPoint.position;

        return true;
    }

    public void LeaveWaiting(GuestBase guest)
    {
        if (waitingGuests.Remove(guest))
            RearrangeWaitingSpots();
    }

    private void RearrangeWaitingSpots()
    {
        for (int i = 0; i < waitingGuests.Count; i++)
        {
            if (waitingPoint != null && i < waitingPoint.Length)
            {
                GuestMover mover = waitingGuests[i].GetComponent<GuestMover>();

                if (mover != null)
                    mover.MoveTo(waitingPoint[i].position);
                else
                    waitingGuests[i].transform.position = waitingPoint[i].position;
            }
        }
    }

    public void StopSpawn()
    {
        CancelInvoke();
        ForceAllGuestsExit();
    }

    private void ForceAllGuestsExit()
    {
        foreach (var guest in activeGuests)
        {
            if (guest == null) continue;

            guest.ForceExit();
        }

        waitingGuests.Clear();
    }
}


