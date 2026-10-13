# Character ID / Content Mapping 리팩토링 결과

2026-10-08. 기준은 이번 작업 시작 시점의 파일 바이트 snapshot이다. 기존 미커밋 Phase 0 변경을 이번 변경으로 계산하지 않았다. 사용자는 **새 저장·전송도 canonical ID 사용, 기존 데이터는 읽기 변환으로 지원** 정책을 선택했다.

## A. Canonical ID Policy

CharacterId는 캐릭터 종류의 논리 식별자이며 기존 `string` 타입을 유지한다. 이번 등록 ID는 소문자 `p01`, `p02`, `p03`, `p04`, `e00`이다. 파일명·Object.name·DB 내부 식별자와 같아야 하는 규칙은 없다.

| 기존 입력 값 | canonical ID | 이유 |
|---|---|---|
| p01 / P01 | p01 | 기존 master/owned 값과 맞추고 대표 캐릭터의 P01 alias 지원 |
| p02 / P02 | p02 | JSON 파일 P02와 논리 ID p02 분리 |
| p03 / P03 | p03 | Prefab/Portrait P03과 논리 ID p03 분리 |
| P04 / p04 | p04 | 플레이어 ID의 소문자 규칙 통일 |
| E00 / e00 | e00 | 적 ID도 논리 ID의 소문자 규칙 통일 |

근거: [CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs:29)의 5개 등록과 `Canonicalize`. `OrdinalIgnoreCase`로 등록 ID의 대소문자 alias를 수용한다. 빈 값/알 수 없는 값은 임의 캐릭터로 바꾸거나 전역 lower-case하지 않는다. 등록되지 않은 ID의 콘텐츠 요청은 `Get`에서 `ArgumentException`으로 명시적으로 실패한다. 새 캐릭터 추가 시 논리 ID와 실제 콘텐츠 이름을 명시적으로 등록해야 한다.

`c01`/`c02`는 [GameManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/GameManager.cs:375)의 구형 `ConnectAccountCorutine`에 남아 있다. 실제 5개 캐릭터와의 대응 근거가 없어 p01/p02로 임의 치환하지 않았다. `initializelist`의 `"1"` placeholder와 빈 ID도 유지한다. 이것들은 신규 콘텐츠 mapping 대상이 아니다.

## B. Character Content Mapping

아래는 파일 확장자를 제외한 실제 콘텐츠 이름이다. 대소문자는 추측으로 통일하지 않았다.

| CharacterId | JSON / master | Prefab | Portrait | Animator |
|---|---|---|---|---|
| p01 | p01 | P01 | p01 | p01 |
| p02 | P02 | P02 | P02 | p02 |
| p03 | p03 | P03 | P03 | p03 |
| p04 | P04 | P04 | P04 | **null: 존재하지 않음** |
| e00 | E00 | E00 | E00 | E00 |

정의: [CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs:32). 실제 JSON 5개, Prefab 5개, Portrait 5개, Animator 4개의 존재와 각각의 meta bundle label(`characterdb`, `character`, `charactersprite`, `characteranimation`)을 정적으로 확인했다. 기존 경로·파일명·GUID를 보존했다. 기존 `(Past)P01.prefab`은 그대로 남으며 mapping은 현재 `P01`을 사용한다.

캐릭터 조각 item 키는 기존 UI 동작 보존을 위해 추가적인 `LegacyFragmentItemId` 값으로 명시했다: `Ip01`, `Ip02`, `Ip03`, `IP04`, `IE00`. 이 키의 item 리소스를 새로 생성하지 않았다. 기존 master ID로 만들어지던 값 그대로이며, 캐릭터 조각 시스템 자체를 재설계하지 않았다.

## C. Changed Files

기존 파일 **22개 수정**, Unity 스크립트/meta **2개 신규**, 보고서/검증/diff **3개 신규**. 삭제·이동·rename 없음.

