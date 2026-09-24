/*
============================================================
Procedure : usp_RecordQualityResult

[저장 프로시저를 사용하는 이유]

품질 검사 PASS / FAIL 이벤트가 발생하면
한 테이블에 결과만 INSERT하고 끝나는 것이 아니다.

하나의 품질 이벤트로 인해:

1. MQTT eventId 중복 여부 확인
2. quality_results 검사 결과 저장
3. products.quality_status 변경
4. 해당 LOT의 PASS + FAIL 수량 계산
5. PASS + FAIL = 60이면 LOT COMPLETED
6. 다음 WAITING LOT을 RUNNING으로 전환
7. 모든 LOT이 완료되면 WorkOrder COMPLETED

여러 테이블의 상태가 하나의 품질 이벤트를 기준으로
동시에 변경되어야 한다.

따라서 일부 작업만 성공하여 LOT / Product / 품질 데이터가
서로 불일치하는 것을 방지하기 위해
전체 로직을 TRANSACTION으로 처리한다.

현재 업무 규칙:
LOT 완료 = PASS + FAIL = 60
동시에 RUNNING LOT은 1개
재작업 없음
============================================================
*/
USE MES_SYSTEM;

CREATE PROCEDURE dbo.usp_RecordQualityResult
    @EventId                UNIQUEIDENTIFIER,
    @ProductId              BIGINT,
    @Result                 VARCHAR(10),
    @SourceProductType      NVARCHAR(50) = NULL,
    @InspectedAt            DATETIME2(3),
    @SourceSystem           VARCHAR(20) = 'GEMINI'
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @QualityMachineId INT,
        @LotId BIGINT,
        @WorkOrderId BIGINT,
        @CurrentLotSequence INT,
        @LotTargetQty INT,
        @ProcessedQty INT,
        @NextLotId BIGINT;

    BEGIN TRY

        BEGIN TRANSACTION;


        ----------------------------------------------------
        -- PASS / FAIL 값 검증
        ----------------------------------------------------
        IF @Result NOT IN ('PASS', 'FAIL')
        BEGIN
            THROW 50201, '품질 결과는 PASS 또는 FAIL이어야 합니다.', 1;
        END;


        ----------------------------------------------------
        -- 동일 MQTT EventId 중복 처리 방지
        ----------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM quality_results
            WHERE event_id = @EventId
        )
        BEGIN
            THROW 50202, '이미 처리된 품질 이벤트입니다.', 1;
        END;


        ----------------------------------------------------
        -- 대상 제품 및 소속 LOT 정보 조회
        ----------------------------------------------------
        SELECT
            @LotId = p.lot_id,
            @WorkOrderId = l.work_order_id,
            @CurrentLotSequence = l.lot_sequence,
            @LotTargetQty = l.target_qty

        FROM products p WITH (UPDLOCK, HOLDLOCK)

        INNER JOIN lots l
            ON p.lot_id = l.lot_id

        WHERE p.product_id = @ProductId;


        IF @LotId IS NULL
        BEGIN
            THROW 50203, '존재하지 않는 제품입니다.', 1;
        END;


        ----------------------------------------------------
        -- 제품 하나에는 최종 품질 결과가 1건만 존재
        ----------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM quality_results
            WHERE product_id = @ProductId
        )
        BEGIN
            THROW 50204, '이미 품질 판정이 완료된 제품입니다.', 1;
        END;


        ----------------------------------------------------
        -- QUALITY_CHECK 설비 ID 조회
        ----------------------------------------------------
        SELECT
            @QualityMachineId = machine_id
        FROM machines
        WHERE machine_code = 'QUALITY_CHECK'
          AND is_active = 1;


        IF @QualityMachineId IS NULL
        BEGIN
            THROW 50205, 'QUALITY_CHECK 설비가 등록되어 있지 않습니다.', 1;
        END;


        ----------------------------------------------------
        -- 품질 결과 저장
        ----------------------------------------------------
        INSERT INTO quality_results
        (
            event_id,
            product_id,
            machine_id,
            result,
            source_product_type,
            inspected_at,
            source_system
        )
        VALUES
        (
            @EventId,
            @ProductId,
            @QualityMachineId,
            @Result,
            @SourceProductType,
            @InspectedAt,
            @SourceSystem
        );


        ----------------------------------------------------
        -- 제품 품질 상태 갱신
        ----------------------------------------------------
        UPDATE products
        SET quality_status = @Result
        WHERE product_id = @ProductId;


        ----------------------------------------------------
        -- 현재 LOT의 처리 완료 수량 계산
        --
        -- 품질 결과가 존재한다는 것은
        -- PASS 또는 FAIL 판정을 받은 제품이라는 의미
        ----------------------------------------------------
        SELECT
            @ProcessedQty = COUNT(*)

        FROM quality_results qr

        INNER JOIN products p
            ON qr.product_id = p.product_id

        WHERE p.lot_id = @LotId;


        ----------------------------------------------------
        -- LOT 목표 수량에 도달한 경우 LOT 완료 처리
        ----------------------------------------------------
        IF @ProcessedQty >= @LotTargetQty
        BEGIN

            UPDATE lots
            SET
                status = 'COMPLETED',
                completed_at = @InspectedAt

            WHERE lot_id = @LotId
              AND status <> 'COMPLETED';


            ------------------------------------------------
            -- 다음 WAITING LOT 조회
            ------------------------------------------------
            SELECT TOP 1
                @NextLotId = lot_id

            FROM lots WITH (UPDLOCK, HOLDLOCK)

            WHERE work_order_id = @WorkOrderId
              AND status = 'WAITING'
              AND lot_sequence > @CurrentLotSequence

            ORDER BY lot_sequence;


            ------------------------------------------------
            -- 다음 LOT이 존재하면 RUNNING
            ------------------------------------------------
            IF @NextLotId IS NOT NULL
            BEGIN

                UPDATE lots
                SET
                    status = 'RUNNING',
                    started_at =
                        COALESCE(
                            started_at,
                            @InspectedAt
                        )

                WHERE lot_id = @NextLotId;


            END
            ELSE
            BEGIN

                ------------------------------------------------
                -- 남은 LOT이 없고 모든 LOT이 완료됐는지 확인
                ------------------------------------------------
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM lots
                    WHERE work_order_id = @WorkOrderId
                      AND status <> 'COMPLETED'
                )
                BEGIN

                    UPDATE work_orders
                    SET
                        status = 'COMPLETED',
                        completed_at = @InspectedAt

                    WHERE work_order_id = @WorkOrderId;

                END;

            END;

        END;


        COMMIT TRANSACTION;


        ----------------------------------------------------
        -- 현재 생산 상태 반환
        ----------------------------------------------------
        SELECT
            wo.work_order_code,
            wo.status AS work_order_status,

            l.lot_code,
            l.status AS lot_status,

            @ProcessedQty AS processed_qty,
            l.target_qty,

            SUM(
                CASE
                    WHEN qr.result = 'PASS'
                    THEN 1
                    ELSE 0
                END
            ) AS good_qty,

            SUM(
                CASE
                    WHEN qr.result = 'FAIL'
                    THEN 1
                    ELSE 0
                END
            ) AS defect_qty

        FROM work_orders wo

        INNER JOIN lots l
            ON wo.work_order_id = l.work_order_id

        INNER JOIN products p
            ON l.lot_id = p.lot_id

        LEFT JOIN quality_results qr
            ON p.product_id = qr.product_id

        WHERE l.lot_id = @LotId

        GROUP BY
            wo.work_order_code,
            wo.status,
            l.lot_code,
            l.status,
            l.target_qty;


    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;

END;
GO