# TCP 계정 서버 복구 최종 보고서

2026-10-17 기준: **서버 build/listen, File 및 MariaDB 계정 TCP 왕복 재검증 완료. Unity Play는 미실시.**
이번 마감 작업은 계정 서버 복구 범위에서 종료합니다. 실제 비밀번호는 소스·문서에 저장하지 않습니다.

## A. 이번 최종 수정 파일

| 파일 | 변경 이유 |
| --- | --- |
| Server/01Server/Server/script/ServerSetting.cs | DB 비밀번호 literal 제거, UNITYFIGHT_DB_PASSWORD 환경 변수 조회, 누락 시 명확한 오류 |
| Server/01Server/Server/script/Program.cs | mysql 시작 시 비밀번호 설정 검증, 누락 시 오류 출력·종료 코드 1, file 모드는 비밀번호 불필요 |
| .gitignore | 로컬 테스트 계정 및 임시 harness/build 산출물 .online-recovery/ 제외 |
| docs/online-recovery/test_server_startup.py | 비밀번호 미설정 mysql 거부 및 다른 CWD에서 file 시작 회귀 검사 |
| docs/online-recovery/run_db_read.ps1 | 기존 MariaDbReadCheck.cs의 별도 읽기 전용 SQL 검증을 재실행하는 스크립트 |
| docs/online-recovery/README.md | 최종 설정·검증 범위·사용자 작업·후속 이슈 정리 |
| docs/online-recovery/final-verification.json | 최종 결과 및 테스트 UID 기록 |
| docs/online-recovery/final-*.log | 새 설정을 사용한 build, TCP 왕복, 시작 오류, DB 직접 조회 증거 |

이 표는 이번 마감 작업의 변경 범위입니다. 이전 복구 소스 변경은 verification.json의 modified_product_files 및 ServerPaths.cs에 기록되어 있습니다.
ClientRunMode, UseStandaloneMode/UseOnlineMode, Title UI, Standalone 기본값, Client endpoint, Addressables/LoadManager 및 packet 소스는 이번에 수정하지 않았습니다.

## B. DB secret 관리

- 환경 변수: `UNITYFIGHT_DB_PASSWORD`
- 저장된 기본 비밀번호 및 자동 fallback 없음.
- mysql 모드에서 값이 없거나 빈 문자열이면 listen 전에 `Configuration error: MariaDB mode requires UNITYFIGHT_DB_PASSWORD...`를 출력하고 종료 코드 1로 종료합니다.
- 비밀번호를 로그에 출력하지 않습니다. 연결 문자열도 출력하지 않습니다.
- host=127.0.0.1, port=3306, user=root, database=01database는 기존 sqlSetting에 유지합니다.
- 환경 변수 존재 검사와 DB 인증 성공은 별개입니다. 실제 인증은 DB 연결 시 수행하며 이번에는 TCP 왕복 및 독립 SELECT로 확인했습니다.

PowerShell에서 입력값을 화면/명령 이력에 직접 남기지 않고 실행하는 예:

```powershell
dotnet build Server/01Server/Server/Server.csproj --no-restore
$env:UNITYFIGHT_DB_PASSWORD = [System.Net.NetworkCredential]::new('', (Read-Host 'MariaDB password' -AsSecureString)).Password
try {
    dotnet Server/01Server/Server/bin/Debug/net7.0/Server.dll --storage mysql --host 127.0.0.1 --port 7777
}
finally {
    Remove-Item Env:UNITYFIGHT_DB_PASSWORD -ErrorAction SilentlyContinue
}
```

환경 변수는 서버를 시작하는 프로세스에 전달해야 합니다. 다른 터미널에 설정된 값은 현재 터미널에 자동 적용되지 않습니다.
root를 그대로 사용하는 현재 로컬 테스트 설정은 공개 배포용 인증/권한 설계가 아닙니다.

## C. File / MariaDB 선택 구조

Program.Main의 `--storage`가 ServerSetting.Instance.DBSaveSetting을 한 번 결정합니다.

| 실행 옵션 | DBSaveSetting | 저장 위치 |
| --- | --- | --- |
| 생략 또는 --storage file | Directory | data root/DBprototype/{uid}.json |
| --storage mysql | Mysql | 01database.accountinfojson의 uid/accountjson |

계정 handler는 기존 enum 분기에 따라 LocalTask 또는 SqlTask를 호출합니다. 서로 자동 전환하지 않습니다.
file 모드는 MariaDB 및 DB secret이 필요 없습니다. Client의 Standalone/Online 선택과 서버의 저장 backend 선택은 독립적입니다.
기본 서버 endpoint는 두 저장 모드 모두 127.0.0.1:7777입니다.