| 파일 | 상태 | 수정 이유 |
|---|---|---|
| [Client/1_backup/Assets/_Project/Data/Json/AccountTemplet.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/AccountTemplet.json) | 수정 | 소유 캐릭터 P04 → p04. 다른 필드와 형식 보존. |
| [Client/1_backup/Assets/_Project/Data/Json/Character/E00.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/Character/E00.json) | 수정 | master 내부 id E00 → e00. 파일명 E00.json 보존. |
| [Client/1_backup/Assets/_Project/Data/Json/Character/P04.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/Character/P04.json) | 수정 | master 내부 id P04 → p04. 파일명 P04.json 보존. |
| [Client/1_backup/Assets/_Project/Data/Json/stage/Main0-0.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/stage/Main0-0.json) | 수정 | SpawnCharacterList의 E00 → e00; 스테이지 ID/보상/웨이브 보존. |
| [Client/1_backup/Assets/_Project/Data/Json/stage/Main0-1.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/stage/Main0-1.json) | 수정 | SpawnCharacterList의 E00 → e00; 스테이지 ID/보상/웨이브 보존. |
| [Client/1_backup/Assets/_Project/Data/Json/stage/Main0-3.json](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Data/Json/stage/Main0-3.json) | 수정 | SpawnCharacterList의 E00 → e00; 스테이지 ID/보상/웨이브 보존. |
| [Client/1_backup/Assets/_Project/Prefabs/Player4/P04.prefab](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Prefabs/Player4/P04.prefab) | 수정 | GameCharacter.ID와 _information.id 두 값만 p04로 변경. |
| [Client/1_backup/Assets/_Project/Prefabs/Test_enemy/E00.prefab](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Prefabs/Test_enemy/E00.prefab) | 수정 | GameCharacter.ID와 _information.id 두 값만 e00으로 변경. |
| [Client/1_backup/Assets/_Project/Scenes/DeveloperTest.unity](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scenes/DeveloperTest.unity) | 수정 | accountEditor의 소유 P04, StageCreater의 적 E00 두 값만 변경. |
| [Client/1_backup/Assets/_Project/Scripts/Editor/AccountTempletEditor.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Editor/AccountTempletEditor.cs) | 수정 | 계정 JSON.NET 입력과 캐릭터 master 입력을 정규화. |
| [Client/1_backup/Assets/_Project/Scripts/Editor/PlayerCharacterEditor.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Editor/PlayerCharacterEditor.cs) | 수정 | ID 입력/기존 ID 및 master/owned 생성 값을 정규화. master 경로는 UserDataManager의 mapping 사용. |
| [Client/1_backup/Assets/_Project/Scripts/Editor/StageCreaterEditor.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Editor/StageCreaterEditor.cs) | 수정 | 기존 stage JSON 입력 및 SpawnCharacterList ID 입력 정규화. |
| [Client/1_backup/Assets/_Project/Scripts/Manager/FIleIOManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/FIleIOManager.cs) | 수정 | 로컬 JSON 입출력 경계 정규화. 암호화/캐시 형식 유지. |
| [Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs) | 수정 | 캐릭터 요청 10개 mapping 적용, Portrait 조회 mapping 적용, bundle JSON 입력 정규화, 대표 기본값 p01. |
| [Client/1_backup/Assets/_Project/Scripts/Manager/UserDataManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/UserDataManager.cs) | 수정 | JSON 입출력 정규화; 캐릭터 JSON 읽기/쓰기 파일명은 mapping.JsonName 사용. |
| [Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/GameCharacter.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/GameCharacter.cs) | 수정 | master/owned 초기화 및 기존 bundle Prefab ID 정규화. E00 모델 위치 분기는 e00으로 갱신하여 기존 동작 보존. |
| [Client/1_backup/Assets/_Project/Scripts/Object/UserAccountData.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/UserAccountData.cs) | 수정 | 추가/설정/삭제/수신/송신/직접 저장 경계 정규화. dictionary 객체 보존, alias 충돌 시 임의 병합 금지. |
| [Client/1_backup/Assets/_Project/Scripts/Object/UserAccountDataNetworkMember.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/UserAccountDataNetworkMember.cs) | 수정 | 대표 캐릭터 wire 입력 정규화와 출력 DTO 정규화. 필드/직렬화 순서 보존. |
| [Client/1_backup/Assets/_Project/Scripts/Object/dataclass.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/dataclass.cs) | 수정 | CharacterData.setData 및 CharacterData/PlayerData wire ID 입력·출력 정규화. 기타 item/stage ID 보존. |
| [Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs) | 수정 | 동기 master 이름 조회 mapping 적용; Portrait cache 실제 이름 사용; 없는 Animator null 슬롯/진행량 처리. |
| [Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeUI.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeUI.cs) | 수정 | 조각 item 조회를 명시적인 LegacyFragmentItemId로 변경하여 기존 키/화면 동작 보존. |
| [Server/01Server/DBprototype/DataTemplet/AccountTemplet.json](D:/GIthub/UnityFight_Portfolio/Server/01Server/DBprototype/DataTemplet/AccountTemplet.json) | 수정 | 서버 신규 계정 template의 P04 값만 p04로 변경. 서버 코드/DTO/DB 구조는 그대로. |
| [Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs) | 신규 | 신규: 불변 콘텐츠 이름 5개 매핑, 기존 대소문자 alias, 데이터 경계 정규화. |
| [Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs.meta](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs.meta) | 신규 | 신규 스크립트에만 새로운 GUID 부여. 기존 meta 변경 없음. |
| [docs/character-content-mapping/REPORT.md](D:/GIthub/UnityFight_Portfolio/docs/character-content-mapping/REPORT.md) | 신규 | 요청한 A–H 보고서. |
| [docs/character-content-mapping/verification.json](D:/GIthub/UnityFight_Portfolio/docs/character-content-mapping/verification.json) | 신규 | 작업 시작 시점과 비교한 파일 목록/정적 검증 결과. |
| [docs/character-content-mapping/task.diff](D:/GIthub/UnityFight_Portfolio/docs/character-content-mapping/task.diff) | 신규 | 수정 전 snapshot 대비 git diff --no-index 결과. 이전 Phase 0 변경과 구분. |

