CREATE DATABASE MES_SYSTEM;
USE MES_SYSTEM;

-- MES 계정 관리용
CREATE TABLE users
(
    user_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_users PRIMARY KEY,

    login_id NVARCHAR(50) NOT NULL
        CONSTRAINT UQ_users_login_id UNIQUE,

    password_hash VARCHAR(255) NOT NULL,

    display_name NVARCHAR(50) NOT NULL,

    role VARCHAR(20) NOT NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_users_is_active DEFAULT 1,

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_users_created_at DEFAULT SYSUTCDATETIME(),

    last_login_at DATETIME2(3) NULL,

    CONSTRAINT CK_users_role
        CHECK (role IN ('ADMIN', 'OPERATOR'))
);
GO



-- 작업지시용 테이블
CREATE TABLE work_orders
(
    work_order_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_work_orders PRIMARY KEY,

    work_order_code VARCHAR(30) NOT NULL
        CONSTRAINT UQ_work_orders_code UNIQUE,

    target_qty INT NOT NULL
        CONSTRAINT DF_work_orders_target_qty DEFAULT 300,

    lot_count SMALLINT NOT NULL
        CONSTRAINT DF_work_orders_lot_count DEFAULT 5,

    lot_size SMALLINT NOT NULL
        CONSTRAINT DF_work_orders_lot_size DEFAULT 60,

    status VARCHAR(20) NOT NULL
        CONSTRAINT DF_work_orders_status DEFAULT 'IDLE',

    created_by_user_id BIGINT NULL,

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_work_orders_created_at DEFAULT SYSUTCDATETIME(),

    started_at DATETIME2(3) NULL,

    completed_at DATETIME2(3) NULL,

    CONSTRAINT FK_work_orders_users
        FOREIGN KEY (created_by_user_id)
        REFERENCES users(user_id),

    CONSTRAINT CK_work_orders_target_qty
        CHECK (target_qty > 0),

    CONSTRAINT CK_work_orders_lot_count
        CHECK (lot_count > 0),

    CONSTRAINT CK_work_orders_lot_size
        CHECK (lot_size > 0),

    CONSTRAINT CK_work_orders_status
        CHECK (
            status IN (
                'IDLE',
                'RUNNING',
                'PAUSE',
                'COMPLETED'
            )
        ),

    CONSTRAINT CK_work_orders_qty_structure
        CHECK (target_qty = lot_count * lot_size)
);
GO


-- lot전용 테이블
CREATE TABLE lots
(
    lot_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_lots PRIMARY KEY,

    work_order_id BIGINT NOT NULL,

    lot_code VARCHAR(40) NOT NULL
        CONSTRAINT UQ_lots_code UNIQUE,

    lot_sequence SMALLINT NOT NULL,

    target_qty SMALLINT NOT NULL
        CONSTRAINT DF_lots_target_qty DEFAULT 60,

    status VARCHAR(20) NOT NULL
        CONSTRAINT DF_lots_status DEFAULT 'WAITING',

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_lots_created_at DEFAULT SYSUTCDATETIME(),

    started_at DATETIME2(3) NULL,

    completed_at DATETIME2(3) NULL,

    CONSTRAINT FK_lots_work_orders
        FOREIGN KEY (work_order_id)
        REFERENCES work_orders(work_order_id),

    CONSTRAINT UQ_lots_work_order_sequence
        UNIQUE (work_order_id, lot_sequence),

    CONSTRAINT CK_lots_sequence
        CHECK (lot_sequence > 0),

    CONSTRAINT CK_lots_target_qty
        CHECK (target_qty > 0),

    CONSTRAINT CK_lots_status
        CHECK (
            status IN (
                'WAITING',
                'RUNNING',
                'COMPLETED'
            )
        )
);
GO

-- 작업지시 하나에서 RUNNING LOT이 2개 생기는 걸 DB 자체에서 차단
CREATE UNIQUE INDEX UX_lots_one_running_per_work_order
ON lots(work_order_id)
WHERE status = 'RUNNING';
GO
 

 -- 설비 관리용 테이블
CREATE TABLE machines
(
    machine_id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_machines PRIMARY KEY,

    machine_code VARCHAR(30) NOT NULL
        CONSTRAINT UQ_machines_code UNIQUE,

    machine_name NVARCHAR(100) NOT NULL,

    machine_type VARCHAR(30) NOT NULL,

    ideal_cycle_time_sec DECIMAL(10,3) NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_machines_is_active DEFAULT 1,

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_machines_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_machines_type
        CHECK (
            machine_type IN (
                'PROCESS',
                'QUALITY'
            )
        )
);
GO

