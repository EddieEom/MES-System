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

        // 시간 표시용 CancellationTokenSource
        private CancellationTokenSource? _clockCts;

        // mes.Server API 호출용 HttpClient
        private readonly MesServerClient _mesServerClient;

        // 대시보드 Polling용 CancellationTokenSource
        private CancellationTokenSource? _dashboardCts;

        // 마지막으로 조회한 알람과 공정 이벤트의 시그니처를 저장하여 중복 업데이트 방지
        private string _lastAlarmSignature = string.Empty;
        private string _lastProcessEventSignature = string.Empty;

        // QUALITY CHECK UI 상태 관리
        private string? _latestPressState;
        private string? _latestQualityRoute;
        private string? _latestPressProductCode;

        // 현재 HMI 세션에서 PLC START가 수행되었는지 여부
        private bool _plcSessionStarted = false;

        // PLC 제어 명령 전송 중 여부를 나타내는 플래그 (중복 클릭 방지용)
        private bool _plcCommandInProgress = false;

        // 전체 화면 모드 여부를 나타내는 플래그
        private bool _isFullScreen = false;

        public MainForm()
        {
            InitializeComponent();
            ApplyDashboardTheme();
            //===============================
            // DataGridView 컬럼 자동 생성 비활성화
            //===============================
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

                // 최근 생산 정보가 없으면 UI 초기화
                if (production == null)
                {
                    ResetProductionUi();
                    ResetLotUi();
                    return;
                }

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
                // 최근 작업지시가 없으면 UI 초기화
                else
                {
                    ResetProductionUi();
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

                    double progress =
                        Math.Clamp(
                            lot.ProgressRate,
                            0,
                            100
                        );

                    Pnl_LotProgressFill.Width =
                        (int)(
                            Pnl_LotProgressTrack.ClientSize.Width
                            * progress / 100.0
                        );
                }
                // 최근 LOT이 없으면 UI 초기화
                else
                {
                    ResetLotUi();
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

                // 최근 공정 제품이 없으면 UI 초기화
                if (product == null)
                {
                    ResetCurrentProductUi();
                    return;
                }

                Lbl_ProductCode.Text =
                    product.ProductCode ?? "---";

                Lbl_ProductStatus.Text =
                    product.Status ?? "---";

                Lbl_ProductQuality.Text =
                    product.QualityStatus ?? "---";

                Lbl_ProductLocation.Text =
                    product.CurrentLocationName ?? "---";
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"현재 제품 조회 실패: {ex.Message}");
            }
        }

        private void ResetCurrentProductUi()
        {
            // 최근 공정 제품이 없으면 UI 초기화
            Lbl_ProductCode.Text = "---";
            Lbl_ProductStatus.Text = "---";
            Lbl_ProductQuality.Text = "---";
            Lbl_ProductLocation.Text = "---";
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
                // 활성 알람 조회
                List<AlarmInfo> alarms =
                    await _mesServerClient.GetActiveAlarmsAsync();

                // 활성 알람 수 표시
                Lbl_ActiveAlarmCount.Text =
                    $"{alarms.Count}건";

                // 활성 알람 시그니처 생성 (중복 업데이트 방지용)
                string signature =
                    string.Join(
                        "||",
                        alarms.Select(a =>
                            $"{a.OccurredAt}|{a.MachineCode}|{a.Severity}|{a.AlarmCode}|{a.AlarmMessage}")
                    );

                // 실제 데이터가 바뀐 경우에만 Grid 갱신
                if (_lastAlarmSignature != signature)
                {
                    _lastAlarmSignature = signature;

                    Dgv_ActiveAlarms.DataSource = null;
                    Dgv_ActiveAlarms.DataSource = alarms;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"활성 알람 조회 실패: {ex.Message}"
                );
            }
        }

        // ====================================================
        // 설비 상태에 따라 UI 색상 및 텍스트 업데이트
        // ====================================================
        private void UpdateMachineProcessState(
            Panel panel,
            Label label,
            string? machineState)
        {
            string state =
                machineState?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            switch (state)
            {
                // 설비 실제 작업 중
                case "BUSY":

                    label.Text = "진행 중";

                    panel.BackColor =
                        Color.FromArgb(254, 243, 199);

                    label.ForeColor =
                        Color.FromArgb(180, 83, 9);

                    break;


                // 작업은 끝났지만 다음 설비로 배출 대기
                case "BLOCKED":

                    label.Text = "공정 완료 / 배출 대기";

                    panel.BackColor =
                        Color.FromArgb(219, 234, 254);

                    label.ForeColor =
                        Color.FromArgb(29, 78, 216);

                    break;


                // 제품 없음 / 대기 상태
                case "IDLE":

                    label.Text = "대기";

                    panel.BackColor =
                        Color.FromArgb(248, 250, 252);

                    label.ForeColor =
                        Color.FromArgb(100, 116, 139);

                    break;


                // 설비 이상
                case "FAILED":

                    label.Text = "이상";

                    panel.BackColor =
                        Color.FromArgb(254, 226, 226);

                    label.ForeColor =
                        Color.FromArgb(220, 38, 38);

                    break;


                // 수리 상태
                case "REPAIR":

                    label.Text = "수리 중";

                    panel.BackColor =
                        Color.FromArgb(243, 232, 255);

                    label.ForeColor =
                        Color.FromArgb(126, 34, 206);

                    break;


                default:

                    label.Text = "---";

                    panel.BackColor =
                        Color.FromArgb(248, 250, 252);

                    label.ForeColor =
                        Color.FromArgb(100, 116, 139);

                    break;
            }
        }


        // =====================================================
        // 최근 설비 상태 조회
        // =====================================================
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

                // =====================================================
                // HMI 실행 후 아직 PLC START를 하지 않은 상태
                // DB에 남아 있는 이전 Blocked/Busy 상태를 표시하지 않고
                // 공정 흐름은 대기 상태로 표시
                // =====================================================

                if (!_plcSessionStarted)
                {
                    UpdateMachineProcessState(
                        Pnl_ProcessVm1,
                        Lbl_ProcessVm1State,
                        "IDLE"
                    );

                    UpdateMachineProcessState(
                        Pnl_ProcessPress,
                        Lbl_ProcessPressState,
                        "IDLE"
                    );

                    return;
                }

                // 최근 VM1 설비 상태가 있으면 UI 업데이트
                if (vm1 != null)
                {
                    Lbl_Vm1State.Text = vm1.State ?? "---";
                    Lbl_Vm1Entered.Text = vm1.PartsEntered.ToString("N0");
                    Lbl_Vm1Exited.Text = vm1.PartsExited.ToString("N0");
                    Lbl_Vm1Utilization.Text = $"{vm1.Utilization:F2} %";

                    // 공정 흐름 VM1 표시
                    UpdateMachineProcessState(
                        Pnl_ProcessVm1,
                        Lbl_ProcessVm1State,
                        vm1.State
                    );
                }
                // 최근 VM1 설비 상태가 없으면 UI 초기화
                else
                {
                    ResetVm1Ui();

                    UpdateMachineProcessState(
                        Pnl_ProcessVm1,
                        Lbl_ProcessVm1State,
                        null
                    );
                }

                // 최근 PRESS 설비 상태가 있으면 UI 업데이트
                if (press != null)
                {
                    Lbl_PressState.Text = press.State ?? "---";
                    Lbl_PressEntered.Text = press.PartsEntered.ToString("N0");
                    Lbl_PressExited.Text = press.PartsExited.ToString("N0");
                    Lbl_PressUtilization.Text = $"{press.Utilization:F2} %";

                    // 현재 PRESS 상태 저장
                    _latestPressState = press.State;

                    // 공정 흐름 PRESS 표시
                    UpdateMachineProcessState(
                        Pnl_ProcessPress,
                        Lbl_ProcessPressState,
                        press.State
                    );
                }
                // 최근 PRESS 설비 상태가 없으면 UI 초기화
                else
                {
                    ResetPressUi();

                    UpdateMachineProcessState(
                        Pnl_ProcessPress,
                        Lbl_ProcessPressState,
                        null
                    );
                    RefreshQualityProcessUi();

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


                // =========================================
                // 가장 최근 PRESS 진입 제품 찾기
                // =========================================

                ProcessEventInfo? latestPressEvent =
                    events
                        .Where(e =>
                            string.Equals(
                                e.ToLocationCode,
                                "PRESS",
                                StringComparison.OrdinalIgnoreCase
                            )
                        )
                        .OrderByDescending(e => e.EventTime)
                        .FirstOrDefault();


                _latestPressProductCode =
                    latestPressEvent?.ProductCode;


                // =========================================
                // 현재 PRESS 제품의 품질 Route만 찾기
                //
                // WAREHOUSE = PASS
                // NG_STACK  = FAIL
                // =========================================

                ProcessEventInfo? qualityRoute = null;


                if (!string.IsNullOrWhiteSpace(
                        _latestPressProductCode))
                {
                    qualityRoute =
                        events
                            .Where(e =>
                                string.Equals(
                                    e.ProductCode,
                                    _latestPressProductCode,
                                    StringComparison.OrdinalIgnoreCase
                                )
                                &&
                                (
                                    string.Equals(
                                        e.ToLocationCode,
                                        "WAREHOUSE",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                    ||
                                    string.Equals(
                                        e.ToLocationCode,
                                        "NG_STACK",
                                        StringComparison.OrdinalIgnoreCase
                                    )
                                )
                            )
                            .OrderByDescending(
                                e => e.EventTime
                            )
                            .FirstOrDefault();
                }


                _latestQualityRoute =
                    qualityRoute?.ToLocationCode;


                RefreshQualityProcessUi();


                // =========================================
                // 최근 공정 이벤트 Grid
                // =========================================

                string signature =
                    string.Join(
                        "||",
                        events.Select(e =>
                            $"{e.EventTime}|{e.ProductCode}|{e.EventType}|{e.FromLocationCode}|{e.ToLocationCode}|{e.Remarks}")
                    );


                if (_lastProcessEventSignature != signature)
                {
                    _lastProcessEventSignature =
                        signature;

                    Dgv_RecentEvents.DataSource =
                        null;

                    Dgv_RecentEvents.DataSource =
                        events;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"최근 공정 이벤트 조회 실패: {ex.Message}"
                );
            }
        }

        // =====================================================
        // 양품 / 불량 최종 경로 카드 표시
        // =====================================================
        private void SetQualityRouteCard(
            Panel panel,
            bool isPass,
            bool isActive)
        {
            Label[] labels = isPass
                ? new[] { Lbl_PassTitle, Lbl_PassDestination }
                : new[] { Lbl_FailTitle, Lbl_FailDestination };


            // 카드 내부 Label이 2개라는 현재 UI 기준
            if (labels.Length >= 2)
            {
                Label resultLabel = labels[0];
                Label destinationLabel = labels[1];


                if (isPass)
                {
                    resultLabel.Text =
                        isActive
                            ? "✓ 양품"
                            : "양품";

                    destinationLabel.Text =
                        "양품 보관";


                    resultLabel.ForeColor =
                        Color.FromArgb(22, 163, 74);

                    destinationLabel.ForeColor =
                        isActive
                            ? Color.FromArgb(21, 128, 61)
                            : Color.FromArgb(71, 85, 105);
                }
                else
                {
                    resultLabel.Text =
                        isActive
                            ? "✕ 불량"
                            : "불량";

                    destinationLabel.Text =
                        "불량품 적재";


                    resultLabel.ForeColor =
                        Color.FromArgb(220, 38, 38);

                    destinationLabel.ForeColor =
                        isActive
                            ? Color.FromArgb(185, 28, 28)
                            : Color.FromArgb(71, 85, 105);
                }
            }


            // =========================================
            // 카드 배경
            // =========================================

            if (!isActive)
            {
                panel.BackColor =
                    Color.FromArgb(248, 250, 252);

                return;
            }


            if (isPass)
            {
                // 활성 양품 경로
                panel.BackColor =
                    Color.FromArgb(187, 247, 208);
            }
            else
            {
                // 활성 불량 경로
                panel.BackColor =
                    Color.FromArgb(254, 202, 202);
            }
        }

        // =====================================================
        // QUALITY CHECK 공정 상태 표시
        // =====================================================
        private void RefreshQualityProcessUi()
        {
            string pressState =
                _latestPressState?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            string route =
                _latestQualityRoute?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            // PASS / FAIL 경로 기본 비활성 상태
            SetQualityRouteCard(
                Pnl_PassRoute,
                true,
                false
            );

            SetQualityRouteCard(
                Pnl_FailRoute,
                false,
                false
            );


            // =========================================
            // PRESS가 Idle이면
            // 이전 제품의 판정 결과를 표시하지 않음
            // =========================================

            if (pressState == "IDLE")
            {
                Lbl_ProcessQualityState.Text =
                    "대기";

                Pnl_ProcessQuality.BackColor =
                    Color.FromArgb(
                        248,
                        250,
                        252
                    );

                Lbl_ProcessQualityState.ForeColor =
                    Color.FromArgb(
                        100,
                        116,
                        139
                    );

                return;
            }


            // =========================================
            // 현재 PRESS 제품 PASS
            // =========================================

            if (route == "WAREHOUSE")
            {
                Lbl_ProcessQualityState.Text =
                    "양품 → 보관";

                SetProcessPass(
                    Pnl_ProcessQuality,
                    Lbl_ProcessQualityState
                );


                // 양품 경로 활성
                SetQualityRouteCard(
                    Pnl_PassRoute,
                    true,
                    true
                );


                // 불량 경로 비활성
                SetQualityRouteCard(
                    Pnl_FailRoute,
                    false,
                    false
                );

                return;
            }


            // =========================================
            // 현재 PRESS 제품 FAIL
            // =========================================

            if (route == "NG_STACK")
            {
                Lbl_ProcessQualityState.Text =
                    "불량 → 적재";

                SetProcessFail(
                    Pnl_ProcessQuality,
                    Lbl_ProcessQualityState
                );


                // 양품 경로 비활성
                SetQualityRouteCard(
                    Pnl_PassRoute,
                    true,
                    false
                );


                // 불량 경로 활성
                SetQualityRouteCard(
                    Pnl_FailRoute,
                    false,
                    true
                );

                return;
            }


            // =========================================
            // PRESS 작업 중이지만
            // 아직 품질 판정 전
            // =========================================

            if (
                pressState == "BUSY"
                ||
                pressState == "BLOCKED"
            )
            {
                Lbl_ProcessQualityState.Text =
                    "검사 대기";

                Pnl_ProcessQuality.BackColor =
                    Color.FromArgb(
                        254,
                        243,
                        199
                    );

                Lbl_ProcessQualityState.ForeColor =
                    Color.FromArgb(
                        180,
                        83,
                        9
                    );

                return;
            }


            // =========================================
            // 데이터 없음
            // =========================================

            Lbl_ProcessQualityState.Text =
                "대기";

            Pnl_ProcessQuality.BackColor =
                Color.FromArgb(
                    248,
                    250,
                    252
                );

            Lbl_ProcessQualityState.ForeColor =
                Color.FromArgb(
                    100,
                    116,
                    139
                );
        }


        // =====================================================
        // PLC 제어 버튼 활성화/비활성화
        // =====================================================
        private void SetPlcControlButtonsEnabled(bool enabled)
        {
            Btn_Start.Enabled = enabled;
            Btn_Stop.Enabled = enabled;
            Btn_Reset.Enabled = enabled;
        }

        //=====================================================
        // START 버튼
        //=====================================================
        private async void Btn_Start_Click(
            object sender,
            EventArgs e)
        {
            // 중복 클릭 방지: 이미 명령 전송 중이면 무시
            if (_plcCommandInProgress)
                return;

            try
            {

                // PLC 제어 명령 전송 시작
                _plcCommandInProgress = true;
                SetPlcControlButtonsEnabled(false);

                // PLC START 명령 전송
                PlcControlResult? result =
                    await _mesServerClient.StartPlcAsync();

                // 명령 전송 결과가 있으면 메시지 박스 표시
                if (result != null)
                {
                    _plcSessionStarted = true;

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
                // PLC 제어 명령 전송 완료
                _plcCommandInProgress = false;
                SetPlcControlButtonsEnabled(true);
            }
        }


        //=====================================================
        // STOP 버튼
        //=====================================================
        private async void Btn_Stop_Click(
            object sender,
            EventArgs e)
        {
            if (_plcCommandInProgress)
                return;

            try
            {
                _plcCommandInProgress = true;
                SetPlcControlButtonsEnabled(false);

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
                _plcCommandInProgress = false;
                SetPlcControlButtonsEnabled(true);
            }
        }


        //=====================================================
        // RESET 버튼
        //=====================================================
        private async void Btn_Reset_Click(
            object sender,
            EventArgs e)
        {
            if (_plcCommandInProgress)
                return;

            try
            {
                _plcCommandInProgress = true;
                SetPlcControlButtonsEnabled(false);

                PlcControlResult? result =
                    await _mesServerClient.ResetPlcAsync();

                if (result != null)
                {
                    _plcSessionStarted = false;

                    UpdateMachineProcessState(
                        Pnl_ProcessVm1,
                        Lbl_ProcessVm1State,
                        "IDLE"
                    );

                    UpdateMachineProcessState(
                        Pnl_ProcessPress,
                        Lbl_ProcessPressState,
                        "IDLE"
                    );

                    _latestPressState = "IDLE";
                    _latestQualityRoute = null;
                    _latestPressProductCode = null;

                    RefreshQualityProcessUi();

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
                _plcCommandInProgress = false;
                SetPlcControlButtonsEnabled(true);
            }
        }

        // ==================================================
        // 공정 Fail UI 표시
        // ==================================================
        private void SetProcessFail(
            Panel panel,
            Label label)
        {
            panel.BackColor =
                Color.FromArgb(
                    254,
                    226,
                    226
                );

            label.ForeColor =
                Color.FromArgb(
                    220,
                    38,
                    38
                );
        }

        // =================================================
        // 공정 PASS UI 표시
        // =================================================
        private void SetProcessPass(
            Panel panel,
            Label label)
        {
            panel.BackColor =
                Color.FromArgb(
                    220,
                    252,
                    231
                );

            label.ForeColor =
                Color.FromArgb(
                    22,
                    163,
                    74
                );
        }

        // ====================================================
        // 최근 생산 정보가 없으면 UI 초기화
        // ===================================================
        private void ResetProductionUi()
        {
            // 현재 작업지시
            Lbl_WorkOrderCode.Text = "-";
            Lbl_WorkOrderStatus.Text = "-";
            Lbl_WorkOrderTarget.Text = "0 EA";
            Lbl_WorkOrderAchievement.Text = "0.00 %";

            // 생산 실적
            Lbl_Target.Text = "0 EA";
            Lbl_Produced.Text = "0 EA";
            Lbl_Good.Text = "0 EA";
            Lbl_Defect.Text = "0 EA";
            Lbl_Yield.Text = "0.00 %";
        }

        //====================================================
        // 최근 LOT 정보가 없으면 UI 초기화
        //====================================================
        private void ResetLotUi()
        {
            Lbl_LotCode.Text = "-";
            Lbl_LotStatus.Text = "-";
            Lbl_LotProduction.Text = "0 / 0 EA";
            Lbl_LotProgress.Text = "0.00 %";

            Pnl_LotProgressFill.Width = 0;
        }

        //====================================================
        // 최근 VM1 설비 상태가 없으면 UI 초기화
        //====================================================
        private void ResetVm1Ui()
        {
            Lbl_Vm1State.Text = "---";
            Lbl_Vm1Entered.Text = "0";
            Lbl_Vm1Exited.Text = "0";
            Lbl_Vm1Utilization.Text = "0.00 %";
        }

        //====================================================
        // 최근 PRESS 설비 상태가 없으면 UI 초기화
        //====================================================
        private void ResetPressUi()
        {
            Lbl_PressState.Text = "---";
            Lbl_PressEntered.Text = "0";
            Lbl_PressExited.Text = "0";
            Lbl_PressUtilization.Text = "0.00 %";
        }

        // ====================================================
        // 전체 화면 전환 (F11) 및 ESC 키 처리
        // ====================================================
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                // F11 키를 눌러 전체 화면 모드로 전환하도록 처리
                if (!_isFullScreen)
                {
                    // 전체 화면 모드로 전환
                    WindowState = FormWindowState.Normal;
                    FormBorderStyle = FormBorderStyle.None;
                    Bounds = Screen.FromControl(this).Bounds;
                    TopMost = true;

                    _isFullScreen = true;
                }
                // F11 키를 다시 눌러 전체 화면 모드에서 벗어나도록 처리
                else
                {
                    TopMost = false;
                    FormBorderStyle = FormBorderStyle.Sizable;
                    WindowState = FormWindowState.Maximized;

                    _isFullScreen = false;
                }
            }

            // ESC 키를 눌러 전체 화면 모드에서 벗어나도록 처리
            if (e.KeyCode == Keys.Escape && _isFullScreen)
            {
                TopMost = false;
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Maximized;

                _isFullScreen = false;
            }
        }
    }
}
