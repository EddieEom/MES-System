/*
============================================================
Procedure : usp_MoveProduct

[저장 프로시저를 사용하는 이유]

제품이 공정 사이를 이동할 때는 단순히
products.current_location_id만 변경하면 안 된다.

MES에서는 반드시:

1. products.current_location_id 현재 위치 변경
2. process_events에 이동 이력 INSERT

두 작업이 동시에 이루어져야 한다.

만약 현재 위치 UPDATE는 성공했지만
process_events INSERT가 실패하면,

현재 위치와 Traceability 이력이 서로 달라지는
데이터 불일치가 발생한다.

따라서 제품의 현재 위치 변경과 공정 이력 기록을
하나의 TRANSACTION으로 보장하기 위해
Stored Procedure로 구현한다.

또한 최종 위치인
WAREHOUSE / NG_STACK에 도착하면
제품 상태를 COMPLETED로 변경한다.
============================================================
*/
USE MES_SYSTEM;

CREATE PROCEDURE dbo.usp_MoveProduct

    @EventId            UNIQUEIDENTIFIER,
    @ProductId          BIGINT,
    @ToLocationCode     VARCHAR(30),
    @EventType          VARCHAR(30) = 'MOVE',
    @MachineCode        VARCHAR(30) = NULL,
    @EventTime          DATETIME2(3) = NULL,
    @SourceSystem       VARCHAR(20) = 'MES',
    @Remarks            NVARCHAR(200) = NULL

AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @FromLocationId INT,
        @ToLocationId INT,
        @MachineId INT,
        @IsTerminal BIT,
        @QualityStatus VARCHAR(10);

    BEGIN TRY

        BEGIN TRANSACTION;


        ----------------------------------------------------
        -- 동일 이벤트 중복 처리 방지
        ----------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM process_events
            WHERE event_id = @EventId
        )
        BEGIN
            THROW 50020, '이미 처리된 공정 이벤트입니다.', 1;
        END;


        ----------------------------------------------------
        -- 제품 존재 확인 및 현재 위치 조회
        ----------------------------------------------------
        SELECT
            @FromLocationId = current_location_id,
            @QualityStatus = quality_status
        FROM products WITH (UPDLOCK, HOLDLOCK)
        WHERE product_id = @ProductId;


        IF @@ROWCOUNT = 0
        BEGIN
            THROW 50101, '존재하지 않는 제품입니다.', 1;
        END;


        ----------------------------------------------------
        -- 이동 대상 위치 조회
        ----------------------------------------------------
        SELECT
            @ToLocationId = location_id,
            @MachineId = machine_id,
            @IsTerminal = is_terminal
        FROM locations
        WHERE location_code = @ToLocationCode
          AND is_active = 1;


        IF @ToLocationId IS NULL
        BEGIN
            THROW 50102, '존재하지 않는 공정 위치입니다.', 1;
        END;


        ----------------------------------------------------
        -- 현재 위치와 동일한 위치로 중복 이동 방지
        ----------------------------------------------------
        IF @FromLocationId = @ToLocationId
           AND @EventType IN ('RELEASE', 'MOVE', 'ROUTE')
        BEGIN
            THROW 50021, '현재 위치와 이동 대상 위치가 동일합니다.', 1;
        END;


        ----------------------------------------------------
        -- 설비 코드가 전달된 경우 Machine ID 조회
        ----------------------------------------------------
        IF @MachineCode IS NOT NULL
        BEGIN

            SELECT
                @MachineId = machine_id
            FROM machines
            WHERE machine_code = @MachineCode
              AND is_active = 1;


            IF @MachineId IS NULL
            BEGIN
                THROW 50103, '존재하지 않는 설비입니다.', 1;
            END;

        END;


        ----------------------------------------------------
        -- 잘못된 최종 경로 방지
        --
        -- PASS 제품 → WAREHOUSE
        -- FAIL 제품 → NG_STACK
        ----------------------------------------------------
        IF @ToLocationCode = 'WAREHOUSE'
           AND @QualityStatus <> 'PASS'
        BEGIN
            THROW 50104, 'PASS 제품만 WAREHOUSE로 이동할 수 있습니다.', 1;
        END;


        IF @ToLocationCode = 'NG_STACK'
           AND @QualityStatus <> 'FAIL'
        BEGIN
            THROW 50105, 'FAIL 제품만 NG_STACK으로 이동할 수 있습니다.', 1;
        END;


        ----------------------------------------------------
        -- 이벤트 시간이 없으면 현재 UTC 사용
        ----------------------------------------------------
        IF @EventTime IS NULL
        BEGIN
            SET @EventTime = SYSUTCDATETIME();
        END;


        ----------------------------------------------------
        -- 현재 위치 변경
        --
        -- 첫 공정 이동 시 WAITING → IN_PROCESS
        -- 최종 위치 도착 시 COMPLETED
        ----------------------------------------------------
        UPDATE products
        SET
            current_location_id = @ToLocationId,

            status =
                CASE
                    WHEN @IsTerminal = 1
                        THEN 'COMPLETED'

                    WHEN status = 'WAITING'
                        THEN 'IN_PROCESS'

                    ELSE status
                END,

            started_at =
                CASE
                    WHEN started_at IS NULL
                        THEN @EventTime

                    ELSE started_at
                END,

            completed_at =
                CASE
                    WHEN @IsTerminal = 1
                        THEN @EventTime

                    ELSE completed_at
                END

        WHERE product_id = @ProductId;


        ----------------------------------------------------
        -- 공정 이동 이력 기록
        ----------------------------------------------------
        INSERT INTO process_events
        (
            event_id,
            product_id,
            machine_id,
            event_type,
            from_location_id,
            to_location_id,
            event_time,
            source_system,
            remarks
        )
        VALUES
        (
            @EventId,
            @ProductId,
            @MachineId,
            @EventType,
            @FromLocationId,
            @ToLocationId,
            @EventTime,
            @SourceSystem,
            @Remarks
        );


        COMMIT TRANSACTION;


        ----------------------------------------------------
        -- 처리 결과 반환
        ----------------------------------------------------
        SELECT
            p.product_id,
            p.product_code,
            p.status,
            p.quality_status,
            l.location_code AS current_location

        FROM products p

        LEFT JOIN locations l
            ON p.current_location_id = l.location_id

        WHERE p.product_id = @ProductId;


    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;

        THROW;

    END CATCH;

END;
GO