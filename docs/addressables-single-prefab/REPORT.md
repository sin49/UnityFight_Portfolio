# 단일 Character Prefab Addressables 전환 결과

2026-10-08. 실제 설치된 Addressables 2.9.1을 기준으로 코드 변경과 정적 검증만 수행했다. Compile/Build/Restore/패키지 변경/Unity 실행은 수행하지 않았다.

## A. 수정 파일

- [LoadManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:136): 단일 Character Prefab 요청 두 overload의 Queue 등록 두 줄을 Addressables Coroutine으로 교체.
- [LoadManager.Addressables.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.Addressables.cs): 기존 LoadManager의 partial. LoadAssetAsync, 세션 핸들 캐시, 실패 처리, 종료 시 Release 구현.
- [LoadManager.Addressables.cs.meta](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.Addressables.cs.meta): 신규 파일용 GUID. 기존 meta 변경 없음.
- 이 REPORT.md, verification.json, task.diff: 결과와 정적 검증 근거. 작업 시작 snapshot 대비 diff이며 기존 Phase 0 변경을 포함하지 않는다.

## B. 기존 로딩 흐름

CharacterId → CharacterContentMapping.Get → PrefabName → LoadingQueue → AssetBundle LoadPrefabAsync → Prefab asset callback → Unload(false) → 소비자의 나중 Instantiate.

기존 구현 근거: [LoadManager.AssetBundle.cs:139](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:139).

## C. 변경 후 로딩 흐름

CharacterId → CharacterContentMapping.Get → PrefabName → LoadingQueue → LoadCharacterPrefabAddressableAsync → 세션 캐시 조회 또는 Addressables.LoadAssetAsync<GameObject> → 완료 확인 → Prefab asset callback → 소비자의 기존 Instantiate.

기존 Queue 실행 방식을 유지하며 요청당 진행량 2를 유지한다. 캐시 hit도 같은 진행량을 반영한다. InstantiateAsync나 생성된 instance 반환은 사용하지 않는다. LoadManager 본체의 다른 요청은 변경하지 않았다.

관측한 실패는 캐시에서 제거하고 소유한 실패 핸들을 Release한 뒤 오류 로그와 null callback을 전달한다. Queue는 다음 요청을 처리할 수 있다. AssetBundle fallback은 없다. 실패 후 새 요청에서는 다시 로드를 시도할 수 있다.

## D. Handle 소유자와 Release 시점

소유자는 LoadManager 내부의 static CharacterPrefabHandles 세션 캐시다. 실제 address별 로드 핸들 하나를 유지하며 중복 요청은 같은 operation을 재사용한다. 핸들은 첫 yield 전에 캐시에 보관하여 Coroutine 중단 시에도 소유자를 잃지 않는다.

현재 public API에는 사용 종료 통지가 없다. 메뉴는 Account.StarCharacterModel에 Prefab을 보관하며, 강화 화면은 Instantiate한 모델을 비활성화해서 modelLibrary에 보관한다. 그러므로 소비자별 마지막 사용 시점을 추측하지 않고 성공 핸들을 세션 종료까지 유지한다.

- 성공: Application.quitting에서 ReleaseCharacterPrefabSessionHandles 실행. 유효한 핸들을 각각 Release하고 캐시와 이벤트 구독을 정리.
- 관측한 실패: 실패 경로에서 Release. 같은 실패 operation을 공유하던 다른 Coroutine은 유효성 검사 후 null callback 처리. 교체된 새 operation을 잘못 제거하지 않도록 handle equality 검사.
- Coroutine 취소 후 아직 캐시에 남은 operation: 다음 요청에서 재사용/실패 정리하거나 세션 종료에서 Release.
- 콜백 직후, 화면 전환, LoadManager의 OnDestroy에서는 성공 핸들을 Release하지 않음. 콜백 예외가 나더라도 캐시 소유권은 유지.

이 정책은 조기 해제를 피하는 최소 소유권 정책이다. 메뉴를 닫아도 메모리가 즉시 회수되는 정책은 아니다. 같은 address의 성공 Prefab은 재사용되며 세션 중 유지된다. 주소별 마지막 instance를 추적하는 시스템이나 새 ContentProvider/Registry는 추가하지 않았다.

