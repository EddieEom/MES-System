# MES SYSTEM

Digital Twin 기반 제조 실행 시스템(MES) 프로젝트입니다.

## Architecture

Gemini Digital Twin
→ MQTT
→ Mes.Gateway
→ HTTP/HTTPS
→ Mes.Server
→ AWS RDS for SQL Server

향후 Vue MES Web, WinForms HMI, PLC 제어를 통합할 예정입니다.

## Projects

### Mes.Server
ASP.NET Core 기반 중앙 MES API 서버

주요 기능:
- WorkOrder 관리
- LOT 관리
- Product 관리
- 생산 공정 이력
- WIP / Traceability
- 품질 결과
- 설비 상태 이력
- Alarm
- AWS RDS 연동

### Mes.Gateway
현장 Edge Gateway

주요 기능:
- Gemini MQTT 수신
- VM-1 / Press 상태 처리
- PASS / FAIL 품질 이벤트 처리
- MQTT Routing
- 생산 현황 계산
- MX Component / PLC 통신 PoC

## 데이터베이스

AWS RDS for SQL Server Express 기반 MES_SYSTEM

주요 테이블:
- users
- work_orders
- lots
- machines
- locations
- products
- process_events
- quality_results
- machine_status_history
- alarms

## 진행 상황

현재 완료:
- Digital Twin 데이터 추출
- MQTT 통신
- Gateway 기초 구현
- PLC / MX Component PoC
- MES DB 설계
- AWS RDS 구축
- Mes.Server Repository 계층
- WorkOrder START / PAUSE / RESUME
- Product WIP / Traceability

다음 개발:
- Mes.Gateway ↔ Mes.Server HTTP 통신
- MQTT 데이터 AWS DB 자동 저장