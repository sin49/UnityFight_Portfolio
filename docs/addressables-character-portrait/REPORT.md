# Character Portrait Addressables 전환

이번 변경은 Character Portrait만 대상으로 한다. Compile / Build / Package Restore / Unity 실행은 수행하지 않았다. UI, Character Prefab, Character JSON, Animator, Stage/Terrain, Item, SFX, 전투 코드는 변경하지 않았다.

## A. 수정 파일

| 파일 | 변경 이유 |
| --- | --- |
| `Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs` | Portrait 단일 두 overload, batch, 메뉴 preload의 실행 경로를 Addressables로 연결. 동기 UI Sprite dictionary를 초기화. 총 5곳만 변경. |
| `Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.PortraitAddressables.cs` | Sprite 전용 주소별 session handle cache, 공통 단일 로더, batch 및 전체 준비, 종료 시 Release 추가. |
| 위 새 partial의 `.meta` | 새 스크립트에만 새 GUID 부여. 기존 `.meta`는 수정하지 않음. |
| `docs/addressables-character-portrait/REPORT.md` | 변경 결과 및 Editor 검증 인수인계. |
| `docs/addressables-character-portrait/verification.json` | 정적 검사 결과와 검증 한계. |
| `docs/addressables-character-portrait/task.diff` | 이번 작업의 코드 및 새 meta 변경만 담은 diff. 이전 작업 변경은 제외. |

## B. 기존 Portrait 로딩 흐름

실제 파일 근거:

- `LoadManager.cs:151` / `:157`의 `PushCharacterSpriteloadrequest` 두 overload: `CharacterContentMapping.Get(s).PortraitName` → `LoadSpriteAsync("charactersprite", ...)` 또는 전달된 bundle 이름 → LoadingQueue.
- `LoadManager.cs:163`의 `PushCharactersSpriteloadrequest`: `Resolve(s, c => c.PortraitName)` → `LoadSpritesAsync("charactersprite", ...)` → LoadingQueue.
- `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:233` / `:286`: bundle 파일 열기 → asset 로딩 → `Texture2D` → 전체 rect / 중앙 pivot의 `Sprite.Create` → callback → `Unload(false)`.
- `LoadManager.cs:316`의 `loadMenuScene`: `LoadAllCharacterSpritesAsync` → `UserAccountData.loadPlayerSprites` → 메뉴 진입. 기존 전체 준비는 `LoadManager.AssetBundle.cs:331`에서 `charactersprite`의 모든 Texture2D를 Sprite로 만들어 dictionary에 보관했다.

`Assets` 전체 C# 검색에서 이 세 요청 경로와 메뉴 preload 외에 `charactersprite`를 여는 Active 경로는 확인되지 않았다. Portrait 요청 API의 외부 호출은 현재 C# 검색에서 확인되지 않았고, 실제 UI는 전체 준비 후 동기 cache를 주로 사용한다.

실제 소비자는 `UserAccountData.cs:198`의 `loadPlayerSprites` → `LoadManager.cs:20`의 `getcharactersprite` → `CharacterData.charactersprite`이다. `spawncharactericonUI.cs:23`은 동기 조회, `:30`은 `Image.sprite` 대입을 수행한다. `UI/Menu/Stage/CharacterIconUI.cs:23`, `UI/Game/GameClearCharacterIcon.cs:41`도 저장된 Sprite를 대입한다. `GameCharacter.cs:112`의 `set_Character`에서도 해당 Sprite를 보관한다. 이 소비자 파일들은 수정하지 않았다.

## C. 변경 후 Addressables 흐름

```text
CharacterId
  → CharacterContentMapping.PortraitName
  → LoadCharacterPortraitAddressableAsync
  → 주소별 기존 handle 재사용 또는 Addressables.LoadAssetAsync<Sprite>
  → CharacterSpriteDictionary에 저장
  → 기존 Action<Sprite> callback / getcharactersprite
```

`LoadManager.PortraitAddressables.cs:16`의 단일 로더를 두 overload, batch(`:62`), 전체 준비(`:76`)가 공유한다. 기존 Prefab/JSON의 static typed handle cache, in-flight 선등록, 유효성 확인, 실패 handle 제거, `Application.quitting` 해제 패턴을 재사용했다. Sprite 타입의 handle이 필요하므로 별도 typed dictionary만 추가했고 Prefab/JSON 로더는 수정하지 않았다.

메뉴는 이제 `LoadManager.cs:332`에서 `LoadAllCharacterPortraitsAddressableAsync`를 기다린 후 기존 `Account.loadPlayerSprites()`를 실행한다. 전체 준비 대상은 `CharacterContentMapping.AllContents`의 `PortraitName`이며 ID에서 문자열 이름을 만들어내지 않는다. 기존 Character JSON 준비 및 메뉴 진입 성공 조건은 그대로다.

