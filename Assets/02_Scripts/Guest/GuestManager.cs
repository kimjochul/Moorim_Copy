using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Constants;

/// <summary>
/// Customer 생성 및 관리 매니저
/// </summary>
public class GuestManager : MonoBehaviour
{
    public enum SpawnType { All, Normal, Thief, BadGuest, Rich, Violence }
    [Header("테스트용 스폰 설정")]
    public SpawnType currentSpawnType = SpawnType.All;
    
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
    public GameObject badGuestPrefab;
    public GameObject violenceGuestPrefab;

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
        
        if (badGuestPrefab != null)
            PoolManager.Instance.Register(GuestAppearance.BadGuest.ToString(), badGuestPrefab, 1);

        if (violenceGuestPrefab != null)
            PoolManager.Instance.Register(GuestAppearance.ViolenceGuest.ToString(), violenceGuestPrefab, 1);
    }
    
    void Start()
    {
        if (SeatManager.Instance != null)
        {
            SeatManager.Instance.OnSeatFreed += HandleSeatFreed;
        }
    }

    void OnDisable()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart -= StartSpawn;
            DayManager.Instance.OnDayEnd -= StopSpawn;
        }
        
        if (SeatManager.Instance != null)
        {
            SeatManager.Instance.OnSeatFreed -= HandleSeatFreed;
        }
    }

    private void StartSpawn()
    {
        //InvokeRepeating(nameof(SpawnGuset), startSpawnTime, spawnInterval);
        //Invoke(nameof(SpawnGuset), startSpawnTime);
    }
    
    public GuestBase SpawnGuset()
    {
        bool hasEmptyChair = SeatManager.Instance.FindEmptyChair() != null;
        if (!hasEmptyChair && waitingGuests.Count >= maxWaitingCount) return null;
        
        GuestAppearance appearance = GuestAppearance.NormalMaleA;

        if (currentSpawnType != SpawnType.All)
        {
            switch (currentSpawnType)
            {
                case SpawnType.Normal:
                    appearance = GuestAppearance.NormalFemaleA;
                    break;
                case SpawnType.Thief:
                    appearance = GuestAppearance.ThiefGuy;
                    break;
                case SpawnType.BadGuest:
                    appearance = GuestAppearance.BadGuest; 
                    break;
                case SpawnType.Rich:
                    appearance = GuestAppearance.RichMan;
                    break;
                case SpawnType.Violence:
                    appearance = GuestAppearance.ViolenceGuest;
                    break;
            }
        }
        else
        {
            float rand = UnityEngine.Random.value;
            if (rand < 0.05f)
                appearance = GuestAppearance.ThiefGuy;
            else if (rand < 0.1f)
                appearance = GuestAppearance.RichMan;
            else if (rand < 0.15f) 
                appearance = GuestAppearance.BadGuest;
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, normalGuestPrefabs.Length);
                appearance  = normalGuestPrefabs[randomIndex].appearance;
            }
        }

        return InstantiateAndSetupGuest(appearance);
    }

    public GuestBase SpawnGuset(SpawnType spawnType)
    {
        bool hasEmptyChair = SeatManager.Instance.FindEmptyChair() != null;
        if (!hasEmptyChair && waitingGuests.Count >= maxWaitingCount) return null;

        GuestAppearance appearance = GuestAppearance.NormalMaleA;

        switch (spawnType)
        {
            case SpawnType.Normal:
                if (normalGuestPrefabs.Length > 0)
                    appearance = normalGuestPrefabs[0].appearance;
                break;
            case SpawnType.Thief:
                appearance = GuestAppearance.ThiefGuy;
                break;
            case SpawnType.BadGuest:
                appearance = GuestAppearance.BadGuest; 
                break;
            case SpawnType.Rich:
                appearance = GuestAppearance.RichMan;
                break;
            case SpawnType.Violence:
                appearance = GuestAppearance.ViolenceGuest;
                break;
            case SpawnType.All:
            default:
                if (normalGuestPrefabs.Length > 0)
                    appearance = normalGuestPrefabs[0].appearance;
                break;
        }

        return InstantiateAndSetupGuest(appearance);
    }

    private GuestBase InstantiateAndSetupGuest(GuestAppearance appearance)
    {
        globalIDCounter++; 

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
                    GuestCount++; 
                    OnGuestServed?.Invoke(); 
                };

                guest.OnGuestExited += () =>
                {
                    resultTracker.RecordGuest(guest.CurrentMood); 
                    activeGuests.Remove(guest);
                };

                guest.OnOrderPlaced += (g, order) => { orderPopup.AddOrder(g, order); };
                guest.OnOrderChanged += (g, order) =>
                {
                    orderPopup.RemoveOrder(g);
                    orderPopup.AddOrder(g, order);
                };
                guest.OnOrderServed += (g) => { orderPopup.RemoveOrder(g); };
                guest.OnGuestAngryExited += (g) => { orderPopup.RemoveOrder(g); };

                return guest; // 세팅 끝난 손님 반환
            }
        }
        
        return null;
    }
    
    private void HandleSeatFreed(Chair freedChair)
    {
        // 대기 중인 손님이 있다면
        if (waitingGuests.Count > 0)
        {
            GuestBase firstWaitingGuest = waitingGuests[0];
            
            // 대기열에서 제거
            LeaveWaiting(firstWaitingGuest);
            
            // 손님에게 빈자리 할당 후 입장 상태로 변경
            firstWaitingGuest.AssignChairAndEnter(freedChair);
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


