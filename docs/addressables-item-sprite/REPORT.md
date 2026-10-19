Item Sprite Addressables 전환 — 구현과 정적 검사 완료. Editor 등록 및 실제 표시 검증은 사용자가 수행합니다.

# A. 수정 파일

- Assets/_Project/Scripts/Manager/LoadManager.cs:168–187: Sprite single 두 overload 및 batch enqueue 표현식 세 곳만 변경. public signature, Action<Sprite>/Action<List<Sprite>>, 진행도 budget 그대로.
- Assets/_Project/Scripts/Manager/ItemContentMapping.cs:12–32: SpriteName/SpriteAddress를 별도 readonly 필드와 명시적 I01~I05 entry로 추가. 기존 ItemId/JsonName/JsonAddress 값 그대로.
- Assets/_Project/Scripts/Manager/LoadManager.ItemSpriteAddressables.cs 및 .meta 신규: Texture2D typed session cache, 생성 Sprite cache, single/batch 및 앱 종료 cleanup.
- Assets/_Project/Scripts/Manager/LoadManager.ItemJsonAddressables.cs:97,106,131: 통합 single/batch/메뉴 preload의 Sprite 호출 세 곳과 주석 두 곳만 변경. JSON 로더·파싱·handle cache·Release·dictionary/Account 연결은 변경하지 않음. 기존 private wrapper 이름은 최소 변경을 위해 유지했으므로 LegacySprite라는 이름이 남지만 내부 Sprite backend는 Addressables임.
- docs/addressables-item-sprite/REPORT.md, verification.json, task.diff: 조사 및 작업 전후 검증 기록.

작업 시작부터 이미 존재한 SFX/Terrain/Item JSON 변경은 그대로 보존했습니다. 작업 전 snapshot과 비교한 이번 변경만 위 목록입니다.

# B. I01~I05 importer 조사 결과

실제 파일: Assets/_Project/Art/Sprite/item/I01.png.meta ~ I05.png.meta.

| ID | textureType | spriteMode | slice | pivot | pixelsPerUnit | border |
| --- | --- | --- | --- | --- | --- | --- |
| I01 | 8 (Sprite) | 1 (Single) | sprites: [] — Single에는 별도 slice 불필요 | (0.5,0.5) | 100 | (0,0,0,0) |
| I02 | 8 (Sprite) | 1 (Single) | sprites: [] — Single에는 별도 slice 불필요 | (0.5,0.5) | 100 | (0,0,0,0) |
| I03 | 8 (Sprite) | 2 (Multiple) | sprites: [] — slice 없음 | (0.5,0.5) | 100 | (0,0,0,0) |
| I04 | 8 (Sprite) | 2 (Multiple) | sprites: [] — slice 없음 | (0.5,0.5) | 100 | (0,0,0,0) |
| I05 | 8 (Sprite) | 1 (Single) | sprites: [] — Single에는 별도 slice 불필요 | (0.5,0.5) | 100 | (0,0,0,0) |

다섯 importer 모두 spriteMeshType:1, spriteExtrude:1, assetBundleName:itemsprite. importer 수정 없음.

기존 Sprite.Create는 로드된 Texture2D 전체 rect, 중앙 pivot의 세 인자 overload입니다. PPU 100/border 0은 import 설정과 일치하나 import mesh/extrude와 runtime overload의 기본 옵션이 전부 같다고 단정할 수 없습니다. Single import Sprite는 같은 표시 영역/중앙 pivot을 기대할 수 있으나, 실제 mesh나 padding/플랫폼 출력은 Editor 검증 대상입니다. Multiple+빈 slice는 기존 전체 이미지 Sprite 계약을 보장하지 못합니다. 새 코드는 다섯 항목 모두 기존 overload/인자를 그대로 사용하므로 import Sprite와의 차이를 로더 교체 과정에 추가하지 않습니다.

# C. 기존 Item Sprite 흐름

LoadManager.PushItemspriteLoadRequest 두 overload / PushItemsSpriteLoadRequest
→ LoadingQueue → PlannedLegacy LoadSpriteAsync / LoadSpritesAsync
→ itemsprite AssetBundle.LoadFromFileAsync
→ untyped LoadAssetAsync(assetname), Texture2D cast
→ Sprite.Create(texture, 전체 rect, 중앙 pivot)
→ 기존 Sprite callback → bundle.Unload(false).

