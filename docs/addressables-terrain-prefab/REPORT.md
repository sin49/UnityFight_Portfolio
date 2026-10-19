Terrain Prefab Addressables 전환 — 코드 및 정적 검증 완료. Editor 등록·Unity 실행 검증은 별도입니다.

# A. 수정 파일

| 파일 | 변경 내용 |
| --- | --- |
| `Assets/_Project/Scripts/Manager/LoadManager.cs` | PushLoadTerrainRequest 내부 enqueue 한 표현식만 Terrain Addressables helper로 변경. |
| `Assets/_Project/Scripts/Manager/TerrainContentMapping.cs` | Terrain 선택 값 / 물리 Prefab 이름 / Address를 명시적으로 분리. |
| `Assets/_Project/Scripts/Manager/LoadManager.TerrainAddressables.cs` | Prefab 전용 GameObject typed handle/session cache, Mapping 조회, 실패 처리, 종료 Release. |
| 신규 C# 두 파일의 `.meta` | 새 스크립트에만 GUID 생성. 기존 meta 및 GUID 변경 없음. |
| `docs/addressables-terrain-prefab/REPORT.md`, `verification.json`, `task.diff` | 변경 결과, 정적 검증, 실제 전후 diff. |

public signature는 `PushLoadTerrainRequest(string s, Action<GameObject> action)` 그대로입니다. 새 ContentProvider/Registry/instance lifetime 추상화는 추가하지 않았습니다.

# B. 기존 Terrain 로딩 흐름

`StageInformation.TerrainDataName` → `LoadManager.loadingGamesceneAsync` (`LoadManager.cs:508`) → `PushLoadTerrainRequest` (`:173`) → `LoadPrefabAsync("StageTerrain", s, action)` → AssetBundle 로딩 → Prefab callback → `Unload(false)`.

실제 bundle 파일/label은 `stageterrain`, 직접 asset은 `Assets/_Project/Prefabs/Terrain/Main0.prefab`입니다. 기존 코드 요청 문자열은 `StageTerrain`으로 대소문자가 달랐습니다.

Legacy 구현 근거는 `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:139–183`입니다. 이 파일은 수정하지 않았습니다.

# C. 변경 후 Addressables 흐름

`TerrainDataName` → `LoadTerrainPrefabAddressableAsync` → `TerrainContentMapping.Get(terrainName).PrefabAddress` → `Addressables.LoadAssetAsync<GameObject>` → 기존 Prefab callback → 기존 Stage Instantiate / Pooling.

Mapping 조회는 queue의 coroutine 실행 안에서 수행합니다. public API 호출 시 callback 결과나 Instantiate 위치가 달라지지 않습니다. Instance가 아닌 `handle.Result` Prefab asset을 반환합니다. InstantiateAsync / ReleaseInstance는 사용하지 않습니다.

같은 주소의 in-flight 또는 성공 handle을 재사용합니다. 이름을 직접 Address로 전달하지 않으며, AssetBundle fallback도 없습니다. 단일 요청의 기존 progress budget 2를 유지합니다.

지원되는 주소의 실제 로딩 실패는 오류를 기록하고 callback(null)을 반환합니다. 실패 handle은 제거·해제하여 다음 요청이 재시도할 수 있습니다. 구형 bundle 자체를 열지 못했을 때는 callback이 생략됐으므로 호출 횟수에는 차이가 있습니다. 기존 소비자의 로컬 Terrain 값은 어느 경우에도 null 상태이며, Stage의 기존 null 처리 코드는 변경하지 않았습니다.

# D. Terrain Mapping 구조

| logical TerrainName | physical PrefabName | PrefabAddress |
| --- | --- | --- |
| Main0 | Main0 | `terrain/prefab/Main0` |

세 값은 Content의 독립 readonly 필드이며 explicit entry로 정의합니다. 현재 한 entry만 있으며 파일명이나 요청 값에 prefix를 붙여 Address를 추정하지 않습니다. 대소문자는 현재 선택 값 Main0를 유지합니다. 등록되지 않은 non-null 이름은 Mapping에서 ArgumentException으로 거부합니다.

Stage ID를 Terrain ID로 바꾸거나 Stage JSON의 TerrainDataName을 변경하지 않았습니다. Main0-0은 Main0를 선택하고 Main0-1은 값이 없으며, Main0-3 JSON은 존재하지만 현재 Active Stage JSON 목록에는 없습니다.

# E. null Terrain 처리

기존 generic AssetBundle helper에는 null guard가 없었습니다. Bundle을 성공적으로 연 경우 Unity의 `AssetBundle.LoadAssetAsync(null)`은 `NullReferenceException`을 던지고 callback을 호출하지 않습니다.

