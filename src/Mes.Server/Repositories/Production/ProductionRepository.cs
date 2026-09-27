using Mes.Server.Database;
using Mes.Server.Models.Production;
using Microsoft.Data.SqlClient;

namespace Mes.Server.Repositories.Production;

public class ProductionRepository
    : IProductionRepository
{
    private readonly MesDbConnectionFactory
        _connectionFactory;


    public ProductionRepository(
        MesDbConnectionFactory connectionFactory)
    {
        _connectionFactory =
            connectionFactory;
    }


    public async Task<CurrentProductionInfo?>
        GetCurrentAsync()
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();


        const string sql = """
            DECLARE
                @ActiveWorkOrderCount INT,
                @WorkOrderId BIGINT,
                @LotId BIGINT;


            ------------------------------------------------
            -- 현재 활성 WorkOrder 확인
            ------------------------------------------------

            SELECT
                @ActiveWorkOrderCount = COUNT(*)
            FROM work_orders
            WHERE status IN (
                'RUNNING',
                'PAUSE'
            );


            IF @ActiveWorkOrderCount = 0
            BEGIN
                RETURN;
            END;


            IF @ActiveWorkOrderCount > 1
            BEGIN
                THROW 50601,
                    '활성 상태의 WorkOrder가 2개 이상 존재합니다.',
                    1;
            END;


            SELECT
                @WorkOrderId =
                    work_order_id
            FROM work_orders
            WHERE status IN (
                'RUNNING',
                'PAUSE'
            );


            ------------------------------------------------
            -- 현재 RUNNING LOT
            ------------------------------------------------

            SELECT TOP 1
                @LotId =
                    lot_id
            FROM lots
            WHERE work_order_id =
                    @WorkOrderId

              AND status =
                    'RUNNING'

            ORDER BY
                lot_sequence;


            ------------------------------------------------
            -- WorkOrder + LOT 생산실적
            ------------------------------------------------

            SELECT
                wo.work_order_id,
                wo.work_order_code,
                wo.status,
                wo.target_qty,

                ISNULL(
                    wo_stats.produced_qty,
                    0
                )
                    AS wo_produced_qty,

                ISNULL(
                    wo_stats.good_qty,
                    0
                )
                    AS wo_good_qty,

                ISNULL(
                    wo_stats.defect_qty,
                    0
                )
                    AS wo_defect_qty,


                l.lot_id,
                l.lot_code,
                l.lot_sequence,
                l.status
                    AS lot_status,
                l.target_qty
                    AS lot_target_qty,

                ISNULL(
                    lot_stats.produced_qty,
                    0
                )
                    AS lot_produced_qty,

                ISNULL(
                    lot_stats.good_qty,
                    0
                )
                    AS lot_good_qty,

                ISNULL(
                    lot_stats.defect_qty,
                    0
                )
                    AS lot_defect_qty


            FROM work_orders wo


            OUTER APPLY
            (
                SELECT

                    SUM(
                        CASE
                            WHEN p.quality_status
                                IN ('PASS', 'FAIL')
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS produced_qty,

                    SUM(
                        CASE
                            WHEN p.quality_status = 'PASS'
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS good_qty,

                    SUM(
                        CASE
                            WHEN p.quality_status = 'FAIL'
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS defect_qty

                FROM lots wl

                INNER JOIN products p
                    ON wl.lot_id =
                       p.lot_id

                WHERE wl.work_order_id =
                      wo.work_order_id

            ) wo_stats


            LEFT JOIN lots l
                ON l.lot_id =
                   @LotId


            OUTER APPLY
            (
                SELECT

                    SUM(
                        CASE
                            WHEN p.quality_status
                                IN ('PASS', 'FAIL')
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS produced_qty,

                    SUM(
                        CASE
                            WHEN p.quality_status = 'PASS'
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS good_qty,

                    SUM(
                        CASE
                            WHEN p.quality_status = 'FAIL'
                            THEN 1
                            ELSE 0
                        END
                    )
                        AS defect_qty

                FROM products p

                WHERE p.lot_id =
                      l.lot_id

            ) lot_stats


            WHERE wo.work_order_id =
                  @WorkOrderId;
            """;


        await using var command =
            new SqlCommand(
                sql,
                connection
            );


        await using var reader =
            await command
                .ExecuteReaderAsync();


        if (!await reader.ReadAsync())
        {
            return null;
        }


        var result =
            new CurrentProductionInfo
            {
                WorkOrder =
                    new WorkOrderProductionInfo
                    {
                        WorkOrderId =
                            reader.GetInt64(0),

                        WorkOrderCode =
                            reader.GetString(1),

                        Status =
                            reader.GetString(2),

                        TargetQty =
                            reader.GetInt32(3),

                        ProducedQty =
                            reader.GetInt32(4),

                        GoodQty =
                            reader.GetInt32(5),

                        DefectQty =
                            reader.GetInt32(6)
                    }
            };


        // 현재 RUNNING LOT이 존재하는 경우
        if (!reader.IsDBNull(7))
        {
            result.CurrentLot =
                new LotProductionInfo
                {
                    LotId =
                        reader.GetInt64(7),

                    LotCode =
                        reader.GetString(8),

                    LotSequence =
                        reader.GetInt16(9),

                    Status =
                        reader.GetString(10),

                    TargetQty =
                        reader.GetInt16(11),

                    ProducedQty =
                        reader.GetInt32(12),

                    GoodQty =
                        reader.GetInt32(13),

                    DefectQty =
                        reader.GetInt32(14)
                };
        }


        return result;
    }
}