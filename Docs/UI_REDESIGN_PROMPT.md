# 말랑블라스트 UI 개편 — AI 인수인계 프롬프트

> 이 문서만 읽고 어떤 AI가 작업해도 같은 결과가 나오도록 쓴 명세입니다.
> **디자인(색/모양/크기/여백/폰트)만 수정**합니다. 로직·기능·참조 연결은 절대 건드리지 않습니다.

---

## 0. 역할 프롬프트 (AI에게 그대로 붙여넣기)

```
당신은 모바일 캐주얼 퍼즐 게임 UI 아티스트 겸 Unity(uGUI) 개발자입니다.
프로젝트: c:\Users\kdy02\mobile-game (Unity 6000.3, uGUI + TextMeshPro)
UI는 에디터 스크립트가 씬을 생성합니다: Assets/Editor/BlockBlastSceneBuilder.cs
(메뉴: Block Blast/Generate Block Blast Mobile Scene)

목표: 현재 UI는 "프레임 안에 프레임, 광택, 작은 글씨, 정보 과다"로 복잡합니다.
이를 "심플한 모바일 게임 UI + 말랑블라스트 젤리 감성"으로 개편하세요.

절대 규칙
1. 기능/로직/SerializeField 연결/SetupXXX 인자/오브젝트 이름은 변경 금지.
2. 오브젝트를 삭제하지 말고, 숨길 때는 SetActive(false)만 사용.
3. 색·스프라이트·크기·폰트크기·여백·RectTransform 값만 수정.
4. 모든 색/크기는 Assets/Scripts/BlockBlast/UITheme.cs 의 상수를 사용 (하드코딩 금지).
5. 한 번에 한 화면만 수정 → 재컴파일 → 씬 재생성 → 스크린샷 확인 → 다음 화면.
6. 토큰 절약: 파일 전체를 읽지 말고 Select-String으로 해당 모달 구간만 찾아 부분 수정.
```

---

## 1. 디자인 컨셉: "말랑 캔디 플랫 (Mallang Candy Flat)"

한 줄 요약: **"흰 도화지 위에 놓인 말랑한 젤리 캔디 같은 파스텔 카드 + 굵고 큰 CTA 버튼"**

참고 감성: 캔디크러시/쿠키런/포코팡 계열의 *깔끔한 팝업 + 큰 둥근 버튼 + 굵은 숫자*.
금지: 이중 테두리, 과한 글리터/광택 오버레이, 12px대 작은 글씨, 한 화면에 5개 이상의 색 강조.

### 1-1. 형태 언어
| 항목 | 규칙 |
|---|---|
| 모서리 | 항상 크게 둥글게 (카드 R≈40, 버튼/게이지 = 완전 캡슐) |
| 테두리 | **1겹만**. 굵기 3~4px, 카드보다 한 톤 진한 파스텔 |
| 그림자 | 카드 아래 부드러운 1겹 (오프셋 y -6, 알파 0.18). 내부 그림자/글로우 금지 |
| 광택 | 버튼 상단 하이라이트 1줄만 허용. 카드 광택 금지 |
| 여백 | 카드 내부 패딩 최소 32px, 요소 간격 최소 20px |
| 정렬 | 중앙 정렬 위주, 한 영역당 정보 3개 이하 |

### 1-2. 색 팔레트 (UITheme.cs와 동일)
| 역할 | HEX | 용도 |
|---|---|---|
| 배경 딤 | #14092A α0.82 | 모달 뒤 어둡게 |
| 카드 면 | #FFFFFF | 모달/패널 바탕 |
| 카드 소프트 | #F4EEFC | 내부 서브 패널 |
| 카드 테두리 | #D9C8F2 | 1겹 테두리 |
| 본문 텍스트 | #3A2455 | 기본 글자 |
| 보조 텍스트 | #8C7AA6 | 설명/라벨 |
| 핑크(메인 CTA) | #FF5C8A | 게임 시작, 돌파 |
| 민트(성장/긍정) | #2ED3A4 | 강화, 증가 수치 |
| 퍼플(보조 CTA) | #9B6BFF | 장착 |
| 골드(재화/별) | #FFB800 | 코인, 별, 희귀도 |
| 블루(시간 스탯) | #4C8DFF | 추가 시간 게이지 |
| 잠금 회색 | #C9C4D1 | 미해금 요소 |

스탯 게이지 색: 시간=블루, 스킵=민트, 점수=골드, 경험치=퍼플.

### 1-3. 타이포 (TextMeshPro, 기존 cuteFont 유지)
| 용도 | 크기 | 스타일 |
|---|---|---|
| 모달 제목 | 40 | Bold |
| 섹션 제목 | 28 | Bold |
| 본문 | 24 | Regular |
| 수치(핵심) | 28 | Bold |
| 보조/설명 | **최소 20** | Regular |
| 버튼 | 30 | Bold, 흰색 |

최소 글자 크기 20 미만 금지. 안 들어가면 문구를 줄이거나 숨긴다.