-- 설비 데이터 적용
INSERT INTO machines
(
    machine_code,
    machine_name,
    machine_type
)
VALUES
('VM1', 'VM-1', 'PROCESS'),
('PRESS', 'Press Machine', 'PROCESS'),
('QUALITY_CHECK', 'Quality Check', 'QUALITY');
GO


SELECT *
FROM machines;
Go

--제품 WIP와 현재 제품 위치 추적 테이블
CREATE TABLE locations
(
    location_id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_locations PRIMARY KEY,

    location_code VARCHAR(30) NOT NULL
        CONSTRAINT UQ_locations_code UNIQUE,

    location_name NVARCHAR(100) NOT NULL,

    location_type VARCHAR(30) NOT NULL,

    sequence_no SMALLINT NOT NULL,

    machine_id INT NULL,

    is_terminal BIT NOT NULL
        CONSTRAINT DF_locations_terminal DEFAULT 0,

    is_active BIT NOT NULL
        CONSTRAINT DF_locations_active DEFAULT 1,

    CONSTRAINT FK_locations_machines
        FOREIGN KEY (machine_id)
        REFERENCES machines(machine_id),

    CONSTRAINT CK_locations_type
        CHECK (
            location_type IN (
                'SOURCE',
                'ROBOT',
                'MACHINE',
                'CONVEYOR',
                'SORTER',
                'OUTPUT'
            )
        )
);
GO


-- 현 실제 공정 순서 데이터 넣기
INSERT INTO locations
(
    location_code,
    location_name,
    location_type,
    sequence_no,
    machine_id,
    is_terminal
)
VALUES
(
    'FEEDER',
    'Feeder 원자재 배출',
    'SOURCE',
    1,
    NULL,
    0
),
(
    'CONVEYOR',
    '투입 Conveyor',
    'CONVEYOR',
    2,
    NULL,
    0
),
(
    'ROBOT_TO_VM1',
    'RV-7FRLL-SH 원자재 이송',
    'ROBOT',
    3,
    NULL,
    0
),
(
    'VM1',
    'VM-1 가공 공정',
    'MACHINE',
    4,
    (SELECT machine_id FROM machines WHERE machine_code = 'VM1'),
    0
),
(
    'ROBOT_TO_PRESS',
    'RV-7FRLL-SH 에폭시 가공품 이송',
    'ROBOT',
    5,
    NULL,
    0
),
(
    'PRESS',
    'Press 가공 및 품질 판정',
    'MACHINE',
    6,
    (SELECT machine_id FROM machines WHERE machine_code = 'PRESS'),
    0
),
(
    'ROBOT_TO_OUT',
    'RV-7FRLL-SH 완료품 이송',
    'ROBOT',
    7,
    NULL,
    0
),
(
    'CONVEYOR_2',
    '배출 Conveyor #2',
    'CONVEYOR',
    8,
    NULL,
    0
),
(
    'SRX_III',
    'SRX III 분류 공정',
    'SORTER',
    9,
    NULL,
    0
),
(
    'NG_STACK',
    'NG Floorspace Stack',
    'OUTPUT',
    10,
    NULL,
    1
),
(
    'WAREHOUSE',
    'Warehouse Process Shelf',
    'OUTPUT',
    10,
    NULL,
    1
);
GO

-- 작업지시 생성 시 제품 정보 테이블
CREATE TABLE products
(
    product_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_products PRIMARY KEY,

    product_code VARCHAR(50) NOT NULL
        CONSTRAINT UQ_products_code UNIQUE,

    lot_id BIGINT NOT NULL,

    sequence_no SMALLINT NOT NULL,

    status VARCHAR(20) NOT NULL
        CONSTRAINT DF_products_status DEFAULT 'WAITING',

    quality_status VARCHAR(10) NOT NULL
        CONSTRAINT DF_products_quality DEFAULT 'PENDING',

    current_location_id INT NULL,

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_products_created_at DEFAULT SYSUTCDATETIME(),

    started_at DATETIME2(3) NULL,

    completed_at DATETIME2(3) NULL,

    CONSTRAINT FK_products_lots
        FOREIGN KEY (lot_id)
        REFERENCES lots(lot_id),

    CONSTRAINT FK_products_locations
        FOREIGN KEY (current_location_id)
        REFERENCES locations(location_id),

    CONSTRAINT UQ_products_lot_sequence
        UNIQUE (lot_id, sequence_no),

    CONSTRAINT CK_products_sequence
        CHECK (sequence_no > 0),

    CONSTRAINT CK_products_status
        CHECK (
            status IN (
                'WAITING',
                'IN_PROCESS',
                'COMPLETED'
            )
        ),

    CONSTRAINT CK_products_quality
        CHECK (
            quality_status IN (
                'PENDING',
                'PASS',
                'FAIL'
            )
        )
);
GO

