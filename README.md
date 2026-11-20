# 무림객잔 (Moorim Inn)

무협 세계관의 객잔을 운영하는 경영 시뮬레이션 게임입니다.
2인 팀 프로젝트로 개발했으며, 이 저장소는 포트폴리오 공개용 사본입니다.

- **기간:** 2026.05 ~ 2026.08
- **엔진 / 언어:** Unity (URP), C#
- **팀 구성:** 2명
- **내 커밋 내역:** [kimjochul 커밋만 보기](https://github.com/kimjochul/Moorim_Copy/commits/main/?author=kimjochul)

> 에셋 스토어에서 받은 외부 에셋(3D 모델, 사운드, UI 팩 등)은 라이선스 때문에 이 사본에서 제외했습니다.
> 그래서 유니티로 열면 일부 오브젝트와 사운드가 빠져 보일 수 있습니다.

---

## 담당 역할 (kimjochul)

손님·NPC AI, 플레이어 스킬, 튜토리얼, 카메라, 요리·상점 편의 기능, 안정화 및 QA, 메인 씬 구성

### 1. 특수 손님 AI

**스크립트:** [GuestManager](Assets/02_Scripts/Guest/GuestManager.cs) · [BadGuest](Assets/02_Scripts/Place/BadGuest.cs) · [RichManGuest](Assets/02_Scripts/Guest/RichManGuest.cs) · [ViolenceGuest](Assets/02_Scripts/Place/ViolenceGuest.cs)

- 손님 유형별 스폰, 자리가 비면 대기 중인 손님에게 자동 배정
- **진상 손님:** 음식을 기다리다 일정 시간이 지나면 다른 메뉴로 주문 변경
- **부자 손님:** 만족도에 따라 음식값을 2~4배 지불, 매우 불만족이면 명성 감소
- **난동 손님:** 화가 나면 난동을 부려 플레이어의 돈을 잃게 함
- 오브젝트 풀링으로 재사용할 때 상태가 남지 않도록 초기화 처리

### 2. 도둑 도주 AI

**스크립트:** [ThiefGuyGuest](Assets/02_Scripts/Guest/ThiefGuyGuest.cs)

- NavMesh 기반으로 플레이어에게서 도망
- 문이 열려 있으면 출구로 전력 질주
- 문이 닫혀 있으면 5×5 그리드 좌표 중 추격자와 가장 먼 지점을 계산
- 대각선 이동 없이 상하좌우 직선 경유지로만 움직이도록 구현

### 3. 경비병 NPC

**스크립트:** [Guard](Assets/02_Scripts/Place/Guard.cs) · [GuardAnimation](Assets/02_Scripts/Place/GuardAnimation.cs)

- FSM 구조: 대기 → 추격 → 제압 → 복귀
- 난동 이벤트를 구독해 난동 손님이 생기면 자동 출동
- 상태가 바뀔 때 이벤트로 애니메이션 연동

### 4. 플레이어 스킬

**스크립트:** [GhostEffect](Assets/KMJ/GhostEffect.cs) · [SkillUIManager](Assets/02_Scripts/Place/SkillUIManager.cs)

- **유체화:** 스킨드 메시를 베이크해 잔상 이펙트 생성
- **점멸:** 지정한 위치로 순간이동
- 코루틴 기반 스킬 쿨타임 UI

### 5. 튜토리얼

**스크립트:** [TutorialManager](Assets/02_Scripts/Place/TutorialManager.cs) · [TutorialHighlightUI](Assets/02_Scripts/Place/TutorialHighlightUI.cs)

- 단계에 따라 안내 NPC를 바꾸고 대사 타이핑 연출
- 목표물이 화면 밖에 있으면 화면 가장자리에 방향 화살표 표시
- 반투명 가림막 4장으로 특정 UI만 뚫어서 강조

### 6. 카메라

**스크립트:** [CameraController](Assets/02_Scripts/Camera/CameraController.cs)

- WASD 이동, 우클릭 드래그 회전, 마우스 휠 줌
- 맵 밖으로 나가지 않도록 이동 범위 제한
- UI 위에서는 줌이 동작하지 않도록 처리

### 7. 요리 · 상점 · 인벤토리

**스크립트:** [CookingController](Assets/02_Scripts/Cooking/CookingController.cs) · [Shop](Assets/02_Scripts/Inventory/Item/Shop.cs) · [InventoryLengthRegulate](Assets/KMJ/InventoryLengthRegulate.cs)

- 레시피에 필요한 재료를 여러 인벤토리 칸에서 찾아 자동으로 채우기
- 최대 보유량을 넘지 않도록 구매 수량 제한
- 슬롯 개수에 맞춰 인벤토리 UI 그리드 크기 자동 조절

### 8. 안정화 (QA 기간 버그 수정)

**스크립트:** [GuestBase](Assets/02_Scripts/Guest/GuestBase.cs) · [GuestManager](Assets/02_Scripts/Guest/GuestManager.cs) · [PoolManager](Assets/02_Scripts/System/PoolManager.cs) · [SeatManager](Assets/02_Scripts/Seat/SeatManager.cs) · [Gate](Assets/02_Scripts/Place/Gate.cs) · [Quest](Assets/02_Scripts/Quest/Quest.cs)

- **일시정지를 IsPaused 기반으로 전환:** 일시정지 중에는 손님 대기 타이머, 인내심 게이지 코루틴, 손님 스폰이 멈추도록 `DayManager.IsPaused` 플래그로 통일
- **씬 재로드 시 죽은 참조 해결:** 씬이 바뀌면서 파괴된 풀 오브젝트와 의자 참조를 정리. 풀에서 꺼낼 때 파괴된 오브젝트는 건너뛰고, 남은 게 없으면 새로 생성
- **이벤트 구독 누수 해결:** 문(Gate)의 static 이벤트를 씬 단위 인스턴스 이벤트로 바꾸고, `OnDestroy`에서 구독을 해제해 씬 재시작 후 중복 호출과 오류 방지
- **ScriptableObject 보상 오염 해결:** 퀘스트 보상을 에셋에서 직접 쓰지 않고 런타임 사본으로 처리해, 한 퀘스트의 보상 변경이 같은 템플릿의 다른 퀘스트에 번지던 문제 수정

### 9. QA · 밸런싱 도구

**스크립트:** [DailyTestLogger](Assets/KMJ/DailyTestLogger.cs)

- 일차별 수입, 재료비, 순이익, 명성, 손님 수, 만족도 분포를 JSON으로 기록해 밸런스 조정에 활용

### 10. 씬 · 환경

**스크립트:** [Gate](Assets/02_Scripts/Place/Gate.cs)

- 메인 씬 구성과 교체, NavMesh 조정
- Skybox와 셰이더로 분위기 연출
- 객잔 양문 열림 연출

## 팀원 담당

스토리 · 대화 시스템, 캐릭터 UI, 대화 로그, 사운드