CharacterInformation 클래스의 필드/직렬화 형태는 바꾸지 않았다. master JSON의 값과 실제 읽기/쓰기 경계를 정규화했다. PlayerData는 CharacterData의 wire 처리와 account 경계 정규화를 사용하며 진행 필드 변화가 없다.

## D. Reference Changes

외부 UI/Battle 요청 메서드의 이름·인자·콜백 타입은 그대로다. [LoadManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:136)의 10개 캐릭터 queue 경로가 다음과 같이 바뀌었다.

| 진입점 | 변경 전 | 변경 후 |
|---|---|---|
| PushCharacterloadrequest 두 overload / PushCharactersloadrequest | s 또는 List s → prefab backend | Get/Resolve → PrefabName → 기존 backend |
| PushCharacterSpriteloadrequest 두 overload / PushCharactersSpriteloadrequest | s 또는 List s → sprite backend | Get/Resolve → PortraitName → 기존 backend |
| PushCharacteranimatorloadrequest / PushCharacteranimatorsloadrequest | s 또는 List s → animator backend | Get/Resolve → AnimatorName → 기존 backend |
| PushCharacterdbLoadRequest / PushCharacterdbsLoadRequest | ID → text backend | Get/Resolve → JsonName → 기존 backend |
| getcharactersprite | 첫 글자 대문자 → cache key | mapping.PortraitName → cache key |
| ReturnChrDataName | bundle.LoadAsset(data.id) | mapping.JsonName → bundle.LoadAsset(assetName) |
| UserDataManager.readCharacterinformation / createChracterInformation | ID로 Character/ID.json 경로 생성 | mapping.JsonName으로 기존 파일 경로 생성 |
| CharacterUpgradeUI.updateprofil | "I" + master.id | mapping.LegacyFragmentItemId |

동기 이름 조회 근거: [LoadManager.AssetBundle.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:19). JSON 경로 근거: [UserDataManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/UserDataManager.cs:271). backend의 일반 `LoadPrefabAsync`, `LoadSpriteAsync`, `LoadTextAssetAsync` 등은 실제 asset name을 받는 기존 함수로 유지했다. 지형·아이템·SFX 요청은 캐릭터 종류 ID가 아닌 별도 리소스 키이므로 기존 경로를 유지했다.

Portrait 전체 로딩은 각 Texture의 실제 `data.name`을 cache key로 저장한다. cache의 `OrdinalIgnoreCase`는 기존 번들의 이름 casing 차이를 수용하고, 호출자가 ID로 cache를 직접 조회하는 관례는 제거했다.

## E. Compatibility Impact

