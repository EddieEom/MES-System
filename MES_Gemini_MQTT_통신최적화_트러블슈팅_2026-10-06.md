# MES / Gemini MQTT 통신 최적화 및 트러블슈팅 기록

작성일: 2026-10-06  
대상 프로젝트: 디지털 트윈 연동 Press 공정 MES 시스템  
대상 구간: MELSOFT Gemini ↔ MQTT ↔ Mes.Gateway / Mitsubishi FA ↔ GX Simulator2

---

## 1. 현재 프로젝트 통신 구조

```text
[Gemini Digital Twin]
    │
    ├─ Mitsubishi Electric FA
    │       ↕
    │   GX Simulator2
    │       ↕
    │   MX Component V5
    │       ↕
    │   Mes.Gateway
    │
    └─ MQTT
            ↓
        Mosquitto Broker
            ↓
        Mes.Gateway
            ↓
        Mes.Server
            ↓
        MSSQL MES_SYSTEM
```

PLC 제어 흐름은 별도로 다음 구조를 유지한다.

```text
HMI
 ↓ HTTP
Mes.Server
 ↓ SignalR
Mes.Gateway
 ↓ MX Component
GX Simulator2 / PLC
 ↓
M3 = RUN_STATE
 ↓
Gemini Signal Relay
```

---

# 2. Polling 구분

프로젝트에서 Polling은 두 종류이며 서로 다른 기능이다.

| 구간 | 상태 | 역할 |
|---|---|---|
| Gateway → PLC | 구현 완료 | D110 / D111 1초 Polling |
| HMI → Mes.Server | 미구현 | Dashboard 화면 자동 갱신 |

Gateway의 `PlcPollingService`는 이미 구현되어 있다.

```csharp
public async Task RunAsync(
    CancellationToken cancellationToken = default)
{
    while (!cancellationToken.IsCancellationRequested)
    {
        await ReadAndProcessAsync(
            cancellationToken
        );

        await Task.Delay(
            1000,
            cancellationToken
        );
    }
}
```

읽는 값:

```text
D110 = PLC 운전 상태
D111 = PLC Alarm Code
```

따라서 PLC Polling은 다시 구현하지 않는다.

---

# 3. Feeder STOP 제어 문제

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

최종 Persistent Publisher는 다음 Lifecycle을 가진다.

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

# 18. 테스트 체크리스트

## A. 성능

VM1 실행 후:

```text
[MQTT] 전송 요청 : dt/process/event / 수 ms
[MQTT] 전송 요청 : dt/vm1/status / 수 ms
```

정상 목표:

```text
기존 5,000ms 이상
→ 수 ms 수준
```

---

## B. Gateway 수신

Gateway:

```text
[MQTT] 수신 → dt/process/event
[MQTT] 수신 → dt/vm1/status
[MQTT] 수신 → dt/press/status
[MQTT] 수신 → dt/quality/result
```

필요 Topic이 정상 수신되어야 한다.

---

## C. Gemini 장시간 동작

최소:

```text
90초 이상
```

실행해서 기존의:

```text
0:00:48.50
MITSUBISHI ELECTRIC FA
5000 ms 통신 오류
```

가 재발하지 않는지 확인한다.

---

## D. STOP Cleanup

Gemini STOP 후 로그:

```text
[MQTT] Publisher 전체 정리 시작
[MQTT] Publisher 정상 종료 ...
[MQTT] Publisher 전체 정리 완료
```

작업 관리자:

```text
mosquitto_pub.exe = 0개
mosquitto.exe     = Broker이므로 유지
```

---

## E. 재START

다시 Gemini ▶:

```text
첫 MQTT 이벤트
→ 새로운 mosquitto_pub.exe 생성

그 이후
→ 동일 프로세스 재사용
```

되어야 한다.

---

# 19. 별도 확인된 데이터 문제

Gateway 로그에서 다음 메시지가 확인되었다.

```text
[Gateway] Quality Result 처리 중단
[Gateway] 품질판정 대상 Product를 찾지 못했습니다.
```

동시에 Production 정보:

```text
Target   : 0
Produced : 0
Good     : 0
Defect   : 0

현재 RUNNING LOT 없음
```

이는 MQTT 전송 지연 문제와 별개다.

현재 Local MSSQL에는 스키마는 있지만 기존 테스트 생산 데이터가 없기 때문에, 실제 Quality Result를 연결할 Product / RUNNING LOT이 없어서 발생할 수 있다.

MQTT/통신 최적화를 완료한 뒤 다음 단계에서:

```text
WorkOrder
→ LOT
→ Products
```

테스트 데이터를 기존 API / Stored Procedure 흐름을 통해 생성해야 한다.

---

# 20. 현재까지 결론

### 완료

```text
HMI → Server → SignalR → Gateway → PLC
START / STOP / RESET 전달

M3 = RUN_STATE
Gemini Signal Relay 연결

Gateway D110 / D111 1초 Polling

Gemini MQTT
→ Gateway
→ Mes.Server
데이터 전달 확인

MQTT subprocess.run 병목 발견

MQTT Persistent Publisher 적용

약 5.5초
→ 약 1~4ms
통신 최적화 성공
```

### 아직 정리 필요

```text
1. Persistent mosquitto_pub Lifecycle 최종 검증
2. ResourceWarning / orphan process 재발 여부 확인
3. 90초 이상 Gemini 장시간 실행 검증
4. Feeder STOP 방식 최종 개선
   - Output Connector disconnect 방식은
     ProductCreatorNoOutputConnectionDefined 오류 발생
5. Local DB 테스트 WorkOrder / LOT / Product 생성
6. HMI Dashboard 1초 Polling
```

---

# 21. 다음 작업 순서

현재 우선순위:

```text
1. 기존 orphan mosquitto_pub PID 1회 종료
2. VM1 / PRESS에 Lifecycle cleanup 적용
3. Gemini START
4. VM1 / PRESS MQTT 동작 확인
5. 90초 이상 실행
6. Gemini STOP
7. mosquitto_pub.exe 0개 확인
8. ResourceWarning 재발 여부 확인
```

여기까지 성공하면 MQTT 통신 최적화 작업을 완료 처리한다.

그 다음 다시 Feeder Controlled Stop 문제로 돌아간다.
