using Mes.Hmi.Api;
using Mes.Hmi.Models.Alarm;
using Mes.Hmi.Models.Control;
using Mes.Hmi.Models.Machine;
using Mes.Hmi.Models.Process;
using Mes.Hmi.Models.Production;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mes.Hmi.Forms
{
    public partial class MainForm : Form
    {
        // CencellationTokenSource 생성
        private CancellationTokenSource? _clockCts;
        private readonly MesServerClient _mesServerClient;
        private CancellationTokenSource? _dashboardCts;


        public MainForm()
        {
            InitializeComponent();
            Dgv_RecentEvents.AutoGenerateColumns = false;
            Dgv_ActiveAlarms.AutoGenerateColumns = false;

            _mesServerClient = new MesServerClient("https://localhost:7075");

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // ================================
            // 현재 시간 표시 시작
            // ================================

            _clockCts =
                new CancellationTokenSource();

            _ = RunClockAsync(
                _clockCts.Token
            );


            // ================================
            // MES Dashboard 실시간 Polling 시작
            // ================================

            _dashboardCts =
                new CancellationTokenSource();

            _ = RunDashboardPollingAsync(
                _dashboardCts.Token
            );
        }

        // Invoke()를 쓰지 않아도 되는 이유는 RunClockAsync()를
        // UI Thread에서 시작했고 ConfigureAwait(false)도 사용하지 않았기 때문에 await 이후에도 UI 컨텍스트로 돌아온다.
        private async Task RunClockAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    Lbl_CurrentTime.Text =
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    await Task.Delay(1000, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Form 종료 시 정상적으로 취소됨
            }
        }

        //=====================================================
        // MES Dashboard Polling
        //
        // 1초마다 Mes.Server에서 최신 상태 조회
        // Timer 사용하지 않음
        // Task + CancellationToken 방식
        //=====================================================

        private async Task RunDashboardPollingAsync(
            CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    // =====================================
                    // 각 API 요청 시작
                    // =====================================

                    Task productionTask =
                        LoadProductionAsync();

                    Task productTask =
                        LoadCurrentProductAsync();

                    Task alarmTask =
                        LoadActiveAlarmsAsync();

                    Task machineTask =
                        LoadMachineStatusAsync();

                    Task processEventTask =
                        LoadRecentProcessEventsAsync();


                    // =====================================
                    // 모두 완료될 때까지 대기
                    // =====================================

                    await Task.WhenAll(
                        productionTask,
                        productTask,
                        alarmTask,
                        machineTask,
                        processEventTask
                    );


                    // =====================================
                    // 1초 대기
                    // =====================================

                    await Task.Delay(
                        1000,
                        token
                    );
                }
            }
            catch (OperationCanceledException)
                when (token.IsCancellationRequested)
            {
                // Form 종료 시 정상 취소
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[HMI POLLING ERROR] {ex.Message}"
                );
            }
        }

        private async Task LoadProductionAsync()
        {
            try
            {
                ProductionInfo? production =
                    await _mesServerClient.GetCurrentProductionAsync();

                if (production == null)
                    return;

                // =========================
                // 현재 작업지시
                // =========================
                if (production.WorkOrder != null)
                {
                    var wo = production.WorkOrder;

                    Lbl_WorkOrderCode.Text = wo.WorkOrderCode ?? "-";
                    Lbl_WorkOrderStatus.Text = wo.Status ?? "-";
                    Lbl_WorkOrderTarget.Text = $"{wo.TargetQty:N0} EA";
                    Lbl_WorkOrderAchievement.Text =
                        $"{wo.AchievementRate:F2} %";

                    // =========================
                    // 생산 실적
                    // =========================
                    Lbl_Target.Text = $"{wo.TargetQty:N0} EA";
                    Lbl_Produced.Text = $"{wo.ProducedQty:N0} EA";
                    Lbl_Good.Text = $"{wo.GoodQty:N0} EA";
                    Lbl_Defect.Text = $"{wo.DefectQty:N0} EA";
                    Lbl_Yield.Text = $"{wo.YieldRate:F2} %";
                }

                // =========================
                // 현재 LOT
                // =========================
                if (production.CurrentLot != null)
                {
                    var lot = production.CurrentLot;

                    Lbl_LotCode.Text = lot.LotCode ?? "-";
                    Lbl_LotStatus.Text = lot.Status ?? "-";

                    Lbl_LotProduction.Text =
                        $"{lot.ProducedQty:N0} / {lot.TargetQty:N0} EA";

                    Lbl_LotProgress.Text =
                        $"{lot.ProgressRate:F2} %";

                    Pgb_LotProgress.Value =
                        Math.Clamp(
                            (int)Math.Round(lot.ProgressRate),
                            0,
                            100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"생산 정보 조회 실패: {ex.Message}");
            }
        }

        private async Task LoadCurrentProductAsync()
        {
            try
            {
                CurrentProductInfo? product =
                    await _mesServerClient.GetCurrentProcessProductAsync();

                if (product == null)
                    return;

                Lbl_ProductCode.Text =
                    product.ProductCode ?? "---";

                Lbl_ProductStatus.Text =
                    product.Status ?? "---";

                Lbl_ProductQuality.Text =
                    product.QualityStatus ?? "---";

                Lbl_ProductLocation.Text =
                    product.CurrentLocationName ?? "---";

                UpdateProcessFlow(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"현재 제품 조회 실패: {ex.Message}");
            }
        }

        protected override void OnFormClosed(
    FormClosedEventArgs e)
        {
            // ================================
            // Clock 종료
            // ================================

            _clockCts?.Cancel();
            _clockCts?.Dispose();
            _clockCts = null;


            // ================================
            // Dashboard Polling 종료
            // ================================

            _dashboardCts?.Cancel();
            _dashboardCts?.Dispose();
            _dashboardCts = null;


            base.OnFormClosed(e);
        }

        private async Task LoadActiveAlarmsAsync()
        {
            try
            {
                List<AlarmInfo> alarms =
                    await _mesServerClient.GetActiveAlarmsAsync();

                Lbl_ActiveAlarmCount.Text = $"{alarms.Count}건";

                Dgv_ActiveAlarms.DataSource = alarms;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"활성 알람 조회 실패: {ex.Message}");
            }
        }

        private async Task LoadMachineStatusAsync()
        {
            try
            {
                Task<MachineStatusInfo?> vm1Task =
                    _mesServerClient.GetLatestMachineStatusAsync("VM1");

                Task<MachineStatusInfo?> pressTask =
                    _mesServerClient.GetLatestMachineStatusAsync("PRESS");

                await Task.WhenAll(vm1Task, pressTask);

                MachineStatusInfo? vm1 = await vm1Task;
                MachineStatusInfo? press = await pressTask;

                if (vm1 != null)
                {
                    Lbl_Vm1State.Text = vm1.State ?? "---";
                    Lbl_Vm1Entered.Text = vm1.PartsEntered.ToString("N0");
                    Lbl_Vm1Exited.Text = vm1.PartsExited.ToString("N0");
                    Lbl_Vm1Utilization.Text = $"{vm1.Utilization:F2} %";
                }

                if (press != null)
                {
                    Lbl_PressState.Text = press.State ?? "---";
                    Lbl_PressEntered.Text = press.PartsEntered.ToString("N0");
                    Lbl_PressExited.Text = press.PartsExited.ToString("N0");
                    Lbl_PressUtilization.Text = $"{press.Utilization:F2} %";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"설비 상태 조회 실패: {ex.Message}");
            }
        }

        //=====================================================
        // 최근 공정 이벤트 조회
        // GET /api/process/recent?count=10
        //=====================================================
        private async Task LoadRecentProcessEventsAsync()
        {
            try
            {
                List<ProcessEventInfo> events =
                    await _mesServerClient
                        .GetRecentProcessEventsAsync(10);

                Dgv_RecentEvents.DataSource = events;
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"최근 공정 이벤트 조회 실패: {ex.Message}"
                );
            }
        }


        //=====================================================
        // START 버튼
        //=====================================================
        private async void Btn_Start_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Btn_Start.Enabled = false;

                PlcControlResult? result =
                    await _mesServerClient.StartPlcAsync();

                if (result != null)
                {
                    MessageBox.Show(
                        $"{result.Command} 명령 전송 완료",
                        "PLC 제어",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"START 명령 전송 실패\n\n{ex.Message}",
                    "PLC 제어 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Btn_Start.Enabled = true;
            }
        }


        //=====================================================
        // STOP 버튼
        //=====================================================
        private async void Btn_Stop_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Btn_Stop.Enabled = false;

                PlcControlResult? result =
                    await _mesServerClient.StopPlcAsync();

                if (result != null)
                {
                    MessageBox.Show(
                        $"{result.Command} 명령 전송 완료",
                        "PLC 제어",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"STOP 명령 전송 실패\n\n{ex.Message}",
                    "PLC 제어 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Btn_Stop.Enabled = true;
            }
        }


        //=====================================================
        // RESET 버튼
        //=====================================================
        private async void Btn_Reset_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Btn_Reset.Enabled = false;

                PlcControlResult? result =
                    await _mesServerClient.ResetPlcAsync();

                if (result != null)
                {
                    MessageBox.Show(
                        $"{result.Command} 명령 전송 완료",
                        "PLC 제어",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"RESET 명령 전송 실패\n\n{ex.Message}",
                    "PLC 제어 오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                Btn_Reset.Enabled = true;
            }
        }

        //=====================================================
        // 현재 제품 공정 흐름 표시
        //=====================================================
        private void UpdateProcessFlow(
            CurrentProductInfo product)
        {
            // 기본값
            Lbl_ProcessVm1State.Text = "---";
            Lbl_ProcessPressState.Text = "---";
            Lbl_ProcessQualityState.Text = "---";

            string location =
                product.CurrentLocationCode?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            switch (location)
            {
                case "VM1":
                    Lbl_ProcessVm1State.Text = "진행 중";
                    break;

                case "PRESS":
                    Lbl_ProcessVm1State.Text = "완료";
                    Lbl_ProcessPressState.Text = "진행 중";
                    break;

                case "WAREHOUSE":
                    Lbl_ProcessVm1State.Text = "완료";
                    Lbl_ProcessPressState.Text = "완료";
                    Lbl_ProcessQualityState.Text = "PASS";
                    break;

                case "NG_STACK":
                    Lbl_ProcessVm1State.Text = "완료";
                    Lbl_ProcessPressState.Text = "완료";
                    Lbl_ProcessQualityState.Text = "FAIL";
                    break;
            }
        }
    }
}
