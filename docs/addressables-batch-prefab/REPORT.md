# Batch Character Prefab Addressables 전환

2026-10-08. 코드 변경 및 정적 검증만 수행. Compile/Build/Package Restore/Unity 실행/패키지 변경은 수행하지 않았다.

## A. 변경 파일

- [LoadManager.cs:146](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:146): PushCharactersloadrequest의 Queue 등록 한 줄을 batch Addressables Coroutine으로 교체. public API, Mapping.Resolve, MaxLoadingprogress += s.Count + 1 유지.
- [LoadManager.Addressables.cs](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.Addressables.cs): 순차 batch Coroutine 추가. 단일 helper에 private optional progressWeight(기본 2) 추가하여 동일 캐시/로드/실패 처리를 재사용. batch 항목은 1단위로 계산.
- REPORT.md / verification.json / task.diff: 이번 결과와 검증 근거. 신규 제품 클래스·인터페이스·meta는 없음.

## B. 기존 batch 흐름

CharacterId 목록 → CharacterContentMapping.Resolve(PrefabName) → Queue → AssetBundle LoadPrefabsAsync("character", names) → 이름별 순차 로드 → request.asset as GameObject를 List에 추가 → callback(List) → Unload(false).

근거: [LoadManager.AssetBundle.cs:186](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:186). 실제 asset 결과가 null이면 List에도 null이 들어가는 계약이다. bundle 자체를 열지 못한 경우는 callback 없이 yield break하는 기존 별도 실패 경로다.

## C. 변경 후 batch 흐름

CharacterId 목록 → 기존 CharacterContentMapping.Resolve(PrefabName) → Queue → LoadCharacterPrefabsAddressableAsync → 각 address에 대해 기존 단일 LoadCharacterPrefabAddressableAsync 완료 대기 → Prefab/null 결과 추가 → 최종 callback(List).

일반 Instantiate를 호출하거나 instance를 반환하지 않는다. 전투의 master/Animator 후처리 및 Instantiate 위치를 변경하지 않았다. 단일 요청 두 overload도 그대로다. batch 진행량은 setup 1 + 항목별 1 = n + 1이며, 단일 요청은 기존 2단위를 유지한다. setup 단위는 기존 진행량 예산을 보존하기 위한 것이며 별도 bundle 로드 작업을 뜻하지 않는다.

## D. 순서 보존 방법

foreach로 입력 address 순서대로 처리하고 각 단일 Coroutine의 종료를 기다린다. 완료 순으로 병렬 결과를 재배열하는 경로는 없다. 각 callback 결과를 조건 없이 한 번 Add하므로 결과 인덱스가 입력 인덱스와 같다. 중복 address도 슬롯을 제거하지 않는다. 빈 입력은 빈 List로 최종 callback을 한 번 실행한다. 도중 Coroutine이 취소되면 부분 List를 반환하지 않는다.

예: p01, p02, p01 입력에서 P02만 실패하면 결과는 P01 Prefab, null, 동일 캐시의 P01 Prefab 세 슬롯이다. 이는 구조에 대한 정적 확인 예시이며 Unity 실행 테스트 결과는 아니다.

## E. cache / handle 재사용

batch는 기존 단일 helper를 직접 재사용한다. Addressables.LoadAssetAsync 호출, Dictionary, 실패 핸들 정리 코드를 복제하지 않았다. 성공한 같은 address는 단일 요청과 batch 요청 사이에서도 기존 CharacterPrefabHandles operation을 재사용한다. 중복 결과 슬롯은 동일 Prefab asset 참조일 수 있으며 새로운 Instantiate 결과가 아니다.

session cache가 성공 핸들을 유지하고 Application.quitting에서 Release하는 정책은 그대로다. ReleaseCharacterPrefabSessionHandles 메서드는 바이트가 동일하다. 관측한 실패 핸들은 기존 단일 helper에서 제거/Release한다. 따라서 실패한 같은 address를 뒤에서 다시 요청하면 재시도할 수 있다. 시작한 뒤 batch가 취소된 operation도 기존 session cache에 소유된다.