소비 근거: [LoadManager.cs:322](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:322), [CharacterUpgradeUI.cs:91](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeUI.cs:91), [CharacterUpgradeModeling.cs:11](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeModeling.cs:11).

## E. public API 유지 여부

PushCharacterloadrequest(string, Action<GameObject>)와 PushCharacterloadrequest(string, string, Action<GameObject>)의 이름, 인자, 반환형을 유지했다. 모든 기존 LoadManager public 메서드 signature가 snapshot과 동일하다. UI/Battle 호출부는 변경하지 않았다.

bundlename 인자가 있는 overload도 단일 Character 요청이므로 Addressables로 전환했다. 인자는 소스 호환성을 위해 남지만, 이제 bundle을 선택하지 않고 mapping.PrefabName address를 사용한다. 저장소에서 해당 overload의 외부 호출은 확인되지 않았다.

## F. AssetBundle Legacy 경로 보존 여부

PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs 전체 바이트를 보존했다. LoadPrefabAsync도 삭제하지 않았다. batch Character Prefab 및 Terrain 등 기존 일반 Prefab backend는 계속 사용한다.

## G. 아직 전환하지 않은 Character 관련 경로

- PushCharactersloadrequest: batch Character Prefab은 AssetBundle.
- PushCharacterdbLoadRequest / PushCharacterdbsLoadRequest / ReturnChrDataName: Character JSON 및 동기 master 이름 조회는 AssetBundle.
- PushCharacterSpriteloadrequest 두 overload / PushCharactersSpriteloadrequest / LoadAllCharacterSpritesAsync: Portrait은 기존 경로.
- PushCharacteranimatorloadrequest / PushCharacteranimatorsloadrequest: Animator는 AssetBundle.
- Stage/Terrain, Item, SFX: 기존 AssetBundle.
- loadingGamesceneAsync의 batch → set_Character → setAnimation → Stage 생성 흐름: 그대로 유지.

중요한 적용 한계: 현재 Addressables Settings의 등록 address는 P01 하나다. 따라서 P01 단일 요청만 로딩 성공을 기대할 수 있다. P02/P03/P04 등의 단일 요청은 미등록 address 오류와 null callback을 반환하며 fallback하지 않는다. 기존 게임 전체의 단일 캐릭터 UI가 모두 정상 동작한다는 주장은 하지 않는다. 나머지 Addressable 등록도 이번 작업에서 수행하지 않았다.

현재 batch에서 P01을 로드하면 여전히 AssetBundle을 사용하므로 Addressables 캐시의 P01과 별도 객체/의존성으로 존재할 수 있다. 이번 작업은 두 backend의 에셋 공유나 메모리 통합을 구현하지 않는다.

## H. 다음 전환 추천

우선 나머지 Character Prefab을 Addressable로 등록하고 address와 Mapping.PrefabName 일치 여부를 사람이 확인하는 것이 선행 조건이다. 그다음 batch Character Prefab이 자연스러운 전환 대상이다. 리스트 순서, 중복 ID, 이후 set_Character/setAnimation 적용, 동일 Prefab을 공유할 때의 데이터 변경을 먼저 검토해야 한다. JSON/Portrait/Animator 전환은 그 이후 별도 범위로 다룬다.

## 정적 검증

[verification.json](D:/GIthub/UnityFight_Portfolio/docs/addressables-single-prefab/verification.json), [task.diff](D:/GIthub/UnityFight_Portfolio/docs/addressables-single-prefab/task.diff).

작업 시작 파일 9,094개와 비교했다. 기존 제품 파일 수정은 LoadManager.cs 하나, 신규 제품 파일은 partial과 meta 두 개다. 기존 meta 4,761개, Scene/Prefab/JSON, Addressables Settings, Packages/ProjectSettings, Server, 사용자 테스트 스크립트를 보존했다. Git diff whitespace 검사 문제 없음. 별도 정적 코드 리뷰에서 actionable finding 없음.

실행/컴파일 결과는 검증하지 않았다. 수동 Editor 검증에서는 P01 대표 모델과 강화 모델, 화면 전환 후 Prefab 재사용, 반복 요청, 미등록 address 실패, 기존 batch 전투 경로를 확인해야 한다.