근거: [Unity 공식 C# reference source의 LoadAssetAsync](https://github.com/Unity-Technologies/UnityCsReference/blob/master/Modules/AssetBundle/Managed/AssetBundle.bindings.cs).

새 helper도 queue 실행 시 null이면 동일한 예외 유형/메시지(`The input asset name cannot be null.`)로 거부합니다. callback, Mapping 조회, Address 생성, handle/cache 접근, Addressables 로딩은 수행하지 않습니다. Main0로 대체하거나 null을 성공 callback으로 전달하지 않습니다.

구형 bundle을 여는 단계는 제거되었으므로 null 실패 시점은 queue 내부의 더 앞 단계입니다. 기존에는 bundle 열기 자체가 실패하면 예외 대신 callback 없이 종료했지만, 이제 null 입력 자체가 queue 실행 시 예외입니다. 모든 오류 타이밍까지 동일하다는 의미는 아닙니다. 이번 변경은 null Terrain이 정상 실행되도록 고치지 않습니다.

Main0-1의 누락된 TerrainDataName, 재전투의 기본 필드 활성화/선택 문제, queue의 예외 복구 정책은 그대로 둡니다.

# F. handle/cache 소유권 및 Release 정책

- 소유자: LoadManager partial의 static `TerrainPrefabHandles`.
- key: `terrain/prefab/Main0` 등 Mapping에서 얻은 PrefabAddress.
- 유효한 in-flight/완료 handle을 재사용합니다.
- 첫 yield 이전에 새 handle을 등록하므로 queue 또는 Manager가 중단되어도 소유자가 남습니다.
- 대기 중 handle 유효성을 확인합니다.
- 실패 시 동일 handle을 cache가 소유할 때만 제거하고 유효한 경우 Release합니다. 다른 요청이 cache에 넣은 새 operation을 잘못 해제하지 않습니다.
- 성공 callback 직후, Scene 이동, LoadManager 파괴 시에는 Release하지 않습니다.
- `Application.quitting` → `ReleaseTerrainPrefabSessionHandles`에서 유효 handle을 Release하고 cache를 비웁니다.

이는 기존 Character Prefab session handle cache 정책과 같습니다. 해당 Character 구현 자체는 수정하지 않았습니다. Stage의 수동 Instantiate 및 pool 타일들이 Prefab/TerrainData/Material 의존성을 사용할 수 있으므로 성공 handle을 앱 종료까지 유지합니다.

# G. Stage Instantiate/Pooling 및 Legacy 보존

`Stage.cs`는 작업 전 hash와 동일합니다.

- `setStage`: 로드된 Terrain Prefab을 보관.
- `initailizeField` (`:545`): 첫 Terrain instance를 일반 Instantiate로 생성.
- `TerrainGenerate` (`:566`): 이전 타일을 비활성화하며 다음 타일 선택.
- `pulling_Field` (`:582`): 비활성 타일 재사용, 없으면 current_Field instance를 FieldTransform 아래에 복제.

Legacy generic LoadPrefabAsync, StageTerrain/stageterrain label/bundle 파일/manifest 및 기존 Unload(false)는 모두 보존했습니다. Active Terrain API는 Legacy helper를 더 이상 호출하지 않습니다. Item 등 다른 미전환 generic 로더 호출은 제거하지 않았습니다.

정적 검증에서 전환 전 9,132개 제품 파일과 비교했습니다. 기존 파일 변경은 LoadManager enqueue 한 표현식뿐이며 새 script/meta 네 파일만 추가됐습니다. 기존 meta 4,780개 모두 byte 동일입니다. Stage/전투, Stage JSON, Character Addressables 및 Mapping, Item/SFX, Legacy, Settings/Group, Package, 직렬화 Scene/Prefab, TerrainData/Material, 서버 파일도 모두 byte 동일입니다.

public method signature, explicit Mapping, null-before-load, cache/실패/해제 경로, 선언 중복 및 괄호/import 상태, per-task diff whitespace를 정적으로 확인했습니다. read-only 리뷰에서도 수정이 필요한 문제가 발견되지 않았습니다. 정적 검사가 컴파일 성공을 보장하는 것은 아닙니다.

Compile / Build / Package Restore / Unity 실행은 수행하지 않았습니다.

# H. Editor 등록 대상과 남은 기존 Terrain 이슈

| 실제 asset | Address | 기존 GUID |
| --- | --- | --- |
| `Assets/_Project/Prefabs/Terrain/Main0.prefab` | `terrain/prefab/Main0` | `c0b43649f3fdd304b8ced95f620031e9` |

Settings/Group은 자동 수정하지 않았습니다. 위 기존 Prefab을 Editor에서 등록하고, 해당 주소가 다른 asset과 중복되지 않도록 확인합니다. 파일 rename이나 기존 GUID 변경은 필요하지 않습니다.

남은 기존 이슈:

1. Main0-1 JSON에 TerrainDataName이 없습니다. 이번에 값을 추가하거나 fallback을 만들지 않았습니다.
2. ReloadingGamesceneAsync는 Terrain Prefab을 다시 전달하지 않고 새 Stage의 ResetStage를 호출합니다. 새 Scene의 기본 current_Field를 사용하는 구조를 그대로 유지했습니다.
3. StageTest Scene의 기본 Main0 instance는 비활성으로 저장되어 있고, Terrain Prefab이 없으면 initailizeField가 이를 명시적으로 활성화하지 않습니다. 재전투의 활성화/pool 동작은 별도 조사·수정 대상입니다.
4. 구형 bundle 문자열 StageTerrain과 실제 stageterrain의 대소문자 및 과거 manifest 경로도 Legacy 상태로 보존했습니다. 현재 Active Addressables 경로는 그 bundle 이름을 사용하지 않습니다.
5. session cache는 메뉴로 돌아간 뒤에도 Terrain 의존성을 유지합니다. 전투별 해제는 모든 일반 clone의 수명과 연결해야 하므로 이번에 추가하지 않았습니다.

Editor 등록 후 사용자가 Main0 최초 전투 진입, 같은 Terrain 재요청, Stage pool 타일 증가/재사용, Scene 이동 및 앱 종료를 검증해야 합니다. 위 기존 null/재전투 문제의 정상 동작을 이번 전환의 성공으로 주장하지 않습니다.
