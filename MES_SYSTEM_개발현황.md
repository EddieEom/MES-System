# MES SYSTEM 개발 총정리

> **프로젝트:** Digital Twin 연동 MES 생산관리 시스템  
> **정리 범위:** 초기 기획 → 설계 → 구현 → 통합 → 트러블슈팅 → 최종 HMI → 마무리 검증  
> **최종 구현 범위:** MELSOFT Gemini ↔ Mitsubishi PLC ↔ Mes.Gateway ↔ Mes.Server ↔ MSSQL ↔ WinForms HMI  
> **현재 단계:** 핵심 기능 개발 완료 / 최종 검증·문서화·포트폴리오 진행

---

## 1. 프로젝트 개요

### 1.1 최초 아이디어

초기 목표는 **Digital Twin 기반 제조 공정을 PLC와 MES에 연결하여 실제 생산 흐름을 추적하고 제어하는 스마트팩토리 시스템**을 만드는 것이었다.

처음에는 다음 범위를 폭넓게 검토했다.

- Digital Twin 공정 시뮬레이션
- PLC START / STOP / RESET
- WorkOrder / LOT / Product 생산관리
- WIP
- 제품 1EA Traceability
- 품질 PASS / FAIL
- Machine Status
- 생산실적 / Yield
- Alarm
- 작업자 WinForms HMI
- 관리자 Web MES
- Cycle Time / 설비 가동률 / OEE

개발이 진행되면서 핵심 목표를 다음과 같이 좁혔다.

> **Digital Twin과 PLC에서 실제로 발생하는 생산·설비·품질·Alarm 데이터를 MES에 연결하고, 제품 단위 Traceability와 작업자 HMI까지 End-to-End로 구현한다.**

최종적으로 Vue 기반 Web MES와 OEE 전용 화면은 구현 범위에서 제외했다.

---

### 1.2 최종 프로젝트 범위

최종 시스템은 아래 구조로 확정했다.

```text
MELSOFT Gemini
      ↕
GX Works2 / GX Simulator2
      ↕
MX Component V5
      ↕
Mes.Gateway
      ↕
MQTT / HTTP / SignalR
      ↕
Mes.Server
      ↕
MSSQL MES_SYSTEM
      ↕
WinForms Mes.Hmi
```

핵심 원칙은 **OT 통신, MES 비즈니스 로직, DB, UI를 직접 결합하지 않는 것**이다.

- `Gateway → Mes.Server → MSSQL`
- `HMI → Mes.Server`
- HMI가 PLC / MX Component / MQTT / DB에 직접 접근하지 않음
- Gateway가 MSSQL에 직접 접근하지 않음

---

## 2. 초기 공정 기획

### 2.1 초기 OIS Ass'y 공정

초기 설비 기획은 OIS Ass'y 조립 공정을 기준으로 했다.

**전체 공정 개념**

원자재 Cabinet 투입 → LOT/Tray 인출 → 진공 Pick & Place → 에폭시 도포 → 마그넷 공급 → 마그넷 부착 → 압착/안착 → 검사 → OK 적재 / NG 분리 → 완료 Cabinet 배출

초기 생산 단위는 다음과 같이 잡았다.

- 1 LOT = 60 EA
- 1 Cabinet = 6 LOT
- 1 Cabinet = 360 EA

---

### 2.2 Digital Twin 데모 공정 단순화

MES 통신과 Traceability 검증에 집중하기 위해 실제 데모 공정은 아래처럼 단순화했다.

```text
Feeder
↓
VM1
↓
PRESS
↓
QUALITY CHECK
├─ PASS → WAREHOUSE
└─ FAIL → NG_STACK
```

현재 E2E 테스트용 생산 단위:

- 1 WorkOrder = 300 EA
- 1 WorkOrder = 5 LOT
- 1 LOT = 60 EA

> 초기 Cabinet 개념의 `360 EA / 6 LOT`과 현재 MES 검증용 `300 EA / 5 LOT`은 서로 다른 기준이다.

---

## 3. 초기 기능 요구사항

### 3.1 생산관리

처음부터 다음 생산정보를 관리하도록 설계했다.

- 작업지시
- LOT
- Product
- 목표수량
- 생산수량
- 양품수량
- 불량수량
- 달성률
- 수율
- 현재 LOT 진행률
- WorkOrder 진행률

---

### 3.2 WIP / Traceability

제품 1EA 단위로 다음 정보를 추적하도록 설계했다.

- 현재 Product 상태
- 현재 위치
- 공정 이동 이력
- 품질 결과
- 최종 Route
- WIP

현재 상태와 과거 이력을 분리했다.

- 현재 위치 → `products.current_location_id`
- 과거 이동 이력 → `process_events`
- WIP → `status = IN_PROCESS`

---

### 3.3 설비 상태

VM1 / PRESS에서 다음 Gemini Statistics를 수집하도록 설계했다.

- `State`
- `PartsEntered`
- `PartsExited`
- `PartsCurrent`
- `PartsAverageTime`
- `IdlePercentage`
- `BusyPercentage`
- `BlockedPercentage`
- `FailedPercentage`
- `RepairPercentage`
- `Utilization`

이를 기반으로 HMI에서 설비 상태와 가동 관련 정보를 표시한다.

---

### 3.4 품질

