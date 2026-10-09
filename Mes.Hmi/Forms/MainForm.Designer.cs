using System.Drawing;
using System.Windows.Forms;

namespace Mes.Hmi.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Label Lbl_LotProgressTitle;
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle9 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle10 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle11 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle12 = new DataGridViewCellStyle();
            Tlp_ProcessVm1Labels = new TableLayoutPanel();
            Tlp_ProcessPressLabels = new TableLayoutPanel();
            Tlp_ProcessQualityLabels = new TableLayoutPanel();
            Tlp_PassRouteLabels = new TableLayoutPanel();
            Tlp_FailRouteLabels = new TableLayoutPanel();
            Tlp_Root = new TableLayoutPanel();
            Pnl_Header = new Panel();
            Lbl_CurrentTime = new Label();
            Lbl_Title = new Label();
            Pnl_Content = new Panel();
            Tlp_ContentRoot = new TableLayoutPanel();
            Tlp_SummaryCards = new TableLayoutPanel();
            Pnl_ProductionCard = new DashboardCardPanel();
            tableLayoutPanel3 = new TableLayoutPanel();
            Lbl_Yield = new Label();
            Lbl_YieldTitle = new Label();
            Lbl_Defect = new Label();
            Lbl_DefectTitle = new Label();
            Lbl_Good = new Label();
            Lbl_GoodTitle = new Label();
            Lbl_Produced = new Label();
            Lbl_ProducedTitle = new Label();
            Lbl_Target = new Label();
            Lbl_ProductionTitle = new Label();
            Lbl_TargetTitle = new Label();
            Pnl_ProductCard = new DashboardCardPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            Lbl_ProductLocation = new Label();
            Lbl_ProductLocationTitle = new Label();
            Lbl_ProductQuality = new Label();
            Lbl_ProductStatus = new Label();
            Lbl_ProductCode = new Label();
            Lbl_ProductTitle = new Label();
            Lbl_ProductCodeTitle = new Label();
            Lbl_ProductStatusTitle = new Label();
            Lbl_ProductQualityTitle = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            Pnl_LotCard = new DashboardCardPanel();
            Tlp_Lot = new TableLayoutPanel();
            Lbl_LotProgress = new Label();
            Lbl_LotProduction = new Label();
            Lbl_LotQuantityTitle = new Label();
            Lbl_LotStatus = new Label();
            Lbl_LotStatusTitle = new Label();
            Lbl_LotCode = new Label();
            Lbl_LotCodeTitle = new Label();
            Lbl_LotTitle = new Label();
            Pnl_LotProgressTrack = new Panel();
            Pnl_LotProgressFill = new Panel();
            Pnl_WorkOrderCard = new DashboardCardPanel();
            Tlp_WorkOrder = new TableLayoutPanel();
            Lbl_WorkOrderAchievement = new Label();
            Lbl_WorkOrderTarget = new Label();
            Lbl_WorkOrderStatus = new Label();
            Lbl_WorkOrderAchievementTitle = new Label();
            Lbl_WorkOrderTargetTitle = new Label();
            Lbl_WorkOrderStatusTitle = new Label();
            Lbl_WorkOrderTitle = new Label();
            Lbl_WorkOrderCodeTitle = new Label();
            Lbl_WorkOrderCode = new Label();
            Tlp_DashboardBody = new TableLayoutPanel();
            Pnl_MachineArea = new DashboardCardPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            Lbl_MachineAreaTitle = new Label();
            Pnl_Vm1Card = new DashboardCardPanel();
            Tlp_Vm1 = new TableLayoutPanel();
            Lbl_Vm1Utilization = new Label();
            Lbl_Vm1UtilizationTitle = new Label();
            Lbl_Vm1Exited = new Label();
            Lbl_Vm1ExitedTitle = new Label();
            Lbl_Vm1Entered = new Label();
            Lbl_Vm1EnteredTitle = new Label();
            Lbl_Vm1State = new Label();
            Lbl_Vm1Title = new Label();
            Lbl_Vm1StateTitle = new Label();
            Pnl_PressCard = new DashboardCardPanel();
            Tlp_Press = new TableLayoutPanel();
            Lbl_PressUtilization = new Label();
            Lbl_PressUtilizationTitle = new Label();
            Lbl_PressExited = new Label();
            Lbl_PressExitedTitle = new Label();
            Lbl_PressEntered = new Label();
            Lbl_PressEnteredTitle = new Label();
            Lbl_PressState = new Label();
            Lbl_PressStateTitle = new Label();
            Lbl_PressTitle = new Label();
            Pnl_ProcessFlowArea = new DashboardCardPanel();
            Tlp_AlarmArea = new TableLayoutPanel();
            Pnl_AlarmHeader = new Panel();
            Lbl_ActiveAlarmCount = new Label();
            Lbl_AlarmAreaTitle = new Label();
            Dgv_ActiveAlarms = new Krypton.Toolkit.KryptonDataGridView();
            colOccurredAt = new DataGridViewTextBoxColumn();
            colMachineCode = new DataGridViewTextBoxColumn();
            colSeverity = new DataGridViewTextBoxColumn();
            colAlarmCode = new DataGridViewTextBoxColumn();
            colAlarmMessage = new DataGridViewTextBoxColumn();
            Pnl_AlarmArea = new DashboardCardPanel();
            Tlp_ProcessFlow = new TableLayoutPanel();
            Lbl_ProcessFlowTitle = new Label();
            Tlp_ProcessMain = new TableLayoutPanel();
            Pnl_ProcessVm1 = new DashboardCardPanel();
            Lbl_ProcessVm1State = new Label();
            Lbl_ProcessVm1Title = new Label();
            Pnl_ProcessPress = new DashboardCardPanel();
            Lbl_ProcessPressState = new Label();
            Lbl_ProcessPressTitle = new Label();
            Lbl_ArrowPressQuality = new Label();
            Pnl_ProcessQuality = new DashboardCardPanel();
            Lbl_ProcessQualityState = new Label();
            Lbl_ProcessQualityTitle = new Label();
            Lbl_ArrowVm1Press = new Label();
            Tlp_ProcessRoute = new TableLayoutPanel();
            Pnl_PassRoute = new DashboardCardPanel();
            Lbl_PassDestination = new Label();
            Lbl_PassTitle = new Label();
            Pnl_FailRoute = new DashboardCardPanel();
            Lbl_FailDestination = new Label();
            Lbl_FailTitle = new Label();
            Pnl_EventArea = new DashboardCardPanel();
            Tlp_EventArea = new TableLayoutPanel();
            Lbl_EventAreaTitle = new Label();
            Dgv_RecentEvents = new Krypton.Toolkit.KryptonDataGridView();
            Col_EventTime = new DataGridViewTextBoxColumn();
            Col_ProductCode = new DataGridViewTextBoxColumn();
            Col_EventType = new DataGridViewTextBoxColumn();
            Col_FromLocation = new DataGridViewTextBoxColumn();
            Col_ToLocation = new DataGridViewTextBoxColumn();
            Col_Remarks = new DataGridViewTextBoxColumn();
            Pnl_ControlArea = new Panel();
            Tlp_Control = new TableLayoutPanel();
            Btn_Reset = new Krypton.Toolkit.KryptonButton();
            Btn_Stop = new Krypton.Toolkit.KryptonButton();
            Btn_Start = new Krypton.Toolkit.KryptonButton();
            Lbl_LotProgressTitle = new Label();
            Tlp_Root.SuspendLayout();
            Pnl_Header.SuspendLayout();
            Pnl_Content.SuspendLayout();
            Tlp_ContentRoot.SuspendLayout();
            Tlp_SummaryCards.SuspendLayout();
            Pnl_ProductionCard.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            Pnl_ProductCard.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            Pnl_LotCard.SuspendLayout();
            Tlp_Lot.SuspendLayout();
            Pnl_LotProgressTrack.SuspendLayout();
            Pnl_WorkOrderCard.SuspendLayout();
            Tlp_WorkOrder.SuspendLayout();
            Tlp_DashboardBody.SuspendLayout();
            Pnl_MachineArea.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            Pnl_Vm1Card.SuspendLayout();
            Tlp_Vm1.SuspendLayout();
            Pnl_PressCard.SuspendLayout();
            Tlp_Press.SuspendLayout();
            Pnl_ProcessFlowArea.SuspendLayout();
            Tlp_AlarmArea.SuspendLayout();
            Pnl_AlarmHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Dgv_ActiveAlarms).BeginInit();
            Pnl_AlarmArea.SuspendLayout();
            Tlp_ProcessFlow.SuspendLayout();
            Tlp_ProcessMain.SuspendLayout();
            Pnl_ProcessVm1.SuspendLayout();
            Pnl_ProcessPress.SuspendLayout();
            Pnl_ProcessQuality.SuspendLayout();
            Tlp_ProcessRoute.SuspendLayout();
            Pnl_PassRoute.SuspendLayout();
            Pnl_FailRoute.SuspendLayout();
            Pnl_EventArea.SuspendLayout();
            Tlp_EventArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Dgv_RecentEvents).BeginInit();
            Pnl_ControlArea.SuspendLayout();
            Tlp_Control.SuspendLayout();
            Tlp_ProcessVm1Labels.SuspendLayout();
            Tlp_ProcessPressLabels.SuspendLayout();
            Tlp_ProcessQualityLabels.SuspendLayout();
            Tlp_PassRouteLabels.SuspendLayout();
            Tlp_FailRouteLabels.SuspendLayout();
            SuspendLayout();
            // 
            // Lbl_LotProgressTitle
            // 
            Lbl_LotProgressTitle.AutoSize = false;
            Lbl_LotProgressTitle.Dock = DockStyle.Fill;
            Lbl_LotProgressTitle.Font = new Font("Segoe UI", 9F);
            Lbl_LotProgressTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_LotProgressTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_LotProgressTitle.Location = new Point(10, 148);
            Lbl_LotProgressTitle.Margin = new Padding(0);
            Lbl_LotProgressTitle.Name = "Lbl_LotProgressTitle";
            Lbl_LotProgressTitle.Size = new Size(97, 26);
            Lbl_LotProgressTitle.TabIndex = 8;
            Lbl_LotProgressTitle.Text = "진행률";
            Lbl_LotProgressTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Tlp_Root
            // 
            Tlp_Root.ColumnCount = 1;
            Tlp_Root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_Root.Controls.Add(Pnl_Header, 0, 0);
            Tlp_Root.Controls.Add(Pnl_Content, 0, 1);
            Tlp_Root.Dock = DockStyle.Fill;
            Tlp_Root.Location = new Point(0, 0);
            Tlp_Root.Margin = new Padding(0);
            Tlp_Root.Name = "Tlp_Root";
            Tlp_Root.RowCount = 2;
            Tlp_Root.RowStyles.Add(new RowStyle(SizeType.Absolute, 63F));
            Tlp_Root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_Root.Size = new Size(1350, 826);
            Tlp_Root.TabIndex = 0;
            // 
            // Pnl_Header
            // 
            Pnl_Header.BackColor = Color.FromArgb(30, 58, 95);
            Pnl_Header.Controls.Add(Lbl_CurrentTime);
            Pnl_Header.Controls.Add(Lbl_Title);
            Pnl_Header.Dock = DockStyle.Fill;
            Pnl_Header.Location = new Point(0, 0);
            Pnl_Header.Margin = new Padding(0);
            Pnl_Header.Name = "Pnl_Header";
            Pnl_Header.Size = new Size(1350, 63);
            Pnl_Header.TabIndex = 0;
            // 
            // Lbl_CurrentTime
            // 
            Lbl_CurrentTime.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lbl_CurrentTime.AutoSize = true;
            Lbl_CurrentTime.BackColor = Color.Transparent;
            Lbl_CurrentTime.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_CurrentTime.ForeColor = Color.White;
            Lbl_CurrentTime.Location = new Point(1175, 23);
            Lbl_CurrentTime.Name = "Lbl_CurrentTime";
            Lbl_CurrentTime.Size = new Size(159, 20);
            Lbl_CurrentTime.TabIndex = 1;
            Lbl_CurrentTime.Text = "2026-09-29 00:00:00";
            // 
            // Lbl_Title
            // 
            Lbl_Title.AutoSize = true;
            Lbl_Title.BackColor = Color.Transparent;
            Lbl_Title.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            Lbl_Title.ForeColor = Color.White;
            Lbl_Title.Location = new Point(20, 16);
            Lbl_Title.Name = "Lbl_Title";
            Lbl_Title.Size = new Size(109, 30);
            Lbl_Title.TabIndex = 0;
            Lbl_Title.Text = "MES HMI";
            // 
            // Pnl_Content
            // 
            Pnl_Content.Controls.Add(Tlp_ContentRoot);
            Pnl_Content.Dock = DockStyle.Fill;
            Pnl_Content.Location = new Point(0, 63);
            Pnl_Content.Margin = new Padding(0);
            Pnl_Content.Name = "Pnl_Content";
            Pnl_Content.Padding = new Padding(15);
            Pnl_Content.Size = new Size(1350, 763);
            Pnl_Content.TabIndex = 2;
            // 
            // Tlp_ContentRoot
            // 
            Tlp_ContentRoot.ColumnCount = 1;
            Tlp_ContentRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_ContentRoot.Controls.Add(Tlp_SummaryCards, 0, 0);
            Tlp_ContentRoot.Controls.Add(Tlp_DashboardBody, 0, 1);
            Tlp_ContentRoot.Dock = DockStyle.Fill;
            Tlp_ContentRoot.Location = new Point(15, 15);
            Tlp_ContentRoot.Name = "Tlp_ContentRoot";
            Tlp_ContentRoot.RowCount = 2;
            Tlp_ContentRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 249F));
            Tlp_ContentRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_ContentRoot.Size = new Size(1320, 733);
            Tlp_ContentRoot.TabIndex = 0;
            // 
            // Tlp_SummaryCards
            // 
            Tlp_SummaryCards.ColumnCount = 4;
            Tlp_SummaryCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            Tlp_SummaryCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            Tlp_SummaryCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            Tlp_SummaryCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            Tlp_SummaryCards.Controls.Add(Pnl_ProductionCard, 3, 0);
            Tlp_SummaryCards.Controls.Add(Pnl_ProductCard, 2, 0);
            Tlp_SummaryCards.Controls.Add(Pnl_LotCard, 1, 0);
            Tlp_SummaryCards.Controls.Add(Pnl_WorkOrderCard, 0, 0);
            Tlp_SummaryCards.Dock = DockStyle.Fill;
            Tlp_SummaryCards.Location = new Point(0, 0);
            Tlp_SummaryCards.Margin = new Padding(0, 0, 0, 11);
            Tlp_SummaryCards.Name = "Tlp_SummaryCards";
            Tlp_SummaryCards.RowCount = 1;
            Tlp_SummaryCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_SummaryCards.Size = new Size(1320, 238);
            Tlp_SummaryCards.TabIndex = 0;
            // 
            // Pnl_ProductionCard
            // 
            Pnl_ProductionCard.BackColor = Color.White;
            Pnl_ProductionCard.BorderStyle = BorderStyle.None;
            Pnl_ProductionCard.Controls.Add(tableLayoutPanel3);
            Pnl_ProductionCard.Dock = DockStyle.Fill;
            Pnl_ProductionCard.Location = new Point(996, 0);
            Pnl_ProductionCard.Margin = new Padding(6, 0, 0, 0);
            Pnl_ProductionCard.Name = "Pnl_ProductionCard";
            Pnl_ProductionCard.Padding = new Padding(14);
            Pnl_ProductionCard.Size = new Size(324, 238);
            Pnl_ProductionCard.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            tableLayoutPanel3.Controls.Add(Lbl_Yield, 1, 5);
            tableLayoutPanel3.Controls.Add(Lbl_YieldTitle, 0, 5);
            tableLayoutPanel3.Controls.Add(Lbl_Defect, 1, 4);
            tableLayoutPanel3.Controls.Add(Lbl_DefectTitle, 0, 4);
            tableLayoutPanel3.Controls.Add(Lbl_Good, 1, 3);
            tableLayoutPanel3.Controls.Add(Lbl_GoodTitle, 0, 3);
            tableLayoutPanel3.Controls.Add(Lbl_Produced, 1, 2);
            tableLayoutPanel3.Controls.Add(Lbl_ProducedTitle, 0, 2);
            tableLayoutPanel3.Controls.Add(Lbl_Target, 1, 1);
            tableLayoutPanel3.Controls.Add(Lbl_ProductionTitle, 0, 0);
            tableLayoutPanel3.Controls.Add(Lbl_TargetTitle, 0, 1);
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(12, 12);
            tableLayoutPanel3.Margin = new Padding(0);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.Padding = new Padding(0);
            tableLayoutPanel3.RowCount = 6;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.Size = new Size(298, 212);
            tableLayoutPanel3.TabIndex = 0;
            // 
            // Lbl_Yield
            // 
            Lbl_Yield.AutoSize = false;
            Lbl_Yield.BackColor = Color.Transparent;
            Lbl_Yield.Dock = DockStyle.Fill;
            Lbl_Yield.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Yield.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_Yield.Location = new Point(135, 168);
            Lbl_Yield.Margin = new Padding(0);
            Lbl_Yield.Name = "Lbl_Yield";
            Lbl_Yield.Size = new Size(153, 34);
            Lbl_Yield.TabIndex = 10;
            Lbl_Yield.Text = "0.00 %";
            Lbl_Yield.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_YieldTitle
            // 
            Lbl_YieldTitle.AutoSize = false;
            Lbl_YieldTitle.BackColor = Color.Transparent;
            Lbl_YieldTitle.Dock = DockStyle.Fill;
            Lbl_YieldTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_YieldTitle.ForeColor = Color.FromArgb(29, 78, 216);
            Lbl_YieldTitle.Location = new Point(10, 168);
            Lbl_YieldTitle.Margin = new Padding(0);
            Lbl_YieldTitle.Name = "Lbl_YieldTitle";
            Lbl_YieldTitle.Size = new Size(117, 34);
            Lbl_YieldTitle.TabIndex = 9;
            Lbl_YieldTitle.Text = "수율";
            Lbl_YieldTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Defect
            // 
            Lbl_Defect.AutoSize = false;
            Lbl_Defect.BackColor = Color.Transparent;
            Lbl_Defect.Dock = DockStyle.Fill;
            Lbl_Defect.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Defect.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_Defect.Location = new Point(135, 139);
            Lbl_Defect.Margin = new Padding(0);
            Lbl_Defect.Name = "Lbl_Defect";
            Lbl_Defect.Size = new Size(153, 29);
            Lbl_Defect.TabIndex = 8;
            Lbl_Defect.Text = "0 EA";
            Lbl_Defect.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_DefectTitle
            // 
            Lbl_DefectTitle.AutoSize = false;
            Lbl_DefectTitle.BackColor = Color.Transparent;
            Lbl_DefectTitle.Dock = DockStyle.Fill;
            Lbl_DefectTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_DefectTitle.ForeColor = Color.FromArgb(185, 28, 28);
            Lbl_DefectTitle.Location = new Point(10, 139);
            Lbl_DefectTitle.Margin = new Padding(0);
            Lbl_DefectTitle.Name = "Lbl_DefectTitle";
            Lbl_DefectTitle.Size = new Size(117, 29);
            Lbl_DefectTitle.TabIndex = 7;
            Lbl_DefectTitle.Text = "FAIL";
            Lbl_DefectTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Good
            // 
            Lbl_Good.AutoSize = false;
            Lbl_Good.BackColor = Color.Transparent;
            Lbl_Good.Dock = DockStyle.Fill;
            Lbl_Good.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Good.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_Good.Location = new Point(135, 110);
            Lbl_Good.Margin = new Padding(0);
            Lbl_Good.Name = "Lbl_Good";
            Lbl_Good.Size = new Size(153, 29);
            Lbl_Good.TabIndex = 6;
            Lbl_Good.Text = "0 EA";
            Lbl_Good.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_GoodTitle
            // 
            Lbl_GoodTitle.AutoSize = false;
            Lbl_GoodTitle.BackColor = Color.Transparent;
            Lbl_GoodTitle.Dock = DockStyle.Fill;
            Lbl_GoodTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_GoodTitle.ForeColor = Color.FromArgb(21, 128, 61);
            Lbl_GoodTitle.Location = new Point(10, 110);
            Lbl_GoodTitle.Margin = new Padding(0);
            Lbl_GoodTitle.Name = "Lbl_GoodTitle";
            Lbl_GoodTitle.Size = new Size(117, 29);
            Lbl_GoodTitle.TabIndex = 5;
            Lbl_GoodTitle.Text = "PASS";
            Lbl_GoodTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Produced
            // 
            Lbl_Produced.AutoSize = false;
            Lbl_Produced.Dock = DockStyle.Fill;
            Lbl_Produced.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Produced.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_Produced.Location = new Point(135, 81);
            Lbl_Produced.Margin = new Padding(0);
            Lbl_Produced.Name = "Lbl_Produced";
            Lbl_Produced.Size = new Size(153, 29);
            Lbl_Produced.TabIndex = 4;
            Lbl_Produced.Text = "0 EA";
            Lbl_Produced.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProducedTitle
            // 
            Lbl_ProducedTitle.AutoSize = false;
            Lbl_ProducedTitle.Dock = DockStyle.Fill;
            Lbl_ProducedTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProducedTitle.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_ProducedTitle.Location = new Point(10, 81);
            Lbl_ProducedTitle.Margin = new Padding(0);
            Lbl_ProducedTitle.Name = "Lbl_ProducedTitle";
            Lbl_ProducedTitle.Size = new Size(117, 29);
            Lbl_ProducedTitle.TabIndex = 3;
            Lbl_ProducedTitle.Text = "생산수량";
            Lbl_ProducedTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Target
            // 
            Lbl_Target.AutoSize = false;
            Lbl_Target.Dock = DockStyle.Fill;
            Lbl_Target.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Target.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_Target.Location = new Point(135, 52);
            Lbl_Target.Margin = new Padding(0);
            Lbl_Target.Name = "Lbl_Target";
            Lbl_Target.Size = new Size(153, 29);
            Lbl_Target.TabIndex = 2;
            Lbl_Target.Text = "300 EA";
            Lbl_Target.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductionTitle
            // 
            Lbl_ProductionTitle.AutoSize = false;
            tableLayoutPanel3.SetColumnSpan(Lbl_ProductionTitle, 2);
            Lbl_ProductionTitle.Dock = DockStyle.Fill;
            Lbl_ProductionTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_ProductionTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProductionTitle.Location = new Point(10, 11);
            Lbl_ProductionTitle.Margin = new Padding(0);
            Lbl_ProductionTitle.Name = "Lbl_ProductionTitle";
            Lbl_ProductionTitle.Size = new Size(270, 41);
            Lbl_ProductionTitle.TabIndex = 0;
            Lbl_ProductionTitle.Text = "생산 실적";
            Lbl_ProductionTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_TargetTitle
            // 
            Lbl_TargetTitle.AutoSize = false;
            Lbl_TargetTitle.Dock = DockStyle.Fill;
            Lbl_TargetTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_TargetTitle.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_TargetTitle.Location = new Point(10, 52);
            Lbl_TargetTitle.Margin = new Padding(0);
            Lbl_TargetTitle.Name = "Lbl_TargetTitle";
            Lbl_TargetTitle.Size = new Size(117, 29);
            Lbl_TargetTitle.TabIndex = 1;
            Lbl_TargetTitle.Text = "목표수량";
            Lbl_TargetTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Pnl_ProductCard
            // 
            Pnl_ProductCard.BackColor = Color.White;
            Pnl_ProductCard.BorderStyle = BorderStyle.None;
            Pnl_ProductCard.Controls.Add(tableLayoutPanel2);
            Pnl_ProductCard.Controls.Add(tableLayoutPanel1);
            Pnl_ProductCard.Dock = DockStyle.Fill;
            Pnl_ProductCard.Location = new Point(666, 7);
            Pnl_ProductCard.Margin = new Padding(6, 0, 6, 0);
            Pnl_ProductCard.Name = "Pnl_ProductCard";
            Pnl_ProductCard.Padding = new Padding(14);
            Pnl_ProductCard.Size = new Size(318, 224);
            Pnl_ProductCard.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            tableLayoutPanel2.Controls.Add(Lbl_ProductLocation, 1, 4);
            tableLayoutPanel2.Controls.Add(Lbl_ProductLocationTitle, 0, 4);
            tableLayoutPanel2.Controls.Add(Lbl_ProductQuality, 1, 3);
            tableLayoutPanel2.Controls.Add(Lbl_ProductStatus, 1, 2);
            tableLayoutPanel2.Controls.Add(Lbl_ProductCode, 1, 1);
            tableLayoutPanel2.Controls.Add(Lbl_ProductTitle, 0, 0);
            tableLayoutPanel2.Controls.Add(Lbl_ProductCodeTitle, 0, 1);
            tableLayoutPanel2.Controls.Add(Lbl_ProductStatusTitle, 0, 2);
            tableLayoutPanel2.Controls.Add(Lbl_ProductQualityTitle, 0, 3);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(12, 12);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.Padding = new Padding(0);
            tableLayoutPanel2.RowCount = 5;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel2.Size = new Size(292, 198);
            tableLayoutPanel2.TabIndex = 1;
            // 
            // Lbl_ProductLocation
            // 
            Lbl_ProductLocation.AutoSize = false;
            Lbl_ProductLocation.Dock = DockStyle.Fill;
            Lbl_ProductLocation.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProductLocation.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_ProductLocation.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductLocation.Location = new Point(107, 148);
            Lbl_ProductLocation.Margin = new Padding(0);
            Lbl_ProductLocation.Name = "Lbl_ProductLocation";
            Lbl_ProductLocation.Size = new Size(175, 39);
            Lbl_ProductLocation.TabIndex = 8;
            Lbl_ProductLocation.Text = "---";
            Lbl_ProductLocation.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductLocationTitle
            // 
            Lbl_ProductLocationTitle.AutoSize = false;
            Lbl_ProductLocationTitle.Dock = DockStyle.Fill;
            Lbl_ProductLocationTitle.Font = new Font("Segoe UI", 9F);
            Lbl_ProductLocationTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_ProductLocationTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductLocationTitle.Location = new Point(10, 148);
            Lbl_ProductLocationTitle.Margin = new Padding(0);
            Lbl_ProductLocationTitle.Name = "Lbl_ProductLocationTitle";
            Lbl_ProductLocationTitle.Size = new Size(97, 39);
            Lbl_ProductLocationTitle.TabIndex = 7;
            Lbl_ProductLocationTitle.Text = "현재위치";
            Lbl_ProductLocationTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductQuality
            // 
            Lbl_ProductQuality.AutoSize = false;
            Lbl_ProductQuality.Dock = DockStyle.Fill;
            Lbl_ProductQuality.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProductQuality.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_ProductQuality.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductQuality.Location = new Point(107, 116);
            Lbl_ProductQuality.Margin = new Padding(0);
            Lbl_ProductQuality.Name = "Lbl_ProductQuality";
            Lbl_ProductQuality.Size = new Size(175, 32);
            Lbl_ProductQuality.TabIndex = 6;
            Lbl_ProductQuality.Text = "---";
            Lbl_ProductQuality.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductStatus
            // 
            Lbl_ProductStatus.AutoSize = false;
            Lbl_ProductStatus.Dock = DockStyle.Fill;
            Lbl_ProductStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProductStatus.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_ProductStatus.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductStatus.Location = new Point(107, 84);
            Lbl_ProductStatus.Margin = new Padding(0);
            Lbl_ProductStatus.Name = "Lbl_ProductStatus";
            Lbl_ProductStatus.Size = new Size(175, 32);
            Lbl_ProductStatus.TabIndex = 5;
            Lbl_ProductStatus.Text = "---";
            Lbl_ProductStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductCode
            // 
            Lbl_ProductCode.AutoSize = false;
            Lbl_ProductCode.Dock = DockStyle.Fill;
            Lbl_ProductCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProductCode.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_ProductCode.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductCode.Location = new Point(107, 52);
            Lbl_ProductCode.Margin = new Padding(0);
            Lbl_ProductCode.Name = "Lbl_ProductCode";
            Lbl_ProductCode.Size = new Size(175, 32);
            Lbl_ProductCode.TabIndex = 4;
            Lbl_ProductCode.Text = "---";
            Lbl_ProductCode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductTitle
            // 
            Lbl_ProductTitle.AutoSize = false;
            tableLayoutPanel2.SetColumnSpan(Lbl_ProductTitle, 2);
            Lbl_ProductTitle.Dock = DockStyle.Fill;
            Lbl_ProductTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Lbl_ProductTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProductTitle.Location = new Point(13, 11);
            Lbl_ProductTitle.Name = "Lbl_ProductTitle";
            Lbl_ProductTitle.Size = new Size(266, 41);
            Lbl_ProductTitle.TabIndex = 0;
            Lbl_ProductTitle.Text = "공정 추적 제품";
            Lbl_ProductTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductCodeTitle
            // 
            Lbl_ProductCodeTitle.AutoSize = false;
            Lbl_ProductCodeTitle.Dock = DockStyle.Fill;
            Lbl_ProductCodeTitle.Font = new Font("Segoe UI", 9F);
            Lbl_ProductCodeTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_ProductCodeTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductCodeTitle.Location = new Point(10, 52);
            Lbl_ProductCodeTitle.Margin = new Padding(0);
            Lbl_ProductCodeTitle.Name = "Lbl_ProductCodeTitle";
            Lbl_ProductCodeTitle.Size = new Size(97, 32);
            Lbl_ProductCodeTitle.TabIndex = 1;
            Lbl_ProductCodeTitle.Text = "제품코드";
            Lbl_ProductCodeTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductStatusTitle
            // 
            Lbl_ProductStatusTitle.AutoSize = false;
            Lbl_ProductStatusTitle.Dock = DockStyle.Fill;
            Lbl_ProductStatusTitle.Font = new Font("Segoe UI", 9F);
            Lbl_ProductStatusTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_ProductStatusTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductStatusTitle.Location = new Point(10, 84);
            Lbl_ProductStatusTitle.Margin = new Padding(0);
            Lbl_ProductStatusTitle.Name = "Lbl_ProductStatusTitle";
            Lbl_ProductStatusTitle.Size = new Size(97, 32);
            Lbl_ProductStatusTitle.TabIndex = 2;
            Lbl_ProductStatusTitle.Text = "상태";
            Lbl_ProductStatusTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_ProductQualityTitle
            // 
            Lbl_ProductQualityTitle.AutoSize = false;
            Lbl_ProductQualityTitle.Dock = DockStyle.Fill;
            Lbl_ProductQualityTitle.Font = new Font("Segoe UI", 9F);
            Lbl_ProductQualityTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_ProductQualityTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_ProductQualityTitle.Location = new Point(10, 116);
            Lbl_ProductQualityTitle.Margin = new Padding(0);
            Lbl_ProductQualityTitle.Name = "Lbl_ProductQualityTitle";
            Lbl_ProductQualityTitle.Size = new Size(97, 32);
            Lbl_ProductQualityTitle.TabIndex = 3;
            Lbl_ProductQualityTitle.Text = "품질상태";
            Lbl_ProductQualityTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Location = new Point(70, 142);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(8, 9);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // Pnl_LotCard
            // 
            Pnl_LotCard.BackColor = Color.White;
            Pnl_LotCard.BorderStyle = BorderStyle.None;
            Pnl_LotCard.Controls.Add(Tlp_Lot);
            Pnl_LotCard.Dock = DockStyle.Fill;
            Pnl_LotCard.Location = new Point(336, 7);
            Pnl_LotCard.Margin = new Padding(6, 0, 6, 0);
            Pnl_LotCard.Name = "Pnl_LotCard";
            Pnl_LotCard.Padding = new Padding(14);
            Pnl_LotCard.Size = new Size(318, 224);
            Pnl_LotCard.TabIndex = 1;
            // 
            // Tlp_Lot
            // 
            Tlp_Lot.ColumnCount = 2;
            Tlp_Lot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            Tlp_Lot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            Tlp_Lot.Controls.Add(Lbl_LotProgress, 1, 4);
            Tlp_Lot.Controls.Add(Lbl_LotProgressTitle, 0, 4);
            Tlp_Lot.Controls.Add(Lbl_LotProduction, 1, 3);
            Tlp_Lot.Controls.Add(Lbl_LotQuantityTitle, 0, 3);
            Tlp_Lot.Controls.Add(Lbl_LotStatus, 1, 2);
            Tlp_Lot.Controls.Add(Lbl_LotStatusTitle, 0, 2);
            Tlp_Lot.Controls.Add(Lbl_LotCode, 1, 1);
            Tlp_Lot.Controls.Add(Lbl_LotCodeTitle, 0, 1);
            Tlp_Lot.Controls.Add(Lbl_LotTitle, 0, 0);
            Tlp_Lot.Controls.Add(Pnl_LotProgressTrack, 0, 5);
            Tlp_Lot.Dock = DockStyle.Fill;
            Tlp_Lot.Location = new Point(12, 12);
            Tlp_Lot.Margin = new Padding(0);
            Tlp_Lot.Name = "Tlp_Lot";
            Tlp_Lot.Padding = new Padding(0);
            Tlp_Lot.RowCount = 6;
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Lot.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            Tlp_Lot.Size = new Size(292, 198);
            Tlp_Lot.TabIndex = 0;
            // 
            // Lbl_LotProgress
            // 
            Lbl_LotProgress.AutoSize = false;
            Lbl_LotProgress.Dock = DockStyle.Fill;
            Lbl_LotProgress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_LotProgress.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_LotProgress.Location = new Point(107, 148);
            Lbl_LotProgress.Margin = new Padding(0);
            Lbl_LotProgress.Name = "Lbl_LotProgress";
            Lbl_LotProgress.Size = new Size(175, 26);
            Lbl_LotProgress.TabIndex = 9;
            Lbl_LotProgress.Text = "0.00 %";
            Lbl_LotProgress.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotProduction
            // 
            Lbl_LotProduction.AutoSize = false;
            Lbl_LotProduction.Dock = DockStyle.Fill;
            Lbl_LotProduction.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_LotProduction.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_LotProduction.Location = new Point(107, 116);
            Lbl_LotProduction.Margin = new Padding(0);
            Lbl_LotProduction.Name = "Lbl_LotProduction";
            Lbl_LotProduction.Size = new Size(175, 32);
            Lbl_LotProduction.TabIndex = 7;
            Lbl_LotProduction.Text = "0 / 60 EA";
            Lbl_LotProduction.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotQuantityTitle
            // 
            Lbl_LotQuantityTitle.AutoSize = false;
            Lbl_LotQuantityTitle.Dock = DockStyle.Fill;
            Lbl_LotQuantityTitle.Font = new Font("Segoe UI", 9F);
            Lbl_LotQuantityTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_LotQuantityTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_LotQuantityTitle.Location = new Point(10, 116);
            Lbl_LotQuantityTitle.Margin = new Padding(0);
            Lbl_LotQuantityTitle.Name = "Lbl_LotQuantityTitle";
            Lbl_LotQuantityTitle.Size = new Size(97, 32);
            Lbl_LotQuantityTitle.TabIndex = 6;
            Lbl_LotQuantityTitle.Text = "생산수량";
            Lbl_LotQuantityTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotStatus
            // 
            Lbl_LotStatus.AutoSize = false;
            Lbl_LotStatus.Dock = DockStyle.Fill;
            Lbl_LotStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_LotStatus.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_LotStatus.Location = new Point(107, 84);
            Lbl_LotStatus.Margin = new Padding(0);
            Lbl_LotStatus.Name = "Lbl_LotStatus";
            Lbl_LotStatus.Size = new Size(175, 32);
            Lbl_LotStatus.TabIndex = 5;
            Lbl_LotStatus.Text = "---";
            Lbl_LotStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotStatusTitle
            // 
            Lbl_LotStatusTitle.AutoSize = false;
            Lbl_LotStatusTitle.Dock = DockStyle.Fill;
            Lbl_LotStatusTitle.Font = new Font("Segoe UI", 9F);
            Lbl_LotStatusTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_LotStatusTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_LotStatusTitle.Location = new Point(10, 84);
            Lbl_LotStatusTitle.Margin = new Padding(0);
            Lbl_LotStatusTitle.Name = "Lbl_LotStatusTitle";
            Lbl_LotStatusTitle.Size = new Size(97, 32);
            Lbl_LotStatusTitle.TabIndex = 4;
            Lbl_LotStatusTitle.Text = "상태";
            Lbl_LotStatusTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotCode
            // 
            Lbl_LotCode.AutoSize = false;
            Lbl_LotCode.Dock = DockStyle.Fill;
            Lbl_LotCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_LotCode.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_LotCode.Location = new Point(107, 52);
            Lbl_LotCode.Margin = new Padding(0);
            Lbl_LotCode.Name = "Lbl_LotCode";
            Lbl_LotCode.Size = new Size(175, 32);
            Lbl_LotCode.TabIndex = 3;
            Lbl_LotCode.Text = "---";
            Lbl_LotCode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotCodeTitle
            // 
            Lbl_LotCodeTitle.AutoSize = false;
            Lbl_LotCodeTitle.Dock = DockStyle.Fill;
            Lbl_LotCodeTitle.Font = new Font("Segoe UI", 9F);
            Lbl_LotCodeTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_LotCodeTitle.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_LotCodeTitle.Location = new Point(10, 52);
            Lbl_LotCodeTitle.Margin = new Padding(0);
            Lbl_LotCodeTitle.Name = "Lbl_LotCodeTitle";
            Lbl_LotCodeTitle.Size = new Size(97, 32);
            Lbl_LotCodeTitle.TabIndex = 2;
            Lbl_LotCodeTitle.Text = "LOT";
            Lbl_LotCodeTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_LotTitle
            // 
            Lbl_LotTitle.AutoSize = false;
            Tlp_Lot.SetColumnSpan(Lbl_LotTitle, 2);
            Lbl_LotTitle.Dock = DockStyle.Fill;
            Lbl_LotTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_LotTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_LotTitle.Location = new Point(13, 11);
            Lbl_LotTitle.Name = "Lbl_LotTitle";
            Lbl_LotTitle.Size = new Size(266, 41);
            Lbl_LotTitle.TabIndex = 0;
            Lbl_LotTitle.Text = "현재 LOT";
            Lbl_LotTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Pnl_LotProgressTrack
            // 
            Pnl_LotProgressTrack.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            Pnl_LotProgressTrack.BackColor = Color.FromArgb(226, 232, 240);
            Tlp_Lot.SetColumnSpan(Pnl_LotProgressTrack, 2);
            Pnl_LotProgressTrack.Controls.Add(Pnl_LotProgressFill);
            Pnl_LotProgressTrack.Location = new Point(10, 179);
            Pnl_LotProgressTrack.Margin = new Padding(0, 5, 0, 0);
            Pnl_LotProgressTrack.Name = "Pnl_LotProgressTrack";
            Pnl_LotProgressTrack.Size = new Size(272, 15);
            Pnl_LotProgressTrack.TabIndex = 1;
            // 
            // Pnl_LotProgressFill
            // 
            Pnl_LotProgressFill.BackColor = Color.FromArgb(37, 99, 235);
            Pnl_LotProgressFill.Dock = DockStyle.Left;
            Pnl_LotProgressFill.Location = new Point(0, 0);
            Pnl_LotProgressFill.Margin = new Padding(0, 4, 0, 0);
            Pnl_LotProgressFill.Name = "Pnl_LotProgressFill";
            Pnl_LotProgressFill.Size = new Size(0, 15);
            Pnl_LotProgressFill.TabIndex = 0;
            // 
            // Pnl_WorkOrderCard
            // 
            Pnl_WorkOrderCard.BackColor = Color.White;
            Pnl_WorkOrderCard.BorderStyle = BorderStyle.None;
            Pnl_WorkOrderCard.Controls.Add(Tlp_WorkOrder);
            Pnl_WorkOrderCard.Dock = DockStyle.Fill;
            Pnl_WorkOrderCard.Location = new Point(6, 7);
            Pnl_WorkOrderCard.Margin = new Padding(0, 0, 6, 0);
            Pnl_WorkOrderCard.Name = "Pnl_WorkOrderCard";
            Pnl_WorkOrderCard.Padding = new Padding(14);
            Pnl_WorkOrderCard.Size = new Size(318, 224);
            Pnl_WorkOrderCard.TabIndex = 0;
            // 
            // Tlp_WorkOrder
            // 
            Tlp_WorkOrder.ColumnCount = 2;
            Tlp_WorkOrder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            Tlp_WorkOrder.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66F));
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderAchievement, 1, 4);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderTarget, 1, 3);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderStatus, 1, 2);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderAchievementTitle, 0, 4);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderTargetTitle, 0, 3);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderStatusTitle, 0, 2);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderTitle, 0, 0);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderCodeTitle, 0, 1);
            Tlp_WorkOrder.Controls.Add(Lbl_WorkOrderCode, 1, 1);
            Tlp_WorkOrder.Dock = DockStyle.Fill;
            Tlp_WorkOrder.Location = new Point(12, 12);
            Tlp_WorkOrder.Margin = new Padding(0);
            Tlp_WorkOrder.Name = "Tlp_WorkOrder";
            Tlp_WorkOrder.Padding = new Padding(0);
            Tlp_WorkOrder.RowCount = 5;
            Tlp_WorkOrder.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_WorkOrder.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_WorkOrder.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_WorkOrder.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_WorkOrder.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_WorkOrder.Size = new Size(292, 198);
            Tlp_WorkOrder.TabIndex = 0;
            // 
            // Lbl_WorkOrderAchievement
            // 
            Lbl_WorkOrderAchievement.AutoSize = false;
            Lbl_WorkOrderAchievement.Dock = DockStyle.Fill;
            Lbl_WorkOrderAchievement.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_WorkOrderAchievement.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_WorkOrderAchievement.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_WorkOrderAchievement.Location = new Point(113, 151);
            Lbl_WorkOrderAchievement.Margin = new Padding(0);
            Lbl_WorkOrderAchievement.Name = "Lbl_WorkOrderAchievement";
            Lbl_WorkOrderAchievement.Size = new Size(169, 36);
            Lbl_WorkOrderAchievement.TabIndex = 8;
            Lbl_WorkOrderAchievement.Text = "0.00 %";
            Lbl_WorkOrderAchievement.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderTarget
            // 
            Lbl_WorkOrderTarget.AutoSize = false;
            Lbl_WorkOrderTarget.Dock = DockStyle.Fill;
            Lbl_WorkOrderTarget.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_WorkOrderTarget.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_WorkOrderTarget.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_WorkOrderTarget.Location = new Point(113, 118);
            Lbl_WorkOrderTarget.Margin = new Padding(0);
            Lbl_WorkOrderTarget.Name = "Lbl_WorkOrderTarget";
            Lbl_WorkOrderTarget.Size = new Size(169, 33);
            Lbl_WorkOrderTarget.TabIndex = 7;
            Lbl_WorkOrderTarget.Text = "0 EA";
            Lbl_WorkOrderTarget.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderStatus
            // 
            Lbl_WorkOrderStatus.AutoSize = false;
            Lbl_WorkOrderStatus.Dock = DockStyle.Fill;
            Lbl_WorkOrderStatus.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_WorkOrderStatus.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_WorkOrderStatus.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_WorkOrderStatus.Location = new Point(113, 85);
            Lbl_WorkOrderStatus.Margin = new Padding(0);
            Lbl_WorkOrderStatus.Name = "Lbl_WorkOrderStatus";
            Lbl_WorkOrderStatus.Size = new Size(169, 33);
            Lbl_WorkOrderStatus.TabIndex = 6;
            Lbl_WorkOrderStatus.Text = "-";
            Lbl_WorkOrderStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderAchievementTitle
            // 
            Lbl_WorkOrderAchievementTitle.AutoSize = false;
            Lbl_WorkOrderAchievementTitle.Dock = DockStyle.Fill;
            Lbl_WorkOrderAchievementTitle.Font = new Font("Segoe UI", 9F);
            Lbl_WorkOrderAchievementTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_WorkOrderAchievementTitle.Location = new Point(10, 151);
            Lbl_WorkOrderAchievementTitle.Margin = new Padding(0);
            Lbl_WorkOrderAchievementTitle.Name = "Lbl_WorkOrderAchievementTitle";
            Lbl_WorkOrderAchievementTitle.Size = new Size(95, 36);
            Lbl_WorkOrderAchievementTitle.TabIndex = 4;
            Lbl_WorkOrderAchievementTitle.Text = "달성률";
            Lbl_WorkOrderAchievementTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderTargetTitle
            // 
            Lbl_WorkOrderTargetTitle.AutoSize = false;
            Lbl_WorkOrderTargetTitle.Dock = DockStyle.Fill;
            Lbl_WorkOrderTargetTitle.Font = new Font("Segoe UI", 9F);
            Lbl_WorkOrderTargetTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_WorkOrderTargetTitle.Location = new Point(10, 118);
            Lbl_WorkOrderTargetTitle.Margin = new Padding(0);
            Lbl_WorkOrderTargetTitle.Name = "Lbl_WorkOrderTargetTitle";
            Lbl_WorkOrderTargetTitle.Size = new Size(95, 33);
            Lbl_WorkOrderTargetTitle.TabIndex = 3;
            Lbl_WorkOrderTargetTitle.Text = "목표수량";
            Lbl_WorkOrderTargetTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderStatusTitle
            // 
            Lbl_WorkOrderStatusTitle.AutoSize = false;
            Lbl_WorkOrderStatusTitle.Dock = DockStyle.Fill;
            Lbl_WorkOrderStatusTitle.Font = new Font("Segoe UI", 9F);
            Lbl_WorkOrderStatusTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_WorkOrderStatusTitle.Location = new Point(10, 85);
            Lbl_WorkOrderStatusTitle.Margin = new Padding(0);
            Lbl_WorkOrderStatusTitle.Name = "Lbl_WorkOrderStatusTitle";
            Lbl_WorkOrderStatusTitle.Size = new Size(95, 33);
            Lbl_WorkOrderStatusTitle.TabIndex = 2;
            Lbl_WorkOrderStatusTitle.Text = "상태";
            Lbl_WorkOrderStatusTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderTitle
            // 
            Lbl_WorkOrderTitle.AutoSize = false;
            Tlp_WorkOrder.SetColumnSpan(Lbl_WorkOrderTitle, 2);
            Lbl_WorkOrderTitle.Dock = DockStyle.Fill;
            Lbl_WorkOrderTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_WorkOrderTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_WorkOrderTitle.Location = new Point(13, 11);
            Lbl_WorkOrderTitle.Name = "Lbl_WorkOrderTitle";
            Lbl_WorkOrderTitle.Size = new Size(266, 41);
            Lbl_WorkOrderTitle.TabIndex = 0;
            Lbl_WorkOrderTitle.Text = "현재 작업 지시";
            Lbl_WorkOrderTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderCodeTitle
            // 
            Lbl_WorkOrderCodeTitle.AutoSize = false;
            Lbl_WorkOrderCodeTitle.Dock = DockStyle.Fill;
            Lbl_WorkOrderCodeTitle.Font = new Font("Segoe UI", 9F);
            Lbl_WorkOrderCodeTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_WorkOrderCodeTitle.Location = new Point(10, 52);
            Lbl_WorkOrderCodeTitle.Margin = new Padding(0);
            Lbl_WorkOrderCodeTitle.Name = "Lbl_WorkOrderCodeTitle";
            Lbl_WorkOrderCodeTitle.Padding = new Padding(0);
            Lbl_WorkOrderCodeTitle.Size = new Size(95, 33);
            Lbl_WorkOrderCodeTitle.TabIndex = 1;
            Lbl_WorkOrderCodeTitle.Text = "작업지시";
            Lbl_WorkOrderCodeTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_WorkOrderCode
            // 
            Lbl_WorkOrderCode.AutoSize = false;
            Lbl_WorkOrderCode.Dock = DockStyle.Fill;
            Lbl_WorkOrderCode.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_WorkOrderCode.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_WorkOrderCode.ImageAlign = ContentAlignment.MiddleLeft;
            Lbl_WorkOrderCode.Location = new Point(113, 52);
            Lbl_WorkOrderCode.Margin = new Padding(0);
            Lbl_WorkOrderCode.Name = "Lbl_WorkOrderCode";
            Lbl_WorkOrderCode.Size = new Size(169, 33);
            Lbl_WorkOrderCode.TabIndex = 5;
            Lbl_WorkOrderCode.Text = "-";
            Lbl_WorkOrderCode.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Tlp_DashboardBody
            // 
            Tlp_DashboardBody.ColumnCount = 2;
            Tlp_DashboardBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            Tlp_DashboardBody.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            Tlp_DashboardBody.Controls.Add(Pnl_MachineArea, 0, 0);
            Tlp_DashboardBody.Controls.Add(Pnl_ProcessFlowArea, 0, 1);
            Tlp_DashboardBody.Controls.Add(Pnl_AlarmArea, 1, 0);
            Tlp_DashboardBody.Controls.Add(Pnl_EventArea, 1, 1);
            Tlp_DashboardBody.Controls.Add(Pnl_ControlArea, 0, 2);
            Tlp_DashboardBody.Dock = DockStyle.Fill;
            Tlp_DashboardBody.Location = new Point(0, 260);
            Tlp_DashboardBody.Margin = new Padding(0);
            Tlp_DashboardBody.Name = "Tlp_DashboardBody";
            Tlp_DashboardBody.RowCount = 3;
            Tlp_DashboardBody.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            Tlp_DashboardBody.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            Tlp_DashboardBody.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            Tlp_DashboardBody.Size = new Size(1320, 473);
            Tlp_DashboardBody.TabIndex = 1;
            // 
            // Pnl_MachineArea
            // 
            Pnl_MachineArea.Controls.Add(tableLayoutPanel4);
            Pnl_MachineArea.Dock = DockStyle.Fill;
            Pnl_MachineArea.Location = new Point(3, 3);
            Pnl_MachineArea.Name = "Pnl_MachineArea";
            Pnl_MachineArea.Size = new Size(733, 204);
            Pnl_MachineArea.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(Lbl_MachineAreaTitle, 0, 0);
            tableLayoutPanel4.Controls.Add(Pnl_Vm1Card, 0, 1);
            tableLayoutPanel4.Controls.Add(Pnl_PressCard, 1, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(0, 0);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 41F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.Size = new Size(733, 204);
            tableLayoutPanel4.TabIndex = 0;
            // 
            // Lbl_MachineAreaTitle
            // 
            Lbl_MachineAreaTitle.AutoSize = true;
            tableLayoutPanel4.SetColumnSpan(Lbl_MachineAreaTitle, 2);
            Lbl_MachineAreaTitle.Dock = DockStyle.Fill;
            Lbl_MachineAreaTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_MachineAreaTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_MachineAreaTitle.Location = new Point(3, 0);
            Lbl_MachineAreaTitle.Name = "Lbl_MachineAreaTitle";
            Lbl_MachineAreaTitle.Size = new Size(727, 41);
            Lbl_MachineAreaTitle.TabIndex = 0;
            Lbl_MachineAreaTitle.Text = "설비 상태";
            Lbl_MachineAreaTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Pnl_Vm1Card
            // 
            Pnl_Vm1Card.BackColor = Color.White;
            Pnl_Vm1Card.BorderStyle = BorderStyle.None;
            Pnl_Vm1Card.Controls.Add(Tlp_Vm1);
            Pnl_Vm1Card.Dock = DockStyle.Fill;
            Pnl_Vm1Card.Location = new Point(0, 48);
            Pnl_Vm1Card.Margin = new Padding(0, 7, 6, 0);
            Pnl_Vm1Card.Name = "Pnl_Vm1Card";
            Pnl_Vm1Card.Padding = new Padding(12);
            Pnl_Vm1Card.Size = new Size(360, 156);
            Pnl_Vm1Card.TabIndex = 1;
            // 
            // Tlp_Vm1
            // 
            Tlp_Vm1.ColumnCount = 2;
            Tlp_Vm1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            Tlp_Vm1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            Tlp_Vm1.Controls.Add(Lbl_Vm1Utilization, 1, 4);
            Tlp_Vm1.Controls.Add(Lbl_Vm1UtilizationTitle, 0, 4);
            Tlp_Vm1.Controls.Add(Lbl_Vm1Exited, 1, 3);
            Tlp_Vm1.Controls.Add(Lbl_Vm1ExitedTitle, 0, 3);
            Tlp_Vm1.Controls.Add(Lbl_Vm1Entered, 1, 2);
            Tlp_Vm1.Controls.Add(Lbl_Vm1EnteredTitle, 0, 2);
            Tlp_Vm1.Controls.Add(Lbl_Vm1State, 1, 1);
            Tlp_Vm1.Controls.Add(Lbl_Vm1Title, 0, 0);
            Tlp_Vm1.Controls.Add(Lbl_Vm1StateTitle, 0, 1);
            Tlp_Vm1.Dock = DockStyle.Fill;
            Tlp_Vm1.Location = new Point(12, 12);
            Tlp_Vm1.Name = "Tlp_Vm1";
            Tlp_Vm1.RowCount = 5;
            Tlp_Vm1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_Vm1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Vm1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Vm1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Vm1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Vm1.Size = new Size(334, 130);
            Tlp_Vm1.TabIndex = 0;
            // 
            // Lbl_Vm1Utilization
            // 
            Lbl_Vm1Utilization.AutoSize = true;
            Lbl_Vm1Utilization.Dock = DockStyle.Fill;
            Lbl_Vm1Utilization.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Vm1Utilization.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_Vm1Utilization.Location = new Point(153, 103);
            Lbl_Vm1Utilization.Name = "Lbl_Vm1Utilization";
            Lbl_Vm1Utilization.Size = new Size(178, 27);
            Lbl_Vm1Utilization.TabIndex = 8;
            Lbl_Vm1Utilization.Text = "0.00 %";
            Lbl_Vm1Utilization.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1UtilizationTitle
            // 
            Lbl_Vm1UtilizationTitle.AutoSize = true;
            Lbl_Vm1UtilizationTitle.Dock = DockStyle.Fill;
            Lbl_Vm1UtilizationTitle.Font = new Font("Segoe UI", 9F);
            Lbl_Vm1UtilizationTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_Vm1UtilizationTitle.Location = new Point(3, 103);
            Lbl_Vm1UtilizationTitle.Name = "Lbl_Vm1UtilizationTitle";
            Lbl_Vm1UtilizationTitle.Size = new Size(144, 27);
            Lbl_Vm1UtilizationTitle.TabIndex = 7;
            Lbl_Vm1UtilizationTitle.Text = "가동률";
            Lbl_Vm1UtilizationTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1Exited
            // 
            Lbl_Vm1Exited.AutoSize = true;
            Lbl_Vm1Exited.Dock = DockStyle.Fill;
            Lbl_Vm1Exited.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Vm1Exited.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_Vm1Exited.Location = new Point(153, 79);
            Lbl_Vm1Exited.Name = "Lbl_Vm1Exited";
            Lbl_Vm1Exited.Size = new Size(178, 24);
            Lbl_Vm1Exited.TabIndex = 6;
            Lbl_Vm1Exited.Text = "0";
            Lbl_Vm1Exited.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1ExitedTitle
            // 
            Lbl_Vm1ExitedTitle.AutoSize = true;
            Lbl_Vm1ExitedTitle.Dock = DockStyle.Fill;
            Lbl_Vm1ExitedTitle.Font = new Font("Segoe UI", 9F);
            Lbl_Vm1ExitedTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_Vm1ExitedTitle.Location = new Point(3, 79);
            Lbl_Vm1ExitedTitle.Name = "Lbl_Vm1ExitedTitle";
            Lbl_Vm1ExitedTitle.Size = new Size(144, 24);
            Lbl_Vm1ExitedTitle.TabIndex = 5;
            Lbl_Vm1ExitedTitle.Text = "배출수량";
            Lbl_Vm1ExitedTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1Entered
            // 
            Lbl_Vm1Entered.AutoSize = true;
            Lbl_Vm1Entered.Dock = DockStyle.Fill;
            Lbl_Vm1Entered.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Vm1Entered.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_Vm1Entered.Location = new Point(153, 55);
            Lbl_Vm1Entered.Name = "Lbl_Vm1Entered";
            Lbl_Vm1Entered.Size = new Size(178, 24);
            Lbl_Vm1Entered.TabIndex = 4;
            Lbl_Vm1Entered.Text = "0";
            Lbl_Vm1Entered.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1EnteredTitle
            // 
            Lbl_Vm1EnteredTitle.AutoSize = true;
            Lbl_Vm1EnteredTitle.Dock = DockStyle.Fill;
            Lbl_Vm1EnteredTitle.Font = new Font("Segoe UI", 9F);
            Lbl_Vm1EnteredTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_Vm1EnteredTitle.Location = new Point(3, 55);
            Lbl_Vm1EnteredTitle.Name = "Lbl_Vm1EnteredTitle";
            Lbl_Vm1EnteredTitle.Size = new Size(144, 24);
            Lbl_Vm1EnteredTitle.TabIndex = 3;
            Lbl_Vm1EnteredTitle.Text = "투입수량";
            Lbl_Vm1EnteredTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1State
            // 
            Lbl_Vm1State.AutoSize = true;
            Lbl_Vm1State.Dock = DockStyle.Fill;
            Lbl_Vm1State.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_Vm1State.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_Vm1State.Location = new Point(153, 31);
            Lbl_Vm1State.Name = "Lbl_Vm1State";
            Lbl_Vm1State.Size = new Size(178, 24);
            Lbl_Vm1State.TabIndex = 2;
            Lbl_Vm1State.Text = "---";
            Lbl_Vm1State.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_Vm1Title
            // 
            Lbl_Vm1Title.AutoSize = true;
            Tlp_Vm1.SetColumnSpan(Lbl_Vm1Title, 2);
            Lbl_Vm1Title.Dock = DockStyle.Fill;
            Lbl_Vm1Title.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_Vm1Title.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_Vm1Title.Location = new Point(3, 0);
            Lbl_Vm1Title.Name = "Lbl_Vm1Title";
            Lbl_Vm1Title.Size = new Size(328, 31);
            Lbl_Vm1Title.TabIndex = 0;
            Lbl_Vm1Title.Text = "VM-1";
            Lbl_Vm1Title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_Vm1StateTitle
            // 
            Lbl_Vm1StateTitle.AutoSize = true;
            Lbl_Vm1StateTitle.Dock = DockStyle.Fill;
            Lbl_Vm1StateTitle.Font = new Font("Segoe UI", 9F);
            Lbl_Vm1StateTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_Vm1StateTitle.Location = new Point(3, 31);
            Lbl_Vm1StateTitle.Name = "Lbl_Vm1StateTitle";
            Lbl_Vm1StateTitle.Size = new Size(144, 24);
            Lbl_Vm1StateTitle.TabIndex = 1;
            Lbl_Vm1StateTitle.Text = "상태";
            Lbl_Vm1StateTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Pnl_PressCard
            // 
            Pnl_PressCard.BackColor = Color.White;
            Pnl_PressCard.BorderStyle = BorderStyle.None;
            Pnl_PressCard.Controls.Add(Tlp_Press);
            Pnl_PressCard.Dock = DockStyle.Fill;
            Pnl_PressCard.Location = new Point(372, 48);
            Pnl_PressCard.Margin = new Padding(6, 7, 0, 0);
            Pnl_PressCard.Name = "Pnl_PressCard";
            Pnl_PressCard.Padding = new Padding(12);
            Pnl_PressCard.Size = new Size(361, 156);
            Pnl_PressCard.TabIndex = 2;
            // 
            // Tlp_Press
            // 
            Tlp_Press.ColumnCount = 2;
            Tlp_Press.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            Tlp_Press.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            Tlp_Press.Controls.Add(Lbl_PressUtilization, 1, 4);
            Tlp_Press.Controls.Add(Lbl_PressUtilizationTitle, 0, 4);
            Tlp_Press.Controls.Add(Lbl_PressExited, 1, 3);
            Tlp_Press.Controls.Add(Lbl_PressExitedTitle, 0, 3);
            Tlp_Press.Controls.Add(Lbl_PressEntered, 1, 2);
            Tlp_Press.Controls.Add(Lbl_PressEnteredTitle, 0, 2);
            Tlp_Press.Controls.Add(Lbl_PressState, 1, 1);
            Tlp_Press.Controls.Add(Lbl_PressStateTitle, 0, 1);
            Tlp_Press.Controls.Add(Lbl_PressTitle, 0, 0);
            Tlp_Press.Dock = DockStyle.Fill;
            Tlp_Press.Location = new Point(12, 12);
            Tlp_Press.Name = "Tlp_Press";
            Tlp_Press.RowCount = 5;
            Tlp_Press.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_Press.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Press.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Press.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Press.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            Tlp_Press.Size = new Size(335, 130);
            Tlp_Press.TabIndex = 0;
            // 
            // Lbl_PressUtilization
            // 
            Lbl_PressUtilization.AutoSize = true;
            Lbl_PressUtilization.Dock = DockStyle.Fill;
            Lbl_PressUtilization.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PressUtilization.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_PressUtilization.Location = new Point(153, 103);
            Lbl_PressUtilization.Name = "Lbl_PressUtilization";
            Lbl_PressUtilization.Size = new Size(179, 27);
            Lbl_PressUtilization.TabIndex = 9;
            Lbl_PressUtilization.Text = "0.00 %";
            Lbl_PressUtilization.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressUtilizationTitle
            // 
            Lbl_PressUtilizationTitle.AutoSize = true;
            Lbl_PressUtilizationTitle.Dock = DockStyle.Fill;
            Lbl_PressUtilizationTitle.Font = new Font("Segoe UI", 9F);
            Lbl_PressUtilizationTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_PressUtilizationTitle.Location = new Point(3, 103);
            Lbl_PressUtilizationTitle.Name = "Lbl_PressUtilizationTitle";
            Lbl_PressUtilizationTitle.Size = new Size(144, 27);
            Lbl_PressUtilizationTitle.TabIndex = 8;
            Lbl_PressUtilizationTitle.Text = "가동률";
            Lbl_PressUtilizationTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressExited
            // 
            Lbl_PressExited.AutoSize = true;
            Lbl_PressExited.Dock = DockStyle.Fill;
            Lbl_PressExited.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PressExited.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_PressExited.Location = new Point(153, 79);
            Lbl_PressExited.Name = "Lbl_PressExited";
            Lbl_PressExited.Size = new Size(179, 24);
            Lbl_PressExited.TabIndex = 7;
            Lbl_PressExited.Text = "0";
            Lbl_PressExited.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressExitedTitle
            // 
            Lbl_PressExitedTitle.AutoSize = true;
            Lbl_PressExitedTitle.Dock = DockStyle.Fill;
            Lbl_PressExitedTitle.Font = new Font("Segoe UI", 9F);
            Lbl_PressExitedTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_PressExitedTitle.Location = new Point(3, 79);
            Lbl_PressExitedTitle.Name = "Lbl_PressExitedTitle";
            Lbl_PressExitedTitle.Size = new Size(144, 24);
            Lbl_PressExitedTitle.TabIndex = 6;
            Lbl_PressExitedTitle.Text = "배출수량";
            Lbl_PressExitedTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressEntered
            // 
            Lbl_PressEntered.AutoSize = true;
            Lbl_PressEntered.Dock = DockStyle.Fill;
            Lbl_PressEntered.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PressEntered.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_PressEntered.Location = new Point(153, 55);
            Lbl_PressEntered.Name = "Lbl_PressEntered";
            Lbl_PressEntered.Size = new Size(179, 24);
            Lbl_PressEntered.TabIndex = 5;
            Lbl_PressEntered.Text = "0";
            Lbl_PressEntered.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressEnteredTitle
            // 
            Lbl_PressEnteredTitle.AutoSize = true;
            Lbl_PressEnteredTitle.Dock = DockStyle.Fill;
            Lbl_PressEnteredTitle.Font = new Font("Segoe UI", 9F);
            Lbl_PressEnteredTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_PressEnteredTitle.Location = new Point(3, 55);
            Lbl_PressEnteredTitle.Name = "Lbl_PressEnteredTitle";
            Lbl_PressEnteredTitle.Size = new Size(144, 24);
            Lbl_PressEnteredTitle.TabIndex = 4;
            Lbl_PressEnteredTitle.Text = "투입수량";
            Lbl_PressEnteredTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressState
            // 
            Lbl_PressState.AutoSize = true;
            Lbl_PressState.Dock = DockStyle.Fill;
            Lbl_PressState.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PressState.ForeColor = Color.FromArgb(30, 41, 59);
            Lbl_PressState.Location = new Point(153, 31);
            Lbl_PressState.Name = "Lbl_PressState";
            Lbl_PressState.Size = new Size(179, 24);
            Lbl_PressState.TabIndex = 3;
            Lbl_PressState.Text = "---";
            Lbl_PressState.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressStateTitle
            // 
            Lbl_PressStateTitle.AutoSize = true;
            Lbl_PressStateTitle.Dock = DockStyle.Fill;
            Lbl_PressStateTitle.Font = new Font("Segoe UI", 9F);
            Lbl_PressStateTitle.ForeColor = Color.FromArgb(100, 116, 139);
            Lbl_PressStateTitle.Location = new Point(3, 31);
            Lbl_PressStateTitle.Name = "Lbl_PressStateTitle";
            Lbl_PressStateTitle.Size = new Size(144, 24);
            Lbl_PressStateTitle.TabIndex = 2;
            Lbl_PressStateTitle.Text = "상태";
            Lbl_PressStateTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Lbl_PressTitle
            // 
            Lbl_PressTitle.AutoSize = true;
            Tlp_Press.SetColumnSpan(Lbl_PressTitle, 2);
            Lbl_PressTitle.Dock = DockStyle.Fill;
            Lbl_PressTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_PressTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_PressTitle.Location = new Point(3, 0);
            Lbl_PressTitle.Name = "Lbl_PressTitle";
            Lbl_PressTitle.Size = new Size(329, 31);
            Lbl_PressTitle.TabIndex = 0;
            Lbl_PressTitle.Text = "PRESS";
            Lbl_PressTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Pnl_ProcessFlowArea
            // 
            Pnl_ProcessFlowArea.BackColor = Color.White;
            Pnl_ProcessFlowArea.BorderStyle = BorderStyle.None;
            Pnl_ProcessFlowArea.Controls.Add(Tlp_AlarmArea);
            Pnl_ProcessFlowArea.Dock = DockStyle.Fill;
            Pnl_ProcessFlowArea.Location = new Point(3, 213);
            Pnl_ProcessFlowArea.Name = "Pnl_ProcessFlowArea";
            Pnl_ProcessFlowArea.Size = new Size(733, 120);
            Pnl_ProcessFlowArea.TabIndex = 1;
            // 
            // Tlp_AlarmArea
            // 
            Tlp_AlarmArea.ColumnCount = 1;
            Tlp_AlarmArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_AlarmArea.Controls.Add(Pnl_AlarmHeader, 0, 0);
            Tlp_AlarmArea.Controls.Add(Dgv_ActiveAlarms, 0, 1);
            Tlp_AlarmArea.Dock = DockStyle.Fill;
            Tlp_AlarmArea.Location = new Point(0, 0);
            Tlp_AlarmArea.Margin = new Padding(0);
            Tlp_AlarmArea.Name = "Tlp_AlarmArea";
            Tlp_AlarmArea.RowCount = 2;
            Tlp_AlarmArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_AlarmArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_AlarmArea.Size = new Size(731, 118);
            Tlp_AlarmArea.TabIndex = 0;
            // 
            // Pnl_AlarmHeader
            // 
            Pnl_AlarmHeader.BackColor = Color.White;
            Pnl_AlarmHeader.Controls.Add(Lbl_ActiveAlarmCount);
            Pnl_AlarmHeader.Controls.Add(Lbl_AlarmAreaTitle);
            Pnl_AlarmHeader.Dock = DockStyle.Fill;
            Pnl_AlarmHeader.Location = new Point(0, 0);
            Pnl_AlarmHeader.Margin = new Padding(0);
            Pnl_AlarmHeader.Name = "Pnl_AlarmHeader";
            Pnl_AlarmHeader.Size = new Size(731, 41);
            Pnl_AlarmHeader.TabIndex = 0;
            // 
            // Lbl_ActiveAlarmCount
            // 
            Lbl_ActiveAlarmCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lbl_ActiveAlarmCount.AutoSize = false;
            Lbl_ActiveAlarmCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ActiveAlarmCount.ForeColor = Color.Red;
            Lbl_ActiveAlarmCount.Location = new Point(309, 12);
            Lbl_ActiveAlarmCount.Name = "Lbl_ActiveAlarmCount";
            Lbl_ActiveAlarmCount.Size = new Size(90, 36);
            Lbl_ActiveAlarmCount.TabIndex = 1;
            Lbl_ActiveAlarmCount.Text = "0건";
            // 
            // Lbl_AlarmAreaTitle
            // 
            Lbl_AlarmAreaTitle.AutoSize = false;
            Lbl_AlarmAreaTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_AlarmAreaTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_AlarmAreaTitle.Location = new Point(0, 9);
            Lbl_AlarmAreaTitle.Name = "Lbl_AlarmAreaTitle";
            Lbl_AlarmAreaTitle.Size = new Size(160, 36);
            Lbl_AlarmAreaTitle.TabIndex = 0;
            Lbl_AlarmAreaTitle.Text = "활성 알람";
            // 
            // Dgv_ActiveAlarms
            // 
            Dgv_ActiveAlarms.AllowUserToAddRows = false;
            Dgv_ActiveAlarms.AllowUserToDeleteRows = false;
            Dgv_ActiveAlarms.AllowUserToResizeColumns = false;
            dataGridViewCellStyle7.BackColor = Color.FromArgb(248, 250, 252);
            Dgv_ActiveAlarms.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            Dgv_ActiveAlarms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Dgv_ActiveAlarms.BackgroundColor = Color.White;
            Dgv_ActiveAlarms.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle8.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = Color.FromArgb(30, 58, 95);
            dataGridViewCellStyle8.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle8.ForeColor = Color.White;
            dataGridViewCellStyle8.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = DataGridViewTriState.True;
            Dgv_ActiveAlarms.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            Dgv_ActiveAlarms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            Dgv_ActiveAlarms.Columns.AddRange(new DataGridViewColumn[] { colOccurredAt, colMachineCode, colSeverity, colAlarmCode, colAlarmMessage });
            dataGridViewCellStyle9.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = Color.White;
            dataGridViewCellStyle9.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dataGridViewCellStyle9.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle9.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridViewCellStyle9.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle9.WrapMode = DataGridViewTriState.False;
            Dgv_ActiveAlarms.DefaultCellStyle = dataGridViewCellStyle9;
            Dgv_ActiveAlarms.Dock = DockStyle.Fill;
            Dgv_ActiveAlarms.EnableHeadersVisualStyles = false;
            Dgv_ActiveAlarms.GridColor = Color.FromArgb(226, 232, 240);
            Dgv_ActiveAlarms.Location = new Point(0, 46);
            Dgv_ActiveAlarms.Margin = new Padding(0, 5, 0, 0);
            Dgv_ActiveAlarms.MultiSelect = false;
            Dgv_ActiveAlarms.Name = "Dgv_ActiveAlarms";
            Dgv_ActiveAlarms.ReadOnly = true;
            dataGridViewCellStyle10.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = SystemColors.Control;
            dataGridViewCellStyle10.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle10.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = DataGridViewTriState.True;
            Dgv_ActiveAlarms.RowHeadersDefaultCellStyle = dataGridViewCellStyle10;
            Dgv_ActiveAlarms.RowHeadersVisible = false;
            Dgv_ActiveAlarms.RowTemplate.Height = 30;
            Dgv_ActiveAlarms.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Dgv_ActiveAlarms.Size = new Size(731, 72);
            Dgv_ActiveAlarms.TabIndex = 1;
            // 
            // colOccurredAt
            // 
            colOccurredAt.DataPropertyName = "OccurredAt";
            colOccurredAt.HeaderText = "발생시간";
            colOccurredAt.Name = "colOccurredAt";
            colOccurredAt.ReadOnly = true;
            // 
            // colMachineCode
            // 
            colMachineCode.DataPropertyName = "MachineCode";
            colMachineCode.HeaderText = "설비";
            colMachineCode.Name = "colMachineCode";
            colMachineCode.ReadOnly = true;
            // 
            // colSeverity
            // 
            colSeverity.DataPropertyName = "Severity";
            colSeverity.HeaderText = "등급";
            colSeverity.Name = "colSeverity";
            colSeverity.ReadOnly = true;
            // 
            // colAlarmCode
            // 
            colAlarmCode.DataPropertyName = "AlarmCode";
            colAlarmCode.HeaderText = "알람코드";
            colAlarmCode.Name = "colAlarmCode";
            colAlarmCode.ReadOnly = true;
            // 
            // colAlarmMessage
            // 
            colAlarmMessage.DataPropertyName = "AlarmMessage";
            colAlarmMessage.HeaderText = "알람내용";
            colAlarmMessage.Name = "colAlarmMessage";
            colAlarmMessage.ReadOnly = true;
            // 
            // Pnl_AlarmArea
            // 
            Pnl_AlarmArea.BackColor = Color.White;
            Pnl_AlarmArea.BorderStyle = BorderStyle.None;
            Pnl_AlarmArea.Controls.Add(Tlp_ProcessFlow);
            Pnl_AlarmArea.Dock = DockStyle.Fill;
            Pnl_AlarmArea.Location = new Point(742, 3);
            Pnl_AlarmArea.Name = "Pnl_AlarmArea";
            Pnl_AlarmArea.Padding = new Padding(10);
            Pnl_AlarmArea.Size = new Size(575, 204);
            Pnl_AlarmArea.TabIndex = 2;
            // 
            // Tlp_ProcessFlow
            // 
            Tlp_ProcessFlow.ColumnCount = 1;
            Tlp_ProcessFlow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_ProcessFlow.Controls.Add(Lbl_ProcessFlowTitle, 0, 0);
            Tlp_ProcessFlow.Controls.Add(Tlp_ProcessMain, 0, 1);
            Tlp_ProcessFlow.Controls.Add(Tlp_ProcessRoute, 0, 2);
            Tlp_ProcessFlow.Dock = DockStyle.Fill;
            Tlp_ProcessFlow.Location = new Point(8, 8);
            Tlp_ProcessFlow.Margin = new Padding(0);
            Tlp_ProcessFlow.Name = "Tlp_ProcessFlow";
            Tlp_ProcessFlow.RowCount = 3;
            Tlp_ProcessFlow.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_ProcessFlow.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            Tlp_ProcessFlow.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            Tlp_ProcessFlow.Size = new Size(557, 186);
            Tlp_ProcessFlow.TabIndex = 0;
            // 
            // Lbl_ProcessFlowTitle
            // 
            Lbl_ProcessFlowTitle.AutoSize = true;
            Lbl_ProcessFlowTitle.Dock = DockStyle.Fill;
            Lbl_ProcessFlowTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_ProcessFlowTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProcessFlowTitle.Location = new Point(3, 0);
            Lbl_ProcessFlowTitle.Name = "Lbl_ProcessFlowTitle";
            Lbl_ProcessFlowTitle.Size = new Size(551, 41);
            Lbl_ProcessFlowTitle.TabIndex = 0;
            Lbl_ProcessFlowTitle.Text = "공정 흐름";
            Lbl_ProcessFlowTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Tlp_ProcessMain
            // 
            Tlp_ProcessMain.ColumnCount = 5;
            Tlp_ProcessMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            Tlp_ProcessMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            Tlp_ProcessMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            Tlp_ProcessMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 30F));
            Tlp_ProcessMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            Tlp_ProcessMain.Controls.Add(Pnl_ProcessVm1, 0, 0);
            Tlp_ProcessMain.Controls.Add(Pnl_ProcessPress, 2, 0);
            Tlp_ProcessMain.Controls.Add(Lbl_ArrowPressQuality, 3, 0);
            Tlp_ProcessMain.Controls.Add(Pnl_ProcessQuality, 4, 0);
            Tlp_ProcessMain.Controls.Add(Lbl_ArrowVm1Press, 1, 0);
            Tlp_ProcessMain.Dock = DockStyle.Fill;
            Tlp_ProcessMain.Location = new Point(0, 41);
            Tlp_ProcessMain.Margin = new Padding(0);
            Tlp_ProcessMain.Name = "Tlp_ProcessMain";
            Tlp_ProcessMain.RowCount = 1;
            Tlp_ProcessMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_ProcessMain.Size = new Size(557, 79);
            Tlp_ProcessMain.TabIndex = 1;
            // 
            // Pnl_ProcessVm1
            // 
            Pnl_ProcessVm1.BackColor = Color.FromArgb(248, 250, 252);
            Pnl_ProcessVm1.BorderStyle = BorderStyle.None;
            Pnl_ProcessVm1.Controls.Add(Tlp_ProcessVm1Labels);
            Pnl_ProcessVm1.Dock = DockStyle.Fill;
            Pnl_ProcessVm1.Location = new Point(4, 9);
            Pnl_ProcessVm1.Margin = new Padding(4, 9, 4, 9);
            Pnl_ProcessVm1.Name = "Pnl_ProcessVm1";
            Pnl_ProcessVm1.Padding = new Padding(6);
            Pnl_ProcessVm1.Size = new Size(136, 61);
            Pnl_ProcessVm1.TabIndex = 0;
            // 
            // Lbl_ProcessVm1Title
            // 
            Lbl_ProcessVm1Title.AutoSize = false;
            Lbl_ProcessVm1Title.BackColor = Color.Transparent;
            Lbl_ProcessVm1Title.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProcessVm1Title.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProcessVm1Title.Location = new Point(0, 0);
            Lbl_ProcessVm1Title.Name = "Lbl_ProcessVm1Title";
            Lbl_ProcessVm1Title.Size = new Size(134, 20);
            Lbl_ProcessVm1Title.TabIndex = 0;
            Lbl_ProcessVm1Title.Text = "VM1";
            Lbl_ProcessVm1Title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_ProcessPressTitle
            // 
            // 
            // Pnl_ProcessPress
            // 
            Pnl_ProcessPress.BackColor = Color.FromArgb(248, 250, 252);
            Pnl_ProcessPress.BorderStyle = BorderStyle.None;
            Pnl_ProcessPress.Controls.Add(Tlp_ProcessPressLabels);
            Pnl_ProcessPress.Dock = DockStyle.Fill;
            Pnl_ProcessPress.Location = new Point(209, 9);
            Pnl_ProcessPress.Margin = new Padding(4, 9, 4, 9);
            Pnl_ProcessPress.Name = "Pnl_ProcessPress";
            Pnl_ProcessPress.Padding = new Padding(6);
            Pnl_ProcessPress.Size = new Size(136, 61);
            Pnl_ProcessPress.TabIndex = 2;
            // 
            // Lbl_ProcessPressState
            // 
            Lbl_ProcessPressState.AutoSize = false;
            Lbl_ProcessPressState.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Lbl_ProcessPressState.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_ProcessPressState.Location = new Point(0, 0);
            Lbl_ProcessPressState.Name = "Lbl_ProcessPressState";
            Lbl_ProcessPressState.Size = new Size(134, 32);
            Lbl_ProcessPressState.TabIndex = 2;
            Lbl_ProcessPressState.Text = "대기";
            Lbl_ProcessPressState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_ProcessPressTitle
            // 
            Lbl_ProcessPressTitle.AutoSize = false;
            Lbl_ProcessPressTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProcessPressTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProcessPressTitle.Location = new Point(0, 0);
            Lbl_ProcessPressTitle.Name = "Lbl_ProcessPressTitle";
            Lbl_ProcessPressTitle.Size = new Size(50, 19);
            Lbl_ProcessPressTitle.TabIndex = 1;
            Lbl_ProcessPressTitle.Text = "PRESS";
            Lbl_ProcessPressTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_ArrowPressQuality
            // 
            Lbl_ArrowPressQuality.AutoSize = true;
            Lbl_ArrowPressQuality.Dock = DockStyle.Fill;
            Lbl_ArrowPressQuality.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            Lbl_ArrowPressQuality.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_ArrowPressQuality.Location = new Point(352, 0);
            Lbl_ArrowPressQuality.Name = "Lbl_ArrowPressQuality";
            Lbl_ArrowPressQuality.Size = new Size(55, 79);
            Lbl_ArrowPressQuality.TabIndex = 3;
            Lbl_ArrowPressQuality.Text = "→";
            Lbl_ArrowPressQuality.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Pnl_ProcessQuality
            // 
            Pnl_ProcessQuality.BackColor = Color.FromArgb(248, 250, 252);
            Pnl_ProcessQuality.BorderStyle = BorderStyle.None;
            Pnl_ProcessQuality.Controls.Add(Tlp_ProcessQualityLabels);
            Pnl_ProcessQuality.Dock = DockStyle.Fill;
            Pnl_ProcessQuality.Location = new Point(414, 9);
            Pnl_ProcessQuality.Margin = new Padding(4, 9, 4, 9);
            Pnl_ProcessQuality.Name = "Pnl_ProcessQuality";
            Pnl_ProcessQuality.Padding = new Padding(6);
            Pnl_ProcessQuality.Size = new Size(139, 61);
            Pnl_ProcessQuality.TabIndex = 4;
            // 
            // Lbl_ProcessQualityState
            // 
            Lbl_ProcessQualityState.AutoSize = false;
            Lbl_ProcessQualityState.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProcessQualityState.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_ProcessQualityState.Location = new Point(0, 0);
            Lbl_ProcessQualityState.Name = "Lbl_ProcessQualityState";
            Lbl_ProcessQualityState.Size = new Size(27, 19);
            Lbl_ProcessQualityState.TabIndex = 2;
            Lbl_ProcessQualityState.Text = "---";
            Lbl_ProcessQualityState.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_ProcessQualityTitle
            // 
            Lbl_ProcessQualityTitle.AutoSize = false;
            Lbl_ProcessQualityTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProcessQualityTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_ProcessQualityTitle.Location = new Point(0, 0);
            Lbl_ProcessQualityTitle.Name = "Lbl_ProcessQualityTitle";
            Lbl_ProcessQualityTitle.Size = new Size(117, 19);
            Lbl_ProcessQualityTitle.TabIndex = 1;
            Lbl_ProcessQualityTitle.Text = "QUALITY CHECK";
            Lbl_ProcessQualityTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_ArrowVm1Press
            // 
            Lbl_ArrowVm1Press.AutoSize = true;
            Lbl_ArrowVm1Press.Dock = DockStyle.Fill;
            Lbl_ArrowVm1Press.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            Lbl_ArrowVm1Press.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_ArrowVm1Press.Location = new Point(147, 0);
            Lbl_ArrowVm1Press.Name = "Lbl_ArrowVm1Press";
            Lbl_ArrowVm1Press.Size = new Size(55, 79);
            Lbl_ArrowVm1Press.TabIndex = 1;
            Lbl_ArrowVm1Press.Text = "→";
            Lbl_ArrowVm1Press.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Tlp_ProcessRoute
            // 
            Tlp_ProcessRoute.ColumnCount = 2;
            Tlp_ProcessRoute.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            Tlp_ProcessRoute.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            Tlp_ProcessRoute.Controls.Add(Pnl_PassRoute, 0, 0);
            Tlp_ProcessRoute.Controls.Add(Pnl_FailRoute, 1, 0);
            Tlp_ProcessRoute.Dock = DockStyle.Fill;
            Tlp_ProcessRoute.Location = new Point(3, 123);
            Tlp_ProcessRoute.Name = "Tlp_ProcessRoute";
            Tlp_ProcessRoute.RowCount = 1;
            Tlp_ProcessRoute.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_ProcessRoute.Size = new Size(551, 60);
            Tlp_ProcessRoute.TabIndex = 2;
            // 
            // Pnl_PassRoute
            // 
            Pnl_PassRoute.BackColor = Color.FromArgb(236, 253, 245);
            Pnl_PassRoute.BorderStyle = BorderStyle.None;
            Pnl_PassRoute.Controls.Add(Tlp_PassRouteLabels);
            Pnl_PassRoute.Dock = DockStyle.Fill;
            Pnl_PassRoute.Location = new Point(4, 5);
            Pnl_PassRoute.Margin = new Padding(4, 5, 4, 5);
            Pnl_PassRoute.Name = "Pnl_PassRoute";
            Pnl_PassRoute.Size = new Size(267, 50);
            Pnl_PassRoute.TabIndex = 0;
            // 
            // Lbl_PassDestination
            // 
            Lbl_PassDestination.AutoSize = false;
            Lbl_PassDestination.BackColor = Color.Transparent;
            Lbl_PassDestination.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PassDestination.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_PassDestination.Location = new Point(0, 0);
            Lbl_PassDestination.Name = "Lbl_PassDestination";
            Lbl_PassDestination.Size = new Size(132, 48);
            Lbl_PassDestination.TabIndex = 1;
            Lbl_PassDestination.Text = "양품 보관";
            Lbl_PassDestination.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_PassTitle
            // 
            Lbl_PassTitle.AutoSize = false;
            Lbl_PassTitle.BackColor = Color.Transparent;
            Lbl_PassTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_PassTitle.ForeColor = Color.FromArgb(22, 163, 74);
            Lbl_PassTitle.Location = new Point(0, 0);
            Lbl_PassTitle.Name = "Lbl_PassTitle";
            Lbl_PassTitle.Size = new Size(133, 48);
            Lbl_PassTitle.TabIndex = 0;
            Lbl_PassTitle.Text = "양품";
            Lbl_PassTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Pnl_FailRoute
            // 
            Pnl_FailRoute.BackColor = Color.FromArgb(254, 242, 242);
            Pnl_FailRoute.BorderStyle = BorderStyle.None;
            Pnl_FailRoute.Controls.Add(Tlp_FailRouteLabels);
            Pnl_FailRoute.Dock = DockStyle.Fill;
            Pnl_FailRoute.Location = new Point(279, 5);
            Pnl_FailRoute.Margin = new Padding(4, 5, 4, 5);
            Pnl_FailRoute.Name = "Pnl_FailRoute";
            Pnl_FailRoute.Size = new Size(268, 50);
            Pnl_FailRoute.TabIndex = 1;
            // 
            // Lbl_FailDestination
            // 
            Lbl_FailDestination.AutoSize = false;
            Lbl_FailDestination.BackColor = Color.Transparent;
            Lbl_FailDestination.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_FailDestination.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_FailDestination.Location = new Point(0, 0);
            Lbl_FailDestination.Name = "Lbl_FailDestination";
            Lbl_FailDestination.Size = new Size(133, 48);
            Lbl_FailDestination.TabIndex = 2;
            Lbl_FailDestination.Text = "불량품 적재";
            Lbl_FailDestination.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Lbl_FailTitle
            // 
            Lbl_FailTitle.AutoSize = false;
            Lbl_FailTitle.BackColor = Color.Transparent;
            Lbl_FailTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_FailTitle.ForeColor = Color.FromArgb(220, 38, 38);
            Lbl_FailTitle.Location = new Point(0, 0);
            Lbl_FailTitle.Name = "Lbl_FailTitle";
            Lbl_FailTitle.Size = new Size(133, 48);
            Lbl_FailTitle.TabIndex = 1;
            Lbl_FailTitle.Text = "불량";
            Lbl_FailTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Pnl_EventArea
            // 
            Pnl_EventArea.BackColor = Color.White;
            Pnl_EventArea.BorderStyle = BorderStyle.None;
            Pnl_EventArea.Controls.Add(Tlp_EventArea);
            Pnl_EventArea.Dock = DockStyle.Fill;
            Pnl_EventArea.Location = new Point(742, 213);
            Pnl_EventArea.Name = "Pnl_EventArea";
            Pnl_EventArea.Padding = new Padding(10);
            Pnl_EventArea.Size = new Size(575, 120);
            Pnl_EventArea.TabIndex = 3;
            // 
            // Tlp_EventArea
            // 
            Tlp_EventArea.ColumnCount = 1;
            Tlp_EventArea.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_EventArea.Controls.Add(Lbl_EventAreaTitle, 0, 0);
            Tlp_EventArea.Controls.Add(Dgv_RecentEvents, 0, 1);
            Tlp_EventArea.Dock = DockStyle.Fill;
            Tlp_EventArea.Location = new Point(8, 8);
            Tlp_EventArea.Margin = new Padding(0);
            Tlp_EventArea.Name = "Tlp_EventArea";
            Tlp_EventArea.RowCount = 2;
            Tlp_EventArea.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            Tlp_EventArea.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_EventArea.Size = new Size(557, 102);
            Tlp_EventArea.TabIndex = 0;
            // 
            // Lbl_EventAreaTitle
            // 
            Lbl_EventAreaTitle.AutoSize = true;
            Lbl_EventAreaTitle.Dock = DockStyle.Fill;
            Lbl_EventAreaTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Lbl_EventAreaTitle.ForeColor = Color.FromArgb(30, 58, 95);
            Lbl_EventAreaTitle.Location = new Point(0, 0);
            Lbl_EventAreaTitle.Margin = new Padding(0);
            Lbl_EventAreaTitle.Name = "Lbl_EventAreaTitle";
            Lbl_EventAreaTitle.Size = new Size(557, 41);
            Lbl_EventAreaTitle.TabIndex = 0;
            Lbl_EventAreaTitle.Text = "최근 공정 이벤트";
            Lbl_EventAreaTitle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Dgv_RecentEvents
            // 
            Dgv_RecentEvents.AllowUserToAddRows = false;
            Dgv_RecentEvents.AllowUserToDeleteRows = false;
            Dgv_RecentEvents.AllowUserToOrderColumns = true;
            Dgv_RecentEvents.AllowUserToResizeRows = false;
            Dgv_RecentEvents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            Dgv_RecentEvents.BackgroundColor = Color.White;
            Dgv_RecentEvents.BorderStyle = BorderStyle.None;
            dataGridViewCellStyle11.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = Color.FromArgb(30, 58, 95);
            dataGridViewCellStyle11.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle11.ForeColor = Color.White;
            dataGridViewCellStyle11.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = DataGridViewTriState.True;
            Dgv_RecentEvents.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            Dgv_RecentEvents.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            Dgv_RecentEvents.Columns.AddRange(new DataGridViewColumn[] { Col_EventTime, Col_ProductCode, Col_EventType, Col_FromLocation, Col_ToLocation, Col_Remarks });
            dataGridViewCellStyle12.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = Color.White;
            dataGridViewCellStyle12.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle12.ForeColor = Color.FromArgb(30, 41, 59);
            dataGridViewCellStyle12.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dataGridViewCellStyle12.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dataGridViewCellStyle12.WrapMode = DataGridViewTriState.False;
            Dgv_RecentEvents.DefaultCellStyle = dataGridViewCellStyle12;
            Dgv_RecentEvents.Dock = DockStyle.Fill;
            Dgv_RecentEvents.EnableHeadersVisualStyles = false;
            Dgv_RecentEvents.GridColor = Color.FromArgb(226, 232, 240);
            Dgv_RecentEvents.Location = new Point(0, 46);
            Dgv_RecentEvents.Margin = new Padding(0, 5, 0, 0);
            Dgv_RecentEvents.MultiSelect = false;
            Dgv_RecentEvents.Name = "Dgv_RecentEvents";
            Dgv_RecentEvents.ReadOnly = true;
            Dgv_RecentEvents.RowHeadersVisible = false;
            Dgv_RecentEvents.RowTemplate.Height = 30;
            Dgv_RecentEvents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Dgv_RecentEvents.Size = new Size(557, 56);
            Dgv_RecentEvents.TabIndex = 1;
            // 
            // Col_EventTime
            // 
            Col_EventTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col_EventTime.DataPropertyName = "EventTime";
            Col_EventTime.Frozen = true;
            Col_EventTime.HeaderText = "발생시간";
            Col_EventTime.Name = "Col_EventTime";
            Col_EventTime.ReadOnly = true;
            Col_EventTime.Width = 145;
            // 
            // Col_ProductCode
            // 
            Col_ProductCode.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col_ProductCode.DataPropertyName = "ProductCode";
            Col_ProductCode.FillWeight = 155F;
            Col_ProductCode.Frozen = true;
            Col_ProductCode.HeaderText = "제품코드";
            Col_ProductCode.Name = "Col_ProductCode";
            Col_ProductCode.ReadOnly = true;
            Col_ProductCode.Width = 180;
            // 
            // Col_EventType
            // 
            Col_EventType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Col_EventType.DataPropertyName = "EventType";
            Col_EventType.FillWeight = 75F;
            Col_EventType.HeaderText = "이벤트";
            Col_EventType.Name = "Col_EventType";
            Col_EventType.ReadOnly = true;
            Col_EventType.Width = 85;
            // 
            // Col_FromLocation
            // 
            Col_FromLocation.DataPropertyName = "FromLocationCode";
            Col_FromLocation.FillWeight = 85F;
            Col_FromLocation.HeaderText = "이전위치";
            Col_FromLocation.Name = "Col_FromLocation";
            Col_FromLocation.ReadOnly = true;
            // 
            // Col_ToLocation
            // 
            Col_ToLocation.DataPropertyName = "ToLocationCode";
            Col_ToLocation.FillWeight = 85F;
            Col_ToLocation.HeaderText = "현재위치";
            Col_ToLocation.Name = "Col_ToLocation";
            Col_ToLocation.ReadOnly = true;
            // 
            // Col_Remarks
            // 
            Col_Remarks.DataPropertyName = "Remarks";
            Col_Remarks.FillWeight = 200F;
            Col_Remarks.HeaderText = "내용";
            Col_Remarks.Name = "Col_Remarks";
            Col_Remarks.ReadOnly = true;
            // 
            // Pnl_ControlArea
            // 
            Tlp_DashboardBody.SetColumnSpan(Pnl_ControlArea, 2);
            Pnl_ControlArea.Controls.Add(Tlp_Control);
            Pnl_ControlArea.Dock = DockStyle.Fill;
            Pnl_ControlArea.Location = new Point(0, 345);
            Pnl_ControlArea.Margin = new Padding(0, 9, 0, 0);
            Pnl_ControlArea.Name = "Pnl_ControlArea";
            Pnl_ControlArea.Padding = new Padding(0);
            Pnl_ControlArea.Size = new Size(1320, 128);
            Pnl_ControlArea.TabIndex = 4;
            // 
            // Tlp_Control
            // 
            Tlp_Control.ColumnCount = 3;
            Tlp_Control.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            Tlp_Control.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            Tlp_Control.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            Tlp_Control.Controls.Add(Btn_Reset, 2, 0);
            Tlp_Control.Controls.Add(Btn_Stop, 1, 0);
            Tlp_Control.Controls.Add(Btn_Start, 0, 0);
            Tlp_Control.Dock = DockStyle.Fill;
            Tlp_Control.Location = new Point(6, 6);
            Tlp_Control.Name = "Tlp_Control";
            Tlp_Control.RowCount = 1;
            Tlp_Control.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Tlp_Control.Size = new Size(1308, 116);
            Tlp_Control.TabIndex = 0;
            // 
            // Btn_Reset
            // 
            Btn_Reset.BackColor = Color.FromArgb(217, 119, 6);
            Btn_Reset.Cursor = Cursors.Hand;
            Btn_Reset.Dock = DockStyle.Fill;


            Btn_Reset.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            Btn_Reset.ForeColor = Color.White;
            Btn_Reset.Location = new Point(878, 8);
            Btn_Reset.Margin = new Padding(6, 6, 0, 0);
            Btn_Reset.Name = "Btn_Reset";
            Btn_Reset.Size = new Size(422, 100);
            Btn_Reset.TabIndex = 2;
            Btn_Reset.Text = "RESET";

            Btn_Reset.Click += Btn_Reset_Click;
            // 
            // Btn_Stop
            // 
            Btn_Stop.BackColor = Color.FromArgb(220, 38, 38);
            Btn_Stop.Cursor = Cursors.Hand;
            Btn_Stop.Dock = DockStyle.Fill;


            Btn_Stop.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            Btn_Stop.ForeColor = Color.White;
            Btn_Stop.Location = new Point(443, 8);
            Btn_Stop.Margin = new Padding(6, 6, 6, 0);
            Btn_Stop.Name = "Btn_Stop";
            Btn_Stop.Size = new Size(419, 100);
            Btn_Stop.TabIndex = 1;
            Btn_Stop.Text = "STOP";

            Btn_Stop.Click += Btn_Stop_Click;
            // 
            // Btn_Start
            // 
            Btn_Start.BackColor = Color.FromArgb(22, 163, 74);
            Btn_Start.Cursor = Cursors.Hand;
            Btn_Start.Dock = DockStyle.Fill;


            Btn_Start.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            Btn_Start.ForeColor = Color.White;
            Btn_Start.Location = new Point(8, 8);
            Btn_Start.Margin = new Padding(0, 6, 6, 0);
            Btn_Start.Name = "Btn_Start";
            Btn_Start.Size = new Size(419, 100);
            Btn_Start.TabIndex = 0;
            Btn_Start.Text = "START";

            Btn_Start.Click += Btn_Start_Click;
            // 
            // Responsive label layout
            Tlp_ContentRoot.Margin = new Padding(0);
            Lbl_WorkOrderTitle.Margin = new Padding(0);
            Lbl_LotTitle.Margin = new Padding(0);
            Lbl_ProductTitle.Margin = new Padding(0);
            Lbl_WorkOrderAchievement.AutoEllipsis = true;
            Lbl_WorkOrderAchievement.Padding = new Padding(0);
            Lbl_WorkOrderTarget.AutoEllipsis = true;
            Lbl_WorkOrderTarget.Padding = new Padding(0);
            Lbl_WorkOrderStatus.AutoEllipsis = true;
            Lbl_WorkOrderStatus.Padding = new Padding(0);
            Lbl_WorkOrderAchievementTitle.AutoEllipsis = true;
            Lbl_WorkOrderAchievementTitle.Padding = new Padding(0);
            Lbl_WorkOrderTargetTitle.AutoEllipsis = true;
            Lbl_WorkOrderTargetTitle.Padding = new Padding(0);
            Lbl_WorkOrderStatusTitle.AutoEllipsis = true;
            Lbl_WorkOrderStatusTitle.Padding = new Padding(0);
            Lbl_WorkOrderTitle.AutoEllipsis = true;
            Lbl_WorkOrderTitle.Margin = new Padding(0);
            Lbl_WorkOrderTitle.Padding = new Padding(0);
            Lbl_WorkOrderCodeTitle.AutoEllipsis = true;
            Lbl_WorkOrderCode.AutoEllipsis = true;
            Lbl_WorkOrderCode.Padding = new Padding(0);
            Lbl_LotProgress.AutoEllipsis = true;
            Lbl_LotProgress.Padding = new Padding(0);
            Lbl_LotProgressTitle.AutoEllipsis = true;
            Lbl_LotProgressTitle.Padding = new Padding(0);
            Lbl_LotProduction.AutoEllipsis = true;
            Lbl_LotProduction.Padding = new Padding(0);
            Lbl_LotQuantityTitle.AutoEllipsis = true;
            Lbl_LotQuantityTitle.Padding = new Padding(0);
            Lbl_LotStatus.AutoEllipsis = true;
            Lbl_LotStatus.Padding = new Padding(0);
            Lbl_LotStatusTitle.AutoEllipsis = true;
            Lbl_LotStatusTitle.Padding = new Padding(0);
            Lbl_LotCode.AutoEllipsis = true;
            Lbl_LotCode.Padding = new Padding(0);
            Lbl_LotCodeTitle.AutoEllipsis = true;
            Lbl_LotCodeTitle.Padding = new Padding(0);
            Lbl_LotTitle.AutoEllipsis = true;
            Lbl_LotTitle.Margin = new Padding(0);
            Lbl_LotTitle.Padding = new Padding(0);
            Lbl_ProductLocation.AutoEllipsis = true;
            Lbl_ProductLocation.Padding = new Padding(0);
            Lbl_ProductLocationTitle.AutoEllipsis = true;
            Lbl_ProductLocationTitle.Padding = new Padding(0);
            Lbl_ProductQuality.AutoEllipsis = true;
            Lbl_ProductQuality.Padding = new Padding(0);
            Lbl_ProductStatus.AutoEllipsis = true;
            Lbl_ProductStatus.Padding = new Padding(0);
            Lbl_ProductCode.AutoEllipsis = true;
            Lbl_ProductCode.Padding = new Padding(0);
            Lbl_ProductTitle.AutoEllipsis = true;
            Lbl_ProductTitle.Margin = new Padding(0);
            Lbl_ProductTitle.Padding = new Padding(0);
            Lbl_ProductCodeTitle.AutoEllipsis = true;
            Lbl_ProductCodeTitle.Padding = new Padding(0);
            Lbl_ProductStatusTitle.AutoEllipsis = true;
            Lbl_ProductStatusTitle.Padding = new Padding(0);
            Lbl_ProductQualityTitle.AutoEllipsis = true;
            Lbl_ProductQualityTitle.Padding = new Padding(0);
            Lbl_Yield.AutoEllipsis = true;
            Lbl_Yield.Padding = new Padding(0);
            Lbl_YieldTitle.AutoEllipsis = true;
            Lbl_YieldTitle.Padding = new Padding(0);
            Lbl_Defect.AutoEllipsis = true;
            Lbl_Defect.Padding = new Padding(0);
            Lbl_DefectTitle.AutoEllipsis = true;
            Lbl_DefectTitle.Padding = new Padding(0);
            Lbl_Good.AutoEllipsis = true;
            Lbl_Good.Padding = new Padding(0);
            Lbl_GoodTitle.AutoEllipsis = true;
            Lbl_GoodTitle.Padding = new Padding(0);
            Lbl_Produced.AutoEllipsis = true;
            Lbl_Produced.Padding = new Padding(0);
            Lbl_ProducedTitle.AutoEllipsis = true;
            Lbl_ProducedTitle.Padding = new Padding(0);
            Lbl_Target.AutoEllipsis = true;
            Lbl_Target.Padding = new Padding(0);
            Lbl_ProductionTitle.AutoEllipsis = true;
            Lbl_ProductionTitle.Padding = new Padding(0);
            Lbl_TargetTitle.AutoEllipsis = true;
            Lbl_TargetTitle.Padding = new Padding(0);
            Lbl_Good.ForeColor = Color.FromArgb(21, 128, 61);
            Lbl_Defect.ForeColor = Color.FromArgb(185, 28, 28);
            Lbl_Yield.ForeColor = Color.FromArgb(29, 78, 216);
            Pnl_ProcessFlowArea.Padding = new Padding(10);
            Pnl_MachineArea.Margin = new Padding(0, 0, 6, 6);
            Pnl_AlarmArea.Margin = new Padding(6, 0, 0, 6);
            Pnl_ProcessFlowArea.Margin = new Padding(0, 6, 6, 0);
            Pnl_EventArea.Margin = new Padding(6, 6, 0, 0);
            Tlp_Control.Margin = new Padding(0);
            Lbl_ProcessVm1Title.Dock = DockStyle.Fill;
            Lbl_ProcessVm1Title.Margin = new Padding(0);
            Lbl_ProcessVm1Title.AutoEllipsis = true;
            Lbl_ProcessVm1State.AutoSize = false;
            Lbl_ProcessVm1State.Dock = DockStyle.Fill;
            Lbl_ProcessVm1State.Margin = new Padding(0);
            Lbl_ProcessVm1State.TextAlign = ContentAlignment.MiddleCenter;
            Lbl_ProcessVm1State.BackColor = Color.Transparent;
            Lbl_ProcessVm1State.Location = new Point(0, 0);
            Lbl_ProcessVm1State.AutoEllipsis = true;
            Tlp_ProcessVm1Labels.ColumnCount = 1;
            Tlp_ProcessVm1Labels.RowCount = 2;
            Tlp_ProcessVm1Labels.Dock = DockStyle.Fill;
            Tlp_ProcessVm1Labels.Margin = new Padding(0);
            Tlp_ProcessVm1Labels.Name = "Tlp_ProcessVm1Labels";
            Tlp_ProcessVm1Labels.Controls.Add(Lbl_ProcessVm1Title, 0, 0);
            Tlp_ProcessVm1Labels.Controls.Add(Lbl_ProcessVm1State, 0, 1);
            Tlp_ProcessVm1Labels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_ProcessVm1Labels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Tlp_ProcessVm1Labels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Lbl_ProcessPressTitle.Dock = DockStyle.Fill;
            Lbl_ProcessPressTitle.Margin = new Padding(0);
            Lbl_ProcessPressTitle.BackColor = Color.Transparent;
            Lbl_ProcessPressTitle.AutoEllipsis = true;
            Lbl_ProcessPressState.Dock = DockStyle.Fill;
            Lbl_ProcessPressState.Margin = new Padding(0);
            Lbl_ProcessPressState.BackColor = Color.Transparent;
            Lbl_ProcessPressState.AutoEllipsis = true;
            Tlp_ProcessPressLabels.ColumnCount = 1;
            Tlp_ProcessPressLabels.RowCount = 2;
            Tlp_ProcessPressLabels.Dock = DockStyle.Fill;
            Tlp_ProcessPressLabels.Margin = new Padding(0);
            Tlp_ProcessPressLabels.Name = "Tlp_ProcessPressLabels";
            Tlp_ProcessPressLabels.Controls.Add(Lbl_ProcessPressTitle, 0, 0);
            Tlp_ProcessPressLabels.Controls.Add(Lbl_ProcessPressState, 0, 1);
            Tlp_ProcessPressLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_ProcessPressLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Tlp_ProcessPressLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Lbl_ProcessQualityTitle.Dock = DockStyle.Fill;
            Lbl_ProcessQualityTitle.Margin = new Padding(0);
            Lbl_ProcessQualityTitle.BackColor = Color.Transparent;
            Lbl_ProcessQualityTitle.AutoEllipsis = true;
            Lbl_ProcessQualityState.Dock = DockStyle.Fill;
            Lbl_ProcessQualityState.Margin = new Padding(0);
            Lbl_ProcessQualityState.BackColor = Color.Transparent;
            Lbl_ProcessQualityState.AutoEllipsis = true;
            Tlp_ProcessQualityLabels.ColumnCount = 1;
            Tlp_ProcessQualityLabels.RowCount = 2;
            Tlp_ProcessQualityLabels.Dock = DockStyle.Fill;
            Tlp_ProcessQualityLabels.Margin = new Padding(0);
            Tlp_ProcessQualityLabels.Name = "Tlp_ProcessQualityLabels";
            Tlp_ProcessQualityLabels.Controls.Add(Lbl_ProcessQualityTitle, 0, 0);
            Tlp_ProcessQualityLabels.Controls.Add(Lbl_ProcessQualityState, 0, 1);
            Tlp_ProcessQualityLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            Tlp_ProcessQualityLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Tlp_ProcessQualityLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            Pnl_PassRoute.Padding = new Padding(6);
            Lbl_PassTitle.Dock = DockStyle.Fill;
            Lbl_PassTitle.Margin = new Padding(0);
            Lbl_PassTitle.AutoEllipsis = true;
            Lbl_PassDestination.Dock = DockStyle.Fill;
            Lbl_PassDestination.Margin = new Padding(0);
            Lbl_PassDestination.AutoEllipsis = true;
            Tlp_PassRouteLabels.ColumnCount = 2;
            Tlp_PassRouteLabels.RowCount = 1;
            Tlp_PassRouteLabels.Dock = DockStyle.Fill;
            Tlp_PassRouteLabels.Margin = new Padding(0);
            Tlp_PassRouteLabels.Name = "Tlp_PassRouteLabels";
            Tlp_PassRouteLabels.Controls.Add(Lbl_PassTitle, 0, 0);
            Tlp_PassRouteLabels.Controls.Add(Lbl_PassDestination, 1, 0);
            Tlp_PassRouteLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            Tlp_PassRouteLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            Tlp_PassRouteLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Pnl_FailRoute.Padding = new Padding(6);
            Lbl_FailTitle.Dock = DockStyle.Fill;
            Lbl_FailTitle.Margin = new Padding(0);
            Lbl_FailTitle.AutoEllipsis = true;
            Lbl_FailDestination.Dock = DockStyle.Fill;
            Lbl_FailDestination.Margin = new Padding(0);
            Lbl_FailDestination.AutoEllipsis = true;
            Tlp_FailRouteLabels.ColumnCount = 2;
            Tlp_FailRouteLabels.RowCount = 1;
            Tlp_FailRouteLabels.Dock = DockStyle.Fill;
            Tlp_FailRouteLabels.Margin = new Padding(0);
            Tlp_FailRouteLabels.Name = "Tlp_FailRouteLabels";
            Tlp_FailRouteLabels.Controls.Add(Lbl_FailTitle, 0, 0);
            Tlp_FailRouteLabels.Controls.Add(Lbl_FailDestination, 1, 0);
            Tlp_FailRouteLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            Tlp_FailRouteLabels.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            Tlp_FailRouteLabels.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Lbl_ProcessVm1State.Name = "Lbl_ProcessVm1State";
            Lbl_ProcessVm1State.Text = "대기";
            Lbl_ProcessVm1State.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Lbl_ProcessVm1State.ForeColor = Color.FromArgb(71, 85, 105);
            Lbl_ActiveAlarmCount.Dock = DockStyle.Right;
            Lbl_ActiveAlarmCount.TextAlign = ContentAlignment.MiddleRight;
            Lbl_AlarmAreaTitle.Dock = DockStyle.Left;
            Lbl_AlarmAreaTitle.TextAlign = ContentAlignment.MiddleLeft;
            Dgv_ActiveAlarms.ColumnHeadersHeight = 32;
            Dgv_ActiveAlarms.ScrollBars = ScrollBars.Both;
            Dgv_RecentEvents.ColumnHeadersHeight = 32;
            Dgv_RecentEvents.ScrollBars = ScrollBars.Both;
            Col_EventTime.MinimumWidth = 145;
            Col_ProductCode.MinimumWidth = 180;
            Col_EventType.MinimumWidth = 85;
            Col_FromLocation.MinimumWidth = 95;
            Col_ToLocation.MinimumWidth = 95;
            Col_Remarks.MinimumWidth = 220;
            colOccurredAt.MinimumWidth = 145;
            colMachineCode.MinimumWidth = 80;
            colSeverity.MinimumWidth = 70;
            colAlarmCode.MinimumWidth = 100;
            colAlarmMessage.MinimumWidth = 200;
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 247, 250);
            ClientSize = new Size(1350, 826);
            Controls.Add(Tlp_Root);
            Font = new Font("Segoe UI", 10F);
            KeyPreview = true;
            Margin = new Padding(3, 2, 3, 2);
            MinimumSize = new Size(1366, 865);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MES HMI";
            WindowState = FormWindowState.Maximized;
            KeyDown += MainForm_KeyDown;
            Pnl_MachineArea.BackColor = Color.White;
            Pnl_MachineArea.BorderStyle = BorderStyle.None;
            Pnl_MachineArea.Padding = new Padding(10);
            tableLayoutPanel4.RowStyles[0].SizeType = SizeType.Absolute;
            tableLayoutPanel4.RowStyles[0].Height = 36F;
            Lbl_MachineAreaTitle.AutoSize = false;
            Lbl_MachineAreaTitle.Dock = DockStyle.Fill;
            Lbl_MachineAreaTitle.Margin = new Padding(3, 0, 3, 0);
            Lbl_MachineAreaTitle.TextAlign = ContentAlignment.MiddleLeft;
            Pnl_Vm1Card.Margin = new Padding(0, 5, 6, 0);
            Pnl_PressCard.Margin = new Padding(6, 5, 0, 0);
            Tlp_Root.ResumeLayout(false);
            Pnl_Header.ResumeLayout(false);
            Pnl_Header.PerformLayout();
            Pnl_Content.ResumeLayout(false);
            Tlp_ContentRoot.ResumeLayout(false);
            Tlp_SummaryCards.ResumeLayout(false);
            Pnl_ProductionCard.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            Pnl_ProductCard.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            Pnl_LotCard.ResumeLayout(false);
            Tlp_Lot.ResumeLayout(false);
            Tlp_Lot.PerformLayout();
            Pnl_LotProgressTrack.ResumeLayout(false);
            Pnl_WorkOrderCard.ResumeLayout(false);
            Tlp_WorkOrder.ResumeLayout(false);
            Tlp_WorkOrder.PerformLayout();
            Tlp_DashboardBody.ResumeLayout(false);
            Pnl_MachineArea.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            Pnl_Vm1Card.ResumeLayout(false);
            Tlp_Vm1.ResumeLayout(false);
            Tlp_Vm1.PerformLayout();
            Pnl_PressCard.ResumeLayout(false);
            Tlp_Press.ResumeLayout(false);
            Tlp_Press.PerformLayout();
            Pnl_ProcessFlowArea.ResumeLayout(false);
            Tlp_AlarmArea.ResumeLayout(false);
            Pnl_AlarmHeader.ResumeLayout(false);
            Pnl_AlarmHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Dgv_ActiveAlarms).EndInit();
            Pnl_AlarmArea.ResumeLayout(false);
            Tlp_ProcessFlow.ResumeLayout(false);
            Tlp_ProcessFlow.PerformLayout();
            Tlp_ProcessMain.ResumeLayout(false);
            Tlp_ProcessMain.PerformLayout();
            Pnl_ProcessVm1.ResumeLayout(false);
            Pnl_ProcessVm1.PerformLayout();
            Pnl_ProcessPress.ResumeLayout(false);
            Pnl_ProcessPress.PerformLayout();
            Pnl_ProcessQuality.ResumeLayout(false);
            Pnl_ProcessQuality.PerformLayout();
            Tlp_ProcessRoute.ResumeLayout(false);
            Pnl_PassRoute.ResumeLayout(false);
            Pnl_PassRoute.PerformLayout();
            Pnl_FailRoute.ResumeLayout(false);
            Pnl_FailRoute.PerformLayout();
            Pnl_EventArea.ResumeLayout(false);
            Tlp_EventArea.ResumeLayout(false);
            Tlp_EventArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Dgv_RecentEvents).EndInit();
            Pnl_ControlArea.ResumeLayout(false);
            Tlp_Control.ResumeLayout(false);
            Tlp_ProcessVm1Labels.ResumeLayout(false);
            Tlp_ProcessPressLabels.ResumeLayout(false);
            Tlp_ProcessQualityLabels.ResumeLayout(false);
            Tlp_PassRouteLabels.ResumeLayout(false);
            Tlp_FailRouteLabels.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel Tlp_ProcessVm1Labels;
        private TableLayoutPanel Tlp_ProcessPressLabels;
        private TableLayoutPanel Tlp_ProcessQualityLabels;
        private TableLayoutPanel Tlp_PassRouteLabels;
        private TableLayoutPanel Tlp_FailRouteLabels;
        private TableLayoutPanel Tlp_Root;
        private Panel Pnl_Header;
        private Label Lbl_CurrentTime;
        private Label Lbl_Title;
        private Panel Pnl_Content;
        private TableLayoutPanel Tlp_ContentRoot;
        private TableLayoutPanel Tlp_SummaryCards;
        private Panel Pnl_ProductionCard;
        private Panel Pnl_ProductCard;
        private Panel Pnl_LotCard;
        private Panel Pnl_WorkOrderCard;
        private TableLayoutPanel Tlp_WorkOrder;
        private Label Lbl_WorkOrderTitle;
        private Label Lbl_WorkOrderAchievementTitle;
        private Label Lbl_WorkOrderTargetTitle;
        private Label Lbl_WorkOrderStatusTitle;
        private Label Lbl_WorkOrderCodeTitle;
        private Label Lbl_WorkOrderAchievement;
        private Label Lbl_WorkOrderTarget;
        private Label Lbl_WorkOrderStatus;
        private Label Lbl_WorkOrderCode;
        private TableLayoutPanel Tlp_Lot;
        private Label Lbl_LotProgress;
        private Label Lbl_LotProduction;
        private Label Lbl_LotQuantityTitle;
        private Label Lbl_LotStatus;
        private Label Lbl_LotStatusTitle;
        private Label Lbl_LotCode;
        private Label Lbl_LotCodeTitle;
        private Label Lbl_LotTitle;
        private TableLayoutPanel tableLayoutPanel2;
        private Label Lbl_ProductLocation;
        private Label Lbl_ProductLocationTitle;
        private Label Lbl_ProductQuality;
        private Label Lbl_ProductStatus;
        private Label Lbl_ProductCode;
        private Label Lbl_ProductTitle;
        private Label Lbl_ProductCodeTitle;
        private Label Lbl_ProductStatusTitle;
        private Label Lbl_ProductQualityTitle;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel3;
        private Label Lbl_Yield;
        private Label Lbl_YieldTitle;
        private Label Lbl_Defect;
        private Label Lbl_DefectTitle;
        private Label Lbl_Good;
        private Label Lbl_GoodTitle;
        private Label Lbl_Produced;
        private Label Lbl_ProducedTitle;
        private Label Lbl_Target;
        private Label Lbl_ProductionTitle;
        private Label Lbl_TargetTitle;
        private TableLayoutPanel Tlp_DashboardBody;
        private Panel Pnl_MachineArea;
        private Panel Pnl_ProcessFlowArea;
        private Panel Pnl_AlarmArea;
        private Panel Pnl_EventArea;
        private Panel Pnl_ControlArea;
        private TableLayoutPanel Tlp_Control;
        private Krypton.Toolkit.KryptonButton Btn_Reset;
        private Krypton.Toolkit.KryptonButton Btn_Stop;
        private Krypton.Toolkit.KryptonButton Btn_Start;
        private TableLayoutPanel tableLayoutPanel4;
        private Label Lbl_MachineAreaTitle;
        private Panel Pnl_Vm1Card;
        private TableLayoutPanel Tlp_Vm1;
        private Label Lbl_Vm1Utilization;
        private Label Lbl_Vm1UtilizationTitle;
        private Label Lbl_Vm1Exited;
        private Label Lbl_Vm1ExitedTitle;
        private Label Lbl_Vm1Entered;
        private Label Lbl_Vm1EnteredTitle;
        private Label Lbl_Vm1State;
        private Label Lbl_Vm1Title;
        private Label Lbl_Vm1StateTitle;
        private Panel Pnl_PressCard;
        private TableLayoutPanel Tlp_Press;
        private Label Lbl_PressUtilization;
        private Label Lbl_PressUtilizationTitle;
        private Label Lbl_PressExited;
        private Label Lbl_PressExitedTitle;
        private Label Lbl_PressEntered;
        private Label Lbl_PressEnteredTitle;
        private Label Lbl_PressState;
        private Label Lbl_PressStateTitle;
        private Label Lbl_PressTitle;
        private TableLayoutPanel Tlp_ProcessFlow;
        private Label Lbl_ProcessFlowTitle;
        private TableLayoutPanel Tlp_ProcessMain;
        private Panel Pnl_ProcessVm1;
        private Label Lbl_ProcessVm1State;
        private Label Lbl_ProcessVm1Title;
        private Label Lbl_ArrowVm1Press;
        private Panel Pnl_ProcessPress;
        private Label Lbl_ArrowPressQuality;
        private Panel Pnl_ProcessQuality;
        private Label Lbl_ProcessPressState;
        private Label Lbl_ProcessPressTitle;
        private Label Lbl_ProcessQualityState;
        private Label Lbl_ProcessQualityTitle;
        private TableLayoutPanel Tlp_ProcessRoute;
        private Panel Pnl_PassRoute;
        private Label Lbl_PassTitle;
        private Panel Pnl_FailRoute;
        private Label Lbl_PassDestination;
        private Label Lbl_FailDestination;
        private Label Lbl_FailTitle;
        private TableLayoutPanel Tlp_AlarmArea;
        private Panel Pnl_AlarmHeader;
        private Label Lbl_ActiveAlarmCount;
        private Label Lbl_AlarmAreaTitle;
        private Krypton.Toolkit.KryptonDataGridView Dgv_ActiveAlarms;
        private TableLayoutPanel Tlp_EventArea;
        private Label Lbl_EventAreaTitle;
        private Krypton.Toolkit.KryptonDataGridView Dgv_RecentEvents;
        private DataGridViewTextBoxColumn colOccurredAt;
        private DataGridViewTextBoxColumn colMachineCode;
        private DataGridViewTextBoxColumn colSeverity;
        private DataGridViewTextBoxColumn colAlarmCode;
        private DataGridViewTextBoxColumn colAlarmMessage;
        private DataGridViewTextBoxColumn Col_EventTime;
        private DataGridViewTextBoxColumn Col_ProductCode;
        private DataGridViewTextBoxColumn Col_EventType;
        private DataGridViewTextBoxColumn Col_FromLocation;
        private DataGridViewTextBoxColumn Col_ToLocation;
        private DataGridViewTextBoxColumn Col_Remarks;
        private Panel Pnl_LotProgressTrack;
        private Panel Pnl_LotProgressFill;
    }
}
