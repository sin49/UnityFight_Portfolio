Item JSON Addressables 전환 — 코드 및 정적 검증 완료. Item Sprite는 기존 AssetBundle 경로를 유지합니다.

# A. 수정 파일

| 파일 | 변경 내용 |
| --- | --- |
| `Assets/_Project/Scripts/Manager/LoadManager.cs` | Item JSON single/batch, 통합 single/batch, 메뉴 전체 준비의 다섯 enqueue/호출 표현식 변경. |
| `Assets/_Project/Scripts/Manager/ItemContentMapping.cs` | I01~I05 logical ID / JsonName / Address 명시적 Mapping. |
| `Assets/_Project/Scripts/Manager/LoadManager.ItemJsonAddressables.cs` | TextAsset session cache, fresh parse, JSON batch, 기존 Sprite와 결합하는 wrapper, 메뉴 dictionary 준비. |
| 신규 C# 두 파일의 `.meta` | 새 script GUID 생성. 기존 meta 변경 없음. |
| `docs/addressables-item-json/REPORT.md`, `verification.json`, `task.diff` | 결과, 정적 검증, 실제 작업 전후 diff. |

Legacy 파일과 Sprite helper 자체, Character/SFX/Stage/Terrain 로더 및 Mapping, 전투/UI/Account 코드, Settings/Group/Package는 변경하지 않았습니다.

# B. 기존 Item JSON 흐름

- `PushitemdbLoadRequest` / `PushitemdbsLoadRequest` (`LoadManager.cs:242–251`)는 generic itemdb TextAsset helper를 enqueue했습니다.
- 통합 `PushitemLoadRequest` / `PushitemsLoadRequest` (`:252–261`)는 Legacy LoaditemAsync / LoaditemsAsync를 enqueue했습니다. 내부에서 itemdb JSON 후 itemsprite를 로딩해 ItemData.itemSprite를 채웠습니다.
- `loadMenuScene:330`은 Legacy LoadAllitemAsync를 기다렸습니다. 이 helper는 itemdb의 TextAsset 전체를 읽어 Version 텍스트를 제외하고, 각 JSON을 파싱한 뒤 itemsprite를 로딩하고 itemDictionary에 추가했습니다.
- `loadMenuScene:336`의 `Account.PushItemDicLoadRequest`가 dictionary의 ItemData를 소유 Item에 연결합니다. 그 뒤 MainMenu 진입, 보상/Inventory/Shop/강화 UI가 동기 metadata를 사용합니다.

Legacy 근거: `PlannedLegacy/ContentLoading/LoadManager.AssetBundle.cs:562–633`. 전체 구현을 보존했습니다. `LoaditemDictionaryAsync`는 현재 확인된 호출자가 없어 보존만 했습니다.

# C. 변경 후 Addressables 흐름

`Item ID` → `ItemContentMapping.Get(itemId).JsonAddress` → `LoadAssetAsync<TextAsset>` 또는 유효 handle 재사용 → `convertTextasset<ItemData>` 새 파싱 → 기존 ItemData callback.

- JSON single은 공통 단일 helper를 사용합니다.
- JSON batch는 같은 helper를 입력 순서대로 호출하며 결과 개수/순서/중복을 유지합니다. 실패 결과는 null slot입니다. 빈 입력은 빈 리스트 callback입니다.
- 통합 single은 JSON만 새 helper로 교체하고 기존 `LoadSpriteAsync("itemsprite", itemId, ...)`를 계속 사용합니다.
- 통합 batch도 JSON만 바꾸고 기존 `LoadSpritesAsync("itemsprite", itemIds, ...)`를 계속 사용합니다. 실패한 JSON slot에 Sprite를 대입할 때 null dereference만 방지했습니다.
- 메뉴 전체 준비는 Mapping의 5개 JSON을 새로 파싱하고, 같은 기존 Sprite helper로 Sprite를 채운 뒤 동일한 dictionary에 저장합니다.