품질은 제품 1EA 단위로 관리한다.

- 제품 유형 #2 → PASS
- 제품 유형 #1 → FAIL

품질 이벤트 주요 데이터:

- `eventId`
- `timestamp`
- `machine`
- `result`
- `productType`

최종 Route:

- PASS → `WAREHOUSE`
- FAIL → `NG_STACK`

재작업은 현재 프로젝트 범위에서 제외했다.

---

### 3.5 초기 분석 기능

초기 기획에는 다음 분석 기능도 포함되어 있었다.

- Cycle Time
- Busy / Idle / Blocked 비율
- 설비 가동률
- 불량률
- OEE

현재는 Machine Statistics와 품질/생산 데이터를 저장할 수 있는 기반만 유지하고, 별도 Web 분석 화면은 구현하지 않았다.

---

## 4. MES 데이터베이스 설계

### 4.1 최종 테이블

`MES_SYSTEM`은 총 10개 테이블로 구성했다.

| Table | 역할 |
|---|---|
| `users` | 사용자 / 권한 |
| `work_orders` | 작업지시 |
| `lots` | LOT |
| `machines` | 설비 마스터 |
| `locations` | 공정 위치 마스터 |
| `products` | 제품 1EA |
| `process_events` | 제품 이동 / 공정 이력 |
| `quality_results` | PASS / FAIL 품질 결과 |
| `machine_status_history` | 설비 상태 이력 |
| `alarms` | Alarm 발생 / 해제 이력 |

---

### 4.2 Stored Procedure

핵심 Stored Procedure는 3개다.

- `usp_CreateWorkOrder`
- `usp_MoveProduct`
- `usp_RecordQualityResult`

---

### 4.3 WorkOrder → LOT → Product

작업지시 생성 시 LOT과 Product를 사전 생성한다.

```text
WorkOrder 1개
↓
LOT 5개
↓
LOT당 Product 60개
↓
Product 총 300개
```

Digital Twin에 실제 Barcode가 없더라도 MES에서 ProductCode를 미리 발급하여 제품 1EA Traceability를 가능하게 했다.

---

### 4.4 상태 모델

**WorkOrder**

- `IDLE`
- `RUNNING`
- `PAUSE`
- `COMPLETED`

**LOT**

- `WAITING`
- `RUNNING`
- `COMPLETED`

**Product**

- `WAITING`
- `IN_PROCESS`
- `COMPLETED`

**Quality**

- `PENDING`
- `PASS`
- `FAIL`

---

### 4.5 LOT / WorkOrder 완료 규칙

DB 로직은 아래 규칙으로 설계했다.

```text
PASS + FAIL = 60
↓
현재 LOT COMPLETED
↓
다음 LOT RUNNING
```

전체 LOT 완료:

```text
LOT #1 ~ #5 COMPLETED
↓
WorkOrder COMPLETED
```

> 이 로직은 DB/Stored Procedure 수준에서 구현되어 있다.  
> 실제 Digital Twin을 60EA / 300EA까지 장시간 연속 실행하는 최종 재검증만 남아 있다.

---

### 4.6 중복 및 데이터 정합성

MQTT 재전송, HTTP Retry, Gateway 재시작을 고려하여 다음을 적용했다.

- Quality `eventId` 중복 방지
- Process Event 중복 방지
- 동일 위치 중복 이동 방지
- 품질 완료 Product 재사용 방지
- Gateway 재시작 시 `IN_PROCESS` Product 복구
- Traceability 데이터는 누적 이력으로 유지

---

## 5. 설비 / Location 모델

### 5.1 MES 핵심 Machine

독립 KPI 대상 Machine은 다음 세 개로 제한했다.

- `VM1`
- `PRESS`
- `QUALITY_CHECK`

다음 요소는 Location / Process 단계로 관리한다.

- FEEDER
- CONVEYOR
- ROBOT
- CONVEYOR_2
- SRX_III
- WAREHOUSE
- NG_STACK

---

### 5.2 핵심 Traceability 범위

MES에서는 실제 Digital Twin 전체 기구 동작보다 다음 흐름을 핵심으로 추적한다.

```text
VM1
↓
PRESS
↓
QUALITY_CHECK
├─ PASS → WAREHOUSE
└─ FAIL → NG_STACK
```

---

## 6. AWS RDS 초기 구축

초기에는 **Amazon RDS for SQL Server Express**를 사용했다.

완료한 항목:

- [x] RDS SQL Server Express 생성
- [x] 원격 SSMS 연결
- [x] `MES_SYSTEM`
- [x] Table 10개
- [x] Index
- [x] Stored Procedure 3개
- [x] Machine / Location 초기 데이터
- [x] Gateway → Server → AWS DB 저장 검증

이후 개발 비용과 로컬 개발 편의를 위해 개발 DB를 **Local MSSQL**로 전환했다.

현재 구조:

```text
Mes.Gateway
↓ HTTP
Mes.Server
↓
Local MSSQL MES_SYSTEM
```

DB 위치가 Local인지 RDS인지 Gateway가 알 필요 없도록 아키텍처를 유지했다.

---

## 7. Mes.Server 구축

### 7.1 기본 구조

ASP.NET Core Web API 기반 `Mes.Server`를 구축했다.

주요 구성:

- Web API
- Swagger
- `MesDbConnectionFactory`
- User Secrets
- Repository
- Service
- Controller

DB Health 확인 결과:

- Database = `MES_SYSTEM`
- Tables = `10/10`
- Stored Procedures = `3/3`

---

### 7.2 Repository

구현 완료:

- `MachineStatusRepository`
- `QualityResultRepository`
- `WorkOrderRepository`
- `LotRepository`
- `ProductRepository`
- `ProcessRepository`
- `AlarmRepository`
- `ProductionRepository`

---

### 7.3 Service

구현한 주요 Service:

- `WorkOrderService`
- `AlarmService`
- `MachineStatusService`
- `QualityService`
- `ProductionService`

Controller에서 직접 SQL을 실행하지 않고 Repository / Service 계층을 분리했다.

---

## 8. 주요 Server API

### 8.1 Database

- `GET /api/database/health`

### 8.2 WorkOrder

- `POST /api/work-orders`
- `GET /api/work-orders/{workOrderCode}`
- `POST /api/work-orders/{workOrderCode}/start`
- `POST /api/work-orders/{workOrderCode}/pause`
- `POST /api/work-orders/{workOrderCode}/resume`

### 8.3 LOT

- `GET /api/lots/{lotCode}`
- `GET /api/lots/work-order/{workOrderCode}`
- `GET /api/lots/work-order/{workOrderCode}/running`

### 8.4 Product

- `GET /api/products/process/current`
- `GET /api/products/quality/current`
- `GET /api/products/{productCode}`
- `GET /api/products/lot/{lotCode}`
- `GET /api/products/lot/{lotCode}/next-waiting`

### 8.5 Process

- `POST /api/process/move`
- `GET /api/process/products/{productCode}/history`
- `GET /api/process/recent`

### 8.6 Quality

- `POST /api/quality-results`
- `GET /api/quality-results/{eventId}`

### 8.7 Machine Status

- `POST /api/machine-status`
- `GET /api/machine-status/{machineCode}/latest`

### 8.8 Alarm

- `POST /api/alarms`
- `GET /api/alarms/active`
- `GET /api/alarms/machine/{machineCode}`
- `POST /api/alarms/{alarmId}/clear`

### 8.9 Production

- `GET /api/production/current`

### 8.10 PLC Control

- `POST /api/plc-control/start`
- `POST /api/plc-control/stop`
- `POST /api/plc-control/reset`

---

## 9. Mes.Gateway 구축

### 9.1 구조

C# / .NET 8 기반으로 구성했다.

- Models
- Mqtt
- Routing
- Services
- Plc
- Api

사용 기술:

- .NET 8
- MQTTnet
- HttpClient
- SignalR Client
- MX Component V5

Gateway는 Mosquitto의 `dt/#`를 Subscribe한다.

---

### 9.2 Gateway 역할

- Gemini MQTT 수신
- Machine Status 전달
- Process Event 전달
- Quality Result 전달
- Active Product Context 관리
- PLC Device Read / Write
- D110 / D111 Polling
- SignalR PLC Command 수신
- Mes.Server API 호출
- Retry / Timeout
- 실패 이벤트 기록

---

## 10. Gemini Digital Twin 초기 연동

### 10.1 실제 구성 확인

확인한 Digital Twin 주요 요소:

- Feeder
- Conveyor
- RV-7FRLL-SH
- VM-1
- Press Machine
- Conveyor #2
- SRX III
- Warehouse Process Shelf
- NG Floorspace Stack

---

### 10.2 MQTT Topic

최종 Topic:

- `dt/vm1/status`
- `dt/press/status`
- `dt/quality/result`
- `dt/process/event`

Gateway:

- `dt/#` Subscribe

---

### 10.3 Process Event

주요 정보:

- `eventId`
- `timestamp`
- `toLocationCode`
- `eventType`
- `remarks`

이를 MES Product와 연결하여 `process_events`에 저장한다.

---

## 11. MQTT → Server Routing

### 11.1 Machine Status

`dt/vm1/status`, `dt/press/status`

→ `MachineStatusService`  
→ Mes.Server  
→ `machine_status_history`

---

### 11.2 Process Event

`dt/process/event`

→ `ActiveProductService`  
→ 현재 MES Product 결정  
→ Mes.Server  
→ `usp_MoveProduct`  
→ `process_events` + 현재 위치 갱신

---

### 11.3 Quality Result

`dt/quality/result`

→ Quality 처리  
→ Mes.Server  
→ `quality_results`

이후:

- PASS → WAREHOUSE Route
- FAIL → NG_STACK Route

를 추가 기록한다.

---

## 12. Active Product Context

Gemini 자체에는 MES ProductCode가 존재하지 않는다.

따라서 Gateway가 Digital Twin 이벤트와 실제 MES Product를 연결한다.

```text
첫 Process Event
↓
다음 WAITING Product 확보
↓
Active Product Context
↓
VM1
↓
PRESS
↓
Quality
↓
Product 완료
↓
Context 해제
```

Gateway가 재시작되어도 Server에서 `IN_PROCESS` Product를 다시 조회해 Context를 복구할 수 있도록 구현했다.

---

## 13. Gateway ↔ Server 통신 안정성

다음 장애 상황까지 테스트했다.

