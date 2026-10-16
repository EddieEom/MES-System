# MES / Gemini MQTT 통신 최적화 및 트러블슈팅 기록

최초 작성일: 2026-10-06  
구현 현황 재검토: 2026-10-16  
대상 프로젝트: 디지털 트윈 연동 PLC 설비 제어·MES 생산관리 시스템  
가상 생산설비 모델 제공: **㈜퍼씨엠** (설비 모델은 외부 제공, Gateway·Server·DB·HMI 연동은 개발 범위)  
대상 구간: MELSOFT Gemini ↔ MQTT/Mosquitto ↔ Mes.Gateway ↔ Mes.Server ↔ SQL Server / HMI ↔ PLC 시뮬레이터

> **문서 읽는 법**: 3~17장은 2026-10-06 당시 문제 재현 및 수정 **과정의 기록**으로 보존했습니다. 그때의 오류·시도·시험 코드가 현재 모두 적용돼 있거나 끝까지 검증됐다는 뜻은 아닙니다. 1~2장 및 18~22장은 **2026-10-16 현재 공개된 GitHub 코드/README와 대조해 갱신**했습니다. MELSOFT Gemini 내부 Python 스크립트, PLC 래더, MX Component 설치 버전은 저장소만으로 완전 검증할 수 없습니다.

**현 단계 구분**
- **소스코드로 확인**: HMI의 1초 대기 기반 HTTP 조회, Gateway의 D110/D111 1초 대기 기반 폴링, HMI→Server→SignalR→Gateway→MX Component 제어, MQTT `dt/#` 수신, 작업지시/LOT/제품 생성 흐름.
- **기존 시연·테스트로 확인했다고 기록된 범위**: 제어 버튼, 개별 제품 PASS/FAIL 이동 이력, 설비 알람 발생·해제 및 HMI 반영.
- **미완료 또는 근거 부족**: Feeder 중단·재개 장시간 안정성, 60EA LOT 전환과 300EA 연속 생산 완료, `mosquitto_pub.exe` 종료 후 프로세스 0건 여부의 최신 재측정.

---

## 1. 현재 프로젝트 통신 구조

```text
[작업자 WinForms HMI]
        │
        ├─ HTTP API 제어 요청(POST /api/plc-control/start|stop|reset)
        │       ↓
        │   ASP.NET Core Mes.Server
        │       ↓ SignalR (PlcCommand)
        │   C# Mes.Gateway
        │       ↓ MX Component (COM, Logical Station 1)
        │   GX Simulator2 / PLC
        │       ↓ PLC 장치 상태/접점
        │   MELSOFT Gemini 가상 설비
        │
        └─ HTTP API 현황 조회 ← Mes.Server ← SQL Server

[MELSOFT Gemini 가상 설비]
        │ 설비 상태·제품 이동·품질 결과 MQTT 송신
        ↓
  Mosquitto Broker
        ↓
  Mes.Gateway (MQTTnet, dt/# 구독)
        ↓ HTTP API
  Mes.Server
        ↓
  SQL Server (MES_SYSTEM)
        ↓ HTTP 조회
  WinForms HMI
```

- `Mes.Gateway`가 PLC·MQTT 연결을 담당하며 SQL Server에 직접 접근하지 않습니다. DB 읽기/쓰기는 `Mes.Server`를 통해 처리합니다.
- PLC 제어 장치는 **M0 START, M1 STOP, M2 RESET**이고 Gateway에서 **300ms 펄스**로 명령을 전달합니다. **D110 운전 상태, D111 알람 코드**를 읽습니다. `M3=RUN_STATE`는 가상 설비 동작 연결을 설명하기 위한 PLC/Gemini 측 신호로서, 관련 래더·Gemini 설정은 이 GitHub 저장소만으로 확인되지 않습니다.
- 제어 API가 반환하는 `status: sent`는 **SignalR 명령 전송** 결과입니다. PLC 실행 완료에 대한 종단 간 확인 응답(ACK)으로 해석하면 안 됩니다. 실제 PLC 제어는 Gateway 로그, PLC 상태 및 시뮬레이터 동작을 함께 확인해야 합니다.
- **MX Component 버전 주의**: 저장소의 README/구 문서는 V5를 표기하지만, 이후 실제 사용 환경은 **V4**로 변경했다고 확인되었습니다. C#의 `ActUtlType64Lib` 참조만으로 설치된 제품 버전까지 확정할 수는 없습니다. 현재 버전을 명시하는 문서·사진은 실제 PC 설치 환경에서 재확인하세요.
- **DB 위치 주의**: 초기에 AWS RDS for SQL Server를 구축했고 이후 개발 DB를 **로컬 SQL Server**로 전환했습니다. README의 RDS 표기는 초기 환경을 반영하는 부분이 남아 있습니다.

---

## 2. Polling 구분 — 2026-10-16 코드 확인

Polling은 서로 다른 두 경로에서 구현돼 있습니다.

| 구간 | 구현 확인 | 코드 근거 및 동작 |
|---|---|---|
| **Gateway → PLC 시뮬레이터** | **구현** | `src/Mes.Gateway/Services/PlcPollingService.cs`: D110/D111 읽기 후 `Task.Delay(1000)` |
| **WinForms HMI → Mes.Server** | **구현** | `Mes.Hmi/Forms/MainForm.cs`: 생산·현재 제품·알람·설비 상태·최근 이벤트 API를 병렬 조회한 후 `Task.Delay(1000)` |

