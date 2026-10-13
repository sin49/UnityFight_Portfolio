# Character Addressables namespace 분리

## A. 변경 파일

| 파일 (`Client/1_backup/Assets/_Project/` 기준) | 변경 내용 |
| --- | --- |
| `Scripts/Object/Character/basic/CharacterContentMapping.cs` | 기존 물리 이름을 유지하고 `PrefabAddress`, `JsonAddress`, `PortraitAddress`, `AnimatorAddress` readonly 필드 추가. |
| `Scripts/Manager/LoadManager.cs` | Prefab/JSON/Portrait 단일·batch의 Mapping 선택값 및 동기 Portrait dictionary 조회를 Address로 변경. 정확히 9곳 치환. |
| `Scripts/Manager/LoadManager.CharacterJsonAddressables.cs` | `ReturnChrDataName`의 동기 handle 조회 key를 `JsonAddress`로 변경. |
| `Scripts/Manager/LoadManager.PortraitAddressables.cs` | 전체 Portrait preload 목록을 `PortraitAddress`로 구성. |
| `Scenes/DeveloperTest/AddresablesPortotypeTest.cs` | 수동 테스트도 `content.PrefabAddress`를 전달하도록 한 표현식 변경. 기존 Instantiate/Release 로직은 변경하지 않음. |

산출물은 이 보고서, `verification.json`, `task.diff`다. 코드 파일 추가/삭제/이동은 없다. public method signature, 기존 `.meta` 및 GUID, 물리 에셋, JSON 데이터, 직렬화 Scene/Prefab, Settings/Group, Package, 서버 파일은 변경하지 않았다.

## B. 새 Address 규칙

```text
character/prefab/{canonical CharacterId}
character/json/{canonical CharacterId}
character/portrait/{canonical CharacterId}
character/animator/{canonical CharacterId}
```

주소 정의 근거: `CharacterContentMapping.cs:16–19` 및 `:30–34`. 주소는 파일명에서 만들지 않고, 등록된 Content의 소문자 논리 `CharacterId`에서 만든다. `Get("P02")`와 `Get("p02")`는 기존 case-insensitive Mapping에 의해 같은 Content를 반환하므로 둘 다 `character/prefab/p02` 등의 주소를 얻는다. 알 수 없는 ID의 기존 처리 정책은 그대로다.

## C. 물리 이름과 Address 분리

| 필드 | 역할 | p02 예시 |
| --- | --- | --- |
| `CharacterId` | 캐릭터 종류의 논리 ID | `p02` |
| `PrefabName` / `PrefabAddress` | 기존 물리 Prefab 이름 / Addressables key | `P02` / `character/prefab/p02` |
| `JsonName` / `JsonAddress` | 기존 물리 JSON 이름 / Addressables key | `P02` / `character/json/p02` |
| `PortraitName` / `PortraitAddress` | 기존 물리 Portrait 이름 / Addressables key | `P02` / `character/portrait/p02` |
| `AnimatorName` / `AnimatorAddress` | 기존 물리 Animator 이름 / 향후 Addressables key | `p02` / `character/animator/p02` |

기존 다섯 Content entry 및 네 물리 이름 값은 변경하지 않았다. `p04`는 `AnimatorName == null`이므로 `AnimatorAddress`도 null이다. 없는 콘텐츠의 주소나 fallback은 만들지 않았다.

`UserDataManager.cs:273`, `:285`는 실제 JSON 파일 읽기/쓰기 경로이므로 `JsonName`을 유지한다. `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs`의 이름 참조도 그대로다. 별도 Registry, Provider, ID 구조체는 추가하지 않았다.

## D. Prefab / JSON / Portrait 로더 변경점

