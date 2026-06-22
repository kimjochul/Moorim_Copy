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
    public GameObject blackPanel;

    [Header("조작 가이드 UI")]
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

    [Header("Red Square 애니메이션 설정 (고정 좌표 입력)")]
    public GameObject redSquare0;
    public GameObject redSquare1;
    public GameObject redSquare2;
    public GameObject redSquare3;

    [Header("키 제어")] 
    public bool isInputLocked = false;
    public List<KeyCode> allowedKeys = new List<KeyCode>();

    [Header("콜라이더")] 
    public GameObject cookingStationForColliders;
    public GameObject trayTableForColliders;
    
    [Header("튜토리얼 진행 상태")]
    public int eventCount;

    private void Start()
    {
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
        blackPanel.SetActive(false);
        
        SetChildColliders(false, cookingStationForColliders);
        SetChildColliders(false, trayTableForColliders);
    }

   

    public void StartTutorial()
    {
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
        whiteTutorialText.text = "WASD를 눌러 시점을 이동해보세요.";
        wasd.gameObject.SetActive(true);
        
        
        yield return new WaitUntil(() => 
            Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || 
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)
        );
        
        whiteTutorialText.text = "잘하셨습니다 MoveControlTutorial";
        yield return new WaitForSeconds(5f);


        wasd.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;

        blackTutorialText.text = "그럼 이제 시점 이동에 대해 배워보겠습니다. 마우스 휠을 드래그해 시점을 확대, 또는 축소해보세요.";
    }
    
    IEnumerator ZoomControlTutorial()
    {
        whiteTutorialText.text = "마우스 휠을 위로 올리거나 내려 시점을 옮겨보세요.";
        mouseWheel.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => Input.mouseScrollDelta.y != 0f);
        
        whiteTutorialText.text = "잘하셨습니다 ZoomControlTutorial";
        yield return new WaitForSeconds(5f);

        mouseWheel.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;

        blackTutorialText.text = "그럼 이제 시점 회전에 대해 알아보겠습니다. 마우스를 우클릭한 채 양 옆으로 움직여보세요. 현재 보고 있는 시점을 기준으로 시야가. 회전합니다.";
    }
    
    IEnumerator RotatePovControlTutorial()
    {
        whiteTutorialText.text = "마우스를 우클릭한 채 양 옆으로 움직여보세요.";
        mouseRight.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => Input.GetMouseButton(1) && Input.GetAxis("Mouse X") != 0f);
        
        whiteTutorialText.text = "잘하셨습니다 RotatePovControlTutorial";
        yield return new WaitForSeconds(5f);

        mouseRight.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;

        blackTutorialText.text = "이번엔 명월이를 움직이는 법을 알아보겠습니다. 마우스 커서를 객잔 안에 대고 왼쪽 클릭해주세요.";
    }
    
    IEnumerator MyungwolControlTutorial()
    {
        whiteTutorialText.text = "마우스 커서를 객잔 안에 대고 왼쪽 클릭해주세요.";
        mouseLeft.gameObject.SetActive(true);
        
        Transform myungwolTransform = PlayerManager.Instance.GetPlayer(Constants.PlayerType.Myungwol).transform;
        Vector3 startPosition = myungwolTransform.position;

        yield return new WaitUntil(() => Vector3.Distance(startPosition, myungwolTransform.position) > 0.1f);
        
        whiteTutorialText.text = "잘하셨습니다 MyungwolControlTutorial";
        yield return new WaitForSeconds(5f);

        mouseLeft.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;

        blackTutorialText.text = "이제 손님 접대에 대해 알아보겠습니다.";
    }
    
    IEnumerator GuestTutorial()
    {
        whiteTutorialText.text = "손님이 들어오고 있습니다. WASD와 마우스를 이용해 객잔을 살펴보세요.";
        blackPanel.SetActive(true);
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Normal);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));
        
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
    
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.WaitOrder);

        blackPanel.SetActive(false);
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Default"));

        tutorialPanel.SetActive(true);
        whiteTutorialText.text = "손님을 마우스 왼쪽 클릭해 가까이 다가가서 주문을 받아주세요.";
        mouseLeft.gameObject.SetActive(true);
    
        while (guest.currentState == Constants.GuestState.WaitOrder)
        {
            guestHighlightEffect.highlighted = true;
            yield return null; 
        }

        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);
    
        blackPanel.SetActive(true);
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));

        yield return StartCoroutine(ShowTextAndWaitForNextButton(
            "손님 머리위에 떠있는 음식을 확인해주세요. 옆의 표정은 손님의 기분입니다. 주문을 늦게 받거나, 음식을 늦게 내올 수록 점점 손님의 기분이 나빠집니다. 잘못된 음식을 가져다주면 손님의 기분은 최악이 됩니다."
        ));
        blackPanel.SetActive(false);
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Default"));

        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
    
        blackTutorialText.text = "이제 음식을 만들어보겠습니다.";
    }
    
    IEnumerator SowolControlTutorial()
    {
        pickPlayer.SetActive(true);
        whiteTutorialText.text = "숫자 2번 키를 눌러주세요.";

        redSquare3.SetActive(true);
        yield return new WaitUntil(() => PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Sowol);
        redSquare3.SetActive(false);
        
        pickPlayer.SetActive(false);
        mouseLeft.gameObject.SetActive(true);
        
        potHighlightEffect.highlighted = true;
        whiteTutorialText.gameObject.SetActive(true);
        
        whiteTutorialText.text = "마우스 커서를 냄비에 대고 왼쪽 클릭해주세요.";
        SetChildColliders(true, cookingStationForColliders);

        blackPanel.SetActive(true);
        SetLayerRecursively(potHighlightEffect.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));
        
        while (!mainCookingUICanvas.activeSelf) 
        {
            potHighlightEffect.highlighted = true;
            yield return null; 
        }
        
        SetLayerRecursively(potHighlightEffect.gameObject, LayerMask.NameToLayer("Default"));
        potHighlightEffect.highlighted = false; 
        blackPanel.SetActive(false);

        mouseLeft.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GetComponent<Canvas>().sortingOrder = 20;


        blackTutorialText.text = "냄비로 요리를 시작해보겠습니다.";
    }
    
    IEnumerator CookingTutorial()
    {
        whiteTutorialText.text = "손님이 주문하신 메뉴를 선택해주세요.";
        menuPanel.SetActive(true);
        
        bool isRecipeClicked = false;
        Action<Recipe> onRecipeSelected = _ => isRecipeClicked = true;
        recipeListView.OnRecipeClicked += onRecipeSelected;

        yield return new WaitUntil(() => isRecipeClicked);
        recipeListView.OnRecipeClicked -= onRecipeSelected;
        
        menuPanel.SetActive(false);
        recipePanel.SetActive(true);

        yield return StartCoroutine(ShowTextAndWaitForNextButton("필요한 재료가 나와있는 레시피가 보입니다. 확인 후 다음 버튼을 눌러주세요."));
        
        whiteTutorialText.text = "레시피에 맞춰 재료를 클릭해주세요.";
        recipePanel.SetActive(false);
        ingredientPanel.SetActive(true);
        
        bool isCookStartClicked = false;
        Action onCookSelected = () => isCookStartClicked = true;
        cookingView.OnCookClicked += onCookSelected;

        yield return new WaitUntil(() => isCookStartClicked);
        cookingView.OnCookClicked -= onCookSelected;

        ingredientPanel.SetActive(false);
        
        whiteTutorialText.text = "잘하셨습니다 CookingTutorial";
        yield return new WaitForSeconds(8f);

        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "이제 만들어진 음식을 옮겨 보겠습니다.";
    }

    IEnumerator SowolMoveFoodTutorial()
    {
        whiteTutorialText.text = "조리가 끝난 냄비를 마우스 왼쪽 클릭합니다.";
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().planeDistance = 1f;
        
        bool isFoodCollected = false;
        Action<CookingState, Sprite> onStateChanged = (state, _) => isFoodCollected = (state == CookingState.Idle);
        cookingStation.OnStateChanged += onStateChanged;
        
        blackPanel.SetActive(true);
        SetLayerRecursively(potHighlightEffect.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));

        while (!isFoodCollected)
        {
            potHighlightEffect.highlighted = true;
            yield return null; 
        }

        blackPanel.SetActive(false);
        SetLayerRecursively(potHighlightEffect.gameObject, LayerMask.NameToLayer("Default"));
        
        cookingStation.OnStateChanged -= onStateChanged;
        potHighlightEffect.highlighted = false; 
        
        whiteTutorialText.text = "조리 완료된 음식을 테이블을 클릭해 가져갑니다.";
        SetChildColliders(true, trayTableForColliders);

        
        blackPanel.SetActive(true);
        SetLayerRecursively(trayTableHighlightEffect.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));
        while (!serviceSlotInven.activeSelf) 
        {
            trayTableHighlightEffect.highlighted = true;
            yield return null; 
        }
        trayTableHighlightEffect.highlighted = false;
        blackPanel.SetActive(false);
        SetLayerRecursively(trayTableHighlightEffect.gameObject, LayerMask.NameToLayer("Default"));
        
        whiteTutorialText.text = "조리 완료된 음식을 눌러 테이블로 옮겨주세요.";
        putFoodPanel.SetActive(true);

        bool isFoodServed = false;
        while (!isFoodServed)
        {
            if (servingSlotInventory != null)
            {
                foreach (var slot in servingSlotInventory.Slots)
                {
                    if (!slot.IsEmpty) 
                    {
                        isFoodServed = true;
                        break;
                    }
                }
            }
            yield return null; 
        }

        putFoodPanel.SetActive(false);
        whiteTutorialText.text = "잘하셨습니다 SowolMoveFoodTutorial";
        yield return new WaitForSeconds(2f);

        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        serviceSlotInven.SetActive(false);
        
        blackTutorialText.text = "이제 만들어진 음식을 손님에게 가져가야 합니다.";
    }
    
    IEnumerator BringFoodTutorial()
    {
        whiteTutorialText.text = "숫자 1을 눌러 명월로 바꿔주세요";
        pickPlayer.SetActive(true);
        
        redSquare1.SetActive(true);
        yield return new WaitUntil(() => PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Myungwol);
        redSquare1.SetActive(false);

        pickPlayer.SetActive(false);
        
        whiteTutorialText.text = "명월인 상태로 트레이 테이블을 클릭해주세요.";

        while (!serviceSlotInven.activeSelf) 
        {
            trayTableHighlightEffect.highlighted = true;
            yield return null; 
        }
        trayTableHighlightEffect.highlighted = false;
        
        whiteTutorialText.text = "트레이 테이블 슬롯에 있는 음식을 눌러 명월의 인벤토리로 가져올 수 있습니다.";
        getFoodPanel.SetActive(true);
        
        Inventory myungwolInventory = InventoryManager.Instance.GetInventory(Constants.InventoryType.Server);
        bool isFoodPickedUp = false;
        
        while (!isFoodPickedUp)
        {
            if (myungwolInventory != null)
            {
                foreach (var slot in myungwolInventory.Slots)
                {
                    if (!slot.IsEmpty) 
                    {
                        isFoodPickedUp = true;
                        break;
                    }
                }
            }
            yield return null; 
        }

        getFoodPanel.SetActive(false);
        whiteTutorialText.text = "잘하셨습니다! 이제 음식을 들고 손님을 클릭해 서빙해주세요.";
        mouseLeft.gameObject.SetActive(true);
        
        blackPanel.SetActive(true);
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Tutorial Highlight"));
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();

        while (guest.currentState == Constants.GuestState.WaitFood)
        {
            guestHighlightEffect.highlighted = true;
            yield return null;
        }

        blackPanel.SetActive(false);
        SetLayerRecursively(guest.gameObject, LayerMask.NameToLayer("Default"));
        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);

        if (guest.currentState == Constants.GuestState.Eat)
        {
            whiteTutorialText.text = "일반 손님 접대 튜토리얼을 무사히 완료했습니다!";
            yield return new WaitForSeconds(3f);

            whiteBackground.SetActive(false);
            blackBackground.SetActive(true);
            eventCount++; 
            blackTutorialText.text = "이제부터는 객잔에 찾아오는 특수한 손님들에 대해 알아보겠습니다.";
        }
        else
        {
            yield return StartCoroutine(HandleNormalGuestFailure());
        }
    }

    IEnumerator HandleNormalGuestFailure()
    {
        whiteTutorialText.text = "앗! 잘못된 음식을 가져다 주어 손님이 화가 나서 돌아가버렸습니다...\n다시 올바른 요리를 만들어 서빙해 봅시다.";
        if (guest != null && guest.gameObject.activeInHierarchy)
        {
            guest.ForceExit();
        }

        yield return new WaitForSeconds(4f);
        eventCount = 4;
        StartTutorial(); 
    }

    IEnumerator ThiefGuestTutorial()
    {
        whiteTutorialText.text = "새로운 손님이 오고 있습니다. 기존처럼 주문을 받고 음식을 서빙해주세요.";
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Thief);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        guestHighlightEffect.highlighted = true;
        if (gateHighlightEffect0 != null && gateHighlightEffect1 != null)
        {
            gateHighlightEffect0.highlighted = true;
            gateHighlightEffect1.highlighted = true;
        }
            
        whiteTutorialText.text = "도둑 손님이었습니다. 문을 닫아 빠져나가지 못하게 하세요.";
        
        ThiefGuyGuest thiefGuest = guest as ThiefGuyGuest;
        bool isGateClosedOnce = false; 
        
        while (thiefGuest != null && !thiefGuest.isCaught)
        {
            if (!thiefGuest.gameObject.activeInHierarchy)
            {
                yield return StartCoroutine(HandleTutorialFailure());
                yield break; 
            }

            if (!isGateClosedOnce && !Gate.isOpen)
            {
                isGateClosedOnce = true;
                if (gateHighlightEffect0 != null && gateHighlightEffect1 != null)
                {
                    gateHighlightEffect0.highlighted = false;
                    gateHighlightEffect1.highlighted = false;
                }
                whiteTutorialText.text = "문이 닫혀서 도둑이 나가지 못하게 되었습니다. 도둑을 쫓아 붙잡으세요.";
                mouseLeft.gameObject.SetActive(true);
            }
            yield return null;
        }
        
        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);
        whiteTutorialText.text = "도둑을 무사히 잡아서 음식값을 받아냈습니다.";
        
        yield return new WaitForSeconds(5f);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "다음 손님 모시겠습니다.";
    }

    IEnumerator HandleTutorialFailure()
    {
        mouseLeft.gameObject.SetActive(false);
        if (gateHighlightEffect0 != null && gateHighlightEffect1 != null)
        {
            gateHighlightEffect0.highlighted = false;
            gateHighlightEffect1.highlighted = false;
        }
        
        whiteTutorialText.text = "도둑을 놓쳐서 탈출해버렸습니다... 튜토리얼을 다시 시도합니다.";
        if (guest != null)
        {
            guest.ForceExit();
        }

        yield return new WaitForSeconds(3f);
        StartTutorial(); 
    }

    IEnumerator BadGuestTutorial()
    {
        whiteTutorialText.text = "이번에는 취객이 오고 있습니다. 이 손님은 주문을 한 뒤 변덕을 부려 다른 메뉴로 바꿉니다.";
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.BadGuest);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        guest.TimeSetForTutorial(); 
        
        guestHighlightEffect.highlighted = true;
        mouseLeft.gameObject.SetActive(true);
        whiteTutorialText.text = "먼저 손님을 마우스 왼쪽 클릭해 주문을 받아보세요.";
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.WaitFood);
        mouseLeft.gameObject.SetActive(false);
        whiteTutorialText.text = "하지만 바로 요리하지 말고 손님의 말풍선을 잘 지켜보세요";
        
        bool isOrderChanged = false;
        Action<GuestBase, GuestOrderData> onOrderChanged = (g, order) => { isOrderChanged = true; };
        guest.OnOrderChanged += onOrderChanged;
        
        yield return new WaitUntil(() => isOrderChanged || guest.currentState == Constants.GuestState.Eat);
        guest.OnOrderChanged -= onOrderChanged; 
        
        if (guest.currentState != Constants.GuestState.Eat)
        {
            whiteTutorialText.text = "손님이 변덕을 부려 다른 음식으로 주문을 바꿨습니다. 바뀐 메뉴에 맞춰 요리한 후 서빙해주세요.";
        }
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        guestHighlightEffect.highlighted = false;
        whiteTutorialText.text = "바뀐 주문에 맞춰 무사히 음식을 대접했습니다.";
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Exit || !guest.gameObject.activeInHierarchy);
        whiteTutorialText.text = "잘하셨습니다. 진상 손님이 오면 주문을 바꿀 수 있으니 항상 말풍선을 주의 깊게 확인하세요.";
        
        yield return new WaitForSeconds(5f);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "다음엔 괴한 손님에 대해 알아보겠습니다.";
    }

    IEnumerator ViolenceGuestTutorial()
    {
        whiteTutorialText.text = "이번에 오는 손님은 괴한입니다. 평소에는 일반 손님과 같지만, 기분이 나빠지면 객잔에서 난동을 부립니다.";
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Violence);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        guestHighlightEffect.highlighted = true;
        
        whiteTutorialText.text = "이 손님이 난동을 부리는 모습을 확인하기 위해, 이번에는 일부러 주문을 받지 말고 손님의 기분이 나빠질 때까지 가만히 지켜보세요.";
        
        bool isRiotStarted = false;
        Action<GuestBase, Constants.GuestMood> onMoodChanged = (g, mood) =>
        {
            if (mood == Constants.GuestMood.Bad || mood == Constants.GuestMood.VeryBad)
            {
                isRiotStarted = true;
            }
        };
        guest.OnGuestMoodChanged += onMoodChanged;
        
        yield return new WaitUntil(() => isRiotStarted);
        guest.OnGuestMoodChanged -= onMoodChanged; 
        
        whiteTutorialText.text = "손님의 기분이 나빠져 난동을 부리기 시작했습니다. 마우스 왼쪽 클릭을 여러 번 해서 객잔 밖으로 쫓아내세요!";
        mouseLeft.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => !guest.gameObject.activeInHierarchy);
        
        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);
        whiteTutorialText.text = "무사히 난동 손님을 제압하고 쫓아냈습니다";
        
        yield return new WaitForSeconds(5f);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "이제 마지막 손님을 맞이할 준비를 합시다.";
    }

    IEnumerator RichManGuestTutorial()
    {
        whiteTutorialText.text = "마지막으로 부자가 오고 있습니다! 옷차림부터 아주 화려합니다.";
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Rich);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
        guestHighlightEffect.highlighted = true;
        
        whiteTutorialText.text = "부자 손님은 음식을 대접하면 훨씬 더 많은 돈과 팁을 지불합니다. 단, 그만큼 인내심이 적습니다.";
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Eat);
        
        guestHighlightEffect.highlighted = false;
        whiteTutorialText.text = "부자 손님이 만족하며 식사를 하고 있습니다. 부자 손님의 보너스는 아래와 같습니다";
        richmanPanel.SetActive(true);
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.Exit || !guest.gameObject.activeInHierarchy);
        
        richmanPanel.SetActive(false);
        whiteTutorialText.text = "부자 손님이 기분에 따른 보너스를 주고 갔습니다. ";
        
        yield return new WaitForSeconds(5f);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++; 
        
        blackTutorialText.text = "이것으로 객잔 운영에 필요한 모든 튜토리얼을 마칩니다. 수고하셨습니다.";
    }

    #endregion

    #region 공통 유틸리티 메서드 (텍스트 출력 대기 / 재귀 레이어 변경)

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
}