# 디지털 트윈 기반 PLC 설비 제어 프로그램

**HMI에서 설비에 명령을 보내고, 설비의 생산 데이터를 받아 DB와 화면으로 연결한 개인 프로젝트입니다.**

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-hmi.jpg" alt="작업지시·LOT·실적·설비 상태를 표시하는 WinForms HMI" width="100%"/>

[담당 범위](#담당-범위) · [개발 과정](#개발-과정) · [코드와 확인 범위](#코드와-확인-범위) · [실행 방법](#실행-방법)

## 담당 범위

| 구분 | 범위 |
| --- | --- |
| 외부 제공 | **㈜퍼씨엠이 가상 생산설비 모델 제공** |
| 내가 맡은 일 | 요구사항·처리 순서 정리, Gateway·서버·DB·HMI 구성과 연동, 코드 검토 및 동작 확인 |
| AI 활용 | 설계 검토와 코드 생성 지원. 생성된 코드의 호출 순서·조건을 확인하며 구현 |
| 확인 환경 | GX Simulator2·MELSOFT Gemini. 실제 생산설비 운영 검증과 구분 |

직접 모든 코드를 처음부터 작성했다는 의미의 개인 프로젝트는 아닙니다. 필요한 기능을 정하고, AI가 생성한 코드를 이해하며 연결한 뒤 화면·로그·DB로 결과를 확인하는 역할을 맡았습니다.

## 개발 과정

### 1. 설비에 명령을 보내는 것과 데이터를 받는 것을 나누기

처음 목표는 화면과 설비 사이에서 양방향으로 정보가 오가는 구조를 만드는 것이었습니다. 현재는 생산 데이터에 MQTT를 쓰고 제어 명령에는 SignalR와 PLC 통신을 사용합니다.

| 흐름 | 연결 |
| --- | --- |
| 제어 | HMI → 서버 API → SignalR → Gateway → MX Component → PLC 시뮬레이터 → 가상 설비 |
| 데이터 | 가상 설비 → MQTT → Gateway → 서버 API → DB |
| 조회 | HMI → 서버 API → DB 결과 표시 |

서버의 `sent` 응답만으로 설비 동작 완료를 판단할 수는 없습니다. 명령 전송, PLC 디바이스 값, 실제 시뮬레이션 동작을 구분해서 확인했습니다.

[시스템 구성도](docs/images/system-architecture.svg)

### 2. 통신 설정을 바꿔도 반복되던 지연

Gemini가 공정 중 멈춰 처음에는 PLC 통신 갱신 주기를 의심했습니다. 주기를 바꿔도 문제가 반복돼 Python의 메시지 전송 구간을 측정했습니다.

메시지마다 `mosquitto_pub.exe`를 실행하고 종료를 기다리는 구조에서 대기가 발생했습니다. 발행 프로세스를 유지하고 표준입력으로 메시지를 전달하도록 바꿨습니다. Broker를 재시작한 것이 아니라 **발행 클라이언트를 재사용한 것**입니다.

변경 전은 프로세스 종료까지, 변경 후는 표준입력 전달까지 측정한 값이므로 동일한 통신 지연 비교로 볼 수 없습니다. README에서는 개선 배수를 성과로 쓰지 않았습니다.

[코드·측정 구간·시행착오 기록](MES_Gemini_MQTT_통신최적화_트러블슈팅_2026-10-06.md)

### 3. 검사 결과 하나가 여러 상태를 바꾸는 문제

검사 결과뿐 아니라 제품 품질 상태와 LOT 완료 판단도 함께 처리해야 했습니다. 일부 정보만 반영되지 않도록 저장 프로시저에서 관련 변경을 하나의 트랜잭션으로 묶었습니다.

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-trace.jpg" alt="동일 제품의 이동 이력과 검사 결과를 조회한 기존 DB 확인 화면" width="100%"/>


## 코드와 확인 범위

| 설명할 처리 | 코드·근거 |
| --- | --- |
| MQTT 토픽 구독·수신 | [MqttClientService](src/Mes.Gateway/Mqtt/MqttClientService.cs) |
| 메시지 종류에 따른 분기 | [MqttMessageRouter](src/Mes.Gateway/Routing/MqttMessageRouter.cs) |
| 서버 제어 명령 수신 | [GatewaySignalRService](src/Mes.Gateway/Services/GatewaySignalRService.cs) |
| PLC 디바이스 제어 | [PlcControlService](src/Mes.Gateway/Services/PlcControlService.cs) |
| 테이블·품질 처리 | [SQL 스크립트](database) |

시뮬레이터에서 START·STOP·RESET, 동일 제품의 이동·검사 이력 저장, FAIL 경로, 알람 발생·표시·해제를 확인했습니다. 통신 장애 복구와 장시간 운전은 추가 검증 과제입니다. 위 화면은 기존 개발 당시 캡처입니다.

<details>
<summary>가상 설비와 알람 화면</summary>

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-digital-twin.jpg" alt="㈜퍼씨엠 제공 가상 생산설비 모델" width="100%"/>

<img src="https://raw.githubusercontent.com/EddieEom/EddieEom/main/assets/screenshots/mes-alarm.jpg" alt="HMI의 활성 알람 표시" width="100%"/>


</details>

## 실행 방법

Windows, .NET 8, Visual Studio, SQL Server, Mosquitto, GX Works2·GX Simulator2, MX Component, MELSOFT Gemini가 필요합니다. 가상 설비 모델·PLC 프로그램과 MX Component의 COM 연결 환경도 별도로 준비해야 합니다.

1. `database/`의 테이블·저장 프로시저와 서버의 `ConnectionStrings:MesDatabase`를 준비합니다.
2. Mosquitto, PLC 시뮬레이터, MX Component 연결을 준비합니다.
3. `MES_SYSTEM.sln`에서 서버 → Gateway → Gemini → HMI 순서로 실행합니다.
4. Gateway 로그·PLC 디바이스·DB 저장 결과를 함께 확인합니다.

Gateway 서버 기본 주소는 `https://localhost:7075`이며 `MES_SERVER_BASE_URL`로 변경합니다. MQTT 기본값은 `127.0.0.1:1883`, 구독은 `dt/#`, MX Component 논리국 번호는 1입니다. HMI 주소와 COM 참조는 설치 환경에 맞춥니다. DB는 초기 RDS 구성 이후 로컬 SQL Server로 전환했습니다.