- `LoadManager.cs:138`, `:143`: 단일 Prefab 두 overload → `Get(s).PrefabAddress`.
- `LoadManager.cs:148`: batch Prefab → `Resolve(s, c => c.PrefabAddress)`.
- `LoadManager.cs:202`, `:207`: 단일/batch Character JSON → `JsonAddress`.
- `LoadManager.CharacterJsonAddressables.cs:18`: `ReturnChrDataName`의 cache 조회 → `JsonAddress`. 메뉴 전 preload 및 미준비 예외 정책 유지.
- `LoadManager.cs:154`, `:160`, `:165`: 단일 두 overload / batch Portrait → `PortraitAddress`.
- `LoadManager.PortraitAddressables.cs:80`: 전체 Portrait 준비 → `content.PortraitAddress`.
- `LoadManager.cs:25`: `getcharactersprite` 동기 dictionary 조회 → 같은 `PortraitAddress`.
- `AddresablesPortotypeTest.cs:16`: 테스트 `InstantiateAsync` → `content.PrefabAddress`.

public API의 이름/인자/반환형, callback 타입, Prefab asset 반환 계약, batch 순서/실패 처리, Instantiate 위치, queue 및 메뉴 진입 조건은 그대로다. 로더는 여전히 `GameObject` / `TextAsset` / `Sprite` 타입으로 로드하지만, 콘텐츠 간 key는 독립적이므로 같은 문자열을 타입으로 구분하는 방식에 의존하지 않는다.

## E. cache key 변경점

| 저장소 | 이전 예시 | 변경 후 예시 |
| --- | --- | --- |
| `CharacterPrefabHandles` | `P02` | `character/prefab/p02` |
| `CharacterJsonHandles` | `P02` | `character/json/p02` |
| `CharacterPortraitHandles` | `P02` | `character/portrait/p02` |
| 동기 UI `CharacterSpriteDictionary` | `P02` | `character/portrait/p02` |

세 typed handle cache는 이미 전달받은 address 매개변수를 key로 쓰고 있다. 따라서 입구에서 새 Address를 전달하면 `TryGetValue`, 저장, 실패 제거까지 같은 namespace key로 동작한다. `LoadManager.Addressables.cs:26–31`, `LoadManager.CharacterJsonAddressables.cs:37–40`, `LoadManager.PortraitAddressables.cs:25–29`가 근거다. Prefab cache 구현 파일 자체는 수정하지 않았다.

Portrait dictionary 저장은 `LoadManager.PortraitAddressables.cs:58`의 `CharacterSpriteDictionary[portraitAddress]`이며, 동기 조회도 같은 새 key로 맞췄다. JSON 동기 이름 조회 역시 새 key를 사용한다. 성공 handle의 앱 종료 시 Release, 실패 handle 제거, 중복 요청의 in-flight/완료 cache 재사용 정책은 변경하지 않았다. 구형 주소 alias, fallback, 실행 중 구형 cache 이관은 추가하지 않았다.

## F. Editor 등록 목록

**아래 Prefab / JSON / Portrait 15개는 현재 게임 경로에서 필요하다. Animator 4개는 향후 전환을 위한 주소 정의이며 지금 등록할 필요는 없다. `p04` Animator는 등록하지 않는다.**

| CharacterId | Prefab Address | JSON Address | Portrait Address | Animator Address (준비만) |
| --- | --- | --- | --- | --- |
| p01 | `character/prefab/p01` | `character/json/p01` | `character/portrait/p01` | `character/animator/p01` |
| p02 | `character/prefab/p02` | `character/json/p02` | `character/portrait/p02` | `character/animator/p02` |
| p03 | `character/prefab/p03` | `character/json/p03` | `character/portrait/p03` | `character/animator/p03` |
| p04 | `character/prefab/p04` | `character/json/p04` | `character/portrait/p04` | null / 미지원 |
| e00 | `character/prefab/e00` | `character/json/e00` | `character/portrait/e00` | `character/animator/e00` |

등록할 실제 파일 (`Assets/_Project/` 기준):

