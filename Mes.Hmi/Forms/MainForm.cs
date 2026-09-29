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

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 현재 시간 표시 작업 시작
            _clockCts = new CancellationTokenSource();
            _ = RunClockAsync(_clockCts.Token);
        }

        // Invoke()를 쓰지 않아도 되는 이유는 RunClockAsync()를 UI Thread에서 시작했고 ConfigureAwait(false)도 사용하지 않았기 때문에 await 이후에도 UI 컨텍스트로 돌아온다.
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

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockCts?.Cancel();
            _clockCts?.Dispose();
            _clockCts = null;

            base.OnFormClosed(e);
        }
    }
}