## D. 서버 시작 경로 / endpoint

- ServerPaths.Initialize는 AppContext.BaseDirectory에서 상위 경로를 탐색합니다.
- 탐색 기준은 DBprototype/DataTemplet/AccountTemplet.json이며 현재 root는 Server/01Server입니다.
- 배포 시 템플릿을 포함한 데이터 디렉터리를 유지하거나 절대 경로 `--data-root`를 지정합니다. 상대 경로는 거부합니다.
- 계정 파일·템플릿은 ServerPaths.AccountDirectory/AccountTemplate 기준입니다. mysql 신규 계정도 template이 필요합니다.
- 관련 없는 CWD에서 --data-root 없이 file 서버를 시작해 올바른 root와 listen을 확인했습니다.
- ApplicationVersion.txt 및 ApplicationDownLoadURL.txt는 현재 계정 시작·접속 흐름에서 읽지 않습니다.
- 보존된 구형 버전 handler와 ShopDBManager 내부의 CWD 의존 경로는 여전히 남아 있습니다. 해당 기능은 이번 복원 대상이 아니며 안정화했다고 주장하지 않습니다.
- Server --host/--port 기본값은 127.0.0.1/7777, Client NetworkManager.ServerHost/ServerPort도 명시적 설정을 유지합니다. hostname 첫 주소 자동 선택을 사용하지 않습니다.

## E. Packet / SQL 최종 상태

Packet ID 및 binary wire format은 변경하지 않았습니다.

| 요청 / 응답 | ID |
| --- | --- |
| C_ConnectRequest (UID 조회) | 0 |
| C_SaveAccountData (enum C_SaveUserAccountData) | 1 |
| S_ErrorPacket | 2 |
| S_LoadAccount | 3 |
| C_CreateAccountData (enum C_CreateAccount) | 4 |
| S_Message | 5 |

2-byte size + 2-byte ID header, UTF-16LE 문자열 및 ushort byte length, 계정 scalar/list 순서를 유지합니다.
성공 계정 packet의 Client/Server layout을 비교했습니다. 로깅/DTO type alias를 제외하면 일치합니다.
Client DTO에는 ID canonicalization 및 LastestConnect 갱신 차이가 있지만 wire 필드 순서/크기 변경은 아니며 보존했습니다.

계정 SQL 경로:
- PacketHandler.C_CreateAccountHandler → SqlTask.insertDBsql: INSERT, @uid/@json
- PacketHandler.C_ConnectRequestHandler → SqlTask.LoadDBsql: SELECT, @uid
- 계정 저장 handler → SqlTask.UpdateDBUID2: UPDATE, @uid/@json

세 경로 모두 parameterized query입니다. 이번 작업에서 schema/권한/인증 방식/stored procedure/기존 계정을 수정하거나 테스트 계정을 삭제하지 않았습니다.
보존된 DeleteDBsql/UpdateDBUID 및 상점 SQL은 문자열 조합 방식이 남아 있으므로 전체 서버 SQL이 안전하다고 주장하지 않습니다.
Server S_ErrorPacket.Read에는 Client와 다른 추가 ushort offset이 남아 있습니다. 정상 계정 왕복에서 서버가 수신하는 packet이 아니므로 이번에 변경하지 않았습니다.
구형 다운로드/상점 packet은 재검증하거나 복원하지 않았습니다.

## F. 실제 재검증 결과

| 검사 | 결과 / 증거 |
| --- | --- |
| Server/ServerCore build --no-restore | PASS, 오류 0; 기존 net7.0 지원 종료 경고 NETSDK1138 |
| mysql 비밀번호 미설정 | PASS, 명확한 설정 오류·종료 코드 1, listen 안 함 |
| file 비밀번호 미설정 / 다른 CWD | PASS, executable 기준 root 탐색 및 listen |
| MariaDB TCP | PASS, 생성 → 계정 응답 → 저장 → 조회 → 새 TCP 연결 → 조회 |
| File TCP | PASS, 동일한 실제 Client 통신 소스 기반 왕복 |
| Standalone 접속 차단 | PASS, transport harness의 기존 no-connect 검사 유지 |
| 독립 DB 연결 SELECT | PASS, root@localhost / 01database / 3306, 저장값 확인 |
| Unity Play / 실제 Scene·UI | 미실시 |