| CharacterId | Prefab (GameObject) | JSON (TextAsset) | Portrait (Sprite) | 향후 Animator |
| --- | --- | --- | --- | --- |
| p01 | `Prefabs/Player1/P01.prefab` | `Data/Json/Character/p01.json` | `Art/Sprite/Character/p01.png` | `Art/Source/FIle1/SD_unitychan/Prefabs/P01.controller` |
| p02 | `Prefabs/Player2/P02.prefab` | `Data/Json/Character/P02.json` | `Art/Sprite/Character/P02.png` | `Art/Animator/p02.controller` |
| p03 | `Prefabs/Player3/P03.prefab` | `Data/Json/Character/p03.json` | `Art/Sprite/Character/P03.png` | `Art/Animator/p03.controller` |
| p04 | `Prefabs/Player4/P04.prefab` | `Data/Json/Character/P04.json` | `Art/Sprite/Character/P04.png` | 없음 |
| e00 | `Prefabs/Test_enemy/E00.prefab` | `Data/Json/Character/E00.json` | `Art/Sprite/Character/E00.png` | `Art/Animator/E00.controller` |

현재 저장소 Settings에는 `Default Local Group.asset:19`의 기존 `P01` Prefab entry 한 개만 확인된다. Editor에서 해당 **기존 Prefab entry의 Address를 `character/prefab/p01`로 수정**하고 나머지 14개 Active 콘텐츠를 표에 맞게 등록한다. 같은 asset의 entry를 복제하거나 물리 파일명을 rename할 필요는 없다. Settings/Group은 자동 편집하지 않았다.

등록 전에는 새 key를 찾지 못하므로 게임 실행 검증을 먼저 진행하지 않는다. 등록 후 새 Play 세션에서 확인한다. 기존 이름으로 계속 로드하는 alias는 없다.

## G. 중복 / 보존 정적 검사 결과

- 실제 Mapping의 5개 Content와 주소 정의를 읽어 검사: non-null Address 19개, 고유 Address 19개, 중복 0개. 이 중 Active 15개, Animator 준비 4개.
- `p04` AnimatorName / AnimatorAddress 둘 다 null.
- 전체 Assets C# 검색: Addressables 호출 또는 Active Character Addressables 로더 호출에 물리 `*Name`을 직접 전달하는 잔여 참조 0개. 테스트 호출도 포함한다.
- 기존 Settings entry는 1개이고 현재 중복 0개다. 새 namespace 19개가 Settings에 이미 등록됐다는 의미는 아니다.
- 작업 전 9,100개 제품 파일 hash와 비교: 위 C# 5개만 변경. 추가/삭제 제품 파일 0개. 기존 meta 4,764개 전부 byte 동일.
- 그 외 파일은 모두 hash 동일: PlannedLegacy, Settings/Group, Package, 직렬화 Scene/Prefab, 데이터 JSON, 이미지/Animator, UI/전투 코드, 서버 포함.
- 기존 public method signature, Mapping entry/물리 이름, 위 주소 치환 외 로더 처리 동일. per-task Git diff whitespace 검사 통과.
- 별도 read-only 리뷰: 수정이 필요한 문제 없음. 원본 5개 snapshot과 비교하여 검토.

Compile / Build / Package Restore / Unity 실행은 하지 않았다. runtime catalog 및 Editor 등록 결과는 사람이 검증해야 한다. 정적 검증 원문은 `verification.json`, 실제 이번 변경 diff는 `task.diff`에 보관했다. 이전 보고서들은 당시 작업 기록으로 그대로 두었다.

## H. 다음 Animator 전환 준비 상태

Mapping에는 `AnimatorAddress`가 준비됐지만 실제 로딩은 계속 `LoadManager.cs:191`, `:196`의 `AnimatorName` → `characteranimation` AssetBundle 경로다. Legacy partial은 변경하지 않았다.

다음 전환에서는 `RuntimeAnimatorController` 반환 계약, 전투 초기화 후처리, batch 순서 및 `p04` 부재 처리를 먼저 조사한다. 이번에는 Addressables Animator 로더나 P04 대체 Animator를 구현하지 않았다.

후속 정정: p01 Animator 물리 대상은 Prefab GUID 대조 결과 `Art/Source/FIle1/SD_unitychan/Prefabs/P01.controller`이다. AnimatorName은 `P01`로 정정했고 AnimatorAddress는 `character/animator/p01` 그대로다. 위 본문의 최초 namespace 분리 당시 물리 이름 보존 검증과 구분되는 후속 수정이다.
