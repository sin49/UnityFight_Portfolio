Character Hit SFX Addressables 전환 — 코드 및 정적 검증 완료. Editor 등록과 Unity 실행 검증은 별도입니다.

# A. 수정 파일

| 파일 | 변경 내용 |
| --- | --- |
| `Assets/_Project/Scripts/Manager/LoadManager.cs` | PushCharacterSFX의 enqueue 표현식 한 곳만 새 SFX helper로 연결. |
| `Assets/_Project/Scripts/Manager/LoadManager.CharacterSfxAddressables.cs` | 명시적 Hit 주소, AudioClip typed handle/session cache, 실패 처리, 종료 Release. |
| 새 partial의 `.meta` | 새 script GUID만 생성. 기존 meta 변경 없음. |
| `docs/addressables-hit-sfx/REPORT.md`, `verification.json`, `task.diff` | 조사·수정 결과, 정적 검사, 실제 작업 전후 diff. |

Character Prefab/JSON/Portrait/Animator 구현, Stage JSON/Terrain/Item/SFX 소비자, 전투 로직과 오디오 초기화, Settings/Group, Package는 변경하지 않았습니다.

# B. 기존 SFX 흐름

`LoadManager.loadingGamesceneAsync` (`LoadManager.cs:537`)와 `_T` (`:460`)는 `PushCharacterSFX("Hit", callback)`을 enqueue하고, Prefab의 Character_Audio_Player에 callback 결과를 HitClip으로 보관합니다.

기존 `PushCharacterSFX` (`:131–135`) → `LoadCharacterSfxasync("charactersfx", ...)` → AssetBundle.LoadFromFileAsync → LoadAssetAsync → AudioClip callback → Unload(false).

`PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:28–52`가 기존 구현입니다. bundle 파일이 없으면 callback(null)이었습니다. batch helper도 같은 파일 `:54`에 있으나 Active 호출자는 확인되지 않았습니다.

# C. 변경 후 Addressables 흐름

`PushCharacterSFX("Hit", callback)` → LoadingQueue → `LoadCharacterSfxAddressableAsync` → 명시적 `character/sfx/Hit` → `Addressables.LoadAssetAsync<AudioClip>` 또는 기존 handle 재사용 → AudioClip callback.

주소는 임의 sfxname 문자열에 prefix를 붙여 만들지 않습니다. 현재 존재하는 Hit만 constant에 명시적으로 대응합니다. Hit/hit 등 알려진 이름의 대소문자는 같은 주소를 사용합니다. 미등록 이름/null은 오류 기록 후 callback(null), 실제 로딩 없음입니다.

성공 callback은 AudioClip asset을 반환합니다. 재생, AudioSource 설정, Instantiate, 게임 오디오 필드 초기화는 helper가 수행하지 않습니다. 자동 AssetBundle fallback과 WaitForCompletion도 없습니다.

# D. AudioClip asset / Address

| logical SFX name | 실제 asset | Address | 기존 GUID |
| --- | --- | --- | --- |
| Hit | `Assets/_Project/Audio/Hit.wav` | `character/sfx/Hit` | `9a3a7582b349ff74dbbd7db211958ea5` |

현재 charactersfx의 직접 AudioClip은 Hit.wav 한 개입니다. AudioImporter 설정과 기존 bundle label `charactersfx`를 유지했습니다. 파일 이동/rename/meta GUID 변경은 없습니다.

# E. handle/cache 정책

소유자는 LoadManager partial의 static `CharacterSfxHandles`이며 key는 전체 Address입니다.

- 첫 yield 전에 handle을 등록합니다.
- 동일 Hit 요청은 유효한 in-flight 또는 완료 handle을 공유합니다.
- 캐시 공유와 별개로 각 요청의 callback은 유지됩니다.
- 대기 중 handle 유효성을 확인합니다.
- 실패 시 cache가 같은 handle을 소유한 경우에만 제거하고 유효하면 Release합니다. 이후 요청은 재시도할 수 있습니다.
- Addressables 실패는 오류 기록과 callback(null)로 완료합니다. 구형 손상 bundle에서 발생할 수 있었던 예외와 모든 오류 동작이 동일하다는 의미는 아닙니다.
- 성공 callback 직후, Scene 이동, LoadManager 파괴 시 Release하지 않습니다.
- `Application.quitting` → `ReleaseCharacterSfxSessionHandles`에서 유효 handle Release 후 cache를 비웁니다.

