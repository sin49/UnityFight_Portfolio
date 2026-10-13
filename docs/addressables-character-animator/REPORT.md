# Character Animator Addressables 전환

## A. 수정 파일

| 파일 | 변경 이유 |
| --- | --- |
| `Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs` | 기존 단일·batch public API의 enqueue 경로 두 곳만 `AnimatorAddress` 기반 Addressables helper로 변경. |
| `Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.AnimatorAddressables.cs` | RuntimeAnimatorController typed session handle cache, 단일·batch 로더, null 지원, 종료 시 Release 추가. |
| 위 새 partial의 `.meta` | 새 스크립트에만 새 GUID 부여. 기존 meta 변경 없음. |
| `docs/addressables-character-animator/REPORT.md` | 조사 및 변경 결과, Editor 등록 안내. |
| `docs/addressables-character-animator/verification.json` | 정적 검증 결과. |
| `docs/addressables-character-animator/task.diff` | 이번 변경만 담은 실제 작업 전 snapshot 기반 diff. |

기존 public API를 유지한다:

- `PushCharacteranimatorloadrequest(string, Action<RuntimeAnimatorController>)`
- `PushCharacteranimatorsloadrequest(List<string>, Action<List<RuntimeAnimatorController>>)`

Mapping, Legacy partial, Prefab/JSON/Portrait 구현, UI, 전투 로직, Stage/Terrain/Item/SFX, Settings/Group, Package, 기존 meta 및 물리 에셋은 변경하지 않았다.

## B. 기존 Animator 로딩 흐름

```text
CharacterId → CharacterContentMapping.AnimatorName
  → LoadingQueue → characteranimation AssetBundle
  → LoadAssetAsync → RuntimeAnimatorController callback → Unload(false)
```

근거:

- 작업 전 `LoadManager.cs:188–197`: 단일 API는 `LoadAnimatorAsync("characteranimation", Get(s).AnimatorName, action)`, batch API는 `LoadAnimatorsAsync("characteranimation", Resolve(...AnimatorName), action)`을 enqueue했다.
- `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:378`: `LoadAnimatorAsync`는 null 이름을 특별 처리하고, 그 외에는 bundle 파일을 열어 asset을 RuntimeAnimatorController로 cast해 반환한다.
- 같은 파일 `:429`: `LoadAnimatorsAsync`는 이름 순서대로 로드하고, null 이름 및 null cast 결과도 List에 추가한다. callback 후 `Unload(false)`를 실행한다.

전체 Assets C# 검색에서 Active `characteranimation` 선택 경로는 위 public API 두 곳이었다. 단일 API의 외부 C# 호출은 확인되지 않았다. 실제 batch 소비자는 `LoadManager.cs`의 `loadingGamesceneAsync`와 `loadingGamesceneAsync_T`다. `loadgame`은 전자를 실행하며, `_T`의 직접 호출자는 현재 C# 검색에서 확인되지 않는다.

전투의 결과 결합 근거:

- `LoadManager.cs:428–461`, `:506–539`: 같은 `CharacterID` 목록으로 Prefab, CharacterInformation, Animator 요청을 enqueue한다.
- Animator callback `:448`, `:526` 및 `:452`, `:456`, `:530`, `:534`: `loadedcharacter[i]`에 `List[i]`를 전달해 `setAnimation`한다. 그러므로 Animator list를 축소하거나 재정렬하면 잘못된 캐릭터에 controller가 연결된다.
- `GameCharacter.cs:696`: `setAnimation`이 controller 또는 null을 소비한다. 이후 `:689`의 `initialize_animation`과 `Character_Animation.cs:9–14`가 저장된 controller를 실제 Animator에 연결한다.

이 소비 코드들은 변경하지 않았다.

## C. 변경 후 Addressables 흐름

```text
CharacterId → CharacterContentMapping.AnimatorAddress
  → LoadingQueue → LoadCharacterAnimatorAddressableAsync
  → 주소별 handle 재사용 또는 LoadAssetAsync<RuntimeAnimatorController>
  → 기존 RuntimeAnimatorController callback
```

- `LoadManager.cs:191`: 단일은 `Get(s).AnimatorAddress`를 전달한다.
- `LoadManager.cs:196`: batch는 `Resolve(s, c => c.AnimatorAddress)`를 전달한다.
- `LoadManager.AnimatorAddressables.cs:16`: 공통 단일 로더.
- 새 partial `:72`: batch는 공통 단일 로더를 순서대로 호출한다.

CharacterId / AnimatorName을 직접 Address로 넘기지 않는다. Mapping이 가진 namespace Address를 그대로 사용한다. AnimatorName의 대소문자와 물리 controller 파일명은 유지한다. fallback, 새 Provider/Registry, Animator instance 생성은 추가하지 않았다.

## D. P04 null 처리

`CharacterContentMapping.cs:48`의 p04 entry는 AnimatorName null이며, `:34`에서 AnimatorAddress도 null이다. Mapping은 변경하지 않았다.