**정확한 주기 표현:** 두 작업 모두 **조회/처리가 끝난 다음 1초를 대기하는 반복 루프**입니다. 따라서 네트워크/API 응답 시간까지 포함한 화면 갱신 간격이 항상 정확히 1,000ms인 것은 아닙니다.

### Gateway PLC Polling 핵심

```csharp
while (!cancellationToken.IsCancellationRequested)
{
    await ReadAndProcessAsync(cancellationToken);
    await Task.Delay(1000, cancellationToken);
}
```

읽는 값:

```text
D110 = PLC 운전 상태
D111 = PLC Alarm Code (0: 정상, 0 초과: 알람)
```

현재 `PlcPollingService`는 `D111` 값을 MES ACTIVE 알람과 동기화합니다. `src/Mes.Gateway/Program.cs`에서 `machineCode: "PRESS"`로 연결돼 있으므로 PLC 알람 집계 대상의 범위를 참고해야 합니다.

### HMI Dashboard Polling 핵심

```csharp
Task productionTask = LoadProductionAsync();
Task productTask = LoadCurrentProductAsync();
Task alarmTask = LoadActiveAlarmsAsync();
Task machineTask = LoadMachineStatusAsync();
Task processEventTask = LoadRecentProcessEventsAsync();

await Task.WhenAll(
    productionTask, productTask, alarmTask, machineTask, processEventTask
);
await Task.Delay(1000, token);
```

따라서 옛 문서의 **"HMI → Server 미구현"**, **"HMI Dashboard Polling 구현 필요"** 설명은 현재 코드와 맞지 않습니다.

---

# 3. Feeder STOP 제어 문제

> **2026-10-16 검토**: 아래는 Feeder 중단·재개를 시험한 과거 이력입니다. 저장소에 Gemini 모델의 현재 Python 로직이 완전하게 포함돼 있지 않아 **해결 완료로 판단할 수 없습니다**. 특히 Connector 분리 방식은 `ProductCreatorNoOutputConnectionDefined` 오류를 동반했으므로 **최종 채택 설계로 소개하지 않습니다**. 장시간 STOP→START→RESET 재생성 안정성은 별도 시연 로그가 필요합니다.

## 최초 상태

Gemini Runtime을 실행하면 `ProductCreator`가 자동으로 원자재를 생성했다.

```text
Gemini ▶
 ↓
Feeder ProductCreator
 ↓
원자재 지속 생성
```

HMI STOP을 눌러도 Feeder가 계속 제품을 생성하는 문제가 있었다.

목표는 다음과 같다.

```text
RUN_STATE=True
→ Feeder 신규 원자재 생성 가능

RUN_STATE=False
→ Feeder 신규 원자재 생성 금지
```

---

## 3.1 SingleFeed IsEnabled 제어 시도

초기에는 다음 속성을 제어했다.

```python
creator.SingleFeed.getProperty('IsEnabled')
```

결과:

```text
RUN_STATE=False
→ IsEnabled=False
→ 생성 중지 성공

RUN_STATE=True
→ IsEnabled=True
→ 로그는 ON
→ 실제 Product 생성 재개 실패
```

즉 속성은 다시 `True`가 되었지만 ProductCreator 내부 생성 루틴이 재시작되지 않았다.

이 방식은 중단했다.

---

## 3.2 ProductCreator Output Connector 제어

Feeder Component Graph 확인 결과:

```text
ProductCreator
 ├─ Output
 └─ Input

Output
 ↓
Path / Input
```

`Output`은 Behaviour가 아니라 Connector였다.

따라서 다음 코드는 실패했다.

```python
comp.findBehaviour('Output')
```

결과:

```text
[FEEDER] ERROR: Output not found
```

이후 Connector로 접근하도록 수정했다.

```python
output_connector = creator.getConnector('Output')
path_input_connector = output_connector.Connection
```

STOP:

```python
output_connector.connect(None)
```

START:

```python
output_connector.connect(path_input_connector)
```

결과:

```text
STOP
→ RUN_STATE=False
→ OUTPUT DISCONNECTED
→ 신규 원자재 생성 중지 성공
```

다만 Output 연결을 실제로 끊으면 ProductCreator에서 다음 오류가 발생했다.

```text
Feeder::ProductCreator 오류:
ProductCreatorNoOutputConnectionDefined
```

따라서 **Feeder 신규 생성 차단 자체는 성공했지만 이 방식은 최종 구현으로 확정하지 않는다.**

향후 ProductCreator 연결 자체를 해제하지 않고 생성 Trigger 또는 안전한 Flow Gate 방식으로 정리할 필요가 있다.

---

# 4. Gemini가 정확히 48.5초에 멈추는 문제

Gemini 실행 중 다음 오류가 반복 발생했다.

```text
0:00:48.50

연결성경고:
MITSUBISHI ELECTRIC FA
커뮤니케이션 지연 경고 역치 300 ms 도달

연결성오류:
커뮤니케이션 지연 오류 역치 5000 ms 도달

마지막 5 작업 평균 커뮤니케이션 완료 시간:
약 5,300 ~ 5,568 ms

→ 시뮬레이션 일시 중지
```