이는 기존 Character Prefab/Animator session cache 패턴과 같습니다. 요청 budget 2는 유지하고 새 helper에서 그 진행도를 반영합니다. 기존 SFX helper는 진행도 집계가 없었으므로 이 부분은 새 패턴에 맞춘 변경입니다.

# F. 기존 public API 유지

`public void PushCharacterSFX(string sfxname, Action<AudioClip> action)`의 이름, 인자, 반환형을 유지했습니다. LoadingQueue와 callback 의미도 그대로입니다. 두 전투 요청 및 HitClip 할당 callback은 원본과 byte 동일합니다.

# G. Legacy charactersfx 보존 및 남은 Active 참조

`LoadManager.AssetBundle.cs` 전체는 작업 전 hash와 동일합니다. single/batch SFX helper와 기존 Unload(false)를 삭제하거나 수정하지 않았습니다.

Active C#의 charactersfx bundle 선택 및 기존 `LoadCharacterSfxasync`/`LoadCharacterSfxsasync` 호출은 0개입니다. 보존된 Legacy helper의 정의, 기존 Hit.wav bundle label, 과거 bundle/manifest는 남아 있습니다. Item 등의 다른 AssetBundle 경로는 이번 대상이 아닙니다.

정적 검증에서 전환 전 9,136개 제품 파일과 비교해 기존 파일은 LoadManager enqueue 한 표현식만 변경됐고, 새 partial/meta 두 파일만 추가됐습니다. 기존 meta 4,782개와 GUID 모두 byte 동일입니다. 다른 로더, Mapping, 전투, 오디오 소비자, Settings/Group, Package, Scene/Prefab/JSON/AudioClip, 서버 파일도 모두 byte 동일입니다.

public API, 지원 이름/주소, handle 선등록·재사용·실패·Release, 실제 Hit GUID, Active bundle 참조, 선언 중복/괄호/import 및 per-task diff whitespace를 확인했습니다. read-only 리뷰에서도 수정이 필요한 문제는 발견되지 않았습니다. 정적 검사는 컴파일 성공이나 실제 오디오 재생 성공을 보증하지 않습니다.

Compile / Build / Package Restore / Unity 실행은 수행하지 않았습니다.

# H. Editor 등록 및 실행 검증 항목

사용자가 기존 `Assets/_Project/Audio/Hit.wav`를 Addressable로 등록하고 Address를 `character/sfx/Hit`로 지정해야 합니다. 현재 확인한 Group에는 해당 주소가 없습니다. Settings/Group은 자동 수정하지 않았습니다.

사용자가 확인할 항목:

1. Hit 요청 callback이 정상 AudioClip을 반환하는지.
2. 여러 캐릭터/재전투의 반복 요청이 동일 address handle을 재사용하는지.
3. Scene 이동 뒤에도 필요한 AudioClip이 유지되는지.
4. 미등록 주소/실패에서 오류와 null callback이 발생하고 AssetBundle fallback이 없는지.
5. 앱 종료 시 성공 handle이 해제되는지.

기존 오디오 연결 문제는 별개입니다. `GameCharacter.cs:714`의 AudioPlayer 필드 초기화는 주석 처리되어 있고, `Act` (`:379`)는 null 검사 후에만 PlayHitClip을 호출합니다. 이번에는 해당 필드나 초기화를 수정하지 않았습니다. 따라서 로딩 성공 후 소리가 들리지 않더라도 Addressables 로딩 실패와 기존 재생 연결 문제를 구분해야 합니다. 이 전환만으로 실제 재생 정상화를 주장하지 않습니다.
