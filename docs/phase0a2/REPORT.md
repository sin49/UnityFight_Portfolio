# Unity Fighter Phase 0-A2 결과

2026-10-01. Phase 0-A 완료 직후의 작업 트리를 기준으로 자체 코드의 참조를 다시 조사했다. **Legacy는 삭제 대상이라는 뜻이 아니다.** 파일 이동과 폴더 분류만 수행했으며 기존 C# 내용, namespace, API, JSON, 전투·네트워크·저장 동작을 변경하지 않았다.

이번 Phase에서는 Compile / Player Build / Server Build / Package restore / 패키지 설치·수정 / DLL 복구 / 환경 설정 변경을 시도하지 않았다. 실제 Unity 실행 검증은 사람이 Editor에서 수행한다.

조사 범위는 `_Project`의 자체 C# **167개**다. `Scripts` 165개 외에 Data 안의 주석 처리된 XML reader와 Particle의 `ParticleAutoDisable`도 포함했다. ThirdParty는 품질/Legacy 분류 대상에서 제외하고 기존 참조의 소비자 여부와 파일 보존만 확인했다. 서버·DB는 분류를 위한 읽기 외에 변경하지 않았다.

전체 근거는 [code-inventory.csv](code-inventory.csv), [reference-evidence.json](reference-evidence.json), [method-candidates.csv](method-candidates.csv), [동적 호출 근거](dynamic-call-evidence.txt)에 있다. 모든 파일의 클래스/인터페이스/구조체, 주석을 제외한 C# 타입 이름 소비자, Scene/Prefab/asset/controller/animation GUID 소비자를 기록했다. 자동 inventory는 실행 도달성의 증명이 아니며, Unity lifecycle, CustomEditor/MenuItem, UnityEvent/AnimationEvent, 문자열 타입 생성은 별도로 고려했다.

## A. Legacy-Unused

현재 저장소에서 실행 연결 근거를 확인하지 못한 구형/중단된 구현이다. 불가능한 외부 호출까지 부정하지 않으며 파일을 삭제하지 않았다. 아래 경로는 `Assets/_Project/Scripts` 기준이다.

| File/Class | Evidence | Action |
|---|---|---|
| `AssetBundleManager` | 주석 제외 타입 소비자 0, Scene/Prefab/asset GUID 소비자 0. 현재 콘텐츠 로딩은 `LoadManager`. 오래된 별도 Bundle 구현과 빈 public 메서드가 남음 | `Legacy/Unused/AssetBundle/AssetBundleManager.cs` |
| `StageTest.cs` | 클래스/구현 전체가 주석 처리됨, GUID 소비자 0. 실행 중인 `StageTest.unity`와 다른 대상 | `Legacy/Unused/Misc/StageTest.cs` |
| `CharacterInformationXMLreader.cs` | XML reader 클래스/구현이 주석 처리됨, GUID 소비자 0. 현재 JSON reader는 `UserDataManager`/`LoadManager` | Data 내부에서 `Legacy/Unused/Character/CharacterInformationXMLreader.cs`로 이동 |
| `Player1` | 타입 소비자 0, MonoBehaviour GUID 소비자 0, 현재 생성/상속 소비자 확인 안 됨. 전용 행동 객체를 만들던 구형 `GameCharacter` 파생 구현 | `Legacy/Unused/Character/Player1.cs`. 전용 행동 두 파일은 F의 이유로 현 위치 유지 |
| `AudioManager` | 타입/GUID 소비자 0, 빈 Start/Update의 오디오 매니저 껍데기. 현재 연결된 AudioClipUser/BackGroundAudioPlayer 등을 유지 | `Legacy/Unused/Audio/AudioManager.cs` |
| `SoundManager` | 타입/GUID 소비자 0. AudioClip 목록을 dictionary로 만들지만 외부 사용 연결 확인 안 됨 | `Legacy/Unused/Audio/SoundManager.cs` |
| `Distance_priority` | 타입 소비자 0, 현재 priority 타입의 문자열 생성 확인 안 됨. 현재 `Character_Priority.get_Characters`가 `Priority.Distance`를 직접 처리 | `Legacy/Unused/Character/Distance_priority.cs` |
| `SpawnInformation` | struct 타입 소비자 0. 자체 소스에서 해당 구조체 사용 연결 확인 안 됨 | `Legacy/Unused/Misc/SpawnInformation.cs` |
| `CListSortMember.cs` | 빈 파일, 선언/실행 내용 및 GUID 소비자 없음 | `Legacy/Unused/Misc/CListSortMember.cs` |
| `DebugButton` | 타입/GUID 소비자 0. Awake에서 Button listener를 등록하지만 컴포넌트 연결이 확인되지 않는 개발 지급 helper | `Legacy/Unused/UI/DebugButton.cs` |
| `Character_Children_obj_effect` | 타입/GUID 소비자 0. observer/lifecycle 구현은 있으나 연결된 컴포넌트가 확인되지 않음 | `Legacy/Unused/Character/Character_Children_obj_effect.cs` |
| `character_UI`, `icon_uii_creater`, `StageeSelectUI` | 각각 타입/GUID 소비자 0. 빈 Start/Update의 UI 껍데기 | 각 파일을 `Legacy/Unused/UI`로 이동 |
| `StageRoundPanel` | 타입/GUID 소비자 0. observer 구현과 animation 호출이 있지만 연결된 인스턴스가 확인되지 않음 | `Legacy/Unused/UI/StageRoundPanel.cs` |