초기에는 Mitsubishi FA 업데이트 주기 문제를 의심했다.

기존:

```text
업데이트 모드 = 주기적
주기 = 50 ms
```

테스트:

```text
50 ms → 1000 ms
```

그러나 **1000 ms에서도 정확히 48.5초에 동일 오류가 발생했다.**

따라서 단순 PLC 업데이트 주기 문제는 아니었다.

---

# 5. MQTT 코드가 실제 병목이었던 이유

VM1 Python Statement에서 MQTT 전송을 다음 방식으로 수행하고 있었다.

```python
result = subprocess.run(
    [
        MOSQUITTO_PUB,
        "-h", BROKER,
        "-p", PORT,
        "-t", topic,
        "-m", message,
        "-q", "1"
    ],
    capture_output=True,
    text=True
)
```

`subprocess.run()`은 `mosquitto_pub.exe` 프로세스가 종료될 때까지 현재 Python 실행 흐름을 기다린다.

그리고 VM1 한 사이클에서 두 번 호출했다.

```python
publish_mqtt(PROCESS_TOPIC, process_data)
publish_mqtt(VM1_STATUS_TOPIC, status_data)
```

실제 측정 결과:

```text
dt/process/event
5645.37 ms

dt/vm1/status
5504.06 ms
```

한 VM1 처리 시 총 Blocking 시간:

```text
5645.37 + 5504.06
= 11149.43 ms
≈ 11.15초
```

이 시점이 Gemini의 48.5초 공정 이벤트 발생 시점과 정확히 겹쳤다.

즉:

```text
VM1 Process 종료
 ↓
mosquitto_pub.exe 생성
 ↓
Broker 연결
 ↓
QoS 1 Publish
 ↓
ACK 대기
 ↓
프로세스 종료
 ↓
약 5.5초 Blocking

두 번째 Topic도 동일 반복
 ↓
총 약 11초 Blocking
```

이 Blocking 때문에 Gemini의 다른 Connectivity 작업도 밀리면서 Mitsubishi FA 5000ms 오류 임계치를 넘어간 것으로 판단했다.

---

# 6. MQTT Persistent Publisher 방식으로 최적화

기존의 매 전송마다 프로세스를 실행하는 구조:

```text
이벤트
→ mosquitto_pub.exe 생성
→ 연결
→ Publish
→ 종료
```

를 다음 구조로 변경했다.

```text
첫 전송
→ mosquitto_pub.exe -l 실행
→ Broker 연결 유지

이후
→ stdin.write(JSON)
→ flush()
→ 연결 재사용
```

핵심:

```python
subprocess.Popen(
    [
        MOSQUITTO_PUB,
        "-h", BROKER,
        "-p", PORT,
        "-t", topic,
        "-q", "1",
        "-l"
    ],
    stdin=subprocess.PIPE,
    stdout=subprocess.DEVNULL,
    stderr=subprocess.DEVNULL,
    text=True
)
```

전송:

```python
publisher.stdin.write(
    message + "\n"
)

publisher.stdin.flush()
```

---

# 7. 최적화 결과

## 기존

```text
dt/process/event
5645.37 ms

dt/vm1/status
5504.06 ms
```

## 최적화 후

```text
dt/process/event
3.84 ms

dt/vm1/status
1.88 ms
```

## 개선율

`dt/process/event`

```text
5645.37 / 3.84
≈ 1470배
```

`dt/vm1/status`

```text
5504.06 / 1.88
≈ 2928배
```

두 전송 합계:

```text
기존:
11149.43 ms

개선:
5.72 ms

약 1949배 개선
약 99.95% 지연 감소
```

따라서 MQTT 메시지 크기나 Broker 자체가 느린 것이 아니라, **매 Publish마다 `mosquitto_pub.exe`를 새로 실행하고 종료하던 구조가 병목**이었다.

### 후속 반복 전송 측정(별도 측정값, 사용자 확인)

- 초기: **약 5,000ms/전송 요청**(외부 프로세스 실행·종료 방식)
- 후속 반복 전송: **0.1~0.3ms/호출**(프로세스 유지·재사용)
- **측정 대상**: Python 송신 함수에서 요청 처리에 걸린 시간(`stdin.write`·`flush` 등을 포함하는 계측 구간). **브로커 도착 지연, MQTT 왕복시간, PLC 통신 전체 지연 또는 전체 생산 Cycle Time이 아닙니다.**
- **검증 근거 구분**: 위의 **3.84ms/1.88ms**는 이 문서에 보존된 당시 기록값이고, **0.1~0.3ms**는 이후 사용자가 확인한 별도 반복 측정값입니다. 같은 실행·로그 조건의 값이 아니므로 임의로 합산하거나 서로 대체하지 않습니다. 현재 GitHub에는 0.1~0.3ms 원본 측정 로그가 확인되지 않습니다.


---

# 8. 새로 발생한 ResourceWarning

Persistent Publisher 적용 후 다음 경고가 발생했다.