public API 이름/인자/반환형과 callback 타입을 유지했습니다. Addressables Sprite 로딩, Texture/Sprite 재생성 방식 변경, UI/전투/소유 Item 연결 변경은 없습니다. 자동 fallback이나 WaitForCompletion도 없습니다.

# D. Item ID / JsonName / Address Mapping

| ItemId | JsonName | JsonAddress |
| --- | --- | --- |
| I01 | I01 | `item/json/I01` |
| I02 | I02 | `item/json/I02` |
| I03 | I03 | `item/json/I03` |
| I04 | I04 | `item/json/I04` |
| I05 | I05 | `item/json/I05` |

별도 readonly 필드와 explicit entry로 보관합니다. ID와 물리 파일명은 현재 우연히 같지만 loader가 이를 Address로 직접 사용하지 않습니다. 알려진 ID의 대소문자는 같은 Mapping을 반환합니다. JSON의 기존 id/Name/desc 값은 변경하지 않았습니다.

I06은 JSON 파일이 존재하지만 기존 itemdb label이 없으므로 Mapping/전체 준비/등록 목록에서 제외했습니다. 파일/meta는 그대로 보존했습니다. 미등록 ID는 Mapping에서 명확한 ArgumentException으로 거부하며 Address나 데이터를 임의 생성하지 않습니다.

# E. 전체 preload 및 동기 dictionary 계약

메뉴의 기존 preload 위치와 `Account.PushItemDicLoadRequest` 순서를 유지했습니다. 정상 로딩에서 I01~I05 JSON과 기존 Sprite 준비가 끝난 뒤 Account 연결과 MainMenu 진입이 진행됩니다.

`LoadManager.getitem` (`:39`)은 기존 dictionary 동기 조회 그대로입니다. `UserAccountData.PushItemDicLoadRequest` (`:101`)도 기존 per-ID metadata를 소유 Item에 연결합니다. 별도 동기 Addressables API는 만들지 않았습니다.

전체 준비는 기존처럼 새 dictionary를 구성합니다. JSON 실패 시 오류를 기록하고 준비를 중단합니다. parent의 기존 메뉴 진입/실패 정책에는 새 gate를 추가하지 않았으므로, 실패 시 dictionary가 부분 준비 상태일 수 있습니다. 이번 전환이 실패 주소에서도 정상 메뉴 동작을 보장한다는 뜻은 아닙니다. Editor 등록 후 정상 preload를 확인해야 합니다.

JSON single/batch의 기존 public budget 2는 유지하며 batch에서는 항목에 분배합니다. 통합 API의 JSON 부분은 budget 0으로 처리해 기존 Sprite 진행도를 그대로 사용합니다. 메뉴 준비는 JSON 1 + 기존 Sprite 2를 각 entry에 반영합니다. 통합 batch의 구형 Sprite progress/전체 budget 불일치는 이번에 재설계하지 않았습니다.

# F. handle/cache 및 ItemData 객체 소유권

`ItemData` (`Object/dataclass.cs:219`)는 id/Name/desc/itemSprite가 가변 필드입니다. 특히 로더가 Sprite를 뒤늦게 대입하므로 callback 객체를 cache하면 다른 요청과 상태가 공유될 수 있습니다.

- LoadManager의 static ItemJsonHandles는 source TextAsset handle만 주소별로 cache합니다.
- 첫 yield 전 handle을 저장하고 유효한 in-flight/완료 handle을 재사용합니다.
- 모든 성공 요청/cache hit/batch 중복 slot에서 기존 convertTextasset을 새로 실행합니다. callback ItemData는 다른 JSON 요청의 객체와 공유하지 않습니다.
- 준비된 dictionary의 per-ID ItemData를 Account/UI가 공유하는 기존 metadata 계약은 유지합니다. 소유 수량 Amount는 별도 item 객체에 있으므로 JSON cache와 공유하지 않습니다.
- 실패 시 cache가 같은 handle을 소유할 때만 제거하고 유효하면 Release합니다. callback은 null입니다.
- 성공 handle은 callback 직후/Scene 전환/LoadManager 파괴 시 해제하지 않습니다. Application.quitting → ReleaseItemJsonSessionHandles에서 해제합니다.

