# Unity Fighter Phase 0-A 결과

2026-10-01. 대상: `Client/1_backup/Assets`. 기존 AssetBundle 구현의 물리적 분리와 Assets 전체 폴더 정리를 수행했다. Addressables나 새로운 로딩 구조는 추가하지 않았다.

## A. Changed Files

전체 개별 파일의 이전/이후 경로, 내용 보존 여부, 기존 GUID는 [changed-files.csv](changed-files.csv)에 기록했다. 기존 Assets 파일 8,736개 중 8,593개를 이동했으며 `.meta`도 개별 행으로 포함한다. Git에 stage/commit하지 않았으므로 작업 트리에서는 삭제와 새 경로가 따로 표시될 수 있다.

내용이 변경된 기존 Assets 파일은 다음 세 개뿐이다.

| 파일 | 변경 |
|---|---|
| `_Project/Scripts/Manager/LoadManager.cs` | `partial` 선언, 기존 AssetBundle 필드/메서드의 동반 파일 이동 |
| `_Project/Scripts/Manager/UserDataManager.cs` | 실제 JSON 폴더 이동에 맞춰 기존 파일 경로 문자열 연결 |
| `_Project/Scripts/UI/TextScriptReader.cs` | 두 기존 Text 파일 경로 문자열 연결 |

추가된 구현 파일은 `_Project/Scripts/AssetBundle/LoadManager.AssetBundle.cs`와 그 `.meta`다. 새 분류 폴더의 `.meta`도 추가했다. 기존 `ProjectSettings/EditorBuildSettings.asset`의 다섯 씬 경로와 루트 `README.md`의 네 소스 링크를 새 경로로 연결했다. README의 작업 시작 전 사용자 수정은 보존했다. 보고서, 대응표, 검증 JSON 및 컴파일 로그를 `docs/phase0a`에 추가했다.

작업할 때는 아래 구조를 기준으로 찾으면 된다. 내부 파일명·대소문자·서드파티 내부 구조는 유지했다.

```text
Assets/
  _Project/
    Scripts/
      Manager/                   실행 흐름과 기존 매니저
      AssetBundle/               실행 중인 LoadManager의 기존 Bundle 구현
      Legacy/AssetBundle/        기존 런타임 Legacy 후보
      Editor/Legacy/AssetBundle/ 기존 Editor 도구
      Presentation/             기존 루트 UI·전투 표현·오디오 스크립트
      Development/              기존 루트 개발/테스트 스크립트
      ...                       기존 나머지 스크립트 하위 구조
    Scenes/Recovery/
    Data/Json/                  원본 JSON 구조 그대로
    Data/Text/
    Prefabs/Environment/
    Art/                        Animation, Animator, Sprite, Shader,
                                Terrain, Font, Materials, Textures,
                                Source/FIle1
    Audio/
    Effects/Particle/
    Settings/Rendering/
  ThirdParty/                   16개 공급자 폴더와 관련 설명문
  Resources/                    Unity 특수 경로 유지
  StreamingAssets/              기존 Bundle·manifest·cache 유지
  TextMesh Pro/                 패키지/Editor 경로 관례 유지
```

`Development` 분류는 삭제나 미사용 판정을 뜻하지 않는다. 서드파티 폴더의 내부 `Editor`, `Resources` 등은 그대로 두었다.

## B. Legacy Classification

판정은 현재 정적 C# 참조, 씬 연결, Editor 메뉴 및 시작 흐름을 기준으로 한다. 동적 외부 호출 가능성을 부정하거나 코드를 삭제하는 판정은 아니다.

| 대상 | 분류 | 근거와 위치 |
|---|---|---|
| `AssetBundleManager` | Legacy-Unused | 현재 C# 호출/씬 참조를 찾지 못함. `Scripts/Legacy/AssetBundle`로 이동 |
| `DownLoadManager` | Legacy-Referenced | Title 씬 컴포넌트와 패킷 처리 코드의 참조가 남음. 시작 다운로드 호출은 비활성 상태. 같은 Legacy 폴더로 이동 |
| `Developer_VersionManager`, `Developer_VersionCheck` | Legacy-Referenced | 빌드 목록 밖 개발 씬에서 연결됨. 같은 Legacy 폴더로 이동 |
| `AssetBundleMaker`, `VersionEditor` | Legacy-Referenced | 수동 Editor 메뉴 진입점. `Scripts/Editor/Legacy/AssetBundle`로 이동하여 Editor 컴파일 소속 유지 |
| `LoadManager` 및 `LoadManager.AssetBundle.cs` | Active-Core | 현재 콘텐츠 로딩의 중심. 두 파일은 동일한 논리 클래스이며 새 서비스가 아님 |
| `StreamingAssets/Assetbundle`의 기존 Bundle·manifest | Active-Core | 현 로더가 읽는 배포 콘텐츠. 내용/파일명을 유지 |
| 다운로드·버전·manifest 처리 | Legacy-Referenced / Active-Core | 다운로드/개발/Editor 코드는 위 Legacy 영역, 실제 파일 로딩은 Active-Core 동반 파일에 남음 |

