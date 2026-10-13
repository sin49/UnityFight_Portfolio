완료 — Stage JSON 코드 전환 및 정적 검증 기준. Editor Address 등록과 Unity 실행 검증은 별도이며 아직 수행하지 않았습니다.

# Stage JSON Addressables 전환 상태 재검증

## A. 수정 파일 및 발견된 누락

재개 시 실제 파일 목록에는 신규 C# 두 파일이 없었고, 기존 `.meta`와 LoadManager의 호출 연결은 남아 있었습니다. 이 상태에서는 helper/Mapping 심볼 누락 오류가 예상됩니다. 보존된 `task.diff`에서 기존 구현을 복원했습니다. 새로운 로딩 설계나 public API는 추가하지 않았습니다.

이번 재개에서 복원한 파일:

- `Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.StageJsonAddressables.cs`
- `Client/1_backup/Assets/_Project/Scripts/UI/Menu/Stage/StageContentMapping.cs`

기존 meta/GUID를 재사용했습니다. `LoadManager.cs`는 이미 두 연결이 완료되어 있어 이번에는 수정하지 않았습니다. 이전 기록과 현황을 일치시키도록 이 보고서, `verification.json`, `task.diff`를 갱신했습니다.

## B. 기존 Stage JSON 로딩 흐름

원본 snapshot의 `LoadManager.cs:210` 단일 API는 `LoadTextAssetAsync("stagedb", ID, action)`을 enqueue했습니다. 전체 목록 API `:215`는 `LoadAllTextAssetsAsync<StageInformation>("stagedb", ...)`을 enqueue해 아직 Account에 없는 StageData를 추가했습니다.

소비자 근거:

- `UI/Menu/Stage/stageinfoUI.cs:75–90`: `setData` → 단일 요청 → `DataLoadExecute` → callback에서 `UpdateInfo`.
- `UI/Menu/Stage/StageUI.cs:24–27`: `OnEnable` → 전체 목록 요청 → `DataLoadExecute`.
- `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:478`, `:535`: generic TextAsset / 전체 TextAsset bundle 구현. 기존 구현은 삭제하거나 수정하지 않았습니다.

## C. 현재 Addressables 연결

`LoadManager.cs:212`:

`PushStagedbLoadRequest` → `StageContentMapping.Get(ID).JsonAddress` → `LoadStageJsonAddressableAsync` → `LoadAssetAsync<TextAsset>` → `StageInformation` callback.

`LoadManager.cs:217`:

`PushStageDownLoadRequest` → `LoadAllStageJsonsAddressableAsync` → Mapping 전체 대상 → 같은 단일 helper → 모든 성공 결과 준비 후 기존 Account 갱신 callback.

두 public API의 이름, 인자, 반환형과 기존 호출부를 유지했습니다. 전체 목록의 Account 갱신 callback도 원본과 동일합니다. 기존 Stage가 있으면 덮어쓰지 않는 동작을 유지합니다.

실패한 단일 요청은 오류를 기록하고 성공 callback을 호출하지 않습니다. UI는 null을 받은 뒤 `info.name` 등을 읽는 구조이므로 null을 전달하지 않습니다. 전체 목록은 하나라도 실패하면 Account 갱신 callback을 생략해 부분 목록을 적용하지 않습니다. 자동 AssetBundle fallback은 없습니다.

## D. Stage ID / 파일명 / Address 관계

| 기존 논리 ID | 물리 JSON | Address |
| --- | --- | --- |
| Main0-0 | Main0-0.json | `stage/json/Main0-0` |
| Main0-1 | Main0-1.json | `stage/json/Main0-1` |

`StageContentMapping`에는 위 두 entry만 있습니다. JsonName과 JsonAddress는 명시적으로 별도 보관하며 요청 ID를 파일 경로/Address로 단순 연결하지 않습니다. JSON의 `StageInformation.ID`와 Account `StageData.ID` 값을 그대로 유지합니다. `Main` 대소문자는 Account/UI의 문자열 검사에 사용되므로 변경하지 않았습니다.

`Main0-3.json`은 존재하지만 meta의 bundle label이 비어 있어 기존 stagedb 대상이 아닙니다. Mapping과 새 전체 목록에 포함하지 않았습니다. 파일/JSON/meta를 그대로 보존하며, 이미 저장된 StageData를 삭제하는 작업도 하지 않았습니다. `Main0-2`는 존재하지 않아 생성하거나 등록하지 않았습니다. Mapping에 없는 ID 요청은 명확한 ArgumentException으로 거부합니다.

## E. 동기 API 및 preload 필요 여부

Runtime Stage 선택/목록 갱신은 이미 비동기 queue API입니다. 동기 Addressables 접근이나 새 runtime preload가 필요하지 않습니다.

`UserDataManager.cs:104`의 `LoadStageinfo`는 로컬 DB 디렉터리의 JSON 파일을 동기 읽기합니다. 확인된 호출자는 `Editor/StageCreaterEditor.cs:55`입니다. 이 Editor 파일 경로는 그대로 유지했습니다.