```text
ResourceWarning:
subprocess 35112 is still running

ResourceWarning:
unclosed file <_io.TextIOWrapper ...>

ResourceWarning:
subprocess 31556 is still running
```

이 메시지의 의미:

```text
Popen 객체가 정리됨
그런데 자식 mosquitto_pub.exe 프로세스는 아직 실행 중

또는

stdin=subprocess.PIPE 로 만든 파일 핸들이
close되지 않은 상태에서 객체가 정리됨
```

즉 Persistent Publisher의 성능 최적화는 성공했지만 **프로세스 Lifecycle 정리가 부족했다.**

---

# 9. 기존 orphan process 1회 정리

현재 로그에 나온 기존 PID:

```text
35112
31556
```

이 프로세스가 아직 작업 관리자에 존재한다면 **한 번만 종료**한다.

CMD 예:

```bat
taskkill /PID 35112 /F
taskkill /PID 31556 /F
```

주의:

```text
mosquitto.exe
```

는 Broker이므로 종료하지 않는다.

종료 대상은:

```text
mosquitto_pub.exe
```

이다.

---

# 10. ResourceWarning 방지 설계

> **상태 구분**: 이 장부터 17장까지는 당시 작성한 Gemini Python 스크립트의 **정리 설계 및 적용 예시**입니다. 현재 MES-System GitHub C# 소스만으로는 실제 Gemini 런타임에 동일한 코드가 탑재됐는지, STOP·RESET·재컴파일 후 orphan process가 0건인지 확정할 수 없습니다. 기존의 "최종" 표현은 **설계상 목표 상태**를 뜻하며 최신 재현 테스트가 필요합니다.

설계한 Persistent Publisher는 다음 Lifecycle을 가집니다.

```text
Script 실행
 ↓
이전 Script Instance가 남긴 Publisher가 있으면 정리
 ↓
첫 MQTT 발생
 ↓
mosquitto_pub.exe -l 생성
 ↓
Persistent 연결 유지

Gemini STOP / RESET
 ↓
stdin.close()
 ↓
wait()
 ↓
필요 시 terminate()
 ↓
그래도 안 죽으면 kill()
 ↓
wait()로 프로세스 회수

Script 재컴파일
 ↓
이전 Runtime Registry를 찾아 정리

Gemini/Python 종료
 ↓
atexit cleanup
```

또한 Windows 창은 생성하지 않는다.

```python
creationflags=subprocess.CREATE_NO_WINDOW
```

---

# 11. 공통 MQTT Lifecycle 코드

VM1과 PRESS 모두 아래 구조를 사용한다.

> `SCRIPT_KEY`만 각각 `"VM1_MQTT"`와 `"PRESS_MQTT"`로 다르게 사용한다.