`LoadManager`의 호출되지 않는 일부 보조 메서드도 삭제하지 않고 원래 코드 덩어리와 함께 이동했다. 서버의 버전/프로토콜 코드는 이번 이동 대상이 아니다.

## C. LoadManager Separation

기존 `ReturnChrDataName`, 전체 `LoadCorutine` region, 비직렬화 private 필드 `bundleDirectory`를 `Scripts/AssetBundle/LoadManager.AssetBundle.cs`로 그대로 옮겼다. 이 파일에 기존 `AssetBundle.LoadFromFile*`, `LoadAsset*`, `LoadAllAssets*`, `Unload`를 사용하는 메서드가 모인다.

원래 `LoadManager.cs`와 `.meta`는 유지했다. 클래스 이름, namespace, 상속, public API, 직렬화 필드와 MonoBehaviour 연결은 유지하고 선언에 `partial`만 추가했다. 새 클래스·인터페이스·forwarding·상태 관리 로직은 없다. 초기화 순서를 유지하기 위해 `bundleDirectory = Application.streamingAssetsPath + "/Assetbundle"` 할당은 기존 초기화 위치에 남겼다.

이동한 메서드와 필드를 원래 위치로 재조합하고 `partial` 키워드를 되돌리면 변경 전 소스와 바이트 단위로 일치한다. Coroutine 실행 순서, callback, 반환값, unload 시점, 캐시와 queue 코드는 재작성하지 않았다. 외부 호출부는 같은 `LoadManager` API를 사용한다.

## D. Behavior Changes

**의도된 동작 변경: None.** 두 직접 파일 접근 스크립트와 빌드 씬 목록의 경로만 실제 이동 위치에 맞게 연결했다. 실행 검증 완료를 의미하지 않으며 해당 한계는 G에 기록했다.

CharacterId, JSON, 서버 프로토콜, DB, 전투, PvP, 기존 버그, 로딩 알고리즘, callback 계약을 변경하지 않았다. Addressables 설치/코드, ContentRegistry, 데이터 모델, 새 인터페이스를 추가하지 않았다.

## E. Unchanged Couplings

- `LoadManager`의 기존 캐시·queue·씬 흐름과 Bundle 메서드는 동일 클래스의 필드를 공유한다. 물리적 분리이며 독립 provider 경계는 아니다.
- 기존 Bundle 이름, 파일 접근 방식, JSON/Prefab/Portrait/Animator 이름 및 대소문자를 유지했다.
- 기존 다운로드/Editor 출력 경로와 런타임 `StreamingAssets/Assetbundle` 경로 차이를 유지했다.
- 직접 JSON/Text 파일 읽기의 현재 작업 디렉터리 의존과 빌드 배포 문제를 고치지 않았다.
- Mock/singlemode 시작 조건, 비활성 서버 연결, 기존 전투 및 로딩 문제를 유지했다.
- 기존 manifest의 소스 asset 경로는 이동 전 경로다. 기존 prebuilt Bundle과 함께 보존했으며 재빌드/manifest 재작성은 수행하지 않았다.

### CharacterId → 리소스 대응

전체 정확한 경로는 [character-resource-map.csv](character-resource-map.csv)에 기록했다. 아래 경로는 `_Project` 기준이다. 파일명 대응은 런타임 Bundle 내부 조회의 성공 판정이 아니다.