- [x] 요청별 5초 Timeout
- [x] Retry 정책
- [x] HTTP 5xx Retry
- [x] Timeout 장애 테스트
- [x] 응답 유실 테스트
- [x] 중복 EventId 테스트
- [x] 실패 이벤트 JSONL 저장
- [x] Quality Event 멱등성
- [x] Process Event 멱등성
- [x] Gateway 재시작 Product 복구

목표는 통신 실패나 중복 이벤트 때문에 **Produced / Good / Defect가 이중 반영되지 않도록 하는 것**이었다.

---

## 14. WorkOrder 실제 생성 검증

테스트 WorkOrder:

- `WO-20261006-001`
- Target = 300 EA
- LOT = 5개
- Product = 300개

작업지시 생성 시 LOT과 Product가 자동 생성되는 것을 확인했다.

WorkOrder START:

- WorkOrder → `RUNNING`
- 첫 LOT → `RUNNING`

---

## 15. 생산실적 DB 기준 전환

초기에는 Gateway에서 일부 실적을 계산했지만 최종적으로 DB/Server를 기준으로 통일했다.

현재 흐름:

```text
MSSQL
↓
Mes.Server Production API
↓
Gateway / HMI
```

관리 항목:

- Target
- Produced
- Good
- Defect
- Achievement
- Yield
- 현재 LOT Progress
- WorkOrder Progress

Gateway를 재시작해도 DB 기준으로 생산실적을 복구할 수 있다.

---

## 16. PLC / MX Component PoC

### 16.1 초기 PoC

검증:

- [x] GX Works2
- [x] GX Simulator2
- [x] Q03UDECPU
- [x] Logical Station 1
- [x] `GetDevice`
- [x] `SetDevice`
- [x] C# ↔ PLC 양방향 통신
- [x] MX Component V5
- [x] `ActUtlType64Lib`
- [x] x64 COM Reference
- [x] `.NET 8 → MX Component → GX Simulator2`

초기에 `.NET Framework 4.8 Mes.PlcBridge`를 고려했지만 **.NET 8에서 MX Component V5 직접 통신이 가능**하여 별도 Bridge는 사용하지 않았다.

---

### 16.2 Device Map 발전 과정

초기 PoC:

```text
M100 = START
M101 = STOP
M102 = RESET
D110 = Machine State
D111 = Alarm Code
```

최종 Simulation / Demo Map:

```text
M0 = START
M1 = STOP
M2 = RESET
M3 = RUN_STATE
D110 = Machine State
D111 = Alarm Code
```

현재 문서와 코드에서는 **M0 / M1 / M2 / M3 / D110 / D111**을 기준으로 한다.

---

## 17. PLC 제어 구조

최종 제어 경로:

```text
WinForms HMI
↓ HTTP
Mes.Server
↓ SignalR
Mes.Gateway
↓ PlcControlService
↓ MX Component
PLC
↓ Signal Relay
MELSOFT Gemini
```

검증 완료:

- [x] START
- [x] STOP
- [x] RESET
- [x] RUN_STATE
- [x] D110 Polling
- [x] D111 Polling

---

## 18. PLC Polling

`PlcPollingService`는 다음 방식이다.

- `Task`
- `CancellationToken`
- 1초 Polling

대상:

- D110 → Machine State
- D111 → Alarm Code

상태가 바뀔 때만 의미 있는 로그를 출력한다.

---

## 19. SignalR 사용 범위

초기에는 SignalR을 두 용도로 고려했다.

1. Server → Gateway PLC 제어
2. Server → HMI MES 데이터 Push

최종적으로는 **PLC 제어 채널만 SignalR 사용**.

MES 화면 데이터는 HMI의 1초 Polling으로 충분한 실시간성을 확보했기 때문에 데이터 Push는 구현하지 않았다.

---

## 20. MQTT 성능 문제

초기 방식:

```text
subprocess.run(mosquitto_pub.exe)
```

메시지마다 새 Process를 실행하면서 다음 작업을 반복했다.

Process 시작 → Broker 연결 → Publish → ACK → 종료

측정 결과:

- `dt/process/event` ≈ 5645 ms
- `dt/vm1/status` ≈ 5504 ms

VM1 한 Cycle에서 MQTT 전송만 약 11초가 소요되었다.

그 결과:

- Gemini / Mitsubishi FA 통신 지연
- Connectivity Error
- Simulation Pause

문제가 발생했다.

---

## 21. MQTT Persistent Publisher 최적화

`subprocess.run()`을 제거하고 `mosquitto_pub -l` 기반 Persistent Publisher를 적용했다.

Publisher를 Topic별로 한 번 실행하고 이후 JSON만 stdin으로 전달한다.

**성능 변화**

| 구분 | 시간 |
|---|---:|
| 기존 | 약 5,500 ms |
| 개선 후 | 약 0.01 ~ 0.05 ms |

개선 결과:

- [x] MQTT Blocking 제거
- [x] Simulation Pause 제거
- [x] Connectivity Error 감소
- [x] 장시간 생산 가능
- [x] MES 이벤트 실시간 반영

---

## 22. Persistent Publisher 종료 처리

Persistent Process 적용 후 발생한:

- `ResourceWarning`
- `subprocess is still running`
- `unclosed file`

문제를 해결했다.

Simulation STOP 시:

stdin close → wait → 필요 시 terminate → 필요 시 kill