-- 제품의 공정 전체 이력 테이블
CREATE TABLE process_events
(
    process_event_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_process_events PRIMARY KEY,

    event_id UNIQUEIDENTIFIER NOT NULL
        CONSTRAINT DF_process_events_event_id DEFAULT NEWSEQUENTIALID(),

    product_id BIGINT NOT NULL,

    machine_id INT NULL,

    event_type VARCHAR(30) NOT NULL,

    from_location_id INT NULL,

    to_location_id INT NULL,

    event_time DATETIME2(3) NOT NULL,

    source_system VARCHAR(20) NOT NULL,

    remarks NVARCHAR(200) NULL,

    created_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_process_events_created_at DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_process_events_event_id
        UNIQUE (event_id),

    CONSTRAINT FK_process_events_products
        FOREIGN KEY (product_id)
        REFERENCES products(product_id),

    CONSTRAINT FK_process_events_machines
        FOREIGN KEY (machine_id)
        REFERENCES machines(machine_id),

    CONSTRAINT FK_process_events_from_location
        FOREIGN KEY (from_location_id)
        REFERENCES locations(location_id),

    CONSTRAINT FK_process_events_to_location
        FOREIGN KEY (to_location_id)
        REFERENCES locations(location_id),

    CONSTRAINT CK_process_events_type
        CHECK (
            event_type IN (
                'RELEASE',
                'MOVE',
                'PROCESS_START',
                'PROCESS_COMPLETE',
                'ROUTE'
            )
        ),

    CONSTRAINT CK_process_events_source
        CHECK (
            source_system IN (
                'GEMINI',
                'PLC',
                'MES'
            )
        )
);
GO

-- 제품 하나당 최종 결과 1건 / 재작업은 없으므로 product_id UNIQUE / MQTT 중복 수신을 차단 하기 위해 event_id UNIQUE로 둠 
CREATE TABLE quality_results
(
    quality_result_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_quality_results PRIMARY KEY,

    event_id UNIQUEIDENTIFIER NOT NULL,

    product_id BIGINT NOT NULL,

    machine_id INT NOT NULL,

    result VARCHAR(10) NOT NULL,

    source_product_type NVARCHAR(50) NULL,

    inspected_at DATETIME2(3) NOT NULL,

    source_system VARCHAR(20) NOT NULL
        CONSTRAINT DF_quality_results_source DEFAULT 'GEMINI',

    received_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_quality_results_received DEFAULT SYSUTCDATETIME(),

    CONSTRAINT UQ_quality_results_event_id
        UNIQUE (event_id),

    CONSTRAINT UQ_quality_results_product_id
        UNIQUE (product_id),

    CONSTRAINT FK_quality_results_products
        FOREIGN KEY (product_id)
        REFERENCES products(product_id),

    CONSTRAINT FK_quality_results_machines
        FOREIGN KEY (machine_id)
        REFERENCES machines(machine_id),

    CONSTRAINT CK_quality_results_result
        CHECK (
            result IN ('PASS', 'FAIL')
        ),

    CONSTRAINT CK_quality_results_source
        CHECK (
            source_system IN (
                'GEMINI',
                'PLC',
                'MES'
            )
        )
);
GO

-- 설비 상태 기록용 테이블
CREATE TABLE machine_status_history
(
    machine_status_history_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_machine_status_history PRIMARY KEY,

    machine_id INT NOT NULL,

    measured_at DATETIME2(3) NOT NULL,

    state VARCHAR(20) NOT NULL,

    parts_entered INT NOT NULL
        CONSTRAINT DF_machine_status_entered DEFAULT 0,

    parts_exited INT NOT NULL
        CONSTRAINT DF_machine_status_exited DEFAULT 0,

    parts_current INT NOT NULL
        CONSTRAINT DF_machine_status_current DEFAULT 0,

    parts_average_time DECIMAL(18,6) NOT NULL
        CONSTRAINT DF_machine_status_average DEFAULT 0,

    idle_percentage DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_idle DEFAULT 0,

    busy_percentage DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_busy DEFAULT 0,

    blocked_percentage DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_blocked DEFAULT 0,

    failed_percentage DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_failed DEFAULT 0,

    repair_percentage DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_repair DEFAULT 0,

    utilization DECIMAL(7,4) NOT NULL
        CONSTRAINT DF_machine_status_utilization DEFAULT 0,

    source_system VARCHAR(20) NOT NULL
        CONSTRAINT DF_machine_status_source DEFAULT 'GEMINI',

    received_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_machine_status_received DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_machine_status_machines
        FOREIGN KEY (machine_id)
        REFERENCES machines(machine_id),

    CONSTRAINT CK_machine_status_source
        CHECK (
            source_system IN (
                'GEMINI',
                'PLC'
            )
        )
);
GO