```python
import subprocess
import time
import builtins
import atexit

CREATE_NO_WINDOW = getattr(
    subprocess,
    "CREATE_NO_WINDOW",
    0x08000000
)

REGISTRY_ATTR = "_MES_GEMINI_MQTT_RUNTIMES"
ATEXIT_FLAG = "_MES_GEMINI_MQTT_ATEXIT_REGISTERED"

runtime_registry = getattr(
    builtins,
    REGISTRY_ATTR,
    None
)

if runtime_registry is None:
    runtime_registry = {}
    setattr(
        builtins,
        REGISTRY_ATTR,
        runtime_registry
    )


publishers = {}


def close_publisher(topic, publisher):

    if publisher is None:
        return

    pid = getattr(
        publisher,
        "pid",
        None
    )

    try:

        # stdin EOF → mosquitto_pub -l 정상 종료 유도
        if publisher.stdin is not None:

            try:
                publisher.stdin.flush()
            except Exception:
                pass

            try:
                publisher.stdin.close()
            except Exception:
                pass

            publisher.stdin = None


        # stdin EOF 후 자연 종료 대기
        if publisher.poll() is None:

            try:

                publisher.wait(
                    timeout=0.5
                )

                print(
                    "[MQTT] Publisher 정상 종료 : %s / PID=%s"
                    % (
                        topic,
                        pid
                    )
                )

                return

            except subprocess.TimeoutExpired:
                pass


        # 아직 살아 있으면 terminate
        if publisher.poll() is None:

            try:

                publisher.terminate()

                publisher.wait(
                    timeout=1.0
                )

                print(
                    "[MQTT] Publisher terminate 완료 : %s / PID=%s"
                    % (
                        topic,
                        pid
                    )
                )

                return

            except subprocess.TimeoutExpired:
                pass

            except Exception:
                pass


        # 그래도 살아 있으면 kill
        if publisher.poll() is None:

            try:

                publisher.kill()

                publisher.wait(
                    timeout=1.0
                )

                print(
                    "[MQTT] Publisher kill 완료 : %s / PID=%s"
                    % (
                        topic,
                        pid
                    )
                )

            except Exception as e:

                print(
                    "[MQTT] Publisher kill 실패 : %s / %s"
                    % (
                        topic,
                        str(e)
                    )
                )


        # 이미 종료된 프로세스도 wait/poll로 회수
        else:

            try:
                publisher.wait(
                    timeout=0
                )
            except Exception:
                pass


    except Exception as e:

        print(
            "[MQTT] Publisher 종료 오류 : %s / %s"
            % (
                topic,
                str(e)
            )
        )


def cleanup_publishers(
    reason="UNKNOWN"
):

    if not publishers:
        return

    print(
        "[MQTT] Publisher 전체 정리 시작 / Reason=%s"
        % reason
    )

    current_publishers = list(
        publishers.items()
    )

    publishers.clear()

    for topic, publisher in current_publishers:

        close_publisher(
            topic,
            publisher
        )

    print(
        "[MQTT] Publisher 전체 정리 완료"
    )


def create_publisher(
    topic,
    mosquitto_pub,
    broker,
    port
):

    print(
        "[MQTT] Persistent Publisher 시작 :",
        topic
    )

    publisher = subprocess.Popen(
        [
            mosquitto_pub,
            "-h", broker,
            "-p", port,
            "-t", topic,
            "-q", "1",
            "-l"
        ],
        stdin=subprocess.PIPE,
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL,
        text=True,
        bufsize=1,
        creationflags=CREATE_NO_WINDOW
    )

    print(
        "[MQTT] Publisher 생성 : %s / PID=%s"
        % (
            topic,
            publisher.pid
        )
    )

    return publisher


def get_publisher(
    topic,
    mosquitto_pub,
    broker,
    port
):

    publisher = publishers.get(
        topic
    )

    if (
        publisher is None
        or publisher.poll() is not None
    ):

        old_publisher = publishers.pop(
            topic,
            None
        )

        if old_publisher is not None:

            close_publisher(
                topic,
                old_publisher
            )

        publisher = create_publisher(
            topic,
            mosquitto_pub,
            broker,
            port
        )

        publishers[topic] = publisher

    return publisher


def publish_mqtt(
    topic,
    data,
    mosquitto_pub,
    broker,
    port
):

    import json

    message = json.dumps(
        data,
        ensure_ascii=True
    )

    start_time = time.perf_counter()

    try:

        publisher = get_publisher(
            topic,
            mosquitto_pub,
            broker,
            port
        )

        if publisher.stdin is None:

            raise RuntimeError(
                "Publisher stdin is None"
            )

        publisher.stdin.write(
            message + "\n"
        )

        publisher.stdin.flush()

        elapsed_ms = (
            time.perf_counter()
            - start_time
        ) * 1000

        print(
            "[MQTT] 전송 요청 : %s / %.2f ms"
            % (
                topic,
                elapsed_ms
            )
        )

    except Exception as e:

        print(
            "[MQTT] 전송 오류 : %s / %s"
            % (
                topic,
                str(e)
            )
        )

        broken_publisher = publishers.pop(
            topic,
            None
        )

        close_publisher(
            topic,
            broken_publisher
        )
```

---

# 12. Script 재컴파일 시 orphan 방지

Process Python Statement를 수정 후 Compile하면 이전 Script Instance가 남긴 Publisher가 살아 있을 수 있다.

이를 막기 위해 `SCRIPT_KEY`별 Registry를 사용한다.

VM1:

```python
SCRIPT_KEY = "VM1_MQTT"
```

PRESS:

```python
SCRIPT_KEY = "PRESS_MQTT"
```

스크립트 초기화 시 이전 Runtime이 있으면 먼저 제거한다.

```python
previous_runtime = runtime_registry.pop(
    SCRIPT_KEY,
    None
)

if previous_runtime is not None:

    old_handler = previous_runtime.get(
        "handler"
    )

    if old_handler is not None:

        try:
            sim.OnStateChanged -= old_handler
        except Exception:
            pass


    old_cleanup = previous_runtime.get(
        "cleanup"
    )

    if old_cleanup is not None:

        try:
            old_cleanup(
                "SCRIPT_RELOAD"
            )

        except Exception as e:

            print(
                "[MQTT] 이전 Runtime 정리 오류 :",
                str(e)
            )
```

이 구조의 목적:

```text
코드 수정
→ Compile
→ 기존 Python Script 객체 교체
→ 이전 Popen 객체 방치
→ ResourceWarning
```

흐름을 차단하는 것이다.

---

# 13. Simulation STOP / RESET 정리

Visual Components/Gemini Simulation State를 감시한다.

```python
def on_simulation_state_changed(
    simulation,
    state
):

    if state == vc.vcSimulationState.STOP:

        cleanup_publishers(
            "SIMULATION_STOP"
        )

    elif state == vc.vcSimulationState.RESET:

        cleanup_publishers(
            "SIMULATION_RESET"
        )
```

등록:

```python
sim.OnStateChanged += (
    on_simulation_state_changed
)
```

Registry에도 현재 callback과 cleanup 함수를 저장한다.

```python
runtime_registry[
    SCRIPT_KEY
] = {
    "handler":
        on_simulation_state_changed,

    "cleanup":
        cleanup_publishers
}
```

---

# 14. Gemini/Python 종료 시 cleanup

앱 자체 종료 시에도 cleanup이 필요하다.

Interpreter 전체에서 한 번만 등록한다.