기존 단일 Legacy helper는 null 이름이면 progress 2를 반영하고 callback(null)을 실행했다. batch Legacy helper는 null 이름이면 List에 null을 추가하고 progress 1을 반영했다.

새 단일 helper `LoadManager.AnimatorAddressables.cs:21–27`도 null 주소이면 callback(null) 후 종료한다. 이 분기는 Addressables 호출, cache 접근, 종료 이벤트 구독보다 먼저 실행된다. P04에 주소, operation/handle, fallback controller, 오류 로그를 만들지 않는다. 단일 budget 2 / batch slot budget 1도 유지한다.

`GameCharacter.cs:696–708`의 기존 `setAnimation(null)`은 `is_animation = false`로 설정하며 저장된 `this_ani_controller`는 덮어쓰지 않는다. `GameCharacter.cs:170–171`의 초기화도 `is_animation`을 확인한다. 따라서 P04의 기존 애니메이션 비활성화 흐름을 유지하며 전투 코드를 변경하지 않는다.

## E. batch 순서 / null slot 처리

`LoadCharacterAnimatorsAddressableAsync`는 setup progress 1 이후 입력 Address를 foreach 순서대로 처리한다. 각 slot은 공통 단일 helper의 callback 결과를 **조건 없이** List에 한 번 추가한다.

- 입력 개수와 결과 개수가 같다.
- 입력 순서 및 중복 entry를 유지한다.
- P04는 같은 인덱스에 null을 유지한다.
- 정상 주소 로딩 실패도 같은 인덱스에 null을 유지한다.
- 빈 입력은 빈 리스트 callback을 실행한다.
- 예: `[p01, p04, p02, p01]` → `[p01 controller, null, p02 controller, 같은 p01 controller]`.

이는 전투가 Prefab/Master/Animator 결과를 인덱스로 결합하는 계약을 보존한다. 중복 주소의 성공 로딩은 cache를 공유하지만 callback/list entry는 생략하지 않는다.

실패 동작의 차이도 명시한다. 구형 Legacy는 개별 asset의 null cast에는 null을 반환했지만, bundle 자체를 열지 못하면 callback 없이 종료했다. 새 Addressables 경로는 정상 주소의 실패를 오류로 기록하고 callback(null)을 완료한다. batch는 실패 slot을 유지하므로 전체 실패에도 입력 개수만큼 null인 리스트를 반환한다. 전투 로직에 새로운 실패 차단 조건이나 fallback은 추가하지 않았다.

## F. handle/cache 소유권

- 소유자: 새 partial `:12–14`의 static `CharacterAnimatorHandles`.
- key: Mapping에서 얻은 `character/animator/{canonical CharacterId}`. `StringComparer.Ordinal`로 비교한다.
- `:36–41`: 유효한 in-flight/완료 handle을 재사용하고 새 handle은 첫 yield 전에 보관한다.
- `:44`: 대기 중 유효성을 확인한다. 동시 요청의 실패 관찰자가 먼저 Release한 handle을 다시 읽지 않는다.
- `:52–65`: 실패 시 cache의 소유 handle이 같은 경우에만 제거하고 유효 handle을 Release한다. 이후 새 요청은 재시도할 수 있다.
- 성공 callback 뒤에는 Release하지 않는다. Scene 이동이나 LoadManager 파괴 시에도 session 소유 handle을 유지한다.
- 종료 이벤트 `:31` → `ReleaseCharacterAnimatorSessionHandles` (`:87`)에서 유효 handle Release 후 cache를 비운다.

Prefab/JSON/Portrait의 typed cache 및 session lifetime 패턴을 따랐고, 기존 구현 파일 자체는 변경하지 않았다. controller와 관련 clip 의존성을 사용하는 캐릭터가 남아 있는 동안 handle을 유지한다. UI/캐릭터별 ref-count나 추가 lifetime 추상화는 도입하지 않았다.

## G. Legacy 및 남은 Active characteranimation 참조

`PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs`는 작업 전 hash와 byte 동일하다. `LoadAnimatorAsync`, `LoadAnimatorsAsync`, null 처리 및 `Unload(false)`를 전부 보존했다. Active 호출자는 더 이상 없다. 두 helper는 전달된 bundle 이름을 사용하는 기존 구현 그대로다.

전체 Assets C# 검색에서 Active `"characteranimation"` bundle 호출 및 `LoadAnimatorAsync` / `LoadAnimatorsAsync` 호출은 0개다.

기존 `characteranimation` label은 아래 meta 9개에 남아 있으며 변경하지 않았다:

- `Art/Animator/`의 p01, p02, p03, E00 controller 4개.
- `Art/Animation/`의 stunned, melee_basic 및 `Player1/`의 p01Special, p01Skill1 clip 4개.
- `Data/Json/Version/characteranimation/BundleVersion.txt.meta` 1개.