각 행의 실제 코드/GUID 소비자 목록은 inventory에서 확인할 수 있다. 위 15개 C# 파일은 모두 내용과 `.meta`가 동일한 상태로 이동했다. namespace와 runtime assembly 소속은 유지된다.

### 파일 내부의 미사용 구현

| File/Method | Evidence | Action |
|---|---|---|
| `LoadManager.AssetBundle.cs`: `LoadCharacterSfxsasync`, `LoadAssetBundleAsync`, `LoaditemDictionaryAsync` | 주석을 제외한 자체 코드에서 각 이름은 선언 1회만 확인. 직렬화 event 이름 소비자 없음 | 삭제/추가 추출 없이 기존 partial 안에 유지 |
| `LoadManager.cs`: `loadingGamesceneAsync_T` | 선언 외 소비자와 event 이름 연결 확인 안 됨. 현재 사용 중인 별도 씬 로딩 메서드도 함께 존재 | 현 파일에 유지. 클래스 전체를 Unused로 분류하지 않음 |
| `UserDataManager` 주석 XML 영역 | 실행 코드가 아닌 주석 처리된 과거 XML 구현 | 주석까지 그대로 유지. 혼합 책임 파일을 다시 분리/작성하지 않음 |
| `UserDataManager`의 구형 Encrypt/cache 하위 흐름 | 자체 `MakeConnectionCache(string,string)`/`loadConnectionCache()` 진입 호출 확인 안 됨. Encrypt/Decrypt 및 helper는 이 하위 흐름 내부에서 연결됨. 실제 `GameManager`/UIDInputUI의 호출은 별도 `FIleIOManager`로 연결 | 파일에 유지. 암호화 helper 각각을 참조 0이라고 단정하지 않음 |
| `FIleIOManager.LoadGameplayCache` | 실제 호출은 주석 처리된 `GameManager` 코드에만 남고, 현재는 직접 GamePlayCache 객체 초기화. 자동 토큰 조사에서 선언만 확인 | 메서드 후보 기록만. 현재 연결 캐시/설정 기능을 가진 파일은 이동하지 않음 |

`method-candidates.csv`는 889개 메서드 **선언 후보**의 동일 이름 토큰/직렬화 이벤트 조사표다. C# semantic binding 결과가 아니므로 overload, interface/override, Unity 메시지, reflection, delegate 및 외부 payload를 고려하지 않고 Unused로 자동 판정하지 않는다. 예를 들어 `OnGUI`와 `OnApplicationQuit`는 명시적 호출이 없어도 Unity가 호출하므로 미사용 판정에서 제외했다. public 로딩 wrapper·전투 helper·패킷 handler 등의 singleton 이름은 이 조사표에 남기고 임의 삭제/이동하지 않았다.

### 서버 과거 프로젝트

`Server/01Server/01Server/01Server.csproj`는 .NET Framework 4.7.2의 별도 `Program/Listner/Session` 구현이다. 현재 `Server/01Server/01Server.sln`에는 `01dummyclient`, `ServerCore`, `Server`만 있으며 이 과거 프로젝트가 포함되지 않는다. 현재 프로젝트에서 해당 csproj에 대한 ProjectReference도 확인하지 못했다. **현재 solution 기준 Legacy-Unused**로 보고만 하고 파일/폴더를 이동하거나 수정하지 않았다. 이름에 `01Server`가 들어가는 상위 solution 전체는 미사용으로 판정하지 않는다.

## B. Legacy-Referenced

