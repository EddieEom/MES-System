using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Krypton.Toolkit;

namespace Mes.Hmi.Forms
{
    public partial class MainForm
    {
        // Visual styling only: no Name, Text, event, or binding changes.
        private void ApplyDashboardTheme()
        {
            SuspendLayout();
            try
            {
                components ??= new Container();
                var fonts = new DashboardFonts();
                components.Add(fonts);
                Color ink = Color.FromArgb(30, 41, 59);
                Color muted = Color.FromArgb(90, 106, 128);
                Color blue = Color.FromArgb(37, 99, 235);

                BackColor = Color.FromArgb(239, 243, 248);
                Pnl_Content.BackColor = BackColor;
                Pnl_Header.BackColor = Color.FromArgb(18, 35, 58);
                Lbl_Title.Font = fonts.Header;
                Lbl_CurrentTime.Font = fonts.Code;
                Lbl_CurrentTime.ForeColor = Color.FromArgb(219, 234, 254);

                foreach (Label label in DashboardLabels(Tlp_ContentRoot))
                {
                    label.Font = fonts.Body;
                    label.ForeColor = muted;
                    label.BackColor = Color.Transparent;
                }

                Label[] headings = {
                    Lbl_WorkOrderTitle, Lbl_LotTitle, Lbl_ProductTitle,
                    Lbl_ProductionTitle, Lbl_MachineAreaTitle,
                    Lbl_ProcessFlowTitle, Lbl_AlarmAreaTitle, Lbl_EventAreaTitle
                };
                foreach (Label label in headings)
                {
                    label.Font = fonts.Heading;
                    label.ForeColor = ink;
                }
                Label[] values = {
                    Lbl_WorkOrderCode, Lbl_WorkOrderStatus, Lbl_LotCode,
                    Lbl_LotStatus, Lbl_ProductCode, Lbl_ProductStatus,
                    Lbl_ProductQuality, Lbl_ProductLocation,
                    Lbl_Vm1State, Lbl_PressState
                };
                foreach (Label label in values)
                {
                    label.Font = fonts.Value;
                    label.ForeColor = ink;
                }
                Label[] numbers = {
                    Lbl_WorkOrderTarget, Lbl_WorkOrderAchievement,
                    Lbl_LotProduction, Lbl_LotProgress,
                    Lbl_Target, Lbl_Produced, Lbl_Good, Lbl_Defect, Lbl_Yield,
                    Lbl_Vm1Entered, Lbl_Vm1Exited, Lbl_Vm1Utilization,
                    Lbl_PressEntered, Lbl_PressExited, Lbl_PressUtilization
                };
                foreach (Label label in numbers)
                {
                    label.Font = fonts.Number;
                    label.ForeColor = ink;
                }
                Lbl_Produced.ForeColor = blue;
                Lbl_LotProgress.ForeColor = blue;
                Lbl_WorkOrderAchievement.ForeColor = blue;
                Lbl_Good.ForeColor = Lbl_GoodTitle.ForeColor = Color.FromArgb(21, 128, 61);
                Lbl_Defect.ForeColor = Lbl_DefectTitle.ForeColor = Color.FromArgb(185, 28, 28);
                Lbl_Yield.ForeColor = Lbl_YieldTitle.ForeColor = blue;
                Lbl_ActiveAlarmCount.Font = fonts.Value;
                Lbl_ActiveAlarmCount.ForeColor = Color.FromArgb(185, 28, 28);

                Panel[] surfaces = {
                    Pnl_WorkOrderCard, Pnl_LotCard, Pnl_ProductCard, Pnl_ProductionCard,
                    Pnl_MachineArea, Pnl_AlarmArea, Pnl_ProcessFlowArea, Pnl_EventArea
                };
                foreach (Panel panel in surfaces)
                {
                    panel.BackColor = Color.White;
                    panel.BorderStyle = BorderStyle.None;
                    panel.Padding = new Padding(14);
                }
                AccentCard(Pnl_WorkOrderCard, blue);
                AccentCard(Pnl_LotCard, Color.FromArgb(8, 145, 178));
                AccentCard(Pnl_ProductCard, Color.FromArgb(99, 102, 241));
                AccentCard(Pnl_ProductionCard, Color.FromArgb(5, 150, 105));

                foreach (Panel panel in new[] { Pnl_Vm1Card, Pnl_PressCard })
                {
                    panel.BackColor = Color.FromArgb(247, 249, 252);
                    panel.BorderStyle = BorderStyle.None;
                    panel.Padding = new Padding(12);
                }
                foreach (Label label in new[] {
                    Lbl_Vm1Title, Lbl_PressTitle,
                    Lbl_ProcessVm1Title, Lbl_ProcessPressTitle, Lbl_ProcessQualityTitle
                })
                {
                    label.Font = fonts.Heading;
                    label.ForeColor = ink;
                }
                foreach (Label label in new[] {
                    Lbl_ProcessVm1State, Lbl_ProcessPressState, Lbl_ProcessQualityState,
                    Lbl_PassTitle, Lbl_PassDestination, Lbl_FailTitle, Lbl_FailDestination
                }) label.Font = fonts.Value;
                Lbl_PassTitle.ForeColor = Color.FromArgb(21, 128, 61);
                Lbl_FailTitle.ForeColor = Color.FromArgb(185, 28, 28);
                foreach (Label label in new[] { Lbl_ArrowVm1Press, Lbl_ArrowPressQuality })
                {
                    label.Font = fonts.Arrow;
                    label.ForeColor = Color.FromArgb(100, 116, 139);
                }
                Pnl_LotProgressTrack.BackColor = Color.FromArgb(226, 232, 240);
                Pnl_LotProgressFill.BackColor = blue;

                StyleDashboardGrid(Dgv_ActiveAlarms, fonts);
                StyleDashboardGrid(Dgv_RecentEvents, fonts);
                StyleDashboardButton(Btn_Start, Color.FromArgb(22, 139, 77), fonts);
                StyleDashboardButton(Btn_Stop, Color.FromArgb(210, 49, 49), fonts);
                StyleDashboardButton(Btn_Reset, Color.FromArgb(180, 104, 8), fonts);
            }
            finally { ResumeLayout(true); }
        }