수명 및 데이터 소유권 개선 제안만 기록한다: 캐시는 마지막 소비자/instance 사용 종료를 알지 못하므로 세션 중 메모리를 유지한다. 또한 전투는 로드된 Prefab asset의 GameCharacter에 set_Character/setAnimation을 적용한다. 같은 ID의 중복 슬롯은 기존 AssetBundle batch에서도 같은 에셋일 수 있었으며, 이제 단일 경로와 batch까지 세션 Prefab을 공유한다. 향후 instance 데이터 주입과 에셋 참조 소유권을 별도 범위로 검토할 수 있지만 이번에는 변경하지 않았다.

## F. 실패 항목 처리

Addressables 실패는 단일 helper가 로그를 남기고 null을 callback한다. batch는 해당 null을 같은 인덱스에 보존하고 다음 항목을 계속 처리한 뒤 원래 길이의 List를 반환한다. 리스트를 필터링하거나 줄이지 않는다. AssetBundle fallback은 없다.

기존의 asset-null 슬롯 계약을 유지한다. 기존 bundle 전체 오픈 실패의 callback 생략 방식은 새 address별 실패에 기계적으로 적용하지 않았다. 새 backend에서는 개별 실패를 null 슬롯으로 표면화한다. Mapping에 등록되지 않은 논리 ID는 기존처럼 Resolve 단계에서 예외가 발생하여 Queue 등록 전에 실패한다.

현재 Addressables Settings에는 P01만 등록되어 있다. P02/P03/P04/E00 등 미등록 address는 실패한다. [LoadManager.cs:502](D:/GIthub/UnityFight_Portfolio/Client/1_backup/Assets/_Project/Scripts/Manager/LoadManager.cs:502)의 전투 후처리는 loadedcharacter[i].GetComponent를 호출하므로 null 슬롯에서 예외가 날 수 있다. 이는 실패를 안전한 전투 취소로 바꾸는 구현이 완료되었다는 의미가 아니다. 전투 로직 변경 금지 범위에 따라 유지했다. 사람이 게임 전투를 검증하기 전에 필요한 Prefab을 등록해야 한다.

## G. Legacy 보존 상태

PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs와 LoadPrefabsAsync 전체를 그대로 보존했다. 삭제·이동 없음. JSON, 동기 master 이름 조회, Portrait, Animator, Stage/Terrain, Item, SFX는 계속 기존 AssetBundle 경로를 사용한다. UI/Battle 코드 및 Instantiate 위치도 변경하지 않았다.

## H. 다음 추천 대상

먼저 필요한 나머지 Character Prefab address와 Mapping.PrefabName의 대응을 확인하는 것이 현재 전투 검증의 선행 조건이다. 그다음 전환 후보는 Character JSON/master 경로다. 비동기 단일/batch JSON 요청뿐 아니라 CharacterData.Name에서 호출하는 동기 ReturnChrDataName 경로까지 조사해야 한다. 동기 이름 조회와 JSON 입력 정규화를 어떻게 유지할지 먼저 정하고 별도 작업으로 진행하는 것을 권장한다. 이번에는 구현하지 않았다.

## 검증 범위

[verification.json](D:/GIthub/UnityFight_Portfolio/docs/addressables-batch-prefab/verification.json), [task.diff](D:/GIthub/UnityFight_Portfolio/docs/addressables-batch-prefab/task.diff).

작업 시작 snapshot의 제품/설정 파일 9,096개와 바이트 비교. 제품 코드 수정 두 파일 외 추가·삭제 없음. 기존 meta 4,762개, Legacy backend, Scene/Prefab/JSON, Settings/Packages/Server 및 사용자 테스트 코드 보존. public API 시그니처 동일, Git diff whitespace 검사 문제 없음. 독립 정적 리뷰에서 actionable regression 없음. Unity 실행은 검증하지 않았다.
