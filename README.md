# UnityFight\_Portfolio







개요

C# 소켓 서버와 MariaDB를 연동하여 클라이언트-서버 간 데이터 통신 아키텍처를 직접 구축한 수집형 RPG 프로젝트입니다. 현재는 포트폴리오 시연의 안정성을 극대화하기 위해

&#x20;서버 의존성을 제거하고 스탠드얼론 버전으로 리팩토링되어 있습니다.



활용 기술: C# Socket Server (TCP 비동기), MariaDB, JSON 패킷 통신

핵심 아키텍처: Observer Pattern, Strategy Pattern, Data-Driven Design



\---



\## 📌 핵심 소스코드 바로가기 (Core Scripts)

본 프로젝트에서 직접 아키텍처를 설계하고 구현을 주도한 클라이언트, 서버, 그리고 DB 핵심 코드입니다.



| 구분 | 파일명 (링크) | 담당 역할 및 핵심 기술 |

| :--- | :--- | :--- |

| \*\*네트워크 (서버 엔진)\*\* | \[`Session.cs`](./Server/01Server/ServerCore/Session.cs) | `SocketAsyncEventArgs`를 활용한 비동기(Async) 기반 TCP 소켓 수신 및 세션 라이프사이클 관리 |

| \*\*네트워크 (데이터)\*\* | \[`01Packet.cs`](./Server/01Server/Server/script/Packet/01Packet.cs) | `BitConverter`를 활용하여 통짜 JSON 없이 Byte 단위 커스텀 직렬화/역직렬화 구현 |

| \*\*객체지향 설계\*\* | \[`iAct.cs`](./Client/1\_backup/Assets/script/interface/iAct.cs) | 인터페이스 기반 전략 패턴(Strategy Pattern)으로 행동(공격, 스킬)을 객체화하여 OCP 준수 |

| \*\*구조 디커플링\*\* | \[`ItemBroadcaster.cs`](./Client/1\_backup/Assets/script/interface/ItemBroadcaster.cs) | 옵저버 패턴(Observer Pattern)을 도입하여 데이터(Model)와 UI(View) 간의 강결합 해소 |

| \*\*클라이언트 제어\*\* | \[`NetworkManager.cs`](./Client/1\_backup/Assets/script/NetWorkManager/NetworkManager.cs) | 코루틴 활용 서버 응답 비동기 대기 및 타임아웃(Timeout) 제어로 무한 로딩(Deadlock) 방지 |

| \*\*최적화 및 자료구조\*\* | \[`GameManager.cs`](./Client/1\_backup/Assets/script/Manager/GameManager.cs) | LINQ 정렬 알고리즘, Stack 기반 UI 히스토리 등 자료구조를 활용한 런타임 최적화 |

| \*\*데이터베이스 (DB)\*\* | \[`schema.sql`](./DB/schema.sql) | MariaDB 기반 유저 계정, 스테이지 클리어, 상점 재화 관리를 위한 RDBMS 테이블 구조 설계 |