순으로 Process를 종료한다.

Zombie `mosquitto_pub.exe`가 남지 않도록 정리했다.

---

## 23. Gemini Feeder ↔ PLC RUN_STATE

Gemini Runtime과 생산 Run 상태를 분리했다.

- Gemini Play = Simulation Runtime ON
- PLC RUN_STATE = 실제 생산 허용 여부

Feeder:

- RUN_STATE = FALSE → Output Disconnect → 신규 제품 생성 중단
- RUN_STATE = TRUE → Output Connect → 생산 재개

STOP은 현재 **Controlled Stop** 방식이다.

> 신규 Product / 신규 Cycle 진입은 막고, 이미 처리 중인 Product는 현재 Cycle을 완료한다.

Emergency Motion Freeze 방식과는 구분한다.

---

## 24. Product 없이 설비가 동작하던 문제

기존 ProcessExecutor가 제품 존재 여부와 무관하게 반복 실행될 가능성이 있었다.

이를 `ReserveProduct` 기반으로 변경했다.

```text
ReserveProduct
↓
TransportIn
↓
Process
```

실제 Product가 확보되어야 공정이 시작된다.

추가로 `PartsCurrent <= 0`인 경우 가짜 Process Event와 Machine Status MQTT를 전송하지 않도록 방어했다.

---

## 25. VM1 Status 구조 개선

기존에는 공정 후반에서 Machine Status를 보내 `Blocked` 상태 위주로 저장되는 문제가 있었다.

최종 구조:

```text
ReserveProduct
↓
TransportIn
↓
Process Event MQTT
↓
Busy + Status MQTT
↓
VM1 Process
↓
Blocked + Status MQTT
↓
TransportOut
↓
Idle + Status MQTT
```

이제 실제 `Busy → Blocked → Idle` 전환을 HMI에서 확인할 수 있다.

---

## 26. PRESS MQTT 구조 개선

기존 하나의 Script에서 처리하던 기능을 분리했다.

- `Press Process Event MQTT`
- `Press Status MQTT`
- `Quality Result MQTT`

최종 흐름:

```text
ReserveProduct
↓
TransportIn
↓
Process Event
↓
Busy
↓
Press Process
↓
Quality Result
↓
Blocked
↓
TransportOut
↓
Idle
```

기존 Failure Logic과 OK / NG Count는 유지했다.

---

## 27. PASS / FAIL 비율 수정

초기 조건 때문에 PASS / FAIL 비율이 반대로 설정되어 있었다.

최종:

- 제품 유형 #2 → PASS 약 70%
- 제품 유형 #1 → FAIL 약 30%

---

## 28. Gemini Physical Flow와 MES Traceability 분리

업체 제공 Gemini 내부 Warehouse / NG Stack의:

- ProductMatcher
- ProcessBuffer
- TransportItem

정합성 문제가 확인됐다.

제품은 물리적으로 도착하지만 Matcher에서 정상적으로 찾지 못하는 경우가 있었다.

ProductMatcher 방식도 테스트했지만 최종 공정에서 `No next process found` 오류가 발생했다.

프로젝트 범위를 고려해 책임을 분리했다.

- Gemini → Physical Flow
- Gateway / Mes.Server → Logical MES Traceability

따라서 MES에서는 Quality Result를 기준으로:

- PASS → WAREHOUSE
- FAIL → NG_STACK

Route를 별도로 기록한다.

---

## 29. PASS End-to-End 검증

실제 Product 기준:

```text
WAITING
↓
VM1
↓
IN_PROCESS
↓
PRESS
↓
PASS
↓
COMPLETED
↓
WAREHOUSE
```

검증 완료.

결과:

- Produced +1
- Good +1
- Yield 재계산
- LOT 실적 갱신
- WorkOrder 실적 갱신
- HMI 반영

---

## 30. FAIL End-to-End 검증

실제 FAIL 흐름:

```text
Gemini 제품 유형 #1
↓
dt/quality/result = FAIL
↓
Mes.Gateway
↓
Mes.Server
↓
quality_results
↓
Product COMPLETED
↓
QualityStatus = FAIL
↓
CurrentLocation = NG_STACK
↓
Produced +1
↓
Defect +1
↓
Yield 감소
↓
HMI 반영
```

확인 완료:

- [x] FAIL MQTT
- [x] Server 저장
- [x] `quality_results = FAIL`
- [x] Product COMPLETED
- [x] NG_STACK
- [x] Defect 증가
- [x] Produced 증가
- [x] Yield 감소
- [x] HMI FAIL 증가
- [x] `PRESS → NG_STACK / ROUTE`

---

## 31. Alarm End-to-End 검증

PLC Alarm:

```text
D111
↓
PlcPollingService
↓
AlarmService
↓
Mes.Server
↓
alarms
↓
HMI
```

테스트:

- D111 = 1 → Alarm ACTIVE → HMI 1건
- D111 = 0 → Alarm CLEAR → HMI 0건

검증:

- [x] Alarm INSERT
- [x] `is_active = 1`
- [x] HMI 표시
- [x] `cleared_at`
- [x] `is_active = 0`
- [x] HMI Clear

---

## 32. WinForms HMI 구축

2026-09-27 시점에는 HMI가 아직 구현되지 않은 상태였다.

