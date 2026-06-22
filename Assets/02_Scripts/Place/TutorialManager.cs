using System;
using System.Collections;
using HighlightPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject blackBackground;
    public GameObject whiteBackground;
    
    public TextMeshProUGUI blackTutorialText;
    public TextMeshProUGUI whiteTutorialText;
    
    public Button nextButton;
    
    public GameObject wasd;
    public GameObject mouseWheel;
    public GameObject mouseLeft;
    public GameObject mouseRight;

    public HighlightEffect potHighlightEffect;
    public GameObject mainCookingUICanvas;

    public GameObject pickPlayer;
    public GameObject myeongWol;
    public GameObject sowol;
    public GameObject menuPanel;
    public RecipeListView recipeListView;
    public GameObject recipePanel;
    public GameObject ingredientPanel;
    public CookingView cookingView;
    
    public CookingStation cookingStation;
    public HighlightEffect trayTableHighlightEffect;
    public GameObject serviceSlotInven;
    public GameObject putFoodPanel;
    public Inventory servingSlotInventory;
    
    public GuestManager guestManager;
    private GuestBase guest;
    public RecipeData mapoTofuRecipe;
    public FoodData mapoTofuFood;
    public GameObject getFoodPanel;
    public GameObject targetCinemachineCamera;
    public Camera tutorialCamera;
    
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

        
        
        //eventCount = 5;
    }

    public void StartTutorial()
    {
        switch (eventCount)
        {
            case 0:
                StartCoroutine(MoveControlTutorial());
                
                break;
            case 1:
                StartCoroutine(ZoomControlTutorial());
                break;
            case 2:
                StartCoroutine(RotatePovControlTutorial());
                break;
            case 3:
                StartCoroutine(MyungwolControlTutorial());
                break;
            case 4:
                StartCoroutine(GuestTutorial());
                break;
            case 5:
                StartCoroutine(SowolControlTutorial());
                break;
            case 6:
                StartCoroutine(CookingTutorial());
                break;
            case 7:
                StartCoroutine(SowolMoveFoodTutorial());
                break;
            case 8:
                StartCoroutine(BringFoodTutorial());
                break;

            
        }
        
        whiteBackground.SetActive(true);
        blackBackground.SetActive(false);
    }

    IEnumerator MoveControlTutorial()
    {
        whiteTutorialText.text = "WASD를 눌러 시점을 이동해보세요.";
        wasd.gameObject.SetActive(true);
        
        // W, A, S, D 중 하나라도 누를 때까지 대기합니다.
        yield return new WaitUntil(() => 
            Input.GetKeyDown(KeyCode.W) || 
            Input.GetKeyDown(KeyCode.A) || 
            Input.GetKeyDown(KeyCode.S) || 
            Input.GetKeyDown(KeyCode.D)
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
        
        yield return new WaitUntil(() =>
            Input.mouseScrollDelta.y != 0f);
        
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
        
        yield return new WaitUntil(() => 
            Input.GetMouseButton(1) && Input.GetAxis("Mouse X") != 0f
        );
        
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
        
        // 1. 명월이 트랜스폼과 위치를 가져옵니다.
        Transform myungwolTransform = PlayerManager.Instance.GetPlayer(Constants.PlayerType.Myungwol).transform;
        Vector3 startPosition = myungwolTransform.position;

        // 2. 명월이가 움직이면 벗어남.
        yield return new WaitUntil(() => 
            Vector3.Distance(startPosition, myungwolTransform.position) > 0.1f
        );
        
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
        // 손님이 자리에 앉을 때까지 대기
        whiteTutorialText.text = "손님이 들어오고 있습니다. WASD와 마우스를 이용해 객잔을 살펴보세요.";
        
        guest = guestManager.SpawnGuset(GuestManager.SpawnType.Normal);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();
        
        guest.myOrder.recipe = mapoTofuRecipe;
        guest.myOrder.orderedFood = mapoTofuFood;
        guest.TimeSetForTutorial();
        
        yield return new WaitUntil(() => guest.currentState == Constants.GuestState.WaitOrder);
        
        
        // 손님이 앉으면 가서 주문을 받음.
        whiteTutorialText.text = "손님을 마우스 왼쪽 클릭해 가까이 다가가서 주문을 받아주세요.";
        mouseLeft.gameObject.SetActive(true);
        
        while (guest.currentState == Constants.GuestState.WaitOrder)
        {
            guestHighlightEffect.highlighted = true;
            yield return null; 
        }

        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);
        
        // 주문을 받은 지 확인
        whiteTutorialText.text = "손님 머리위에 떠있는 음식을 확인해주세요. 옆의 표정은 손님의 기분입니다. 주문을 늦게 받거나, 음식을 늦게 내올 수록 점점 손님의 기분이 나빠집니다. 잘못된 음식을 가져다주면 손님의 기분은 최악이 됩니다.";

        nextButton.gameObject.SetActive(true);
        bool isNextClicked = false;
        
        UnityEngine.Events.UnityAction onNextClicked = () => isNextClicked = true;
        nextButton.onClick.AddListener(onNextClicked);

        yield return new WaitUntil(() => isNextClicked);
        
        nextButton.onClick.RemoveListener(onNextClicked);
        nextButton.gameObject.SetActive(false);
        
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "이제 음식을 만들어보겠습니다.";
    }
    
    IEnumerator SowolControlTutorial()
    {
        pickPlayer.SetActive(true);
        
        // 소월이 선택되면
        whiteTutorialText.text = "숫자 2번 키를 눌러주세요.";

        yield return new WaitUntil(() => 
            PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Sowol
        );
        
        pickPlayer.SetActive(false);

        mouseLeft.gameObject.SetActive(true);
        
        // 카메라 강제 이동중
        whiteTutorialText.text = "주방의 위치는 이곳입니다.";
        potHighlightEffect.highlighted = true;
        
        tutorialCamera.gameObject.SetActive(true);
        tutorialCamera.gameObject.transform.position = targetCinemachineCamera.gameObject.transform.position;
        tutorialCamera.transform.rotation = targetCinemachineCamera.gameObject.transform.rotation;
        Quaternion startRotation = tutorialCamera.transform.rotation;
            
        Vector3 directionToSowol = (sowol.transform.position - tutorialCamera.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToSowol);
            
        float duration = 2.0f;
        float timePassed = 0f;

        while (timePassed < duration)
        {
            tutorialCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null; 
        }
            
        tutorialCamera.transform.rotation = targetRotation; 
        yield return new WaitForSeconds(5f);
        
        tutorialCamera.gameObject.SetActive(false);
        whiteTutorialText.gameObject.SetActive(true);

        
        // Cooking UI가 켜지기 전까지 계속 반복
        whiteTutorialText.text = "마우스 커서를 냄비에 대고 왼쪽 클릭해주세요.";
        
        while (!mainCookingUICanvas.activeSelf) 
        {
            potHighlightEffect.highlighted = true;
            yield return null; 
        }
        
        potHighlightEffect.highlighted = false; 
        
        mouseLeft.gameObject.SetActive(false);
        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;

        blackTutorialText.text = "이제 음식을 만들어보겠습니다.";
    }
    
    
    
    IEnumerator CookingTutorial()
    {
        // 메뉴 선택
        whiteTutorialText.text = "메뉴를 선택해주세요.";
        menuPanel.SetActive(true);
        
        bool isRecipeClicked = false;
        Action<Recipe> onRecipeSelected = _ => isRecipeClicked = true;
        recipeListView.OnRecipeClicked += onRecipeSelected;

        yield return new WaitUntil(() => isRecipeClicked);
        
        recipeListView.OnRecipeClicked -= onRecipeSelected;
        
        // 레시피 확인
        whiteTutorialText.text = "필요한 재료가 나와있는 레시피가 보입니다. 확인 후 다음 버튼을 눌러주세요.";
        
        menuPanel.SetActive(false);
        recipePanel.SetActive(true);

        nextButton.gameObject.SetActive(true);
        bool isNextClicked = false;
        
        UnityEngine.Events.UnityAction onNextClicked = () => isNextClicked = true;
        nextButton.onClick.AddListener(onNextClicked);

        yield return new WaitUntil(() => isNextClicked);
        
        nextButton.onClick.RemoveListener(onNextClicked);
        nextButton.gameObject.SetActive(false);
        
        // 재료 넣기
        whiteTutorialText.text = "레시피에 맞춰 재료를 클릭해주세요.";

        recipePanel.SetActive(false);
        ingredientPanel.SetActive(true);
        
        bool isCookStartClicked = false;
        Action onCookSelected = () => isCookStartClicked = true;
        cookingView.OnCookClicked += onCookSelected;

        yield return new WaitUntil(() => isCookStartClicked);
        
        cookingView.OnCookClicked -= onCookSelected;

        ingredientPanel.SetActive(false);
        
        // 끝
        whiteTutorialText.text = "잘하셨습니다 CookingTutorial";

        yield return new WaitForSeconds(8f);

        whiteBackground.SetActive(false);
        blackBackground.SetActive(true);
        eventCount++;
        
        blackTutorialText.text = "이제 만들어진 음식을 옮겨 보겠습니다.";

    }

    IEnumerator SowolMoveFoodTutorial()
    {
        // 1. 조리 완료된 음식 인벤토리로
        whiteTutorialText.text = "조리가 끝난 냄비를 마우스 왼쪽 클릭합니다.";
        
        // 음식이 완성되서 FoodCollect가 실행되면 CookingState가 Idle이 되므로 그에 반응함.
        bool isFoodCollected = false;
        Action<CookingState, Sprite> onStateChanged = (state, _) => isFoodCollected = (state == CookingState.Idle);
        
        cookingStation.OnStateChanged += onStateChanged;

        while (!isFoodCollected)
        {
            potHighlightEffect.highlighted = true;
            yield return null; 
        }

        cookingStation.OnStateChanged -= onStateChanged;
        potHighlightEffect.highlighted = false; 
        
        
        // 2. 인벤토리의 조리 완료 된 음식을 가지고 트레이테이블로 다가가기
        whiteTutorialText.text = "조리 완료된 음식을 테이블을 클릭해 가져갑니다.";
        
        while (!serviceSlotInven.activeSelf) 
        {
            trayTableHighlightEffect.highlighted = true;
            yield return null; 
        }
        
        trayTableHighlightEffect.highlighted = false;
        
        // 3. 테이블로 음식 옮기기
        whiteTutorialText.text = "조리 완료된 음식을 눌러 테이블로 옮겨주세요.";
        
        putFoodPanel.SetActive(true);

        bool isFoodServed = false;
        
        while (!isFoodServed)
        {
            trayTableHighlightEffect.highlighted = true;
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
        
        // --- 마무리 연출 ---
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
        // 플레이어 명월로 변경
        whiteTutorialText.text = "숫자 1을 눌러 명월로 바꿔주세요";
        pickPlayer.SetActive(true);
        
        yield return new WaitUntil(() => 
            PlayerManager.Instance.CurrentPlayer == Constants.PlayerType.Myungwol
        );
        
        pickPlayer.SetActive(false);
        
        // 명월이 트레이 테이블로 올 때까지 대기
        whiteTutorialText.text = "명월인 상태로 트레이 테이블을 클릭해주세요.";

        while (!serviceSlotInven.activeSelf) 
        {
            trayTableHighlightEffect.highlighted = true;
            yield return null; 
        }
        
        trayTableHighlightEffect.highlighted = false;
        
        // 음식을 명월 인벤토리로 가져올 때까지 대기
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
        
        // 음식 서빙
        whiteTutorialText.text = "잘하셨습니다! 이제 음식을 들고 손님을 클릭해 서빙해주세요.";
        mouseLeft.gameObject.SetActive(true);
        HighlightEffect guestHighlightEffect = guest.GetComponentInChildren<HighlightEffect>();


        while (guest.currentState == Constants.GuestState.WaitFood)
        {
            guestHighlightEffect.highlighted = true;
            yield return null;
        }

        guestHighlightEffect.highlighted = false;
        mouseLeft.gameObject.SetActive(false);

        whiteTutorialText.text = "튜토리얼을 모두 완료했을까";
    }
}