이 label과 Legacy 데이터 보존은 Active 로딩 fallback을 의미하지 않는다. 과거 bundle 파일도 삭제하지 않았다.

정적 검증:

- 작업 전 9,100개 제품 파일과 비교: 기존 파일은 LoadManager.cs 한 개만 변경, 새 partial 및 meta 2개 추가, 삭제 0개.
- LoadManager 변경은 정확히 두 enqueue 표현식뿐. 전투 인덱스 결합 코드 및 모든 다른 로더의 hash 동일.
- 기존 meta 4,764개와 GUID 모두 byte 동일. 새 script GUID는 기존 meta와 충돌하지 않는다.
- Mapping, Settings/Group, Package, 직렬화 Scene/Prefab, JSON/이미지/controller/clip, 서버 파일 모두 hash 동일.
- public API signature, Mapping 주소 사용, null-before-load, dense batch, cache/실패/해제 경로 및 per-task diff whitespace 확인.
- 별도 read-only 리뷰에서도 수정이 필요한 문제 없음.

Compile / Build / Package Restore / Unity 실행은 수행하지 않았다. 정적 검사로 runtime 동작 성공을 주장하지 않는다. 결과는 `verification.json`, 변경 diff는 `task.diff`에 보관했다.

## H. Editor 등록 목록과 다음 추천 작업

아래 기존 controller asset을 Addressable로 등록한다. 경로는 `Client/1_backup/Assets/_Project/` 기준이다.

| CharacterId | 실제 asset | Address | 기존 GUID |
| --- | --- | --- | --- |
| p01 | `Art/Source/FIle1/SD_unitychan/Prefabs/P01.controller` | `character/animator/p01` | `e2af5dd28e7152349826c98264eb97c2` |
| p02 | `Art/Animator/p02.controller` | `character/animator/p02` | `ce58bfbe462b75344ac226fe40c465d3` |
| p03 | `Art/Animator/p03.controller` | `character/animator/p03` | `1804781fcd7aa084f931152950caec5d` |
| e00 | `Art/Animator/E00.controller` | `character/animator/e00` | `50a4c8a97d1195341892f78e02061686` |
| p04 | 없음 | null / 등록하지 않음 | 없음 |

현재 저장소 Settings에는 기존 `P01` Prefab 한 entry만 확인된다. 이번 Animator 4개 Address 등록은 되어 있지 않으며, 이전 Prefab/JSON/Portrait namespace 등록도 실제 Editor 상태와 확인해야 한다. Settings/Group은 수정하지 않았다.

Editor 등록 후 사람이 검증할 항목:

1. p01/p02/p03/e00의 정상 controller 로딩과 전투 애니메이션.
2. p04를 섞은 batch에서 뒤 캐릭터의 controller 인덱스가 밀리지 않고 p04가 애니메이션 비활성 상태인지.
3. 중복 캐릭터, 재전투/Scene 이동에서 controller가 유지되는지.
4. 일부 미등록/실패 주소에서 null slot과 오류가 유지되고 뒤 항목 로딩이 계속되는지.
5. 앱 종료 해제와 Controller/Clip 의존성 유지가 정상인지.

다음 전환 후보는 Stage JSON / stagedb다. `LoadManager.cs:210`의 `PushStagedbLoadRequest` 및 소비자를 먼저 조사하면 Character JSON에서 사용한 TextAsset 로딩 패턴을 비교할 수 있다. Terrain은 실제 instance와 해제 시점을 따로 조사해야 하므로 함께 전환하지 않는 것이 적절하다. 이번에는 Stage 관련 구현을 변경하지 않았다.

## p01 Animator 대상 정정

P01 Prefab의 Animator `m_Controller` (`Prefabs/Player1/P01.prefab:3370`)는 GUID `e2af5dd28e7152349826c98264eb97c2`를 참조하며, 실제 asset은 `Art/Source/FIle1/SD_unitychan/Prefabs/P01.controller`이다. 이전 조사는 Mapping의 `AnimatorName=p01`과 bundle label만 따라 `Art/Animator/p01.controller`를 선택했고 Prefab GUID 대조를 놓쳤다. Mapping의 p01 AnimatorName을 `P01`로 정정했다. Address `character/animator/p01`과 로더 정책은 유지한다. p02/p03/e00/p04는 변경하지 않았다.

Prefab의 GameCharacter `this_ani_controller`에는 별도의 GUID `b278e9f759082184198a81510b2b9087`가 남아 있고, 현재 Assets meta에서 대응 asset을 확인하지 못했다. 정상 로딩 경로는 `GameCharacter.setAnimation`에서 로드 결과로 해당 필드를 갱신하고 `Character_Animation`이 실제 Animator에 적용한다. 이 정정은 Prefab이나 이 필드를 수정하지 않는다. 실제 runtime catalog가 어떤 asset을 반환하는지는 Unity 실행 없이 확정하지 않는다.