| File/Class | Current Consumer | Why Legacy | Action |
|---|---|---|---|
| `DownLoadManager` | `PacketHandler`, Title 씬, Recovery/0 씬 GUID 연결. 시작 다운로드 진입은 비활성 상태 | 기존 Bundle 버전/다운로드 체계. 참조가 남아 있어 제거 불가 | `Legacy/Referenced/AssetBundle/DownLoadManager.cs` |
| `Developer_VersionManager` | `Developer-VersopnManagement.unity` GUID 연결 | 기존 개발 버전 관리 화면/흐름 | `Legacy/Referenced/Versioning` |
| `Developer_VersionCheck` | 위 manager의 타입 참조와 같은 개발 씬 GUID 연결 | 기존 버전 확인 구현 | `Legacy/Referenced/Versioning` |
| `AssetBundleMaker` | `[UnityEditor.MenuItem("Assets/Build All AssetBundle")]` | 기존 수동 Bundle 생성 도구. 호출 문자열 검색 0은 미사용 근거가 아님 | `Editor/Legacy/Referenced/AssetBundle` |
| `VersionEditor` | `[UnityEditor.MenuItem("Version/Make Version")]` | 기존 수동 버전/Bundle 생성 도구 | 같은 Editor 폴더. 메뉴를 실행하지 않음 |

개발 씬이 빌드 목록 밖에 있다는 이유로 연결된 개발 컴포넌트를 Unused로 처리하지 않았다. 자체 CustomEditor 다섯 개도 `[CustomEditor]`에 의해 소비되므로 현 Active 영역에 유지했다.

## C. Planned-Legacy

현재 사용 여부와 향후 교체 여부를 별개로 기록한다. **Planned-Legacy는 현재 Active-Core 역할을 수행할 수 있다.**

| File/Class | Current Role | Future Replacement Reason | Moved? |
|---|---|---|---|
| `LoadManager.AssetBundle.cs` | 현재 `LoadManager`의 Bundle 파일 접근·조회·unload와 기존 callback/Coroutine 구현. queue와 캐시를 같은 클래스에서 공유 | 사용자 지정 후속 Content Provider/Addressables 작업의 명확한 교체 대상인 기존 backend | Yes: `Scripts/AssetBundle` → `Scripts/PlannedLegacy/ContentLoading`. 기존 폴더 `.meta`도 함께 이동 |

이 파일은 새 서비스나 새 클래스가 아니다. Phase 0-A에서 만든 동일 `LoadManager` partial의 위치만 바뀌었다. 분리 후의 field/method 내용, 원래 `LoadManager.cs`의 초기화 할당과 public API는 이번에 일절 변경하지 않았다.

## D. Planned-Legacy Candidates

확실한 교체 책임이 있지만 파일 전체에는 유지할 책임이 섞여 있어 현 위치에 두었다. 새 인터페이스/registry/분리 클래스를 구현하지 않았다.

| File/Region | Current Role / Evidence | Future Replacement / 이동하지 않은 이유 |
|---|---|---|
| `LoadManager.cs`의 `PushCharacter*`, `Pushitem*`, `ReturnChrDataName` 호출과 CharacterID 목록 생성 흐름 | CharacterId를 Prefab/JSON/Animator/Portrait 조회 이름 및 Bundle 상수에 직접 연결. UI/전투의 기존 public 호출 계약 | Registry/Content Provider 후보. queue·씬 흐름·API까지 파일 전체를 PlannedLegacy로 보내거나 새 구조로 추출하지 않음 |
| `GameManager.cs`의 초기 콘텐츠 읽기, singlemode 시작/계정 생성·저장 흐름 | `UserDataManager`를 생성해 템플릿을 읽고, `singlemode = true`, `FIleIOManager` 캐시 저장 등을 수행 | Local/Mock provider 후보. 게임 상태·UI·설정 등 Active 책임과 섞임 |
| `UserDataManager.cs` | 현재 JSON 템플릿/아이템/스테이지/캐릭터 및 Editor 작성 도구의 직접 파일 I/O. 오래된 XML/crypto/cache도 함께 남음 | 직접 경로와 데이터 조회 결합의 provider 후보. 현재 Editor/런타임 소비자를 유지해야 함 |
| `FIleIOManager.cs` | 현재 `GameManager`·UIDInputUI의 설정 및 연결 캐시 읽기/쓰기. 게임플레이 캐시의 중단된 흐름도 함께 존재 | 저장/local 책임 정리 후보. 현재 설정과 연결 캐시 전체를 미사용으로 이동하지 않음 |
| `UI/Title/TitleScreen.cs` | singlemode 전환과 기존 계정/진입 UI | Local/Mock 진입 후보. UI lifecycle과 현재 동작이 섞임 |
| `NetWorkManager/NetworkManager.cs`, `PacketHandler.cs` | `GameManager`의 생성 참조, 기존 패킷 송수신·callback 및 전역 `ServerMessage` 응답 대기. 명시적 requestId 없이 문자열/상태를 대기하는 흐름 | 서버 재구성 때 교체 후보. 현재 통신 계약 변경/기능 제거 없이 보고만 함 |
| `Object/Character/basic/Character_Animation.cs`, `UI/Menu/Character/CharacterUpgradeModeling.cs` | animation 이름/모델 및 콘텐츠 API 결합, 현재 전투/UI의 소비자 존재 | 데이터 결합 경계 후보지만 전투 표현/UI 책임이 섞여 파일 이동 근거가 부족함 |
| 서버 `Server/script/Packet/PacketHandler.cs`, 패킷 `01Packet.cs`, 저장 helper | 기존 version/download packet, client의 전체 Account payload 저장 및 서버 LocalTask/DB 경로 | 서버 재구성 후보. 서버 코드 이동·수정 금지에 따라 분류만 함 |

