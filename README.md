# PLC 설비 제어 · 생산정보 수집 프로그램

**C# HMI에서 제어 명령을 보내고, 가상 설비의 데이터를 받아 생산 현황과 이력을 조회하는 프로그램입니다.**

C# · WinForms · ASP.NET Core · MSSQL · MQTT · SignalR · MX Component

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-hmi.jpg" alt="작업지시·생산 현황·설비 상태·알람을 표시하는 HMI" width="100%"/>


## 어떤 프로그램인가요?

화면의 버튼이 설비 동작으로 이어지고, 설비에서 나온 정보가 다시 화면에 표시되는 과정을 직접 연결해 보고 싶었습니다. WinForms 화면과 서버·DB를 구성한 뒤 PLC 시뮬레이터와 가상 설비를 연결했습니다.

| 구현 기능 | 처리 내용 |
| --- | --- |
| 설비 제어 | HMI의 시작·정지·초기화 요청을 PLC 시뮬레이터로 전달 |
| 데이터 수집 | MQTT로 설비 상태·제품 이동·검사 결과 수신 |
| 생산 이력 | 제품 이동과 검사 결과 저장, 품질·LOT 관련 변경의 트랜잭션 처리 |
| 화면 조회 | 작업지시·생산 수량·설비 상태·알람 표시 |

## 개발 범위와 진행 방식

**㈜퍼씨엠이 제공한 가상 생산설비 모델**을 활용한 개인 프로젝트입니다. Gateway·ASP.NET Core 서버·MSSQL·WinForms HMI와 연동을 구현했습니다.

필요한 기능과 데이터 처리 순서를 정리하고, AI를 활용해 설계와 코드를 작성했습니다. 생성된 코드는 호출 순서와 조건별 동작을 따라가며 검토하고, 화면·로그·DB의 결과를 확인했습니다.

## 통신 구조

| 구간 | 흐름 |
| --- | --- |
| 제어 명령 | HMI → 서버 API → SignalR → Gateway → MX Component → PLC 시뮬레이터 → 가상 설비 |
| 생산 데이터 | 가상 설비 → MQTT Broker → Gateway → 서버 API → DB |
| 화면 조회 | HMI → 서버 API → DB 결과 표시 |

MQTT로 생산 데이터를 수신하고, SignalR와 PLC 통신으로 제어 명령을 전달했습니다. 두 방향의 흐름을 구성하면서 수신·처리·저장의 역할을 나누는 방법을 공부했습니다.

[전체 구성도](docs/images/system-architecture.svg)

## 개발하면서 해결한 문제

### 통신 설정을 바꿔도 멈추던 가상 설비

Gemini가 공정 중 멈춰 처음에는 PLC 통신 갱신 주기를 의심했습니다. 설정을 바꿔도 문제가 반복돼 Python 메시지 전송 구간의 실행 시간을 확인했습니다.

메시지마다 `mosquitto_pub.exe`를 실행하고 종료를 기다리는 과정에서 대기가 발생했습니다. 발행 프로세스를 유지하고 표준입력으로 메시지를 전달하도록 바꿨습니다. 이 과정에서 통신 설정뿐 아니라 호출하는 프로그램의 실행 방식도 확인해야 한다는 점을 배웠습니다.

[코드와 측정 과정을 정리한 트러블슈팅](MES_Gemini_MQTT_통신최적화_트러블슈팅_2026-10-06.md)

변경 전후 측정의 종료 조건이 달라 개선 배수를 제시하지 않았습니다. 기록된 시간은 Python 호출 구간이며 Gateway 도착 시간과는 구분합니다.

### 검사 결과만 저장해서는 처리가 끝나지 않았습니다

검사 결과를 저장할 때 제품 품질 상태와 LOT 완료 판단도 함께 처리해야 했습니다. 관련 정보가 일부만 반영되지 않도록 저장 프로시저에서 하나의 트랜잭션으로 묶었습니다.

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-trace.jpg" alt="동일 제품 코드의 이동 기록과 검사 결과 조회" width="100%"/>


## 코드에서 확인할 부분

| 내용 | 코드 |
| --- | --- |
| MQTT 구독과 수신 | [MqttClientService](src/Mes.Gateway/Mqtt/MqttClientService.cs) |
| 메시지 종류에 따른 처리 분기 | [MqttMessageRouter](src/Mes.Gateway/Routing/MqttMessageRouter.cs) |
| 서버 제어 명령 수신 | [GatewaySignalRService](src/Mes.Gateway/Services/GatewaySignalRService.cs) |
| PLC 디바이스 제어 | [PlcControlService](src/Mes.Gateway/Services/PlcControlService.cs) |
| 테이블·저장 프로시저 | [database](database) |

제어 API의 `sent`는 서버가 명령을 보냈다는 뜻이므로, 실제 설비 동작과 PLC 상태를 함께 확인했습니다.

## 동작 확인

GX Simulator2와 MELSOFT Gemini에서 시작·정지·초기화, 동일 제품의 이동·검사 이력 저장, FAIL 경로, 알람 발생·표시·해제를 확인했습니다. 통신 장애 복구도 확인했습니다.

현재 확인 환경은 시뮬레이터입니다. **실물 생산설비 연동과 장시간 연속 운전은 추가로 검증할 항목**입니다.

<details>
<summary>가상 설비·알람 화면</summary>

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-digital-twin.jpg" alt="㈜퍼씨엠에서 제공한 가상 생산설비 모델" width="100%"/>

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-alarm.jpg" alt="HMI의 활성 알람 표시" width="100%"/>


</details>

## 실행 방법

Windows, .NET 8, Visual Studio, SQL Server, Mosquitto, GX Works2·GX Simulator2, MX Component, MELSOFT Gemini가 필요합니다. 가상 설비 모델·PLC 프로그램과 COM 연결 환경은 별도로 준비해야 합니다.

1. `database/`의 테이블·저장 프로시저와 서버의 `ConnectionStrings:MesDatabase`를 준비합니다.
2. Mosquitto, PLC 시뮬레이터, MX Component 연결을 준비합니다.
3. `MES_SYSTEM.sln`에서 서버 → Gateway → Gemini → HMI 순서로 실행합니다.
4. Gateway 로그·PLC 디바이스·DB 저장 결과를 확인합니다.

Gateway 서버 기본 주소는 `https://localhost:7075`이며 `MES_SERVER_BASE_URL`로 변경합니다. MQTT 기본값은 `127.0.0.1:1883`, 구독은 `dt/#`, MX Component 논리국 번호는 1입니다. HMI 주소와 COM 참조는 실행 환경에 맞춥니다.