Legacy 근거: PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:234–331.
통합 Item API와 메뉴 preload도 같은 Sprite helper를 호출해 ItemData.itemSprite를 채웠습니다.
단일은 Texture가 없으면 callback을 호출하지 않았습니다. batch는 성공한 Texture만 Sprite 리스트에 추가하며 입력 중복을 유지했습니다. bundle 자체가 열리지 않는 경우는 batch callback도 생략하는 Legacy 경로였습니다.

# D. 변경 후 Addressables 흐름

Item ID → ItemContentMapping.Get(itemId).SpriteAddress
→ 유효한 기존 handle 재사용 또는 Addressables.LoadAssetAsync<Texture2D>
→ 동일 Sprite.Create overload로 생성 / 생성 Sprite 재사용
→ 기존 Sprite callback
→ 통합 API 및 메뉴에서는 기존 방식으로 ItemData.itemSprite에 연결.

Item Sprite single 두 overload와 batch, JSON+Sprite 통합 single/batch 및 전체 메뉴 준비가 동일 helper를 사용합니다.
bundlename 인자는 기존 signature 호환용으로 남으며 Item Mapping이 로딩 대상을 결정합니다. 일반 임의 bundle 로더로 사용하지 않습니다. 현재 확인된 외부 호출자는 없고, 임의 bundle을 해석하는 fallback은 추가하지 않았습니다.

단일 실패는 오류 기록 및 실패 handle 제거/Release 후 success callback 생략.
batch는 입력 순서대로 single helper를 호출하며 성공한 결과만 추가합니다. 성공 입력의 순서/중복은 유지하며 실패 항목에 null slot을 새로 추가하지 않습니다. 모든 주소 실패 시 빈 리스트 callback을 호출합니다. 예전 bundle 전체 open 실패의 callback 미호출과 완전히 같은 모델은 아닙니다. Addressables에는 공유 bundle-open 요청이 없으므로, 기존 항목별 누락 계약을 적용했습니다.

통합 batch는 기존대로 JSON/Sprite 결과를 인덱스로 결합합니다. 중간 Sprite 실패로 리스트가 줄면 뒤 항목이 잘못 결합되는 기존 문제가 그대로 남아 있습니다. 이번에는 수정하지 않았습니다.
single 진행도 2, Sprite batch count+1, preload JSON 1+Sprite 2 정책 유지. 통합 batch의 기존 budget 불일치도 이번 범위 밖입니다.

# E. 선택 이유

선택: 다섯 항목 모두 LoadAssetAsync<Texture2D> + 기존 Sprite.Create.

LoadAssetAsync<Sprite>는 runtime Sprite 생성이 없어 더 단순하지만, I03/I04의 빈 Multiple slice로 기존 표시 계약을 보장할 수 없습니다. I01/I02/I05까지 Texture 경로로 통일하면 loader 분기·importer 변경 없이 기존 Sprite 생성 인자를 유지할 수 있습니다.
실제 Texture importer 및 PNG는 unchanged이므로 플랫폼의 Texture 출력 조건도 그대로입니다. Editor에서 기존 이미지 표시와 비교해야 하며 정적 검증만으로 픽셀 동일성을 주장하지 않습니다.

# F. Mapping / cache / lifetime

| ItemId | SpriteName (physical) | SpriteAddress |
| --- | --- | --- |
| I01 | I01 | item/sprite/I01 |
| I02 | I02 | item/sprite/I02 |
| I03 | I03 | item/sprite/I03 |
| I04 | I04 | item/sprite/I04 |
| I05 | I05 | item/sprite/I05 |

기존 JSON Address는 item/json/I01~I05 그대로입니다. I06은 추가하지 않았습니다.
알려진 ID 대소문자는 기존 Mapping의 OrdinalIgnoreCase 규칙을 유지하고 고정 Address로 해결합니다. ID 자체를 Address/asset path로 사용하지 않습니다. unknown/null ID는 기존 Mapping의 명확한 예외 정책 유지.

LoadManager static ItemSpriteTextureHandles가 address별 AsyncOperationHandle<Texture2D> 소유. 첫 yield 전에 cache에 넣어 in-flight/완료 요청 모두 재사용합니다.
ItemSessionSprites가 runtime Sprite를 address별 보관합니다. 기존에는 요청마다 새 Sprite를 만들었으나 이제 중복 요청은 동일 Sprite 참조를 받습니다. 현재 소비 코드에 Sprite 자체 mutation/Destroy는 확인되지 않았고, ItemData.itemSprite assignment/UI 표시 계약은 그대로입니다.