-- 중복 스냅샷 방지를 위한 인덱스.
CREATE UNIQUE INDEX UX_machine_status_snapshot
ON machine_status_history
(
    machine_id,
    measured_at,
    source_system
);
GO

-- 설비의 상태 조회 성능을 높히기 위한 인덱스
CREATE INDEX IX_machine_status_machine_time
ON machine_status_history
(
    machine_id,
    measured_at DESC
);
GO

-- 알람용 테이블
CREATE TABLE alarms
(
    alarm_id BIGINT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_alarms PRIMARY KEY,

    machine_id INT NOT NULL,

    alarm_code VARCHAR(30) NOT NULL,

    alarm_message NVARCHAR(200) NULL,

    severity VARCHAR(20) NOT NULL
        CONSTRAINT DF_alarms_severity DEFAULT 'WARNING',

    occurred_at DATETIME2(3) NOT NULL,

    cleared_at DATETIME2(3) NULL,

    is_active BIT NOT NULL
        CONSTRAINT DF_alarms_active DEFAULT 1,

    source_system VARCHAR(20) NOT NULL
        CONSTRAINT DF_alarms_source DEFAULT 'PLC',

    received_at DATETIME2(3) NOT NULL
        CONSTRAINT DF_alarms_received DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_alarms_machines
        FOREIGN KEY (machine_id)
        REFERENCES machines(machine_id),

    CONSTRAINT CK_alarms_severity
        CHECK (
            severity IN (
                'INFO',
                'WARNING',
                'CRITICAL'
            )
        ),

    CONSTRAINT CK_alarms_source
        CHECK (
            source_system IN (
                'GEMINI',
                'PLC'
            )
        )
);
GO

-- 활성 알람 조회 시 성능을 높이기 위한 인덱스
CREATE INDEX IX_alarms_active
ON alarms
(
    machine_id,
    is_active,
    occurred_at DESC
);
GO


-- 작업지시 상태(IDLE/RUNNING/PAUSE/COMPLETED)별 목록을 빠르게 조회할 때 사용
CREATE INDEX IX_work_orders_status
ON work_orders(status);
GO


-- 특정 작업지시의 LOT들을 상태별(WAITING/RUNNING/COMPLETED)로 빠르게 조회할 때 사용
CREATE INDEX IX_lots_work_order_status
ON lots(work_order_id, status);
GO


-- 특정 LOT에 속한 제품들을 생산상태별(WAITING/IN_PROCESS/COMPLETED)로 빠르게 조회할 때 사용
CREATE INDEX IX_products_lot_status
ON products(lot_id, status);
GO


-- 현재 위치별 WIP 제품 수량 및 진행 중 제품 목록을 빠르게 조회할 때 사용
CREATE INDEX IX_products_location
ON products(current_location_id, status);
GO


-- PASS/FAIL/PENDING 상태별 제품 수량 및 품질 현황을 빠르게 조회할 때 사용
CREATE INDEX IX_products_quality
ON products(quality_status);
GO


-- 특정 제품의 공정 이동 및 처리 이력을 최신순으로 빠르게 조회할 때 사용
CREATE INDEX IX_process_events_product_time
ON process_events
(
    product_id,
    event_time DESC
);
GO


-- 최근 품질 검사 결과를 시간순으로 빠르게 조회할 때 사용
CREATE INDEX IX_quality_results_time
ON quality_results
(
    inspected_at DESC
);
GO


-- PASS/FAIL 결과별 품질 통계 및 불량 제품 목록을 빠르게 조회할 때 사용
CREATE INDEX IX_quality_results_result
ON quality_results(result);
GO



-- 주의 할 점 생산량을 별도 컬럼으로 중복 저장하지 않는 것이 중요하다.
SELECT
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
GO