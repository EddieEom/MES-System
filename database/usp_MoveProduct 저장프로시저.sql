/*
============================================================
Procedure : usp_MoveProduct

[목적]

제품이 공정 사이를 이동할 때:

1. products.current_location_id 갱신
2. Product 생산상태 갱신
3. process_events 이동 이력 기록

위 작업을 하나의 Transaction으로 처리한다.

MachineId는 외부에서 전달받지 않는다.

ToLocationCode
    ↓
locations
    ↓
locations.machine_id

구조를 통해 해당 Location과 연결된 설비를
자동으로 process_events.machine_id에 기록한다.

최종 위치:

PASS → WAREHOUSE
FAIL → NG_STACK

도착 시 Product를 COMPLETED 처리한다.
============================================================
*/

USE MES_SYSTEM;
GO


CREATE OR ALTER PROCEDURE dbo.usp_MoveProduct

    @EventId            UNIQUEIDENTIFIER,
    @ProductId          BIGINT,
    @ToLocationCode     VARCHAR(30),
    @EventType          VARCHAR(30) = 'MOVE',
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
        -- 동일 EventId 중복 처리 방지
        ----------------------------------------------------

        IF EXISTS
        (
            SELECT 1
            FROM process_events
            WHERE event_id = @EventId
        )
        BEGIN

            THROW 50020,
                '이미 처리된 공정 이벤트입니다.',
                1;

        END;


        ----------------------------------------------------
        -- Product 존재 확인
        --
        -- 동시에 처리되는 이동으로 인한 충돌을 막기 위해
        -- 해당 Product Row Lock
        ----------------------------------------------------

        SELECT
            @FromLocationId =
                current_location_id,

            @QualityStatus =
                quality_status

        FROM products
            WITH (UPDLOCK, HOLDLOCK)

        WHERE product_id =
            @ProductId;


        IF @@ROWCOUNT = 0
        BEGIN

            THROW 50101,
                '존재하지 않는 제품입니다.',
                1;

        END;


        ----------------------------------------------------
        -- 이동 대상 Location 조회
        --
        -- MachineId 역시 Location에서 자동 결정
        ----------------------------------------------------

        SELECT
            @ToLocationId =
                location_id,

            @MachineId =
                machine_id,

            @IsTerminal =
                is_terminal

        FROM locations

        WHERE location_code =
                @ToLocationCode

          AND is_active = 1;


        IF @ToLocationId IS NULL
        BEGIN

            THROW 50102,
                '존재하지 않는 공정 위치입니다.',
                1;

        END;


        ----------------------------------------------------
        -- 동일 위치 중복 이동 방지
        ----------------------------------------------------

        IF @FromLocationId =
                @ToLocationId

           AND @EventType IN
           (
               'RELEASE',
               'MOVE',
               'ROUTE'
           )
        BEGIN

            THROW 50021,
                '현재 위치와 이동 대상 위치가 동일합니다.',
                1;

        END;


        ----------------------------------------------------
        -- 최종 경로 검증
        --
        -- PASS → WAREHOUSE
        -- FAIL → NG_STACK
        ----------------------------------------------------

        IF @ToLocationCode =
                'WAREHOUSE'

           AND @QualityStatus <>
                'PASS'
        BEGIN

            THROW 50104,
                'PASS 제품만 WAREHOUSE로 이동할 수 있습니다.',
                1;

        END;


        IF @ToLocationCode =
                'NG_STACK'

           AND @QualityStatus <>
                'FAIL'
        BEGIN

            THROW 50105,
                'FAIL 제품만 NG_STACK으로 이동할 수 있습니다.',
                1;

        END;


        ----------------------------------------------------
        -- EventTime 미입력 시 UTC 현재시간
        ----------------------------------------------------

        IF @EventTime IS NULL
        BEGIN

            SET @EventTime =
                SYSUTCDATETIME();

        END;


        ----------------------------------------------------
        -- Product 현재 위치 / 상태 갱신
        --
        -- WAITING
        --   ↓
        -- IN_PROCESS
        --
        -- Terminal Location
        --   ↓
        -- COMPLETED
        ----------------------------------------------------

        UPDATE products

        SET
            current_location_id =
                @ToLocationId,


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

        WHERE product_id =
            @ProductId;


        ----------------------------------------------------
        -- Process Event 기록
        --
        -- MachineId는 locations.machine_id 사용
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

            l.location_code
                AS current_location

        FROM products p

        LEFT JOIN locations l
            ON p.current_location_id =
               l.location_id

        WHERE p.product_id =
            @ProductId;


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