새 테스트 UID:
- MariaDB: `981b7eaed63f`, username=`client_transport_saved`, EXP=41
- File: `3b6d9619e68c`, 동일 이름/EXP. 테스트용 .online-recovery/file-data/DBprototype 아래 파일 사용.
- 이전 MariaDB 테스트 UID `35c3894579d2` 등은 삭제하지 않았습니다.

로그: final-mariadb-roundtrip.log, final-file-roundtrip.log, final-startup.log, final-mariadb-direct-read.log.
Transport harness는 실제 Client NetworkManager/Core/packet/queue를 사용합니다. Unity API·GameManager·계정 컨테이너/DTO adapter는 test double/서버 모델 기반이므로 실제 Unity 전체 통합을 대신하지 않습니다.

재현 명령 (mysql 명령 전 위 방식으로 환경 변수를 설정):

```powershell
./docs/online-recovery/run_smoke.ps1 -Storage file -Transport -Port 17878
./docs/online-recovery/run_smoke.ps1 -Storage mysql -Transport -Port 17879
./docs/online-recovery/run_db_read.ps1 -Uid <이번-mysql-테스트-UID>
python docs/online-recovery/test_server_startup.py
```

기존 .NET SDK 및 package/reference cache를 사용했으며 restore/설치/Unity 실행은 하지 않았습니다.
테스트 서버는 종료했습니다. 빌드 산출물은 소스 변경과 분리하며 실제 서버 실행 전 최신 소스로 재빌드해야 합니다.

## G. 남은 이슈 / 향후 확장

- Unity 실제 GameManager/Client DTO/Title 적용과 Standalone/Online Play 검증
- 구형 version/download/AssetBundle 경로 및 Shop의 CWD 의존 경로
- 계정 이외 SQL 매개변수화와 위 S_ErrorPacket.Read의 비대칭
- 인증·소유권 검증·권한 분리·암호화·대형 packet/동시 요청 및 부하 검증
- net7.0 지원 종료 및 기존 tracked bin/obj 정리
- PvP/Redis/랭킹/자동 재접속/상점 전체 기능

이 항목들은 이번 마감에서 구현하지 않습니다. 현재 서버는 로컬 계정 통신을 검증한 포트폴리오용 범위입니다.

## H. 사용자가 Unity에서 직접 할 작업

1. 원하는 Title UI 버튼/토글 또는 Inspector를 직접 연결합니다. 기존 UseStandaloneMode()/UseOnlineMode()/SetRunMode() 구조를 그대로 사용합니다.
2. 기본 Standalone을 유지하고 서버 없이 기존 Dummy/로컬 데모가 동작하는지 Play로 확인합니다.
3. Online을 선택하고 Client ServerHost=127.0.0.1, ServerPort=7777에 맞춰 서버를 실행합니다.
4. 신규 계정 생성 또는 서버 UID 로그인 → 응답 적용 → 저장 → 재조회 → 재접속 후 조회를 Unity에서 확인합니다.
5. 필요하면 UnityOnlineRoundTripProbe.cs를 임시 컴포넌트로 직접 연결하여 사용합니다. 해당 파일도 Unity compile/Play 미검증입니다. 테스트는 로컬 UID cache를 바꿀 수 있으므로 결과 UID를 기록합니다.
6. Addressables 등록·콘텐츠 로더를 바꿀 필요는 없습니다.

이번 작업에서는 ClientRunMode 구조·기본값·Title UI·버튼·Inspector/Scene 설정을 변경하지 않았습니다.

## I. 포트폴리오 표현 범위

가능한 표현:
“Unity Client/C# TCP 계정 통신 및 File·MariaDB 저장 backend 구현. 계정 생성·조회·저장·새 연결 후 재조회와 DB 직접 저장값을 비Unity 통신 테스트로 검증.”

아직 보류할 표현:
“현재 Unity Play 전체 Client–Server–DB 연동 검증 완료”, “전체 서버 기능 복원 완료”, “공개 서비스 운영 준비 완료”.

Unity Play 검증이 통과한 뒤 그 범위를 추가할 수 있습니다.

## J. 종료 판단

**계정 TCP 왕복 서버 복구 코드 작업은 종료해도 됩니다.**
비밀번호 하드코딩을 제거하고 새 환경 변수 설정으로 실제 왕복을 재검증했습니다. 남은 Unity 모드 선택 UI/Inspector 연결·Play 검증은 사용자가 직접 수행합니다.
향후 확장은 별도 작업이며 현재 구조를 추가로 리팩토링하지 않습니다.