        private static System.Collections.Generic.IEnumerable<Label> DashboardLabels(Control root)
        {
            foreach (Control child in root.Controls)
            {
                if (child is Label label) yield return label;
                foreach (Label descendant in DashboardLabels(child)) yield return descendant;
            }
        }

        private static void AccentCard(Panel panel, Color color)
        {
            if (panel is DashboardCardPanel card) card.AccentColor = color;
        }

        private static void StyleDashboardButton(KryptonButton button, Color color, DashboardFonts fonts)
        {
            button.StateCommon.Back.Color1 = color;
            button.StateCommon.Back.Color2 = color;
            button.StateCommon.Border.Color1 = color;
            button.StateCommon.Border.Color2 = color;
            button.StateCommon.Border.DrawBorders = PaletteDrawBorders.All;
            button.StateCommon.Border.Rounding = 8F;
            button.StateCommon.Border.Width = 1;
            button.StateCommon.Content.ShortText.Color1 = Color.White;
            button.StateCommon.Content.ShortText.Font = fonts.Button;
            button.StateCommon.Content.ShortText.TextH = PaletteRelativeAlign.Center;
            button.StateCommon.Content.ShortText.TextV = PaletteRelativeAlign.Center;
            button.StateTracking.Back.Color1 = ControlPaint.Light(color, 0.12F);
            button.StateTracking.Back.Color2 = button.StateTracking.Back.Color1;
            button.StatePressed.Back.Color1 = ControlPaint.Dark(color, 0.12F);
            button.StatePressed.Back.Color2 = button.StatePressed.Back.Color1;
            button.StateDisabled.Back.Color1 = Color.FromArgb(203, 213, 225);
            button.StateDisabled.Back.Color2 = Color.FromArgb(203, 213, 225);
            button.StateDisabled.Content.ShortText.Color1 = Color.FromArgb(71, 85, 105);
        }

