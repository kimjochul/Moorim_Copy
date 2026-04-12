using UnityEngine;

public class BookController : MonoBehaviour
{
    [SerializeField] private BookView bookView;

    private BookModel bookModel;

    private void Awake()
    {
        this.bookModel = new BookModel();

        this.bookView.OnMenuToggleClicked += HandleMenuClicked;

        for (int i = 0; i < bookView.menus.Length; i++)
        {
            // if문 임시
            if (i == 2)
                break;
            int menuIndex = i;
            this.bookView.menus[i].menuView.OnTabToggleClicked += (tabIndex) => HandleTabClicked(menuIndex, tabIndex);
        }

        this.bookModel.OnMenuChanged += UpdateMenuView;
        this.bookModel.OnTabChanged += UpdateTabView;
    }

    void OnEnable()
    {
        InitData();
    }
    void OnDisable()
    {
        this.bookModel.ChangeMenu(0);
        this.bookModel.ChangeTab(0, 0);
    }


    // 상단 메뉴가 클릭되었을 때 Model에 전달
    private void HandleMenuClicked(int index)
    {
        this.bookModel.ChangeMenu(index);
    }

    // 하단 탭이 클릭되었을 때 Model에 전달
    private void HandleTabClicked(int menuIndex, int tabIndex)
    {
        this.bookModel.ChangeTab(menuIndex, tabIndex);
    }

    // 메뉴 상태가 변했을 때 실제 View의 메뉴와 토글 UI를 동기화
    private void UpdateMenuView(int activeMenuIndex)
    {
        for (int i = 0; i < bookView.menus.Length; i++)
        {
            bool isActive = (i == activeMenuIndex);

            this.bookView.menus[i].toggle.SetIsOnWithoutNotify(isActive);

            this.bookView.menus[i].menuView.ShowMenu(isActive);
        }
    }

    // 탭 상태가 변했을 때, 실제 View의 탭과 토글 UI를 동기화
    private void UpdateTabView(int menuIndex, int activeTabIndex)
    {
        BaseMenu activeMenu = this.bookView.menus[menuIndex].menuView;

        for (int i = 0; i < activeMenu.tabs.Length; i++)
        {
            bool isActive = (i == activeTabIndex);
            BaseTab tab = activeMenu.tabs[i].tabScript;

            activeMenu.tabs[i].toggleTab.SetIsOnWithoutNotify(isActive);
            tab.ShowTab(isActive);

            if (isActive)
            {
                tab.ResetSetting();
            }
        }
    }

    // 책이 처음 열릴 때 모든 탭을 순회하며 초기 데이터 삽입
    private void InitData()
    {
        foreach (var menuPair in this.bookView.menus)
        {
            foreach (var tabPair in menuPair.menuView.tabs)
            {
                if (tabPair.tabScript != null)
                {
                    tabPair.tabScript.SetupData();
                }
            }
        }
    }
}