### 1-4. 버튼
- 크기: 가로 ≥ 260, 세로 ≥ 96 (엄지 터치 영역).
- 면은 단색 캡슐 + 아래쪽 4px 진한 하단 두께(입체감). 광택 오버레이 금지.
- CTA는 화면당 **1개만 가장 크게**, 나머지는 한 단계 작게.
- 눌림: 기존 `ShopUIAnimationController.AttachTactileBounce` 유지.

### 1-5. 게이지(일자 바)
- 높이 22, 완전 캡슐. 트랙 #EEE8F8, 채움은 스탯 고유색 **단색**.
- 라벨(좌) / 현재→다음 수치(우, 증가분은 민트 ▲)를 바 위 한 줄에 배치.
- 채움 바는 Filled 타입에 *캡슐 스프라이트를 쓰면 쐐기 모양으로 찌그러짐* → 아래 "알려진 이슈" 참고.

---

## 2. 화면별 개편 지시서 (작업 순서)

> 각 화면: 찾기 → 수정 → 재컴파일 → 씬 재생성 → 스크린샷 검증.
> 위치 찾기: `Select-String -Path Assets\Editor\BlockBlastSceneBuilder.cs -Pattern "<오브젝트명>"`

### ① 말랑이 상세 모달 `MascotDetailModal` (진행 중 — 약 60% 완료)
이미 적용됨: 딤 진하게, 강화영역 프레임 제거, 고유블록 카드 플랫화, 안내문구 숨김, 게이지 22→20 굵게.
남은 작업:
1. 카드 `DialogCard`·`ShowcaseBox`의 광택 스프라이트(`luxuryShopCardSprite`, `luxuryItemCardSprite`) → 흰 카드 + 1겹 테두리로 교체.
2. 게이지 채움 쐐기 문제 해결 (알려진 이슈 참고).
3. 3단 진화 카드(`StageCard_0..2`): 해금=카드 면 흰색+컬러 블록, 미해금=#C9C4D1 회색+🔒. 글자 최소 20.
4. 하단 버튼 3개: 강화=민트, 돌파=핑크(메인), 장착=퍼플. 가로 280, 세로 96.
5. 캐릭터 이미지는 창에서 **가장 크게** 유지(360px 이상), 터치 젤리 물리 유지.

### ② 로비 `LobbyRoot`
- 4마리 말랑이 + 라벨 알약, 하단 `BtnBottomAction`(게임 시작!) 1개가 주인공.
- 상단: 재화 알약 2개(다이아/코인)는 흰 알약+골드/블루 아이콘, 프로필 원형 버튼.
- 라벨은 단색 캡슐(캐릭터 고유색), 글자 24 Bold 흰색. 말랑이 뒤 글로우 알파 0.5로 약화.
- 하단 팁 텍스트는 보조 텍스트 색, 크기 24.

### ③ 상점 `ShopModal`
- 좌측 세로 탭 5개 → 단색 캡슐 탭(선택=핑크, 비선택=#F4EEFC).
- 상품 카드는 흰 카드 + 1겹 테두리, 가격 버튼이 CTA. 카드당 정보는 이름/보상/가격 3개만.

### ④ 설정·프로필·도감·소환결과·확률표 모달
- 공통 규칙(§1)만 적용: 흰 카드, 1겹 테두리, 닫기(X)는 우상단 56px 원형 단색.
- 확률표는 표 헤더 #F4EEFC, 행 구분선 1px.

### ⑤ 인게임 HUD (마지막)
- 점수/콤보/시간을 큰 굵은 숫자 중심으로. 배경 판은 반투명 흰색 카드.

---

## 3. 알려진 이슈
- **게이지 채움 쐐기 현상**: `Image.Type.Filled` + 캡슐(`tabPillSprite`)이 가로 크롭되며 찌그러짐.
  해결안(택1, 권장 A):
  - A. 채움을 Sliced 단색 캡슐로 바꾸고, `LobbyManager`에서 `fillAmount` 대신 `RectTransform` 앵커 `anchorMax.x = value`로 제어(로직 소폭 수정 허용 — 이 항목만 예외).
  - B. 채움 전용 단색 사각 스프라이트를 생성해 Filled 유지(모서리는 Mask 캡슐로 클리핑).
- `unity cmd menu` 가 30초 타임아웃 에러를 내도 씬은 정상 생성됨(무시 가능). 생성 후 `editor_play`로 확인.

---

## 4. 작업 절차 / 검증 체크리스트
```
unity cmd recompile → unity cmd recompile_status (errors 비어있는지)
unity cmd menu "Block Blast/Generate Block Blast Mobile Scene"   # 타임아웃 무시
unity cmd editor_play
unity cmd eval "UnityEngine.Object.FindAnyObjectByType<BlockBlast.MainMenuCinematicController>().TriggerStartGame(UnityEngine.Vector2.zero);"
unity cmd eval "UnityEngine.Object.FindAnyObjectByType<BlockBlast.LobbyManager>().OpenMascotDetail(0);"
unity cmd screenshot   → 결과 이미지 확인
unity cmd editor_stop
```
완료 기준(화면마다):
- [ ] 테두리 1겹, 광택 없음  - [ ] 20 미만 글자 없음
- [ ] CTA 1개가 가장 큼     - [ ] 팔레트 외 색 없음(UITheme 사용)
- [ ] 컴파일 오류 0, 기능 동작 동일