| 계정/JSON id | JSON | Prefab / 내부 ID | Portrait | 같은 ID의 Animator 파일 |
|---|---|---|---|---|
| `p01` | `Data/Json/Character/p01.json` | `Prefabs/Player1/P01.prefab` / `p01` | `Art/Sprite/Character/p01.png` | `Art/Animator/p01.controller` |
| `p02` | `Data/Json/Character/P02.json` | `Prefabs/Player2/P02.prefab` / `p02` | `Art/Sprite/Character/P02.png` | `Art/Animator/p02.controller` |
| `p03` | `Data/Json/Character/p03.json` | `Prefabs/Player3/P03.prefab` / `p03` | `Art/Sprite/Character/P03.png` | `Art/Animator/p03.controller` |
| `P04` | `Data/Json/Character/P04.json` | `Prefabs/Player4/P04.prefab` / `P04` | `Art/Sprite/Character/P04.png` | 없음 |
| `E00` (stage enemy) | `Data/Json/Character/E00.json` | `Prefabs/Test_enemy/E00.prefab` / `E00` | `Art/Sprite/Character/E00.png` | `Art/Animator/E00.controller` |

다섯 Prefab의 `is_animation`은 모두 `0`이다. 네 Player Prefab의 `this_ani_controller`에는 동일 GUID `b278e9f759082184198a81510b2b9087`가 저장되어 있지만 현재 Assets의 `.meta`에서 대응 파일을 찾지 못했다. E00의 해당 필드는 `fileID: 0`이다. 이 상태는 변경 전과 동일하며 같은 ID의 Animator 파일 존재와 실제 직렬화 연결을 구분해야 한다. `Player1/(Past)P01.prefab`도 그대로 보존했다. P04 Animator나 누락 GUID 대응 파일을 임의 생성/연결하지 않았다.

## F. Blocked / Design Decision Required

| 항목 | 상태 | 이유 |
|---|---|---|
| Unity C# 컴파일 | Blocked: Environment | 기존 `Library/PackageCache`가 없고 Bee response file이 참조하는 package DLL이 누락됨. 변경 전/후 모두 CS0006으로 중단 |
| Unity import, Missing Script 실제 검사, Play Mode | Unverified | 현재 환경에서 의존성 복원 없이 검증하지 못함. 정적 참조/메타 보존은 검사함 |
| 독립 콘텐츠 provider, API/ID 표준화 | Design Decision Required | 새로운 아키텍처와 계약 변경이 필요하여 이번 범위에서 수행하지 않음 |
| 이동 후 Bundle 재빌드와 Legacy 경로 정합화 | Design Decision Required | 기존 콘텐츠/빌드·다운로드 계약 변경이 필요함. 바이너리와 manifest를 보존함 |
| Animator 누락/연결 및 대소문자 정규화 | Design Decision Required | 기존 데이터/동작을 바꾸는 작업이라 기록만 함 |

기존 `ProjectSettings.asset`의 template default scene은 존재하지 않는 `Assets/Scenes/SampleScene.unity`를 가리키지만 해당 씬은 변경 전에도 없었다. 기존 결함으로 유지했다.

## G. Verification

검증 증거는 [verification.json](verification/verification.json) 및 [변경 전 컴파일 결과](verification/before/compile-results.json), [변경 후 컴파일 결과](verification/after/compile-results.json)에 보존했다.

- 원본 8,736개 파일 모두 이동 후 존재. SHA-256 비교에서 허용된 세 스크립트 외 원본 내용 변경 없음.
- 기존 `.meta` 4,709개 내용/GUID 그대로 보존. 새 폴더/동반 파일 GUID 중복 없음.
- 씬·Prefab 950개 바이트 동일. 기존 직렬화 참조를 편집하지 않음.
- `LoadManager` cut-and-paste 재조합 바이트 비교 통과.
- 기존 `Editor` 컴파일 소속과 모든 `Resources` 상대 키 보존. asmdef/asmref 추가 없음.
- 빌드 목록 다섯 씬의 새 경로 존재 및 GUID/순서/활성 플래그 보존.
- Packages manifest/lock, ProjectSettings.asset 내용 보존. 서버/DB/기존 Library 변경 없음.
- 독립 읽기 전용 코드 리뷰: 조치가 필요한 발견 없음. 전체 원본 해시와 추출 코드 보존을 별도로 확인함.
- Unity 6000.4.6f1의 bundled Roslyn과 기존 Bee response file로 변경 전/후 컴파일 시도. 런타임은 각각 328/329 소스, Editor는 각각 33 소스. 양쪽 모두 같은 package 참조 누락으로 실패하여 소스의 의미 분석/컴파일 성공은 확인할 수 없음. package 설치/복원은 수행하지 않음.

실제 Unity import/Play 검증이 남아 있으므로 실행 동작까지 검증 완료했다고 주장하지 않는다.