```python
if not getattr(
    builtins,
    ATEXIT_FLAG,
    False
):

    def cleanup_all_mqtt_runtime():

        registry = getattr(
            builtins,
            REGISTRY_ATTR,
            {}
        )

        for key, runtime in list(
            registry.items()
        ):

            cleanup = runtime.get(
                "cleanup"
            )

            if cleanup is not None:

                try:

                    cleanup(
                        "PYTHON_EXIT"
                    )

                except Exception:

                    pass


    atexit.register(
        cleanup_all_mqtt_runtime
    )

    setattr(
        builtins,
        ATEXIT_FLAG,
        True
    )
```

---

# 15. VM1 적용 포인트

VM1 설정:

```python
SCRIPT_KEY = "VM1_MQTT"

BROKER = "127.0.0.1"
PORT = "1883"

VM1_STATUS_TOPIC = "dt/vm1/status"
PROCESS_TOPIC = "dt/process/event"

MOSQUITTO_PUB = (
    r"C:\Program Files\Mosquitto\mosquitto_pub.exe"
)
```

Publish 호출:

```python
publish_mqtt(
    PROCESS_TOPIC,
    process_data,
    MOSQUITTO_PUB,
    BROKER,
    PORT
)

publish_mqtt(
    VM1_STATUS_TOPIC,
    status_data,
    MOSQUITTO_PUB,
    BROKER,
    PORT
)
```

---

# 16. PRESS 적용 포인트

PRESS 설정:

```python
SCRIPT_KEY = "PRESS_MQTT"

BROKER = "127.0.0.1"
PORT = "1883"

PRESS_TOPIC = "dt/press/status"
QUALITY_TOPIC = "dt/quality/result"
PROCESS_TOPIC = "dt/process/event"

MOSQUITTO_PUB = (
    r"C:\Program Files\Mosquitto\mosquitto_pub.exe"
)
```

Publish 호출:

```python
publish_mqtt(
    PROCESS_TOPIC,
    process_data,
    MOSQUITTO_PUB,
    BROKER,
    PORT
)

publish_mqtt(
    PRESS_TOPIC,
    press_data,
    MOSQUITTO_PUB,
    BROKER,
    PORT
)

if quality_data is not None:

    publish_mqtt(
        QUALITY_TOPIC,
        quality_data,
        MOSQUITTO_PUB,
        BROKER,
        PORT
    )
```

---

# 17. MQTT 프로세스 구분

## Broker

```text
mosquitto.exe
```

- MQTT Broker
- 계속 실행되어 있어야 함
- cleanup 대상 아님

## Publisher

```text
mosquitto_pub.exe
```

- Gemini Python에서 생성
- Persistent 연결용
- 화면에 창을 띄우지 않음
- Gemini STOP / RESET / Script Reload / Python Exit 시 정리

---

# 18. 테스트 체크리스트 — 코드 확인과 실행 검증 분리

GitHub에 구현 코드가 존재하는지와 **현재 사용 PC에서 재현 테스트에 통과했는지**는 다른 판단입니다.

| 항목 | 현재 판정 | 확인 방법 및 근거 |
|---|---|---|
| Gateway MQTT 구독 | 코드 확인 | `MqttClientService`에서 브로커 `127.0.0.1:1883` 연결, `dt/#` 구독 |
| HMI 현황 자동 조회 | 코드 확인 | `MainForm.RunDashboardPollingAsync`에서 5개 API 작업을 `Task.WhenAll`로 조회하고 1초 대기 |
| PLC D110/D111 조회 | 코드 확인 | `PlcPollingService`에서 반복 조회, 상태/알람 동기화 |
| START/STOP/RESET 명령 | 코드 확인·과거 시연 확인 | API→SignalR→Gateway→M0/M1/M2 300ms 펄스; API `sent`만으로 PLC 실행 확인 불가 |
| 작업지시·LOT·제품 초기 생성 | 코드 확인 | `WorkOrderRepository.CreateAsync`가 `usp_CreateWorkOrder` 호출, 1 WO→5 LOT→300 제품 흐름 |
| PASS/FAIL·이동 이력·알람 | 코드 확인·과거 시연 확인 | MQTT 라우터→API→DB 저장, HMI에 결과 표시; 개별 시나리오 검증 |
| MQTT 최적화 후 지연 | 과거 기록 및 후속 사용자 측정 | 기존 기록 3.84/1.88ms, 후속 반복 호출 0.1~0.3ms; 실행 조건을 명시해서 사용 |
| Gemini 90초 이상 장시간 유지 | 최신 로그 확인 필요 | 멈춤 없이 진행되는지, 48.5초 통신 오류 재발 여부 확인 |
| Publisher 종료 후 프로세스 정리 | 최신 로그 확인 필요 | Gemini STOP/RESET/재컴파일 뒤 `mosquitto_pub.exe` 잔존 개수와 ResourceWarning 확인 |
| Feeder STOP→START→RESET | **미완료/재검증** | 제품 생성·중단·재개와 ProductCreator 연결 오류 없이 이어지는지 확인 |
| 60EA LOT 전환, 300EA 전체 생산 | **미완료/재검증** | 개별 이벤트 확인과 달리 전체 연속 생산 완료 검증이 필요 |

## A. 성능·MQTT