이후 `Mes.Hmi` 프로젝트를 구성했다.

- Api
- Models
- Services
- Controls
- Forms

최종 Dashboard 구성:

```text
현재 작업지시 | 현재 LOT | 공정 추적 제품 | 생산실적
---------------------------------------------------
설비 상태                  | 공정 흐름
---------------------------------------------------
활성 알람                  | 최근 공정 이벤트
---------------------------------------------------
START          STOP          RESET
```

---

## 33. HMI Backend 연동

사용 API:

- `/api/production/current`
- `/api/products/process/current`
- `/api/alarms/active`
- `/api/machine-status/{machineCode}/latest`
- `/api/process/recent`
- `/api/plc-control/start`
- `/api/plc-control/stop`
- `/api/plc-control/reset`

HMI가 PLC나 Gateway에 직접 연결되지 않는 구조를 유지했다.

---

## 34. HMI 실시간 Polling

초기에는 Form Load 시 API를 한 번만 조회했다.

최종 방식:

- Task
- CancellationToken
- `Task.Delay(1000)`

1초마다 다음 API를 병렬 조회한다.

- Production
- Current Product
- Active Alarms
- VM1 / PRESS Machine Status
- Recent Process Events

WinForms Timer는 사용하지 않았다.

---

## 35. HMI 기능 안정화

완료:

- [x] Current Product가 없을 때 이전 데이터 제거
- [x] WorkOrder 없을 때 초기화
- [x] LOT 없을 때 초기화
- [x] Server 일시 실패 시 HMI 유지
- [x] Polling 예외 안정화
- [x] Grid 불필요한 Rebind 방지
- [x] Flicker 감소
- [x] Scroll / Selection 유지
- [x] START / STOP / RESET 중복 클릭 방지
- [x] Form 종료 CancellationToken
- [x] SignalR Infinite Retry
- [x] F11 Fullscreen
- [x] ESC Fullscreen 해제

---

## 36. Pipeline-aware HMI

초기 방식은 하나의 Current Product를 기준으로 전체 공정 UI를 제어했다.

하지만 실제 공정에서는:

```text
제품 B → VM1
제품 A → PRESS
```

처럼 서로 다른 제품을 동시에 처리할 수 있다.

따라서 최종적으로 다음처럼 분리했다.

- VM1 UI → VM1 Machine Status
- PRESS UI → PRESS Machine Status
- QUALITY CHECK → PRESS Product + Quality Route
- 상단 `공정 추적 제품` → Current IN_PROCESS Product

이 변경으로 실제 Pipeline 생산 상태와 HMI 표현을 일치시켰다.

---

## 37. HMI 상태 표현

**Machine State**

- IDLE → 대기
- BUSY → 진행 중
- BLOCKED → 공정 완료 / 배출 대기
- FAILED → 이상
- REPAIR → 수리 중

**Quality**

- 검사 전 → 검사 대기
- PASS → 양품 → 보관
- FAIL → 불량 → 적재

**Route Card**

- PASS → 양품
- WAREHOUSE → 양품 보관
- FAIL → 불량
- NG_STACK → 불량품 적재

---

## 38. HMI Session 초기화

DB에는 이전 실행의 마지막 Machine Status가 남을 수 있다.

이를 해결하기 위해 `_plcSessionStarted` 개념을 추가했다.

- HMI 실행 → VM1 / PRESS / QUALITY 대기
- START → 실제 Machine Status 표시
- RESET → 대기 상태 복귀

---

## 39. HMI 구형 코드 제거

기존 단일 Product 중심 Process Flow 코드 제거:

- `UpdateProcessFlow()`
- `ResetProcessFlowUi()`
- `SetProcessRunning()`
- `SetProcessCompleted()`

현재 사용:

- `UpdateMachineProcessState()`
- `RefreshQualityProcessUi()`
- `SetProcessPass()`
- `SetProcessFail()`
- `SetQualityRouteCard()`

---

## 40. HMI UI Polishing

기존 Control의 `Name`, `Text`, Event, Data Binding은 유지하고 UI만 개선했다.

개선:

- Summary Card
- Machine Card
- Process Flow
- Quality Route
- LOT ProgressBar
- Alarm Grid
- Recent Event Grid
- START / STOP / RESET
- 1920×1080 기준 배치

`Krypton.Toolkit`을 적용하여 전체 스타일을 통일했다.

적용 중 발생한 `PaletteDataGridViewTripleStates.StateCommon` API 문제를 수정했다.

또 `MainForm.Theme.resx` 때문에 발생한 중복 Resource 오류를 해결했다.

최종 파일 구조:

- `MainForm.cs`
- `MainForm.Designer.cs`
- `MainForm.resx`
- `MainForm.Theme.cs`

---

## 41. 최종 HMI 대표 화면

현재 대표 Screenshot에서 확인되는 상태 예:

| 항목 | 예시 |
|---|---|
| WorkOrder | `WO-20261006-001` |
| WorkOrder 상태 | RUNNING |
| Target | 300 EA |
| Produced | 112 EA |
| PASS | 99 EA |
| FAIL | 13 EA |
| Yield | 88.39% |
| LOT | 52 / 60 EA |
| Product | IN_PROCESS |
| VM1 | Blocked |
| PRESS | Busy |
| QUALITY | 검사 대기 |
| Recent Events | 표시 정상 |