| CharacterId | PortraitName / Address | 실제 Sprite 파일 | 기존 GUID |
| --- | --- | --- | --- |
| p01 | p01 | `Assets/_Project/Art/Sprite/Character/p01.png` | `fcf6b7eab814db6489393aecfb7151f9` |
| p02 | P02 | `Assets/_Project/Art/Sprite/Character/P02.png` | `29ee4630a92fa8c4da3d08abbc2dd7ae` |
| p03 | P03 | `Assets/_Project/Art/Sprite/Character/P03.png` | `3f96fcc252661e94dbf39978b5967692` |
| p04 | P04 | `Assets/_Project/Art/Sprite/Character/P04.png` | `3584f7861ecb6a242a55b9938be5a70b` |
| e00 | E00 | `Assets/_Project/Art/Sprite/Character/E00.png` | `5b2e2619dc1bf5e4f95f01bdefb79446` |

Mapping 근거는 `Object/Character/basic/CharacterContentMapping.cs:30`이다. 각 PNG `.meta`의 `textureType: 8`, `spriteMode: 1`, 중앙 pivot, 100 PPU, border 0, 빈 packing tag를 확인했다. 따라서 로더가 import된 Single Sprite를 직접 반환하며 Texture/Sprite 재생성이 없다. 기존에는 매번 생성된 Sprite였고 이제는 같은 주소의 import Sprite가 공유된다. 실제 UI 표시와 Editor의 Sprite import 결과는 사용자가 실행 검증해야 한다.

**현재 저장소의 Addressables 등록은 `AssetGroups/Default Local Group.asset:19`의 `P01` Prefab 1개만 확인된다. 위 5개 Portrait Sprite를 표의 정확한 주소로 Editor에서 등록해야 한다. Settings/Group/에셋/meta의 기존 bundle label은 변경하지 않았다.** JSON/Prefab과 같은 문자열 키가 있을 수 있으므로, 각 키가 `LoadAssetAsync<Sprite>`에서 실제 해당 Sprite를 반환하는지 확인해야 한다.

## D. cache / handle 소유권과 Release 시점

- 소유자: `LoadManager` partial의 static `CharacterPortraitHandles` (`:12`). 주소는 `StringComparer.Ordinal`로 비교하고 Mapping의 실제 대소문자를 유지한다.
- 한 주소의 in-flight / 성공 handle은 재사용한다. Dictionary 등록은 첫 yield 전에 수행하므로 queue 중단 시에도 소유권이 남는다.
- 성공 결과는 callback 직후, 화면 종료, Scene 전환, LoadManager 파괴 시 Release하지 않는다. UI와 Account의 Sprite 참조를 유지하기 위해 기존 session 정책을 따른다.
- Release 시점: `Application.quitting` → `ReleaseCharacterPortraitSessionHandles` (`:87`)에서 유효 handle Release 후 cache를 비운다. imported Sprite에 별도의 `Destroy`를 호출하지 않는다.
- 실패: cache가 여전히 같은 handle을 소유할 때만 제거 후 Release한다. 실패를 영구 cache하지 않으므로 이후 요청은 재시도할 수 있다. 자동 AssetBundle fallback은 없다.
- 동일 주소의 성공 요청이 여러 번 있으면 로딩과 Sprite asset을 공유하되 각 요청의 성공 callback 및 batch 중복 entry는 유지한다.

세션 동안 다섯 Portrait와 그 의존성이 상주하는 것은 선택한 기존 정책의 결과다. 세밀한 UI별 해제나 Editor Domain Reload 비활성화 대응은 이번에 추가하지 않았다.

## E. 기존 public API 유지 여부

다음 이름 / 인자 / 반환형을 유지한다.

- `PushCharacterSpriteloadrequest(string, Action<Sprite>)`
- `PushCharacterSpriteloadrequest(string bundlename, string, Action<Sprite>)`
- `PushCharactersSpriteloadrequest(List<string>, Action<List<Sprite>>)`
- `getcharactersprite(string)` → `Sprite`

`bundlename` overload의 인자는 소스 호환을 위해 남지만 이제 Portrait 주소 선택에는 사용하지 않는다. 다른 bundle에서 Portrait를 강제로 읽는 기존 의미는 Addressables 전환으로 종료된다. callback은 Sprite asset을 받으며 UI에는 별도 생성 작업이 없다.

실패 계약:

- 단일 실패: 기존 누락 Texture와 마찬가지로 성공 callback을 호출하지 않고 오류를 기록한다.
- batch: 기존 `LoadSpritesAsync`처럼 실패 항목을 제외한다. 성공 항목은 입력 순서와 중복을 보존한다. Prefab batch와 달리 null slot을 채우지 않는다.
- 전체 주소 실패 시 batch callback에는 빈 리스트를 반환한다. 구형 bundle 자체를 열지 못했을 때는 batch callback이 생략되었으므로 이 경우는 차이가 있다.
- 동기 조회: 미준비/누락 Portrait는 null이다. dictionary를 처음부터 빈 상태로 초기화해 preload 이전에도 미준비 조회의 null 결과를 명확히 한다. 기존 preload 전의 null dictionary 예외 가능성은 제거됐다.
- 메뉴 preload 실패는 기존 Portrait 준비 경로처럼 별도 진입 차단을 추가하지 않는다. 누락 Sprite는 null 상태로 유지되며 JSON 준비 성공 조건은 변경하지 않는다.

단일 요청의 로딩 budget 2, batch 1 + 입력 개수는 유지했다. 전체 준비는 Mapping 개수 + setup 1로 progress budget을 잡는다.

## F. Legacy AssetBundle 보존 상태

`PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs`는 이번 작업에서 수정하지 않았다. `LoadSpriteAsync`, `LoadSpritesAsync`, `LoadAllCharacterSpritesAsync`의 구현 및 `Unload(false)`를 보존했다. 마지막 전체 Character helper는 이제 Active 호출자가 없다.

공용 `LoadSpriteAsync` / `LoadSpritesAsync`는 Item 등 아직 전환하지 않은 경로에서 사용될 수 있어 그대로 유지한다. Portrait 실패를 이 helper들로 돌리는 코드는 추가하지 않았다.

## G. 남은 charactersprite 직접 참조

| 위치 | 상태 |
| --- | --- |
| `LoadManager.AssetBundle.cs:335`의 `charactersprite` bundle 파일 접근 | PlannedLegacy의 `LoadAllCharacterSpritesAsync`에만 남음. Active 호출 없음. |
| Character PNG 5개의 `.meta:122` `assetBundleName: charactersprite` | 기존 bundle label 보존. 수정하지 않음. |
| `UserAccountData` / `CharacterData` / UI / `GameCharacter`의 `charactersprite` 필드 | Sprite 저장/소비용 필드이며 bundle 접근은 아님. 그대로 유지. |

Active 코드에서 `"charactersprite"` 문자열 또는 구형 전체 helper 호출은 남지 않는다. 과거 생성된 bundle 바이너리는 검색/로더 전환 대상 외이며 삭제하지 않았다.

## H. 다음 Addressables 전환 추천 및 검증

다음 후보는 Character Animator다. 이미 `CharacterContentMapping.AnimatorName`이 있으며 P04의 값은 null이므로, 부재 정책과 전투 초기화 후처리를 먼저 조사해야 한다. 이번 작업에서는 Animator 및 전투를 변경하지 않았다.

먼저 Editor에서 위 Portrait 5개 등록 후 다음을 검증한다.

1. 메뉴 최초 진입 후 소유 캐릭터 icon / spawn icon / Stage 선택 / 게임 종료 UI가 같은 이미지로 표시되는지.
2. 같은 Portrait 단일 요청, batch 중복 요청, 메뉴 재진입에서 Sprite가 유지되고 중복 로드가 없는지.
3. 실패 주소에서 오류 로그, 단일 callback 생략, batch 성공 항목 순서 및 누락 정책이 예상대로인지.
4. Scene/LoadManager 재생성 후 메뉴 preload가 session cache에서 UI dictionary를 다시 채우는지.
5. 앱 종료 시 handle 해제가 이루어지는지. 다른 Prefab/JSON/Item/Animator 경로가 기존대로인지.

정적 검사는 public API signature, 5곳 변경 범위, Mapping 경유, 공통 로더/실패/해제 경로, 남은 Active bundle 참조, 새로운 GUID의 기존 GUID 충돌 여부, per-task Git diff whitespace를 확인한다. 별도 read-only 리뷰에서도 수정이 필요한 문제는 발견되지 않았다. 실행 성공을 검증한 것은 아니다.

검증 한계: 작업 전 전체 파일 hash와 원본 사본을 저장했던 도구 임시 경로가 다음 호출에서 유지되지 않아, 전체 저장소의 전후 hash 일치를 증명할 수 없다. `task.diff`의 기존 LoadManager 비교본은 실제 적용한 5개 ASCII 치환을 역변환해 복원했다. 변경 명령의 대상과 정적 diff를 확인했으며, 기존 전체 파일의 전후 hash 일치 검증으로 보고하지 않는다.