```text
[ ] VM1 / PRESS 최초 Publish와 2회차 이후 반복 Publish 시간을 별도로 기록
[ ] Gemini dt/process/event, dt/vm1/status, dt/press/status, dt/quality/result 발행 확인
[ ] Gateway에서 dt/# MQTT 토픽 수신 확인
[ ] Server API 처리와 MES_SYSTEM 저장·조회 확인
[ ] Python 계측시간과 네트워크/PLC 통신시간을 혼동하지 않기
```

## B. PLC·HMI·생산이력

```text
[ ] HMI START·STOP·RESET 조작 → SignalR 명령 → Gateway 로그 → PLC M0/M1/M2 신호 확인
[ ] D110 RUN/STOP, D111 Alarm 값을 Gateway가 반복 읽는지 확인
[ ] HMI 생산/제품/설비/알람/최근 이벤트 값이 API 조회 결과와 일치하는지 확인
[ ] DB에 작업지시/LOT/Product를 생성한 후 개별 제품 PASS/FAIL 및 이동 이력 확인
```

## C. Lifecycle·장시간 테스트

```text
[ ] Gemini 90초 이상 연속 실행하고 48.5초 시점 통신 지연 오류 미재발 확인
[ ] Simulation STOP/RESET 직후 mosquitto_pub.exe 종료 확인(mosquitto.exe Broker와 구분)
[ ] Script 재컴파일 후 중복 mosquitto_pub.exe 프로세스가 남지 않는지 확인
[ ] ResourceWarning / unclosed file / orphan process 로그 확인
[ ] STOP→START→RESET 후 Feeder 제품 생성·재개 안정성 확인
[ ] 가능하면 1 LOT(60EA) 및 전체 300EA 시나리오 별도 검증
```

---

# 19. 데이터 누락·생산 대상 Product 오류 — 역사적 이슈와 현재 상태

과거 Gateway에서 다음 오류가 발생한 기록이 있습니다.

```text
[Gateway] Quality Result 처리 중단
[Gateway] 품질판정 대상 Product를 찾지 못했습니다.

Target: 0 / Produced: 0 / Good: 0 / Defect: 0
현재 RUNNING LOT 없음
```

당시에는 Local MSSQL을 초기화한 뒤 진행 중인 WorkOrder/LOT/Product가 없는데 Gemini 측 품질 이벤트가 먼저 도착하는 상황으로 **추정**했습니다. 이는 MQTT 송신시간 최적화 문제와는 **별도**입니다.

**2026-10-16 현재 GitHub 코드 확인**
- `WorkOrderRepository.CreateAsync`가 SQL Stored Procedure `usp_CreateWorkOrder`를 실행합니다. 문서·코드 기준 작업지시 1건 → LOT 5개 → 제품 300개의 기본 생성 구조가 있습니다.
- HMI `MesServerClient`에 미완료 작업지시 조회, 새 작업지시 코드 조회, 작업지시 생성·시작·재개 API 호출이 있습니다.
- Gateway `MqttMessageRouter`는 현재 Product와 품질 결과를 연결하고, 서버 API를 통해 이벤트를 처리합니다.
- 과거와 달리 **개별 제품 이동 및 PASS/FAIL 결과를 DB에서 조회한 시연은 확인된 것으로 기록**되어 있습니다.

**주의:** 과거 로그의 '현재 RUNNING LOT 없음'을 오늘도 발생 중인 오류로 기술하지 않습니다. 다만 초기화·재시작·데이터 정합성 문제에 대한 자동 복구가 모든 경우에 검증된 것으로도 주장하지 않습니다. 전체 60EA/300EA 연속 공정 검증은 별도 과제입니다.

---

# 20. 현재까지 결론 — 2026-10-16 기준

### 구현 코드 확인

```text
[제어 경로]
HMI → HTTP → Mes.Server → SignalR → Mes.Gateway → MX Component → PLC Simulator
START=M0, STOP=M1, RESET=M2 / 300ms Pulse

[설비 상태 및 알람]
Gateway → PLC D110·D111 반복 조회(각 조회 후 1초 대기)
D111 알람 코드를 Server의 ACTIVE/CLEAR 알람과 동기화

[생산정보 경로]
Gemini → MQTT/Mosquitto → Gateway(dt/#) → Server API → SQL Server
HMI → Server HTTP API로 생산·제품·설비·알람·최근 이벤트 조회
(조회 완료 후 1초 대기)

[작업지시/제품]
새 WorkOrder 생성 및 미완료 WorkOrder 재개에 필요한 API·HMI 코드
WorkOrder 1건 / LOT 5건 / Product 300건 저장 프로시저 연동
```

### 기존 기록과 후속 확인에 근거한 결과

- HMI 명령 및 제어 흐름, 개별 제품 이동·PASS/FAIL, PLC 알람 발생·해제와 HMI 표시가 **시뮬레이터 환경에서 확인**됐습니다.
- MQTT 외부 프로세스 매회 실행 병목을 파악하고 Persistent Publisher 방식으로 변경했습니다.
- 역사적 계측(이 문서): `5645.37ms / 5504.06ms` → `3.84ms / 1.88ms`(토픽별). 후속 사용자 반복 호출 계측은 **약 5초 → 0.1~0.3ms**입니다. **서로 다른 계측 조건을 한 가지 수치로 합치지 않습니다**.

### 최신 검증이 필요한 항목

