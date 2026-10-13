# Character JSON / characterdb Addressables 전환 결과

2026-10-08. 사용자가 선택한 정책: **메뉴 진입 전 모든 매핑된 Character JSON 비동기 준비, 준비 전 동기 이름 조회는 명확한 예외**. 코드 변경과 정적 검증만 수행했다. Compile/Build/Package Restore/Unity 실행/패키지 변경은 수행하지 않았다.

## A. 수정 파일

- [LoadManager.cs:200](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:200): JSON 단일/batch Queue 경로 변경. loadMenuScene에서 전체 master 준비 요청 및 성공 여부 확인 후 MainMenu 진입.
- [CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs): 기존 Entries 값을 열거하는 internal AllContents 추가. 기존 ID/콘텐츠 매핑 값은 동일.
- [LoadManager.AssetBundle.cs:16](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:16): 기존 동기 구현을 private ReturnChrDataNameAssetBundleLegacy로 명명. 구현 본문은 동일.
- [LoadManager.CharacterJsonAddressables.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.CharacterJsonAddressables.cs): 신규 LoadManager partial. TextAsset 세션 캐시, 단일/batch JSON 로딩, 캐시 전용 public ReturnChrDataName 구현.
- 신규 partial의 .meta: 새로운 GUID만 추가. 기존 GUID 변경 없음.
- REPORT.md, verification.json, task.diff: 요청한 A–H 보고와 정적 검증 근거.

## B. 기존 Character JSON 로딩 흐름

단일: PushCharacterdbLoadRequest → CharacterContentMapping.JsonName → Queue → LoadTextAssetAsync("characterdb", name) → JsonUtility.FromJson<CharacterInformation> → ID 정규화 → callback.

batch: PushCharacterdbsLoadRequest → Mapping.Resolve(JsonName) → Queue → LoadTextAssetsAsync("characterdb", names) → 입력 순서대로 master 목록 callback.

동기: [CharacterData.Name](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/dataclass.cs:34) → ReturnChrDataName → characterdb bundle 동기 오픈/asset 로드/JSON 파싱 → 이름 반환 → Unload(false).

활성 소비자는 다음과 같다.

- [CharacterUpgradeUI.set_data](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeUI.cs:87): 단일 master callback 후 getSkillLV 및 프로필 표시.
- [CharacterUI](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUI.cs:45): 이름/position/grade 등 정렬에 batch master 요청 사용. LV 정렬은 먼저 master를 요청하지 않고 바로 icon 생성.
- LoadManager.loadingGamesceneAsync 및 남겨진 _T 메서드: Prefab/master/Animator 목록을 동일 인덱스로 결합. 공통 public JSON API를 통해 새 경로 사용.
- [CharacterListIconUI.Updatedata](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterListIconUI.cs:78), [spawncharactericonUI](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/spawncharactericonUI.cs:14), CharacterUpgradeUI.updateprofil: CharacterData.Name 동기 getter 소비.

검색에 나타난 Editor의 ItemData.Name은 캐릭터 이름 조회와 구분했다. Editor master 파일 읽기/작성은 아래 G의 소스 파일 경로이며 characterdb bundle 소비가 아니다.

## C. 변경 후 Addressables 흐름

CharacterId → 기존 CharacterContentMapping.Get/Resolve → JsonName → Queue → Addressables.LoadAssetAsync<TextAsset>(JsonName) 또는 TextAsset 핸들 캐시 → 기존 convertTextasset<CharacterInformation> → callback.

단일 API 및 batch API의 이름·인자·callback 타입은 그대로다. batch는 단일 JSON helper를 순차 재사용한다. 입력 순서와 중복 슬롯을 유지하고 실패 슬롯은 null로 남긴다. 빈 목록은 빈 결과로 callback한다. 요청 진행량은 단일 2, batch n + 1이다.

convertTextasset의 기존 CharacterContentMapping.NormalizeData 적용을 그대로 사용한다. 캐시 hit를 포함해 callback마다 새 CharacterInformation을 파싱한다. CharacterInformation.getSkillLV는 skills/LB의 skillLV를 변경하므로 파싱된 master 객체 자체를 여러 호출에 공유하지 않는다.

메뉴 초기 로딩은 Mapping.AllContents의 CharacterId 목록을 기존 batch JSON API에 전달한다. Queue의 모든 JSON 준비 결과가 non-null인지 확인한 뒤 MainMenu Scene을 로드한다. 하나라도 실패하면 오류를 기록하고 메뉴 진입을 취소한다. 사용자가 선택한 fail-fast 정책이며, fallback이나 임의 이름 대체는 없다.

## D. ReturnChrDataName 등 동기 API 처리 결과

public ReturnChrDataName(CharacterData)의 시그니처와 string 반환형은 유지했다. 동작은 **준비된 캐시에서만 이름을 읽기**로 바뀐다. 핸들이 없거나 유효하지 않거나 아직 완료되지 않았거나 로딩 실패 상태이면 InvalidOperationException을 발생시킨다. 이름 조회 안에서 Addressables 요청, Coroutine 대기, WaitForCompletion, AssetBundle/파일 fallback을 수행하지 않는다. 준비된 TextAsset의 JSON 파싱은 일반 동기 CPU 작업이다.

이는 임의 시점에 처음 호출해도 bundle에서 이름을 읽어주던 이전 계약과 다르다. 변경 전에 정책을 보고했고 사용자가 캐시 미준비 예외 및 준비 성공 후 메뉴 진입을 선택했다. CharacterData.Name getter와 UI 호출부는 수정하지 않았다.

일반 Title → loadMainMenu 진입에는 전체 master 준비 단계가 있다. MainMenu나 기타 UI를 직접 실행하여 이 단계를 우회하는 수동 테스트는 먼저 JSON 준비를 완료해야 한다. 이름 getter에 비동기 작업을 숨기거나 UI 계약을 새로 설계하지 않았다.

