# MES-System 코드 추적 및 협업용 가이드

이 문서는 제공받은 `MES_System_Code_Walkthrough(1).html`의 핵심 분석을 토대로, 주요 입력부터 실제 C# 함수와 SQL까지 찾아갈 수 있도록 정리한 공개용 요약입니다. 학습 자료는 특정 커밋의 정적 코드 분석이므로 이후 코드와 차이가 있을 수 있습니다.

## 시스템 구조

- HMI: WinForms 작업자 입력·결과 표시
- Server: ASP.NET Core API와 MSSQL 저장·조회
- Gateway: MQTT 수신과 MX Component 기반 PLC 통신
- 외부 구성: Mosquitto Broker, GX Simulator2, MELSOFT Gemini

[전체 아키텍처](images/system-architecture.svg)

## 경로 1: START 명령

```text
HMI 버튼 → HTTP POST → Server 제어 API → SignalR
→ Gateway 명령 수신 → PLC 제어 서비스 → M0 300ms 펄스 → PLC 시뮬레이터
```

| 단계 | 추적 파일 및 메서드 |
| --- | --- |
| HMI 클릭·작업지시 확인 | [MainForm.cs](../Mes.Hmi/Forms/MainForm.cs) — `Btn_Start_Click` |
| HTTP 요청 | [MesServerClient.cs](../Mes.Hmi/Api/MesServerClient.cs) — `StartPlcAsync` |
| 제어 명령 발행 | [PlcControlController.cs](../src/Mes.Server/Controllers/Control/PlcControlController.cs) — `SendCommandAsync` |
| SignalR 수신 | [GatewaySignalRService.cs](../src/Mes.Gateway/Services/GatewaySignalRService.cs) — `HandleCommandAsync` |
| PLC 신호 | [PlcControlService.cs](../src/Mes.Gateway/Services/PlcControlService.cs) — START=M0, STOP=M1, RESET=M2 |
| PLC 통신 | [MxComponentPlcClient.cs](../src/Mes.Gateway/Plc/MxComponentPlcClient.cs) — 전용 STA 스레드와 큐 |

HMI는 START 전에 작업지시와 LOT을 확인합니다. 서버 응답 `sent`는 전송 결과이지 PLC나 Gemini의 실제 동작 완료 ACK가 아닙니다.

## 경로 2: 생산 데이터 수집 및 저장

```text
Gemini → MQTT Broker → Gateway MQTT 수신 → 토픽 분기
→ 서버 API → Controller → Repository / SQL 프로시저 → MSSQL
```

| 단계 | 추적 파일 |
| --- | --- |
| MQTT 수신 | [MqttClientService.cs](../src/Mes.Gateway/Mqtt/MqttClientService.cs) |
| 상태·이동·검사 토픽 분기 | [MqttMessageRouter.cs](../src/Mes.Gateway/Routing/MqttMessageRouter.cs) |
| Gateway HTTP 요청 | [MesApiClient.cs](../src/Mes.Gateway/Api/MesApiClient.cs) |
| 품질 저장 입구 | [QualityResultController.cs](../src/Mes.Server/Controllers/Quality/QualityResultController.cs) |
| MSSQL 호출 | [QualityResultRepository.cs](../src/Mes.Server/Repositories/Quality/QualityResultRepository.cs) |
| 품질·LOT 트랜잭션 | [database 폴더](../database/) — `usp_RecordQualityResult` |

토픽은 `dt/vm1/status`, `dt/press/status`, `dt/process/event`, `dt/quality/result` 등입니다. 검사 API와 이후 제품 이동 API는 서로 다른 요청이므로 전체가 단일 DB 트랜잭션인 것은 아닙니다.

## 경로 3: 저장된 데이터의 화면 조회

```text
HMI polling → Server GET API → Repository → MSSQL → 응답 DTO → HMI UI
```

- [MainForm.cs](../Mes.Hmi/Forms/MainForm.cs) — `RunDashboardPollingAsync`, `LoadProductionAsync`, `LoadActiveAlarmsAsync`
- [ProductionController.cs](../src/Mes.Server/Controllers/Production/ProductionController.cs)
- [ProductionRepository.cs](../src/Mes.Server/Repositories/Production/ProductionRepository.cs)

생산·현재 제품·설비 상태·알람·이벤트를 비동기로 조회하고 완료 후 약 1초 기다립니다. HMI는 MQTT 이벤트마다 갱신하는 Push 방식이 아닌 API polling 방식입니다.

## 장애 공유 시 추적 순서

| 현상 | 확인할 경계 |
| --- | --- |
| START 동작 없음 | HMI 작업지시 확인 → HTTP → Server SignalR → Gateway → MX/PLC → Gemini |
| 검사 결과 DB 미반영 | MQTT topic, EventId → Router → 서버 API → Repository → DB |
| DB 저장됐으나 화면 미변경 | 서버 조회 API → DTO → HMI polling·화면 바인딩 |
| Gemini 정지 | 오류 시각 → Python 발행 호출 계측 → 발행 프로세스 재사용 → 동일 조건 재실행 |

다른 사람에게 공유할 때에는 **증상, 재현 조건, CommandId/EventId, 관련 코드 경로, 로그, 적용한 수정, 재시험 결과**를 함께 전달하도록 정리했습니다. 이는 앞으로도 재사용할 수 있는 협업용 형식이며 모든 항목을 과거에 동일한 양식으로 공유했다는 주장은 아닙니다.

## 검증 범위

이 문서는 코드 추적을 위한 자료입니다. 실제 Gemini 모델의 Python 전체 코드와 GX Works2 래더 전체는 저장소에 포함되지 않습니다. MQTT 수신부 부하 시험을 전체 MES 성능으로 해석하지 않고, Python 발행 호출 구간의 개선을 종단 간 MQTT 전송 시간 개선으로 해석하지 않습니다.

[요구사항·아키텍처 책임과 AI 활용 범위](project-ownership-and-communication.md) · [트러블슈팅 기록](../MES_Gemini_MQTT_통신최적화_트러블슈팅_2026-10-06.md)