이 화면은 포트폴리오 대표 HMI 이미지로 사용할 수 있다.

---

## 42. 초기 기획 대비 최종 결과

| 초기 기획 항목 | 최종 상태 |
|---|---:|
| Digital Twin 연동 | ✅ |
| PLC 제어 | ✅ |
| MQTT Gateway | ✅ |
| WorkOrder / LOT / Product | ✅ |
| Product Traceability | ✅ |
| WIP / Current Location | ✅ |
| Machine Status | ✅ |
| Quality PASS / FAIL | ✅ |
| 생산실적 / Yield | ✅ |
| Alarm | ✅ |
| WinForms HMI | ✅ |
| START / STOP / RESET | ✅ |
| Pipeline 공정 표현 | ✅ |
| AWS RDS 초기 검증 | ✅ |
| Local MSSQL 전환 | ✅ |
| SignalR PLC Command | ✅ |
| MES Data SignalR Push | 제외 — Polling으로 대체 |
| Vue / Web MES | 제외 |
| OEE 전용 UI | 제외 |
| 실제 업체 PLC Device Map | 제외 — Demo Map 사용 |

---

## 43. 현재 시스템 End-to-End

```text
WorkOrder 생성
↓
LOT / Product 자동 생성
↓
WorkOrder START
↓
첫 LOT RUNNING
↓
HMI START
↓
Mes.Server
↓
SignalR
↓
Mes.Gateway
↓
PLC RUN_STATE
↓
Gemini 생산
↓
VM1
↓
PRESS
↓
Quality PASS / FAIL
↓
WAREHOUSE / NG_STACK
↓
Product COMPLETED
↓
LOT / WorkOrder 실적 갱신
↓
MSSQL
↓
HMI 1초 실시간 갱신
```

검증 상태:

- PASS E2E ✅
- FAIL E2E ✅
- Alarm ACTIVE ✅
- Alarm CLEAR ✅
- START ✅
- STOP ✅
- RESET ✅

---

## 44. 핵심 트러블슈팅

### 44.1 MQTT 5초 Blocking

**문제**

`subprocess.run()` 기반 Publish로 메시지 1건에 약 5.5초 소요.

**해결**

Persistent Publisher 적용.

**결과**

약 `5500 ms → 0.01 ~ 0.05 ms`

---

### 44.2 Product 없는 설비 동작

**문제**

Product가 없어도 ProcessExecutor가 반복될 가능성.

**해결**

`ReserveProduct → TransportIn → Process`

실제 Product가 존재할 때만 공정 시작.

---

### 44.3 Machine Status가 Blocked 중심으로 저장

**문제**

공정 마지막에만 MQTT를 보내 실제 상태 전환이 보이지 않음.

**해결**

Busy / Blocked / Idle 전환 시점별 Status MQTT.

---

### 44.4 Current Product 하나로 Pipeline 표현 불가

**문제**

VM1과 PRESS가 서로 다른 Product를 동시에 처리함.

**해결**

Current Product 중심 UI에서 Machine Status 중심 UI로 변경.

---

### 44.5 Gemini ProcessMatcher 정합성

**문제**

업체 Digital Twin 내부 Warehouse / NG Stack Process Modeling이 MES Traceability와 정확히 맞지 않음.

**해결**

- Gemini → Physical Flow
- MES → Logical Traceability

로 책임 분리.

---

### 44.6 Gateway 장애 / 중복 Event

고려한 상황:

- HTTP Timeout
- HTTP 5xx
- 응답 유실
- MQTT 중복
- Gateway 재시작

적용:

- Retry
- Timeout
- EventId Idempotency
- Active Product 복구
- 실패 JSONL

---

## 45. 현재 보류 / 제외 사항

### 45.1 Feeder Startup Warning

Simulation 시작 시:

`ProductCreatorNoOutputConnectionDefined`

Warning이 한 번 발생할 수 있다.

현재 RUN_STATE 활성화 후 실제 생산과 MES 통신에는 영향이 없어 보류했다.

---

### 45.2 Warehouse / NG Stack 내부 Modeling

업체 Digital Twin 내부 ProductMatcher / ProcessBuffer 문제는 현재 MES Traceability와 분리되어 있다.

핵심 MES 데이터에는 영향 없음.

---

### 45.3 실제 업체 PLC Device Map

현재는 Simulation / Demo Map을 사용한다.

최종 현재 Map:

- M0 = START
- M1 = STOP
- M2 = RESET
- M3 = RUN_STATE
- D110 = Machine State
- D111 = Alarm Code

실제 설비 연동 범위가 확정되면 실제 PLC 주소로 교체할 수 있다.

---

## 46. 마지막으로 검증할 항목

### 46.1 LOT 자동 전환

DB 로직은 구현 완료.

실제 Digital Twin에서 최종 확인할 흐름:

```text
LOT #1
PASS + FAIL = 60
↓
LOT #1 COMPLETED
↓
LOT #2 RUNNING
```

- [ ] 실제 60EA 연속 실행 검증

---

### 46.2 WorkOrder 완료

DB 로직은 구현 완료.

최종 확인:

```text
LOT #1 ~ #5 COMPLETED
↓
Produced = 300
↓
WorkOrder COMPLETED
```

- [ ] 실제 300EA 전체 실행 검증

