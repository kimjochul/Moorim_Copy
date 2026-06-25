using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI 배경 및 텍스트")]
    public GameObject blackBackground;
    public GameObject whiteBackground;
    public TextMeshProUGUI blackTutorialText;
    public TextMeshProUGUI whiteTutorialText;
    public Button nextButton;
    public GameObject tutorialPanel;
    public GameObject blackPanelForTutorialHighlight;

    [Header("기본 가이드 UI")]
    public GameObject wasd;
    public GameObject mouseWheel;
    public GameObject mouseLeft;
    public GameObject mouseRight;

    [Header("조리 및 주방 관련")]
    public HighlightEffect potHighlightEffect;
    public GameObject mainCookingUICanvas;
    public CookingStation cookingStation;
    public HighlightEffect trayTableHighlightEffect;
    public GameObject serviceSlotInven;
    public GameObject putFoodPanel;
    public Inventory servingSlotInventory;
    public CookingView cookingView;

    [Header("캐릭터 및 메뉴 관련")]
    public GameObject pickPlayer;
    public GameObject menuPanel;
    public RecipeListView recipeListView;
    public GameObject recipePanel;
    public GameObject ingredientPanel;

    [Header("손님 및 카메라 관련")]
    public GuestManager guestManager;
    private GuestBase guest;
    public RecipeData mapoTofuRecipe;
    public FoodData mapoTofuFood;
    public GameObject getFoodPanel;

    [Header("특수 기믹 오브젝트")]
    public HighlightEffect gateHighlightEffect0;
    public HighlightEffect gateHighlightEffect1;
    public GameObject richmanPanel;

    [Header("콜라이더")] 
    public GameObject cookingStationForColliders;
    public GameObject trayTableForColliders;

    [Header("부연 설명창")] 
    public GameObject sideBackGround;
    public GameObject guestScalePanel;
    public GameObject playerPickPanel;
    public GameObject slotPanel;
    public GameObject cookingEndPanel;
    
    [Header("튜토리얼 진행 상태")]
    public int eventCount;

    private void Start()
    {
        // 초기 UI 상태 세팅
        blackBackground.SetActive(true);
        whiteBackground.SetActive(false);
        wasd.SetActive(false);
        mouseWheel.SetActive(false);
        mouseLeft.SetActive(false);
        mouseRight.SetActive(false);
        
        pickPlayer.SetActive(false);
        menuPanel.SetActive(false);
        recipePanel.SetActive(false);
        ingredientPanel.SetActive(false);
        putFoodPanel.SetActive(false);
        getFoodPanel.SetActive(false);
        blackPanelForTutorialHighlight.SetActive(false);
        
        SetChildColliders(false, cookingStationForColliders);
        SetChildColliders(false, trayTableForColliders);
    }

    public void StartTutorial()
    {
        // 튜토리얼 단계별 실행
        switch (eventCount)
        {
            case 0: StartCoroutine(MoveControlTutorial()); break;
            case 1: StartCoroutine(ZoomControlTutorial()); break;
            case 2: StartCoroutine(RotatePovControlTutorial()); break;
            case 3: StartCoroutine(MyungwolControlTutorial()); break;
            case 4: StartCoroutine(GuestTutorial()); break;
            case 5: StartCoroutine(SowolControlTutorial()); break;
            case 6: StartCoroutine(CookingTutorial()); break;
            case 7: StartCoroutine(SowolMoveFoodTutorial()); break;
            case 8: StartCoroutine(BringFoodTutorial()); break;
            case 9: StartCoroutine(ThiefGuestTutorial()); break;
            case 10: StartCoroutine(BadGuestTutorial()); break;
            case 11: StartCoroutine(ViolenceGuestTutorial()); break;
            case 12: StartCoroutine(RichManGuestTutorial()); break;
        }
        
        whiteBackground.SetActive(true);
        blackBackground.SetActive(false);
    }

    #region 튜토리얼 단계별 코루틴

    IEnumerator MoveControlTutorial()
    {
        ShowGuide("WASD를 눌러 시점을 이동해보세요.", wasd);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D));
        yield return StartCoroutine(CompleteStep("잘하셨습니다 MoveControlTutorial", "그럼 이제 시점 이동에 대해 배워보겠습니다. 마우스 휠을 드래그해 시점을 확대, 또는 축소해보세요.", wasd));
    }
    
    IEnumerator ZoomControlTutorial()
    {
        ShowGuide("마우스 휠을 위로 올리거나 내려 시점을 옮겨보세요.", mouseWheel);
        yield return new WaitUntil(() => Input.mouseScrollDelta.y != 0f);
        yield return StartCoroutine(CompleteStep("잘하셨습니다 ZoomControlTutorial", "그럼 이제 시점 회전에 대해 알아보겠습니다. 마우스를 우클릭한 채 양 옆으로 움직여보세요. 현재 보고 있는 시점을 기준으로 시야가. 회전합니다.", mouseWheel));
    }
    
    IEnumerator RotatePovControlTutorial()
    {
        ShowGuide("마우스를 우클릭한 채 양 옆으로 움직여보세요.", mouseRight);
        yield return new WaitUntil(() => Input.GetMouseButton(1) && Input.GetAxis("Mouse X") != 0f);
        yield return StartCoroutine(CompleteStep("잘하셨습니다 RotatePovControlTutorial", "이번엔 명월이를 움직이는 법을 알아보겠습니다. 마우스 커서를 객잔 안에 대고 왼쪽 클릭해주세요.", mouseRight));
    }
    
    IEnumerator MyungwolControlTutorial()
    {
        ShowGuide("마우스 커서를 객잔 안에 대고 왼쪽 클릭해주세요.", mouseLeft);
        Transform myungwolTrans = PlayerManager.Instance.GetPlayer(Constants.PlayerType.Myungwol).transform;
        Vector3 startPos = myungwolTrans.position;

        yield return new WaitUntil(() => Vector3.Distance(startPos, myungwolTrans.position) > 0.1f);
        yield return StartCoroutine(CompleteStep("잘하셨습니다 MyungwolControlTutorial", "이제 손님 접대에 대해 알아보겠습니다.", mouseLeft));
    }
    
    IEnumerator GuestTutorial()
    {
        ShowGuide("손님이 들어오고 있습니다. WASD와 마우스를 이용해 객잔을 살펴보세요.");
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Normal);
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
    
        // 손님이 자리에 앉을 때까지 대기
        ToggleHighlight(guest.gameObject, true);
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.WaitOrder);
        ToggleHighlight(guest.gameObject, false);

        tutorialPanel.SetActive(true);
        ShowGuide("손님을 마우스 왼쪽 클릭해 가까이 다가가서 주문을 받아주세요.", mouseLeft);
    
        // 주문 완료 대기
        yield return new WaitUntil(() => guest.currentState != Constants.GuestState.WaitOrder);
        mouseLeft.gameObject.SetActive(false);
    
        yield return StartCoroutine(ShowPanelSequentially(guestScalePanel));
        TransitionToNextTutorial("이제 음식을 만들어보겠습니다.");
    }
    
    IEnumerator SowolControlTutorial()
    {
        yield return StartCoroutine(ShowPanelSequentially(playerPickPanel));
        
        pickPlayer.SetActive(true);
        whiteTutorialText.text = "숫자 2번 키를 눌러주세요.";
        yield return new WaitUntil(() => PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Sowol);
        
        pickPlayer.SetActive(false);
        ShowGuide("마우스 커서를 냄비에 대고 왼쪽 클릭해주세요.", mouseLeft);
        SetChildColliders(true, cookingStationForColliders);

        // 요리 UI가 열릴 때까지 하이라이트 유지
        ToggleHighlight(potHighlightEffect.gameObject, true, potHighlightEffect);
        yield return new WaitUntil(() => mainCookingUICanvas.activeSelf);
        ToggleHighlight(potHighlightEffect.gameObject, false, potHighlightEffect);

        mouseLeft.gameObject.SetActive(false);
        
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GetComponent<Canvas>().sortingOrder = 20;
        TransitionToNextTutorial("냄비로 요리를 시작해보겠습니다.");
    }
    
    IEnumerator CookingTutorial()
    {
        ShowGuide("손님이 주문하신 메뉴를 선택해주세요.");
        menuPanel.SetActive(true);
        
        bool isRecipeClicked = false;
        Action<Recipe> onRecipeSelected = _ => isRecipeClicked = true;
        recipeListView.OnRecipeClicked += onRecipeSelected;

        yield return new WaitUntil(() => isRecipeClicked);
        recipeListView.OnRecipeClicked -= onRecipeSelected;
        
        menuPanel.SetActive(false);
        recipePanel.SetActive(true);

        yield return StartCoroutine(ShowTextAndWaitForNextButton("필요한 재료가 나와있는 레시피가 보입니다. 확인 후 다음 버튼을 눌러주세요."));
        
        ShowGuide("레시피에 맞춰 재료를 클릭해주세요.");
        recipePanel.SetActive(false);
        ingredientPanel.SetActive(true);
        
        bool isCookStartClicked = false;
        Action onCookSelected = () => isCookStartClicked = true;
        cookingView.OnCookClicked += onCookSelected;

        yield return new WaitUntil(() => isCookStartClicked);
        cookingView.OnCookClicked -= onCookSelected;

        yield return StartCoroutine(CompleteStep("잘하셨습니다 CookingTutorial", "이제 만들어진 음식을 옮겨 보겠습니다.", ingredientPanel, 8f));
    }

    IEnumerator SowolMoveFoodTutorial()
    {
        yield return StartCoroutine(ShowPanelSequentially(cookingEndPanel));
        ShowGuide("조리가 끝난 냄비를 마우스 왼쪽 클릭합니다.");
        
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().planeDistance = 1f;
        
        bool isFoodCollected = false;
        Action<CookingState, Sprite> onStateChanged = (state, _) => isFoodCollected = (state == CookingState.Idle);
        cookingStation.OnStateChanged += onStateChanged;
        
        // 음식 수거할 때까지 하이라이트 대기
        ToggleHighlight(potHighlightEffect.gameObject, true, potHighlightEffect);
        yield return new WaitUntil(() => isFoodCollected);
        ToggleHighlight(potHighlightEffect.gameObject, false, potHighlightEffect);
        cookingStation.OnStateChanged -= onStateChanged;
        
        ShowGuide("조리 완료된 음식을 테이블을 클릭해 가져갑니다.");
        SetChildColliders(true, trayTableForColliders);
        yield return StartCoroutine(ShowPanelSequentially(slotPanel));

        // 배식대 인벤토리가 열릴 때까지 대기
        ToggleHighlight(trayTableHighlightEffect.gameObject, true, trayTableHighlightEffect);
        yield return new WaitUntil(() => serviceSlotInven.activeSelf);
        ToggleHighlight(trayTableHighlightEffect.gameObject, false, trayTableHighlightEffect);
        
        ShowGuide("조리 완료된 음식을 눌러 테이블로 옮겨주세요.", putFoodPanel);

        // 배식대에 음식이 놓일 때까지 대기
        yield return new WaitUntil(() => servingSlotInventory != null && servingSlotInventory.Slots.Any(slot => !slot.IsEmpty));

        yield return StartCoroutine(CompleteStep("잘하셨습니다 SowolMoveFoodTutorial", "이제 만들어진 음식을 손님에게 가져가야 합니다.", putFoodPanel, 2f));
        serviceSlotInven.SetActive(false);
    }
    
    IEnumerator BringFoodTutorial()
    {
        ShowGuide("숫자 1을 눌러 명월로 바꿔주세요", pickPlayer);
        yield return new WaitUntil(() => PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Myungwol);
        pickPlayer.SetActive(false);
        
        ShowGuide("명월인 상태로 트레이 테이블을 클릭해주세요.");
        
        // 배식대 UI 대기
        ToggleHighlight(trayTableHighlightEffect.gameObject, true, trayTableHighlightEffect);
        yield return new WaitUntil(() => serviceSlotInven.activeSelf);
        ToggleHighlight(trayTableHighlightEffect.gameObject, false, trayTableHighlightEffect);
        
        ShowGuide("트레이 테이블 슬롯에 있는 음식을 눌러 명월의 인벤토리로 가져올 수 있습니다.", getFoodPanel);
        
        // 명월 인벤토리에 음식 들어올 때까지 대기
        Inventory myungwolInventory = InventoryManager.Instance.GetInventory(Constants.InventoryType.Server);
        yield return new WaitUntil(() => myungwolInventory != null && myungwolInventory.Slots.Any(slot => !slot.IsEmpty));

        getFoodPanel.SetActive(false);
        ShowGuide("잘하셨습니다! 이제 음식을 들고 손님을 클릭해 서빙해주세요.", mouseLeft);
        
        // 손님이 음식 받을 때까지 대기
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        ToggleHighlight(guest.gameObject, true, guestHighlightEffect);
        yield return new WaitUntil(() => guest.currentState != Constants.GuestState.WaitFood);
        ToggleHighlight(guest.gameObject, false, guestHighlightEffect);
        
        mouseLeft.gameObject.SetActive(false);

        if (guest.currentState == Constants.GuestState.Eat)
        {
            whiteTutorialText.text = "일반 손님 접대 튜토리얼을 무사히 완료했습니다!";
            yield return new WaitForSeconds(3f);
            TransitionToNextTutorial("이제부터는 객잔에 찾아오는 특수한 손님들에 대해 알아보겠습니다.");
        }
        else
        {
            yield return StartCoroutine(HandleNormalGuestFailure());
        }
    }

    IEnumerator HandleNormalGuestFailure()
    {
        whiteTutorialText.text = "앗! 잘못된 음식을 가져다 주어 손님이 화가 나서 돌아가버렸습니다...\n다시 올바른 요리를 만들어 서빙해 봅시다.";
        if (guest != null && guest.gameObject.activeInHierarchy) guest.ForceExit();

        yield return new WaitForSeconds(4f);
        eventCount = 4;
        StartTutorial(); 
    }

    IEnumerator ThiefGuestTutorial()
    {
        ShowGuide("새로운 손님이 오고 있습니다. 기존처럼 주문을 받고 음식을 서빙해주세요.");
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Thief);
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        // 도둑이 먹기 시작하면 문을 닫으라고 알림
        guest.GetComponentInChildren<HighlightEffect>().highlighted = true;
        if (gateHighlightEffect0) gateHighlightEffect0.highlighted = true;
        if (gateHighlightEffect1) gateHighlightEffect1.highlighted = true;
            
        whiteTutorialText.text = "도둑 손님이었습니다. 문을 닫아 빠져나가지 못하게 하세요.";
        ThiefGuyGuest thiefGuest = guest as ThiefGuyGuest;
        bool isGateClosedOnce = false; 
        
        while (thiefGuest != null && !thiefGuest.isCaught)
        {
            // 도둑 탈출 실패 처리
            if (!thiefGuest.gameObject.activeInHierarchy)
            {
                yield return StartCoroutine(HandleTutorialFailure());
                yield break; 
            }

            // 문이 닫히면 잡으라고 안내
            if (!isGateClosedOnce && !Gate.isOpen)
            {
                isGateClosedOnce = true;
                if (gateHighlightEffect0) gateHighlightEffect0.highlighted = false;
                if (gateHighlightEffect1) gateHighlightEffect1.highlighted = false;
                
                ShowGuide("문이 닫혀서 도둑이 나가지 못하게 되었습니다. 도둑을 쫓아 붙잡으세요.", mouseLeft);
            }
            yield return null;
        }
        
        guest.GetComponentInChildren<HighlightEffect>().highlighted = false;
        mouseLeft.gameObject.SetActive(false);
        whiteTutorialText.text = "도둑을 무사히 잡아서 음식값을 받아냈습니다.";
        
        yield return new WaitForSeconds(5f);
        TransitionToNextTutorial("다음 손님 모시겠습니다.");
    }

    IEnumerator HandleTutorialFailure()
    {
        mouseLeft.gameObject.SetActive(false);
        if (gateHighlightEffect0) gateHighlightEffect0.highlighted = false;
        if (gateHighlightEffect1) gateHighlightEffect1.highlighted = false;
        
        whiteTutorialText.text = "도둑을 놓쳐서 탈출해버렸습니다... 튜토리얼을 다시 시도합니다.";
        if (guest != null) guest.ForceExit();

        yield return new WaitForSeconds(3f);
        StartTutorial(); 
    }

    IEnumerator BadGuestTutorial()
    {
        ShowGuide("이번에는 취객이 오고 있습니다. 이 손님은 주문을 한 뒤 변덕을 부려 다른 메뉴로 바꿉니다.");
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.BadGuest);
        guest.TimeSetForTutorial(); 
        
        guest.GetComponentInChildren<HighlightEffect>().highlighted = true;
        ShowGuide("먼저 손님을 마우스 왼쪽 클릭해 주문을 받아보세요.", mouseLeft);
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.WaitFood);
        ShowGuide("하지만 바로 요리하지 말고 손님의 말풍선을 잘 지켜보세요");
        mouseLeft.gameObject.SetActive(false);
        
        bool isOrderChanged = false;
        Action<GuestBase, GuestOrderData> onOrderChanged = (g, order) => isOrderChanged = true;
        guest.OnOrderChanged += onOrderChanged;
        
        // 주문이 바뀌거나 밥을 먹기 시작할 때까지 대기
        yield return new WaitUntil(() => isOrderChanged || guest.currentState == Constants.GuestState.Eat);
        guest.OnOrderChanged -= onOrderChanged; 
        
        if (guest.currentState != Constants.GuestState.Eat)
            whiteTutorialText.text = "손님이 변덕을 부려 다른 음식으로 주문을 바꿨습니다. 바뀐 메뉴에 맞춰 요리한 후 서빙해주세요.";
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        guest.GetComponentInChildren<HighlightEffect>().highlighted = false;
        whiteTutorialText.text = "바뀐 주문에 맞춰 무사히 음식을 대접했습니다.";
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Exit || !guest.gameObject.activeInHierarchy);
        whiteTutorialText.text = "잘하셨습니다. 진상 손님이 오면 주문을 바꿀 수 있으니 항상 말풍선을 주의 깊게 확인하세요.";
        
        yield return new WaitForSeconds(5f);
        TransitionToNextTutorial("다음엔 괴한 손님에 대해 알아보겠습니다.");
    }

    IEnumerator ViolenceGuestTutorial()
    {
        ShowGuide("이번에 오는 손님은 괴한입니다. 평소에는 일반 손님과 같지만, 기분이 나빠지면 객잔에서 난동을 부립니다.");
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Violence);
        guest.GetComponentInChildren<HighlightEffect>().highlighted = true;
        
        whiteTutorialText.text = "이 손님이 난동을 부리는 모습을 확인하기 위해, 이번에는 일부러 주문을 받지 말고 손님의 기분이 나빠질 때까지 가만히 지켜보세요.";
        
        bool isRiotStarted = false;
        Action<GuestBase, Constants.GuestMood> onMoodChanged = (g, mood) => {
            if (mood == Constants.GuestMood.Bad || mood == Constants.GuestMood.VeryBad) isRiotStarted = true;
        };
        guest.OnGuestMoodChanged += onMoodChanged;
        
        // 난동 피울 때까지 대기
        yield return new WaitUntil(() => isRiotStarted);
        guest.OnGuestMoodChanged -= onMoodChanged; 
        
        ShowGuide("손님의 기분이 나빠져 난동을 부리기 시작했습니다. 마우스 왼쪽 클릭을 여러 번 해서 객잔 밖으로 쫓아내세요!", mouseLeft);
        yield return new WaitUntil(() => !guest.gameObject.activeInHierarchy);
        
        guest.GetComponentInChildren<HighlightEffect>().highlighted = false;
        mouseLeft.gameObject.SetActive(false);
        whiteTutorialText.text = "무사히 난동 손님을 제압하고 쫓아냈습니다";
        
        yield return new WaitForSeconds(5f);
        TransitionToNextTutorial("이제 마지막 손님을 맞이할 준비를 합시다.");
    }

    IEnumerator RichManGuestTutorial()
    {
        ShowGuide("마지막으로 부자가 오고 있습니다! 옷차림부터 아주 화려합니다.");
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Rich);
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
        guest.GetComponentInChildren<HighlightEffect>().highlighted = true;
        
        whiteTutorialText.text = "부자 손님은 음식을 대접하면 훨씬 더 많은 돈과 팁을 지불합니다. 단, 그만큼 인내심이 적습니다.";
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        guest.GetComponentInChildren<HighlightEffect>().highlighted = false;
        whiteTutorialText.text = "부자 손님이 만족하며 식사를 하고 있습니다. 부자 손님의 보너스는 아래와 같습니다";
        richmanPanel.SetActive(true);
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Exit || !guest.gameObject.activeInHierarchy);
        
        richmanPanel.SetActive(false);
        whiteTutorialText.text = "부자 손님이 기분에 따른 보너스를 주고 갔습니다. ";
        
        yield return new WaitForSeconds(5f);
        TransitionToNextTutorial("이것으로 객잔 운영에 필요한 모든 튜토리얼을 마칩니다. 수고하셨습니다.");
    }

    #endregion

    #region 공통 유틸리티 메서드 (헬퍼)

    // 가이드 텍스트 및 UI 활성화 처리
    private void ShowGuide(string text, GameObject guideUI = null)
    {
        whiteTutorialText.text = text;
        if (guideUI != null) guideUI.SetActive(true);
    }

    // 튜토리얼 단계를 완료하고 다음 메시지 대기
    private IEnumerator CompleteStep(string successMsg, string nextMsg, GameObject guideUI = null, float waitTime = 5f)
    {
        whiteTutorialText.text = successMsg;
        if (guideUI != null) guideUI.SetActive(false);
        yield return new WaitForSeconds(waitTime);
        TransitionToNextTutorial(nextMsg);
    }

    // 튜토리얼 하이라이트 및 레이어 일괄 토글
    private void ToggleHighlight(GameObject target, bool isOn, HighlightEffect effect = null)
    {
        blackPanelForTutorialHighlight.SetActive(isOn);
        SetLayerRecursively(target, LayerMask.NameToLayer(isOn ? "Tutorial Highlight" : "Default"));
        if (effect != null) effect.highlighted = isOn;
    }

    // 다음 튜토리얼 텍스트 준비
    private void TransitionToNextTutorial(string nextMessage)
    {
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        blackTutorialText.text = nextMessage;
    }

    private IEnumerator ShowTextAndWaitForNextButton(string message)
    {
        whiteTutorialText.text = message;
        nextButton.gameObject.SetActive(true);
    
        bool isNextClicked = false;
        UnityEngine.Events.UnityAction onNextClicked = () => isNextClicked = true;
    
        nextButton.onClick.AddListener(onNextClicked);
        yield return new WaitUntil(() => isNextClicked);
    
        nextButton.onClick.RemoveListener(onNextClicked);
        nextButton.gameObject.SetActive(false);
    }
    
    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    #endregion
    
    public void SetChildColliders(bool isEnabled, GameObject targetObject)
    {
        if (targetObject == null) return;

        Collider[] allColliders = targetObject.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in allColliders)
        {
            col.enabled = isEnabled;
        }
    }
    
    private IEnumerator ShowPanelSequentially(GameObject panelParent)
    {
        yield return null; 
        
        sideBackGround.SetActive(true);
        panelParent.SetActive(true);
        int childCount = panelParent.transform.childCount;

        // 첫 번째 자식만 활성화
        for (int i = 0; i < childCount; i++)
        {
            panelParent.transform.GetChild(i).gameObject.SetActive(i == 0);
        }

        for (int i = 0; i < childCount; i++)
        {
            Transform currentChild = panelParent.transform.GetChild(i);
            Button childNextButton = currentChild.GetComponentInChildren<Button>(true);
            
            if (childNextButton != null)
            {
                bool isNextClicked = false;
                UnityEngine.Events.UnityAction onNextClicked = () => isNextClicked = true;
                
                childNextButton.onClick.AddListener(onNextClicked);
                yield return new WaitUntil(() => isNextClicked); // 버튼 클릭 대기
                childNextButton.onClick.RemoveListener(onNextClicked);
            }
            else
            {
                Debug.LogWarning($"{currentChild.name}에 Next Button이 없습니다. 2초 대기합니다.");
                yield return new WaitForSeconds(2f);
            }

            currentChild.gameObject.SetActive(false);

            if (i + 1 < childCount)
            {
                panelParent.transform.GetChild(i + 1).gameObject.SetActive(true);
            }
        }

        // 전체 종료
        panelParent.SetActive(false);
        sideBackGround.SetActive(false);
    }
}