이는 Character/Stage JSON의 typed source cache + fresh parse 패턴을 따릅니다. 기존 Character/Stage 구현을 수정하거나 공통 추상화로 재작성하지 않았습니다.

# G. Legacy itemdb 및 남은 Active 참조

Legacy AssetBundle 파일은 작업 전 hash와 동일합니다. generic TextAsset helper, 기존 Item 통합 single/batch, 전체 Item 준비 구현, itemdb label/version/bundle/manifest를 전부 보존했습니다.

Active C#의 itemdb 문자열 호출 및 구형 LoaditemAsync/LoaditemsAsync/LoadAllitemAsync/LoaditemDictionaryAsync 호출은 0개입니다. 정의만 남은 PlannedLegacy를 Active 사용으로 계산하지 않습니다.

**itemsprite는 여전히 Active입니다.** 메뉴 preload와 새 통합 wrapper, 기존 Item Sprite public API가 기존 AssetBundle Sprite helper를 사용합니다. 이는 이번 범위에 맞게 의도적으로 남긴 경로입니다.

정적 검증: 전환 전 9,138개 제품 파일과 비교해 기존 파일은 LoadManager 다섯 표현식만 변경됐고 새 C#/meta 네 파일만 추가됐습니다. 기존 meta 4,783개와 GUID 모두 byte 동일입니다. 다른 로더/Mapping, 전투/UI/Account, Sprite implementation/import, Settings/Group/Package, Scene/Prefab/JSON/이미지/서버도 모두 byte 동일입니다. Mapping 5개가 실제 bundle-labelled JSON 전체와 일치하는지, API/메뉴 순서, fresh parse, cache/Release, Active 참조 및 diff whitespace를 확인했습니다.

Compile / Build / Package Restore / Unity 실행은 수행하지 않았습니다. 정적 검사는 컴파일이나 실제 UI 성공을 보증하지 않습니다.

# H. Editor 등록 목록과 Item Sprite 주의점

경로는 `Assets/_Project/Data/Json/item/` 기준입니다. 기존 TextAsset에 아래 Address를 지정합니다.

| 실제 JSON | Address |
| --- | --- |
| I01.json | `item/json/I01` |
| I02.json | `item/json/I02` |
| I03.json | `item/json/I03` |
| I04.json | `item/json/I04` |
| I05.json | `item/json/I05` |

Settings/Group은 자동 수정하지 않았습니다. I06과 BundleVersion 텍스트는 등록 대상이 아닙니다.

Editor에서 메뉴 진입 전 dictionary 5개 준비, 소유 Item metadata 연결, Result 보상, Inventory/Shop/강화 화면, 반복 JSON 요청과 객체 분리, 실패 callback/null slot 및 앱 종료 Release를 확인해야 합니다. Sprite 검증에는 기존 itemsprite bundle이 계속 필요합니다.

후속 Item Sprite 전환 시 주의점:

1. I03/I04는 Multiple Sprite mode인데 slice 목록이 비어 있습니다. 기존 loader는 Texture2D 전체로 Sprite를 생성하므로 곧바로 LoadAssetAsync<Sprite>로 치환하면 동일 결과를 보장하기 어렵습니다.
2. 기존 Sprite batch는 누락된 texture를 리스트에서 제외합니다. 통합 batch는 JSON/Sprite를 인덱스로 결합하므로 중간 Sprite 누락 시 인덱스가 밀리는 기존 문제가 남아 있습니다. 이번에는 Sprite helper 계약이나 동작을 바꾸지 않았습니다.
3. JSON TextAsset과 Sprite/Texture handle 소유권은 구분해야 합니다. 이번 session cache는 JSON만 관리합니다.
