using TMPro;
using UnityEngine;

public class DateUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dateText;
    [SerializeField] private TextMeshProUGUI _dayOfWeekText;
    [SerializeField] private TextMeshProUGUI _timeText;

    private DayManager _dayManager;

    void Awake()
    {
        _dayManager = FindFirstObjectByType<DayManager>();
        _dayManager.OnDayStart += DayUIUpdate;
    }
    void Update()
    {
        
    }

    void DayUIUpdate()
    {
        
        string dateText = "";
        
        _dateText = _dayManager.DayData.day;
    }

    void OnDestroy()
    {
        _dayManager.OnDayStart -= DayUIUpdate;
    }
}