| 영역 | 영향 / 대응 |
|---|---|
| Client Account JSON template | P04 → p04만 변경. 구조/레벨/잠금/성장 값 보존 |
| Server Account template | P04 → p04만 변경. 새 계정 생성 값에 적용; 서버 코드와 스키마 변경 없음 |
| Stage JSON | SpawnCharacterList의 E00 → e00. stage ID/terrain/reward/wave 변경 없음 |
| Mock / Local account | FakeConnectAccountCorutine의 template 입력과 GetNetworkMember 경계에서 canonical화. 기존 암호화/캐시 형식 보존. 구형 c01/c02 stub은 대응 근거 부재로 유지 |
| Server DTO 문자열 | 새 클라이언트 출력은 소문자 canonical ID. 서버 DTO 클래스/필드/바이너리 레이아웃은 그대로이며 ID를 문자열로 수신/보관 |
| 기존 계정 JSON / 기존 서버 반환 값 | 알려진 P01~P04/E00 alias를 읽을 때 변환. 다음 저장부터 canonical 값 사용; 기존 파일/DB 내용을 일괄 수정하지 않음 |
| 기존 빌드된 AssetBundle | 바이너리 재빌드 없이 현재 이름으로 로딩. JSON 내부의 옛 ID 및 GameCharacter 초기화의 옛 ID를 runtime 경계에서 정규화 |
| 구형 클라이언트 | 새 저장값을 읽는 구형 클라이언트의 호환성은 보장하지 않음. 사용자 선택대로 새 출력 canonical 정책 적용 |
| alias 중복 소유 데이터 | P04와 p04 같은 항목이 함께 있으면 InvalidOperationException. 진행값을 합치거나 하나를 버리지 않음 |

근거: [UserAccountData.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/UserAccountData.cs:343), [UserAccountData.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/UserAccountData.cs:403), [UserAccountDataNetworkMember.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/UserAccountDataNetworkMember.cs:154), [dataclass.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/dataclass.cs:58), [FIleIOManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/FIleIOManager.cs:16), [CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs:70). Account 직접 저장도 대표/소유 ID와 dictionary key를 정규화한다. dictionary 객체와 진행 데이터 객체는 유지한다. 충돌 확인은 변경 전에 수행한다.

바이너리 직렬화는 기존 UTF-16 문자열 길이 prefix/필드 순서를 유지한다. 대소문자 변환은 해당 문자열 값만 바꾼다. 서버 소스/DB 연결을 실행하거나 실제 저장 계정을 열어 검증한 것은 아니다.

## F. Behavior Changes

의도한 변경은 논리 ID 통일, 파일 이름과의 분리, 알려진 alias 읽기 지원, 미등록 콘텐츠 ID의 명시적인 실패다. custom bundle 인자가 있는 Character overload도 CharacterId를 받으며 mapping을 거친다. 예전 raw asset-name을 임의로 전달하던 외부 호출이 있다면 등록된 ID로 전환해야 한다. 저장소에서는 이 overload의 외부 호출을 확인하지 못했다.

P04 Animator는 single 요청에서 null 콜백, batch에서 동일 인덱스의 null 슬롯을 반환한다. 다른 Animator를 대체 연결하지 않고 해당 asset 로딩 요청을 생략한다. batch 길이/순서와 기존 로딩 진행량 단위를 유지한다. 근거: [LoadManager.AssetBundle.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:378), `LoadAnimatorsAsync`의 null 처리. 이는 원래 Animator가 없던 상태를 명시적으로 처리하는 변경이다.

E00 모델의 위치/회전 보정 조건만 e00으로 갱신하여 같은 캐릭터에 기존 동작이 적용된다. 캐릭터 조각 키도 기존 대소문자 그대로 유지한다. 전투 알고리즘·스킬·밸런스·UI 화면 흐름은 변경하지 않았다. 정적 리뷰에서 다른 동작 변경은 확인하지 못했으나 Editor 실행 결과를 검증한 주장은 아니다.

## G. Remaining Direct Couplings

현재 활성 경로에서 CharacterId를 Prefab/JSON/Portrait/Animator asset name으로 그대로 넘기는 호출은 정적 검색 및 변경 검토에서 발견되지 않았다. 다만 다음 항목은 남아 있다.