`UserAccountData.get_Stage` / `get_StageList`는 저장된 StageData 진행 상태 조회이며 StageInformation 원본 JSON 조회가 아닙니다. 여기도 변경하지 않았습니다. WaitForCompletion은 추가하지 않았습니다.

## F. handle/cache 및 파싱 객체 소유권

`LoadManager.StageJsonAddressables.cs:11–13`의 static `StageJsonHandles`가 주소별 TextAsset handle을 session 소유합니다.

- cache에 첫 yield 전에 handle을 등록합니다.
- 유효한 in-flight / 성공 handle을 재사용해 같은 JSON을 중복 로드하지 않습니다.
- 대기와 실패 처리에서 handle 유효성을 확인합니다.
- 실패 시 cache가 같은 handle을 소유할 때만 제거하고 유효 handle을 Release합니다.
- 성공 callback 직후, Scene 전환, LoadManager 파괴 시 Release하지 않습니다.
- `Application.quitting` → `ReleaseStageJsonSessionHandles`에서 Release 후 cache를 비웁니다.

이는 Character JSON의 typed handle/session lifetime 패턴과 일관됩니다. Character 구현은 수정하지 않았습니다.

매 요청은 cache hit에서도 `convertTextasset<StageInformation>(handle.Result)`를 새로 실행합니다. 이 기존 함수는 `JsonUtility.FromJson`과 기존 Character ID normalization을 사용합니다. StageInformation의 spawn, reward, mission, 캐릭터 데이터 등 가변 객체를 cache하지 않아 서로 다른 요청 사이의 상태 공유를 방지합니다. callback으로 받은 객체는 해당 소비자가 소유합니다.

단일 progress budget 2를 유지합니다. 전체 목록은 setup 1 + Active Stage JSON 2개로 budget을 계산합니다.

## G. Legacy / 보존 범위 / 정적 문제

Active C#의 `"stagedb"` bundle 호출은 0개입니다. Legacy generic TextAsset helper와 기존 stagedb bundle label/version 파일은 그대로 남아 있습니다. generic helper는 다른 미전환 콘텐츠에서 계속 사용할 수 있습니다.

원본 전환 전 snapshot과 비교:

- 기존 제품 파일의 변경은 LoadManager enqueue 두 표현식뿐입니다.
- Stage 관련 신규 파일은 두 C#과 기존에 생성된 meta 두 개입니다.
- 기존 meta 4,765개 byte 동일.
- Terrain, 전투, Character Addressables 및 Mapping, UI, Settings/Group, Package, 직렬화 Scene/Prefab, 데이터 JSON, 서버 파일은 모두 byte 동일.

이번 재개 직전 snapshot과 비교:

- 기존 제품 파일 9,104개 모두 byte 동일.
- 누락된 C# 두 파일만 복원해 현재 9,106개입니다.
- 기존 meta를 새로 생성하거나 GUID를 교체하지 않았습니다.

추가 확인: helper 및 타입 선언 중복 없음, 필요한 using/타입 연결 존재, 괄호 균형, 두 API의 실제 연결, Mapping 대상과 stagedb JSON label의 일치, fresh parse, cache/Release, diff whitespace. 별도 read-only 리뷰에서도 수정할 명백한 정적 오류가 발견되지 않았습니다.

이 검사는 실제 C# 컴파일 성공을 보증하지 않습니다. Compile / Build / Package Restore / Unity 실행은 수행하지 않았습니다.

## H. Editor 등록 목록과 다음 추천 작업

아래 두 기존 TextAsset을 등록합니다. 경로는 `Client/1_backup/Assets/_Project/Data/Json/stage/` 기준입니다.

| asset | Address | 기존 GUID |
| --- | --- | --- |
| Main0-0.json | `stage/json/Main0-0` | `ea1a0317665caaf478f9d4485ca2a812` |
| Main0-1.json | `stage/json/Main0-1` | `4bc3a15f72cbdba4f84dc7d1964affad` |

현재 확인한 Addressables Groups에는 위 Stage 주소가 등록되어 있지 않습니다. 사용자가 Editor에서 등록해야 하며 Settings/Group을 자동 수정하지 않았습니다. Main0-3은 이번 등록 대상이 아닙니다.

등록 후 사람이 확인할 항목은 Stage 화면 활성화 시 두 Stage만 갱신되는지, 상세 화면 반복 조회가 정상인지, mission/spawn 변경 상태가 다음 요청으로 새지 않는지, 실패 시 null UI callback 또는 부분 Account 갱신이 없는지입니다.

다음 작업은 Terrain 전환 전에 Prefab/instance 소유자와 생성·해제 시점을 조사하는 것입니다. Stage JSON 수명과 Terrain instance 수명은 분리해야 하며, 이번에는 Terrain을 변경하지 않았습니다.