`PlannedLegacy/DataCoupling`, `PlannedLegacy/LocalMock` 빈 폴더는 만들지 않았다. 이번에는 해당 책임을 독립 파일로 안전하게 분류할 수 없으며 후보라는 표시를 위해 Active 파일 전체를 강제로 이동하지 않았다.

## E. Folder Changes

모든 개별 변경 전/후 경로와 원본 SHA-256·GUID는 [moved-files.csv](moved-files.csv)에 있다. 이번 변경만 **48개 파일(21개 C#, 기존 폴더 메타와 Scene 포함)**이다. Phase 0-A의 대규모 이동과 구분해 기록했다.

```text
Before (_Project 기준)
  Scripts/AssetBundle/LoadManager.AssetBundle.cs
  Scripts/Legacy/AssetBundle/{4 runtime files}
  Scripts/Editor/Legacy/AssetBundle/{2 editor files}
  Scripts/Manager/{StageTest,AudioManager}.cs
  Scripts/Object/Character/Player1/Player1.cs
  Scripts/{기존 여러 폴더의 무참조 후보}
  Data/Json/Character/xml/CharacterInformationXMLreader.cs
  Scenes/Recovery/0.unity

After
  Scripts/
    PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs
    Legacy/
      Unused/
        AssetBundle/AssetBundleManager.cs
        Character/{Player1,Distance_priority,
                   Character_Children_obj_effect,
                   CharacterInformationXMLreader}.cs
        Audio/{AudioManager,SoundManager}.cs
        UI/{DebugButton,character_UI,icon_uii_creater,
            StageRoundPanel,StageeSelectUI}.cs
        Misc/{StageTest,SpawnInformation,CListSortMember}.cs
      Referenced/
        AssetBundle/DownLoadManager.cs
        Versioning/{Developer_VersionManager,Developer_VersionCheck}.cs
    Editor/Legacy/Referenced/AssetBundle/{AssetBundleMaker,VersionEditor}.cs
    Object/Character/Player1/{Player1_Skill1,Player1_LB}.cs  [유지]
    Manager/{GameManager,LoadManager,UserDataManager,FIleIOManager}.cs [유지]
    ... 기존 Active/불확실한 영역 유지
  Scenes/Legacy/Recovery/0.unity
  Prefabs/Player1/(Past)P01.prefab                           [유지]
```

Recovery/0 씬은 빌드 목록 밖이며 GUID 소비자와 `LoadScene("0")`/Recovery 경로 사용을 확인하지 못했다. 보관용 **Legacy-Unused Scene artifact**로 위치만 분류했다. Scene bytes와 GUID를 유지했다. 빌드 목록/프로젝트 설정은 이번에 수정하지 않았다.

## F. Untouched Ambiguous Code

- **`Player1_Skill1`, `Player1_LB`:** 직접 코드 소비자는 Skill1의 경우 구형 `Player1` 내부 생성뿐이며 LB는 찾지 못했다. 현재 JSON action 키에는 두 이름이 없지만 `Character_action.Invoke`와 `GetActionType.checkType`이 `Activator.CreateInstance(Type.GetType(...))`를 사용한다. prebuilt Bundle/외부 payload/Editor 입력의 모든 문자열까지 확인할 수 없어 두 행동 파일과 기존 Player1 폴더 메타는 원위치에 유지했다. Player1 클래스 이름/namespace/assembly도 그대로라 기존 Skill1 생성 연결은 유지된다.
- **`RangeAttack_normal`:** 직접 소비자는 확인되지 않지만 같은 문자열 action 생성 대상이 될 수 있다. 삭제/이동하지 않았다. `LimitBurst`, `attack_basic`, 인터페이스·base class들도 상속/생성 및 데이터 키 연결을 고려해 그대로 두었다.
- **`(Past)P01.prefab`:** GUID `929383f340b3c9042a19b5f161524fd9`의 소비자와 파일명 경로 소비자는 찾지 못했으나 importer에 `assetBundleName: character`가 남는다. GUID 참조가 없어도 동적 Bundle 조회는 가능하므로 과거 이름만 보고 Unused로 확정하지 않고 위치·태그를 유지했다.
- **`StageTest.unity`:** 현재 enabled build scene이며 `StageTest.cs`의 주석 구현과 구분했다. 씬과 연결된 현재 컴포넌트는 이동하지 않았다.
- **개발/Editor 코드:** `Test_Stage`, `CameramoveTest`, `ShowCurrentAnimation`, `CharacterActionCreater` 등은 개발 씬 GUID 연결이 있다. `cameraMove`, `ParticleAutoDisable`, 다수 UI 역시 직접 C# 소비자가 없어도 Scene/Prefab에서 연결되어 현재 위치에 유지했다.
- **미사용처럼 보이는 메서드:** UnityEvent/AnimationEvent, Unity lifecycle, inheritance/delegate/문자열 호출 및 overload 때문에 이름 횟수만으로 분류하지 않았다. 조사표의 추가 singleton helper는 재설계 없이 기록만 했다.

## G. Verification

정적 검증 결과는 [verification.json](verification.json)에 있다. 기준 상태는 Phase 0-A 이후 작업 트리이며 HEAD와 비교했을 때 이미 존재하던 변경을 포함하지 않는다.

- Assets 원본 8,759개 모두 이동 후 존재. 누락 0, 예상 밖 Assets 파일 0.
- 최종 이동 48개 파일의 SHA-256 동일. 기존 C# 내용 변경 0이므로 namespace, 클래스/메서드 signature, public API, 주석 및 기존 코드 연결을 보존했다.
- 기존 `.meta` 4,731개 모두 내용/GUID 보존. 새 GUID는 **새 분류 폴더 11개**의 metadata에만 생성했으며 기존 asset/file/folder GUID를 교체하지 않았다. 전체 GUID 중복 0.
- Scene/Prefab 950개 내용 동일. 이번에 이동한 Recovery 씬도 직렬화 bytes를 보존했다. static GUID/event 소비자와 현재 빌드 씬 경로를 확인했으며 importer 실행을 주장하지 않는다.
- 기존 Editor/Resources/Plugins/StreamingAssets/Gizmos의 special-folder 소속 보존. XML reader는 일반 runtime 위치에서 일반 runtime 위치로 이동. asmdef/asmref 추가 없음.
- 자체 소스에 이번 이동 전 파일 경로를 직접 사용하는 연결이 남지 않음. namespace·파일명·GUID와 기존 이름 기반 로딩 계약은 그대로 유지. 기존 prebuilt Bundle 및 manifest의 과거 source path 기록은 이번 작업에서 수정하지 않음.
- ThirdParty, StreamingAssets, 서버, DB, Packages, ProjectSettings, README 및 이전 Phase 0-A 문서는 원본 해시로 보존 확인. 새 보고서/조사표와 실행 계획 진행 표시만 추가/갱신했다.
- Git diff의 tracked 경로/상태와 기존 README/빌드 목록의 변경 상태를 기준 상태와 비교. 새로운 tracked 수정 없음. 이동 대상은 이전 Phase에서 새 경로에 있던 untracked 파일이므로 `git diff`만으로 내용을 검증할 수 없어 모든 기존 파일의 별도 해시를 비교했다. stage/commit하지 않았다.
- 별도 읽기 전용 리뷰에서 무참조/GUID·reflection 근거와 이동 파일 bytes/Editor 경계를 확인했다. 동적 행동과 bundled Past prefab의 과도한 Unused 판정을 피하도록 분류를 보수적으로 조정했다.
- **Compile / Unity Player Build / Server Build / Package restore / 패키지 설치·수정 / DLL 복구 / 환경 설정 변경: 모두 수행하지 않음.**

의도된 기능 변경은 없다. 정적 참조 보존을 검증했으며 Unity 실제 실행 검증은 사람이 별도로 수행한다.