| 남은 관계 | 근거 / 이번 처리 |
|---|---|
| e00 캐릭터에 특정 모델 보정 적용 | [GameCharacter.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/GameCharacter.cs:153). 캐릭터별 동작 분기이며 asset-name 조회가 아니다. 기능 보존을 위해 유지 |
| 캐릭터와 조각 item 식별자의 구형 관계 | [CharacterContentMapping.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Object/Character/basic/CharacterContentMapping.cs:16). 명시적인 값으로 유지; item 시스템 분리는 이번 범위 밖 |
| 미등록 c01/c02 구형 계정 stub | [GameManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/GameManager.cs:375). 실제 콘텐츠 대응 미확인. 구형 ConnectAccountCorutine의 활성 UI 호출은 확인되지 않으며 현재 UI는 FakeConnectAccountCorutine 사용 |
| 아이템 키 파생 코드의 주석 | [CharacterUpgradeUI.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/UI/Menu/Character/CharacterUpgradeUI.cs:134). 비실행 주석은 보존. 활성 item 조회는 mapping으로 교체 |
| Legacy-Unused AssetBundleManager의 raw assetname API | [AssetBundleManager.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Legacy/Unused/AssetBundle/AssetBundleManager.cs:60). private 일반 asset-name loader가 남으며 실제 CharacterId 전달 호출은 확인되지 않음. Legacy라는 이름만으로 삭제하지 않음 |
| 저수준 bundle loader / bundle 이름 | [LoadManager.AssetBundle.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:139). AssetBundle backend 자체의 결합은 의도적으로 유지. CharacterId 입력은 상위 mapping에서 실제 이름으로 변환됨 |
| 공개 string 필드와 mutable owned 데이터 | CharacterInformation.id, CharacterData.id, GameCharacter.ID, StarChracter는 기존 string/public 계약 유지. 모든 임의 필드 할당을 차단하는 타입 시스템은 도입하지 않았고 실제 입출력 경계에서 정규화 |

`Stage.ID`, `item.id`, terrain 이름, SFX 이름, reflection action 이름은 별도 의미의 키다. CharacterId canonical화 대상에 포함하지 않았다. 서버의 account UID도 캐릭터 논리 ID와 다르며 변경하지 않았다.

## H. Git Diff Summary

이번 구현·데이터 delta는 **22 modified + 2 added, +212 / -49 lines**이다. 보고서 세 파일은 이 집계에서 별도다. 모든 파일 바이트를 snapshot과 비교하고, 각 변경 파일에 대해 `git diff --no-index --no-ext-diff --no-textconv --binary` 결과를 [task.diff](D:/GIthub/UnityFight_Portfolio/docs/character-content-mapping/task.diff)에 모았다. snapshot 경로가 포함된 검토용 diff다.

기존 작업 트리가 이미 대규모 변경 상태였으므로 HEAD 전체 diff 통계는 이번 작업 통계로 사용하지 않았다. 기존 변경을 stage/commit/revert하지 않았다.

정적 검증 결과: [verification.json](D:/GIthub/UnityFight_Portfolio/docs/character-content-mapping/verification.json).

- 기존 파일 9063개를 시작 시점과 비교. 기존 `.meta` 4743개 바이트 보존.
- 모든 기존 경로 및 다른 바이너리 보존. 수정은 위 목록으로 한정.
- JSON은 Character ID 값만 변함. 다른 필드 값 보존.
- YAML은 세 파일의 여섯 ID 값만 변함. GUID/컴포넌트/오브젝트/그 외 직렬화 바이트 보존.
- 5개 mapping과 실제 콘텐츠 존재 및 bundle label 확인. p04 Animator 없음/null 확인.
- LoadManager public 메서드 signature와 10개 캐릭터 queue 경로 mapping 적용 확인.
- 변경된 기존 파일 22개와 신규 C# 파일의 Git diff whitespace 검사에서 문제 없음.
- Packages/ProjectSettings/서버 소스/DB 구조 바이트 보존.
- 독립 정적 코드 리뷰 2회 수행. 직접 UserAccountData 저장 경계 누락 가능성을 보완했고 재리뷰에서 추가 지적 없음.

**Unity Compile / Player Build / Server Build / package restore / 패키지 설치·수정은 수행하지 않았다.** 실행 검증은 사람이 Editor에서 별도로 수행한다. Addressables/ContentProvider 인터페이스/새 backend는 추가하지 않았다.