```text
1. Gemini 실제 배치된 VM1·PRESS Python 스크립트가 10~17장의 cleanup 구현과 동일한지
2. Gemini STOP/RESET/재컴파일 후 mosquitto_pub.exe orphan 0건 및 ResourceWarning 미발생
3. 90초 이상 통신 지연 없이 연속 동작하는지 최신 시연 로그 확보
4. Feeder Controlled STOP/재START 안정성(Connector 해제 오류 없이)
5. 60EA LOT 자동 전환 및 300EA 전체 연속 생산 완료
6. 장애/재연결 후 이벤트 중복·유실 방지 및 정확한 상태 복구
```

**환경 버전 주의:** 코드는 MX Component COM 타입을 사용하지만, C# 소스만으로 V4/V5 설치 제품 버전을 특정할 수 없습니다. 실제 사용자 개발 PC에서는 V4를 사용한다고 확인되었고, README·과거 통신 문서의 V5 설명은 수정 필요합니다. SQL Server도 초기 AWS RDS에서 로컬 개발 DB로 변경된 이력이 있습니다.

---

# 21. 권장 다음 작업 순서 — 기능 구현과 검증 구분

1. **현재 Gemini Python 스크립트 백업·대조**: VM1/PRESS의 Publisher 재사용·cleanup 구현이 이 문서 10~17장과 일치하는지 먼저 확인합니다.
2. **MQTT 성능 재측정**: 첫 Publish와 반복 Publish의 Python 호출 처리시간을 따로 기록합니다. 사용자의 0.1~0.3ms 후속 측정 로그도 확보합니다.
3. **시뮬레이터 연결 및 서비스 실행**: Mosquitto → PLC Simulator/MX Component → Mes.Server → Mes.Gateway → Gemini → HMI 순서로 각 연결을 확인합니다.
4. **단일 제품 검증**: 현재 WorkOrder/LOT/Product가 생성된 상태에서 PASS·FAIL, 공정 이동, 알람 ACTIVE/CLEAR, DB 및 HMI 반영을 확인합니다.
5. **장시간/정리 테스트**: Gemini 90초 이상 실행 후 STOP/RESET/재시작/스크립트 재컴파일. `mosquitto_pub.exe` 잔존 0건과 경고 미발생을 검증합니다.
6. **Feeder 문제 보완**: START/STOP/RESET에 따라 신규 제품 생성이 오류 없이 중단·재개되는 방식을 확정합니다. 기존 Output Connector 해제 방식을 그대로 채택하지 않습니다.
7. **LOT/작업지시 끝단 검증**: 60EA 처리 후 다음 LOT 전환, 5 LOT/300EA 완료까지 결과를 기록합니다.
8. **README·아키텍처 자료 정합성 갱신**: 실제 사용 MX Component 버전, 현재 DB 위치, 장시간 생산 검증 상태를 최신화합니다.

> 각 항목은 **권장 후속 검증/작업**이며, 리스트에 적혔다고 완료된 것은 아닙니다.

---

# 22. 현재 구현과 문서 대조 근거

**대조 대상:** `EddieEom/MES-System` GitHub 기본 브랜치의 현재 파일 및 2026-10-06 원본 트러블슈팅 기록. 아래는 `코드 확인`을 뜻하며 GitHub를 실행한 테스트가 아닙니다.

| 근거 파일 | 확인한 사실 |
|---|---|
| `Mes.Hmi/Forms/MainForm.cs` | 대시보드 5종 조회 Task 병렬 실행, 완료 후 1초 대기 |
| `Mes.Hmi/Api/MesServerClient.cs` | 생산·제품·설비·알람·이벤트 조회, PLC 제어, WorkOrder 생성/시작/재개 API |
| `src/Mes.Server/Controllers/Control/PlcControlController.cs` | HMI 제어 API→SignalR `PlcCommand` 전송, 응답 `sent` |
| `src/Mes.Gateway/Services/GatewaySignalRService.cs` | Gateway 명령 수신·중복 CommandId 확인·PLC 제어 처리 |
| `src/Mes.Gateway/Services/PlcControlService.cs` | M0/M1/M2 및 300ms Pulse 지정 |
| `src/Mes.Gateway/Plc/MxComponentPlcClient.cs` | `ActUtlType64Lib` 기반 PLC 읽기·쓰기·펄스, STA Thread |
| `src/Mes.Gateway/Services/PlcPollingService.cs` | D110/D111 반복 조회 및 D111 기반 PLC 알람 동기화 |
| `src/Mes.Gateway/Mqtt/MqttClientService.cs` | MQTTnet Broker 연결·`dt/#` 구독 |
| `src/Mes.Gateway/Routing/MqttMessageRouter.cs` | 상태·공정·품질 MQTT 토픽 처리 |
| `src/Mes.Gateway/Program.cs` | Server/PLC 연결, SignalR·MQTT·Polling 서비스 시작, 알람 상태 동기화 |
| `src/Mes.Server/Repositories/WorkOrder/WorkOrderRepository.cs` | `usp_CreateWorkOrder` 호출, 1 WO/5 LOT/300 제품 구조 |
| `README.md` | 주요 기능·개별 테스트 기록, 다만 V5·AWS RDS 설명이 예전 환경과 혼재 |