        private static void StyleDashboardGrid(KryptonDataGridView grid, DashboardFonts fonts)
        {
            Color ink = Color.FromArgb(30, 41, 59);
            Color lines = Color.FromArgb(230, 235, 242);
            grid.StateCommon.Background.Color1 = Color.White;
            grid.StateCommon.Background.Color2 = Color.White;
            grid.StateCommon.DataCell.Back.Color1 = Color.White;
            grid.StateCommon.DataCell.Back.Color2 = Color.White;
            grid.StateCommon.DataCell.Content.Color1 = ink;
            grid.StateCommon.DataCell.Content.Font = fonts.Body;
            grid.StateCommon.DataCell.Border.Color1 = lines;
            grid.StateCommon.DataCell.Border.Color2 = lines;
            grid.StateCommon.DataCell.Border.DrawBorders = PaletteDrawBorders.Bottom;
            grid.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(240, 244, 249);
            grid.StateCommon.HeaderColumn.Back.Color2 = Color.FromArgb(240, 244, 249);
            grid.StateCommon.HeaderColumn.Content.Color1 = Color.FromArgb(65, 81, 104);
            grid.StateCommon.HeaderColumn.Content.Font = fonts.GridHeader;
            grid.StateCommon.HeaderColumn.Border.Color1 = lines;
            grid.StateCommon.HeaderColumn.Border.Color2 = lines;
            grid.StateSelected.DataCell.Back.Color1 = Color.FromArgb(225, 237, 255);
            grid.StateSelected.DataCell.Back.Color2 = Color.FromArgb(225, 237, 255);
            grid.StateSelected.DataCell.Content.Color1 = ink;
            grid.DefaultCellStyle.Padding = new Padding(7, 0, 7, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 237, 255);
            grid.DefaultCellStyle.SelectionForeColor = ink;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(7, 0, 7, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 253);
            grid.ColumnHeadersHeight = Math.Max(34, (int)(34 * grid.DeviceDpi / 96F));
            grid.RowTemplate.Height = Math.Max(32, (int)(32 * grid.DeviceDpi / 96F));
            grid.BorderStyle = BorderStyle.None;
        }
    }

    // Remains a Panel: existing Panel-based status methods keep working.
    public class DashboardCardPanel : Panel
    {
        private Color _accentColor = Color.Empty;
        [DefaultValue(typeof(Color), "Empty")]
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public DashboardCardPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ClientSize.Width < 3 || ClientSize.Height < 3) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float radius = 10F * DeviceDpi / 96F;
            using var outline = RoundedPath(new RectangleF(0.5F, 0.5F,
                ClientSize.Width - 1F, ClientSize.Height - 1F), radius);
            Color border = BackColor.R < 240 && BackColor.G > BackColor.R
                ? Color.FromArgb(134, 196, 157) : Color.FromArgb(213, 222, 234);
            using var pen = new Pen(border);
            e.Graphics.DrawPath(pen, outline);
            if (!_accentColor.IsEmpty)
            {
                using var accent = new Pen(_accentColor, 3F * DeviceDpi / 96F);
                e.Graphics.DrawLine(accent, radius, 2F, ClientSize.Width - radius, 2F);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (ClientSize.Width < 2 || ClientSize.Height < 2) return;
            using var path = RoundedPath(new RectangleF(0, 0,
                ClientSize.Width, ClientSize.Height), 10F * DeviceDpi / 96F);
            Region? old = Region;
            Region = new Region(path);
            old?.Dispose();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            Invalidate(true);
        }

        private static GraphicsPath RoundedPath(RectangleF rect, float radius)
        {
            float diameter = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            var path = new GraphicsPath();
            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    internal sealed class DashboardFonts : Component
    {
        public Font Header { get; } = new Font("맑은 고딕", 17F, FontStyle.Bold);
        public Font Heading { get; } = new Font("맑은 고딕", 11F, FontStyle.Bold);
        public Font Body { get; } = new Font("맑은 고딕", 9F);
        public Font Code { get; } = new Font("Segoe UI", 10F, FontStyle.Bold);
        public Font Value { get; } = new Font("맑은 고딕", 10F, FontStyle.Bold);
        public Font Number { get; } = new Font("Segoe UI", 14F, FontStyle.Bold);
        public Font Button { get; } = new Font("Segoe UI", 13F, FontStyle.Bold);
        public Font Arrow { get; } = new Font("Segoe UI", 20F, FontStyle.Bold);
        public Font GridHeader { get; } = new Font("맑은 고딕", 9F, FontStyle.Bold);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Header.Dispose(); Heading.Dispose(); Body.Dispose(); Code.Dispose();
                Value.Dispose(); Number.Dispose(); Button.Dispose(); Arrow.Dispose(); GridHeader.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