callback 직후/Scene 전환/LoadManager 파괴 때 Release하지 않습니다.
Application.quitting → ReleaseItemSpriteSessionHandles:
event unsubscribe → 생성 Sprite Destroy 및 cache clear → valid Texture handle Release 및 cache clear.
생성 Sprite에는 Addressables.Release를 사용하지 않습니다. Texture handle ownership과 runtime 객체 cleanup을 분리합니다.
JSON TextAsset cache/ItemData 새 파싱 정책은 unchanged입니다. getitem dictionary와 UserAccountData metadata 연결도 unchanged입니다.

# G. Legacy 및 남은 Active itemsprite 참조

PlannedLegacy AssetBundle 파일과 모든 기존 meta/GUID/Texture importer는 작업 전과 byte 동일입니다.
기존 itemsprite single/batch/통합/menu helper와 bundle 자체는 삭제하지 않았습니다.
PlannedLegacy/Legacy를 제외한 Active C#에 itemsprite 문자열 호출 또는 generic LoadSpriteAsync/LoadSpritesAsync 호출은 0개입니다. 자동 fallback/WaitForCompletion 없음.

작업 전 제품 파일 9,142개 hash와 비교:
기존 3파일만 변경, 새 script/meta 2파일 추가, 삭제 0.
기존 meta 4,785개 모두 동일.
Character/Stage/Terrain/SFX, UI/Account/전투, Legacy 파일, JSON/PNG/Scene/Prefab, Settings/Group/Packages/Server 모두 보존.
정적 검사 17개 통과: 정확한 라우팅/JSON 비변경, mapping/importer, cache/ownership/quit cleanup, batch 계약, Active 호출, 신규 선언/괄호 및 작업 추가 줄 whitespace 검사.
이는 compile/실행 검증이 아닙니다.

# H. Editor 등록 목록과 Phase 1 남은 작업

프로젝트 Client/1_backup 기준 다음 PNG 원본 asset을 등록합니다. Sprite subasset/slice를 선택하지 않고 Texture2D로 해석 가능한 원본 texture entry를 사용하세요.

| 프로젝트 asset 경로 | Address |
| --- | --- |
| Assets/_Project/Art/Sprite/item/I01.png | item/sprite/I01 |
| Assets/_Project/Art/Sprite/item/I02.png | item/sprite/I02 |
| Assets/_Project/Art/Sprite/item/I03.png | item/sprite/I03 |
| Assets/_Project/Art/Sprite/item/I04.png | item/sprite/I04 |
| Assets/_Project/Art/Sprite/item/I05.png | item/sprite/I05 |

Settings/Group은 자동 수정하지 않았습니다. I06 제외. importer slice 생성/모드 변경 불필요.

Phase 1 종료 전 수동 확인:
- 위 주소 등록 후 LoadAssetAsync<Texture2D> 타입으로 resolve되는지, 특히 Single importer 원본 asset 및 Multiple 빈 slice 상태 확인.
- I01~I05 full-image 표시, pivot/크기/테두리 비교.
- 메뉴 dictionary 준비 및 소유 Item 연결, Inventory/Shop/강화/보상 UI 확인.
- single 두 overload, batch 입력 중복·반복, 통합 single/batch, Scene 왕복과 앱 종료 cleanup 확인.
- 기존 Character/Stage/Terrain/SFX Address 등록 및 Title→Menu→Stage→Battle→Result 회귀 확인.
- 누락 주소에서 single callback 생략/batch 누락의 기존 제한을 이해하고 검증. 기존 batch 인덱스 문제, JSON preload 실패 후 부분 dictionary, 통합 batch progress 제한은 후속 이슈로 남김.

이번 조사상 Active LoadManager의 콘텐츠 helper 호출은 모두 Addressables입니다. bundleDirectory 초기화와 Legacy bundle/manifest/import label은 남아 있으며 자동 삭제하지 않습니다.
서버/다운로드 경로는 별도 범위: PacketHandler.S_DownloadHandler:46 → DownLoadManager.getDownloadInfo, DownLoadManager.StartDownload:53 및 DownLoadBundle의 UnityWebRequestAssetBundle.GetAssetBundle:108은 유지됩니다. 이 경로는 단순 PlannedLegacy 미사용으로 단정하지 않습니다. 로컬 콘텐츠 전환 종료와 다운로드/서버 정리는 분리 판단해야 합니다.
최종 Phase 1 종료 판단은 위 Editor 등록/실행 검증 후 가능합니다. 이번 작업에서 compile/build/restore/Unity 실행은 수행하지 않았습니다.