## E. cache / handle 소유권

LoadManager의 static CharacterJsonHandles가 JsonName address별 TextAsset operation 하나를 세션 동안 소유한다. 요청을 시작한 뒤 첫 yield 전에 캐시에 보관한다. 반복/중복 요청은 같은 로드 operation을 재사용하지만 반환 CharacterInformation은 각각 새 객체다. 네트워크/리소스 재로드 방지와 mutable master 데이터 분리를 동시에 유지한다.

실패 또는 파싱 실패는 오류를 기록하고 해당 소유 operation과 일치하는 캐시 항목만 제거/Release한 뒤 null callback을 전달한다. 다른 요청이 교체한 새 operation을 stale observer가 해제하지 않는다. 성공 핸들은 콜백 직후나 Scene 전환 때 해제하지 않고 Application.quitting에서 Release한다. 취소된 Coroutine도 이미 시작한 operation의 소유권을 잃지 않는다.

Prefab 구현 변경 금지 조건에 따라 기존 LoadManager.Addressables.cs는 바이트 그대로 유지했다. JSON은 TextAsset 타입의 별도 캐시를 두되 기존 Dictionary/유효성 guard/handle equality/세션 종료 이벤트 패턴과 progress 필드, convertTextasset을 재사용한다. 새로운 ContentProvider나 Registry, generic loader 구조는 도입하지 않았다.

## F. Legacy AssetBundle 보존 상태

기존 ReturnChrDataName 본문은 PlannedLegacy 파일의 private ReturnChrDataNameAssetBundleLegacy로 보존했다. 이 메서드를 부르는 활성 호출은 없다. 기존 일반 LoadTextAssetAsync<T>/LoadTextAssetsAsync<T> 구현도 그대로 남아 Stage/Item 등 기존 리소스 로딩에 사용된다. Legacy 코드는 삭제/이동하지 않았다.

Portrait/Animator/Stage/Terrain/Item/SFX, Prefab Addressables 구현, 전투 로직, Instantiate 위치, 서버/DB, Settings/Packages를 수정하지 않았다. 자동 fallback 없음.

## G. 아직 남은 characterdb 직접 참조

- PlannedLegacy의 private ReturnChrDataNameAssetBundleLegacy 안에 "/characterdb" 경로가 남는다. 구현 보존용이며 active callback/name 경로에서는 호출하지 않는다.
- Legacy/Unused/AssetBundle/AssetBundleManager의 CharacterDB 및 private LoadCharacterDBAssetasync는 보존한다. 이번 활성 LoadManager 경로와 연결하지 않았다.
- Editor의 UserDataManager.readCharacterinformation/createChracterInformation 및 TextAsset 직접 파싱은 기존 JSON 소스 파일/Editor tooling 경로로 유지한다. Addressables 실패 시 사용하는 fallback이 아니며 characterdb bundle 직접 참조도 아니다.
- LoadManager의 활성 단일/batch JSON 요청 및 동기 이름 조회에는 characterdb bundle 직접 접근이 남지 않는다.

현재 Addressables Settings에는 P01 Prefab address만 등록되어 있다. 필요한 JSON TextAsset address는 다음과 같다.

| CharacterId | JSON asset | address |
|---|---|---|
| p01 | Assets/_Project/Data/Json/Character/p01.json | p01 |
| p02 | Assets/_Project/Data/Json/Character/P02.json | P02 |
| p03 | Assets/_Project/Data/Json/Character/p03.json | p03 |
| p04 | Assets/_Project/Data/Json/Character/P04.json | P04 |
| e00 | Assets/_Project/Data/Json/Character/E00.json | E00 |

등록 전에는 메뉴 진입이 실패한다. 이번에 Settings를 자동 변경하지 않았다. 일부 JsonName은 PrefabName과 같은 문자열이므로 다른 타입의 등록 항목과 혼동되지 않고 TextAsset을 해결하는지도 사람이 확인해야 한다.

## H. 다음 Addressables 전환 추천

현재 JSON 등록/수동 검증을 먼저 완료해야 한다. 이후 Portrait 경로를 추천한다. getcharactersprite와 전체 Portrait cache 준비가 이미 분리돼 있어 연결 지점을 조사하기 쉽다. Texture2D 핸들뿐 아니라 Sprite.Create로 생성한 Sprite의 소유권/Destroy 시점을 먼저 정해야 한다. Animator 전환은 전투의 setAnimation 후처리와 연결되므로 별도 범위로 유지한다.

## 검증

[verification.json](D:/GIthub/UnityFight_Portfolio/docs/addressables-character-json/verification.json), [task.diff](D:/GIthub/UnityFight_Portfolio/docs/addressables-character-json/task.diff).

작업 시작 snapshot의 9,096개 제품/설정 파일 비교. 수정 제품 파일 3개, 신규 partial/meta 2개, 삭제·이동 없음. 기존 meta 4,762개 바이트 보존. Legacy 동기 본문은 이름/접근제어만 변경한 것으로 검증했다. public LoadManager API 시그니처 유지, Prefab 구현 및 기타 코드/에셋/설정 보존, Git diff whitespace 검사 문제 없음. 독립 정적 리뷰에서 actionable finding 없음.

수동 검증 대상: JSON 등록 후 메뉴 준비 성공, LV 정렬의 이름 표시, stage 적 툴팁, 강화/전투 JSON callback, 반복·중복 요청에서 skillLV가 다른 callback 결과로 전파되지 않는지, 미준비 동기 이름 예외, JSON 실패 시 메뉴 진입 취소. Unity Compile/Build/실행 결과를 확인한 주장은 하지 않는다.