---

### 46.3 장시간 연속 운전

확인 대상:

- MQTT Publisher
- Gateway
- SignalR
- HTTP
- Mes.Server
- MSSQL
- HMI Polling

- [ ] 장시간 Resource / Connection 문제 확인
- [ ] 중복 Event 여부 확인
- [ ] 최종 데이터 정합성 확인

---

## 47. 포트폴리오 핵심 포인트

이 프로젝트의 핵심은 단순 CRUD가 아니다.

> **Digital Twin + PLC + Gateway + Backend + Database + HMI를 실제 제조 데이터 흐름으로 연결한 것**

강조할 항목:

- OT ↔ IT Integration
- Mitsubishi PLC / MX Component
- MQTT 기반 Digital Twin Event
- WorkOrder / LOT / Product 모델링
- 제품 1EA Traceability
- Machine Status History
- PASS / FAIL Quality
- Yield / Production Result
- Alarm ACTIVE / CLEAR
- 작업자 HMI 실시간 Monitoring
- Pipeline-aware UI
- MQTT 성능 병목 해결
- Idempotency / Retry / 장애 복구
- Physical Flow와 Logical Traceability 분리

---

## 48. 포트폴리오 대표 설명

> **MELSOFT Gemini 기반 Digital Twin에서 발생하는 실제 생산 이벤트를 MQTT로 수집하고 Mitsubishi PLC의 운전·Alarm 상태를 MX Component로 연동하여, ASP.NET Core 기반 MES Server와 MSSQL에 WorkOrder·LOT·Product 단위 생산 이력을 기록하고 WinForms HMI에서 실시간 모니터링 및 설비 제어가 가능하도록 구현한 MES 시스템입니다.**

---

## 49. 포트폴리오 Screenshot

### 확보 완료

- [x] `01_HMI_전체_실시간_모니터링`

### 추가 확보 권장

- [ ] `02_Gemini_DigitalTwin_공정동작`
- [ ] `03_GXWorks2_PLC_Online_Monitor`
- [ ] `04_HMI_FAIL_NG_ROUTE`
- [ ] `05_HMI_Alarm_ACTIVE`
- [ ] `06_DB_Traceability_결과`
- [ ] `07_Swagger_MES_API`

---

## 50. 프로젝트 진행 흐름

```text
초기 아이디어
↓
요구사항 정의
↓
실제 제조 공정 분석
↓
공정 범위 단순화
↓
MES DB 설계
↓
AWS RDS 초기 검증
↓
Digital Twin 데이터 추출
↓
MQTT
↓
Mes.Gateway
↓
Mes.Server
↓
Local MSSQL 전환
↓
PLC / MX Component PoC
↓
SignalR PLC Command
↓
WorkOrder / LOT / Product
↓
WIP / Traceability
↓
PASS / FAIL
↓
Alarm
↓
Digital Twin E2E
↓
WinForms HMI
↓
HMI 안정화
↓
HMI UI 완성
↓
현재
최종 검증 / 문서화 / 포트폴리오
```

---

## 51. 현재 완료 현황

| 영역 | 상태 |
|---|---:|
| 기획 / 요구사항 | ✅ |
| Architecture | ✅ |
| DB 설계 | ✅ |
| AWS RDS 초기 검증 | ✅ |
| Local MSSQL | ✅ |
| Mes.Server | ✅ |
| Repository / Service | ✅ |
| Mes.Gateway | ✅ |
| MQTT | ✅ |
| MQTT 최적화 | ✅ |
| MX Component | ✅ |
| PLC PoC | ✅ |
| PLC Control | ✅ |
| PLC Polling | ✅ |
| SignalR PLC Command | ✅ |
| WorkOrder | ✅ |
| LOT | ✅ |
| Product | ✅ |
| WIP | ✅ |
| Traceability | ✅ |
| Machine Status | ✅ |
| Quality | ✅ |
| Production Result | ✅ |
| Alarm | ✅ |
| PASS E2E | ✅ |
| FAIL E2E | ✅ |
| Alarm E2E | ✅ |
| HMI | ✅ |
| HMI Polling | ✅ |
| HMI Pipeline 표현 | ✅ |
| HMI UI | ✅ |
| HMI 대표 Screenshot | ✅ |
| LOT 60EA 실제 전환 검증 | ⬜ |
| WorkOrder 300EA 실제 완료 검증 | ⬜ |
| 장시간 연속 운전 | ⬜ |
| 최종 시연 / 포트폴리오 | ⬜ |

---

## 52. 다음 작업

1. GitHub 최신 작업 Push
2. Gemini / PLC / FAIL / Alarm Screenshot 확보
3. LOT 60EA 자동 전환 실제 최종 검증
4. WorkOrder 300EA 완료 실제 최종 검증
5. 장시간 연속 운전
6. README.md 최종 정리
7. 최종 시연 시나리오 작성
8. 포트폴리오 정리

---

## 53. 현재 한 줄 요약

> **초기 공정 기획과 MES 데이터 모델 설계부터 Digital Twin·PLC·Gateway·Backend·DB·HMI까지 직접 연결하여 생산·품질·설비·Alarm 데이터를 End-to-End로 검증했으며, 현재 핵심 기능 개발을 완료하고 최종 검증 및 포트폴리오 단계에 진입했다.**
