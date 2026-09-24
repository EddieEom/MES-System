USE MES_SYSTEM;
GO


/*
============================================================
Procedure : usp_CreateWorkOrder

[저장 프로시저를 사용하는 이유]

작업지시 1건을 생성할 때 단순히 work_orders 테이블만
INSERT 하는 것이 아니라,

1. work_orders 1건 생성
2. LOT 5건 생성
3. 각 LOT마다 Product 60건 생성
4. 총 Product 300건 생성

이 작업들이 하나의 업무 단위로 반드시 함께 성공해야 한다.

예를 들어 작업지시와 LOT은 생성됐는데 Product 생성 중
오류가 발생하면 불완전한 생산계획 데이터가 남게 된다.

따라서 전체 작업을 하나의 TRANSACTION으로 묶고,
중간에 하나라도 오류가 발생하면 전체 ROLLBACK하기 위해
Stored Procedure로 구현한다.

현재 생산 기준:
1 WorkOrder = 5 LOT
1 LOT       = 60 EA
총 목표      = 300 EA
============================================================
*/

CREATE PROCEDURE dbo.usp_CreateWorkOrder
    @WorkOrderCode      VARCHAR(30),
    @CreatedByUserId    BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE
        @WorkOrderId BIGINT,
        @LotId BIGINT,
        @LotSequence INT,
        @ProductSequence INT,
        @LotCode VARCHAR(40),
        @ProductCode VARCHAR(50),
        @CodeBody VARCHAR(30);

    BEGIN TRY

        BEGIN TRANSACTION;


        ----------------------------------------------------
        -- 동일한 작업지시 코드가 이미 존재하는지 확인
        ----------------------------------------------------
        IF EXISTS
        (
            SELECT 1
            FROM work_orders
            WHERE work_order_code = @WorkOrderCode
        )
        BEGIN
            THROW 50001, '이미 존재하는 작업지시 코드입니다.', 1;
        END;


        ----------------------------------------------------
        -- 사용자 ID가 전달된 경우 실제 사용자인지 확인
        ----------------------------------------------------
        IF @CreatedByUserId IS NOT NULL
           AND NOT EXISTS
           (
               SELECT 1
               FROM users
               WHERE user_id = @CreatedByUserId
                 AND is_active = 1
           )
        BEGIN
            THROW 50002, '유효하지 않은 사용자입니다.', 1;
        END;


        ----------------------------------------------------
        -- 작업지시 생성
        ----------------------------------------------------
        INSERT INTO work_orders
        (
            work_order_code,
            target_qty,
            lot_count,
            lot_size,
            status,
            created_by_user_id
        )
        VALUES
        (
            @WorkOrderCode,
            300,
            5,
            60,
            'IDLE',
            @CreatedByUserId
        );


        SET @WorkOrderId = SCOPE_IDENTITY();


        ----------------------------------------------------
        -- 코드 생성용
        -- WO-20260918-001
        -- → 20260918-001
        ----------------------------------------------------
        SET @CodeBody =
            CASE
                WHEN LEFT(@WorkOrderCode, 3) = 'WO-'
                    THEN SUBSTRING(
                        @WorkOrderCode,
                        4,
                        LEN(@WorkOrderCode)
                    )
                ELSE @WorkOrderCode
            END;


        ----------------------------------------------------
        -- LOT 5개 생성
        ----------------------------------------------------
        SET @LotSequence = 1;

        WHILE @LotSequence <= 5
        BEGIN

            SET @LotCode =
                'LOT-' +
                @CodeBody +
                '-' +
                RIGHT(
                    '00' + CAST(@LotSequence AS VARCHAR(2)),
                    2
                );


            INSERT INTO lots
            (
                work_order_id,
                lot_code,
                lot_sequence,
                target_qty,
                status
            )
            VALUES
            (
                @WorkOrderId,
                @LotCode,
                @LotSequence,
                60,
                'WAITING'
            );


            SET @LotId = SCOPE_IDENTITY();


            ------------------------------------------------
            -- 해당 LOT의 Product 60개 생성
            ------------------------------------------------
            SET @ProductSequence = 1;

            WHILE @ProductSequence <= 60
            BEGIN

                SET @ProductCode =
                    'PRD-' +
                    @CodeBody +
                    '-' +
                    RIGHT(
                        '00' + CAST(@LotSequence AS VARCHAR(2)),
                        2
                    ) +
                    '-' +
                    RIGHT(
                        '000' + CAST(@ProductSequence AS VARCHAR(3)),
                        3
                    );


                INSERT INTO products
                (
                    product_code,
                    lot_id,
                    sequence_no,
                    status,
                    quality_status,
                    current_location_id
                )
                VALUES
                (
                    @ProductCode,
                    @LotId,
                    @ProductSequence,
                    'WAITING',
                    'PENDING',
                    NULL
                );


                SET @ProductSequence =
                    @ProductSequence + 1;

            END;


            SET @LotSequence =
                @LotSequence + 1;

        END;


        COMMIT TRANSACTION;


        ----------------------------------------------------
        -- 생성 결과 반환
        ----------------------------------------------------
        SELECT
            @WorkOrderId AS work_order_id,
            @WorkOrderCode AS work_order_code,
            5 AS lot_count,
            300 AS product_count;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;

END;
GO