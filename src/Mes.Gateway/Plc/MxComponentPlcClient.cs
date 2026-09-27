using ActUtlType64Lib;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace Mes.Gateway.Plc;

public sealed class MxComponentPlcClient :
    IPlcClient
{
    private readonly int _logicalStationNumber;

    private BlockingCollection<Action>? _commandQueue;

    private Thread? _plcThread;

    private ActUtlType64Class? _plc;

    private TaskCompletionSource<bool>?
        _connectionSource;

    private volatile bool _isConnected;


    public bool IsConnected =>
        _isConnected;


    public MxComponentPlcClient(
        int logicalStationNumber)
    {
        _logicalStationNumber =
            logicalStationNumber;
    }


    // ======================================
    // PLC Connect
    // ======================================

    public async Task<bool> ConnectAsync(
        CancellationToken cancellationToken = default)
    {
        if (_isConnected)
        {
            return true;
        }


        if (_plcThread != null)
        {
            throw new InvalidOperationException(
                "PLC Thread가 이미 생성되어 있습니다."
            );
        }


        _commandQueue =
            new BlockingCollection<Action>();


        _connectionSource =
            new TaskCompletionSource<bool>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously
            );


        _plcThread =
            new Thread(
                PlcThreadMain
            )
            {
                IsBackground = true,
                Name = "MX Component STA Thread"
            };


        _plcThread.SetApartmentState(
            ApartmentState.STA
        );


        _plcThread.Start();


        return await _connectionSource
            .Task
            .WaitAsync(
                cancellationToken
            );
    }


    // ======================================
    // MX Component 전용 STA Thread
    // ======================================

    private void PlcThreadMain()
    {
        try
        {
            _plc =
                new ActUtlType64Class();


            _plc.ActLogicalStationNumber =
                _logicalStationNumber;


            int result =
                _plc.Open();


            Console.WriteLine(
                $"[PLC] Open 결과: 0x{result:X8}"
            );


            if (result != 0)
            {
                Console.WriteLine(
                    "[PLC] MX Component 연결 실패"
                );


                _connectionSource?
                    .TrySetResult(false);


                return;
            }


            _isConnected = true;


            Console.WriteLine(
                $"[PLC] MX Component 연결 성공 - Logical Station {_logicalStationNumber}"
            );


            _connectionSource?
                .TrySetResult(true);


            if (_commandQueue == null)
            {
                return;
            }


            foreach (
                var command
                in _commandQueue
                    .GetConsumingEnumerable())
            {
                try
                {
                    command();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"[PLC] 명령 실행 오류: {ex.Message}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[PLC] 연결 예외: {ex.Message}"
            );


            _connectionSource?
                .TrySetException(ex);
        }
        finally
        {
            ClosePlcOnStaThread();
        }
    }


    // ======================================
    // Device Read
    // ======================================

    public async Task<int?> ReadDeviceAsync(
        string device,
        CancellationToken cancellationToken = default)
    {
        var result =
            await InvokeAsync(
                plc =>
                {
                    int resultCode =
                        plc.GetDevice(
                            device,
                            out int value
                        );


                    return (
                        ResultCode: resultCode,
                        Value: value
                    );
                },

                cancellationToken
            );


        Console.WriteLine(
            $"[PLC] GetDevice({device}) 결과: 0x{result.ResultCode:X8}"
        );


        if (result.ResultCode != 0)
        {
            return null;
        }


        return result.Value;
    }


    // ======================================
    // Device Write
    // ======================================

    public async Task<bool> WriteDeviceAsync(
        string device,
        int value,
        CancellationToken cancellationToken = default)
    {
        int result =
            await InvokeAsync(
                plc =>
                    plc.SetDevice(
                        device,
                        value
                    ),

                cancellationToken
            );


        Console.WriteLine(
            $"[PLC] SetDevice({device}, {value}) 결과: 0x{result:X8}"
        );


        return result == 0;
    }

    // ======================================
    // Device Pulse
    //
    // M100 = 1
    // 300ms 유지
    // M100 = 0
    // ======================================

    public async Task<bool> PulseDeviceAsync(
        string device,
        int pulseMilliseconds = 300,
        CancellationToken cancellationToken = default)
    {
        if (pulseMilliseconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pulseMilliseconds),
                "Pulse 시간은 1ms 이상이어야 합니다."
            );
        }


        return await InvokeAsync(
            plc =>
            {
                Console.WriteLine(
                    $"[PLC] Pulse 시작 - {device}"
                );


                // ==============================
                // ON
                // ==============================

                int onResult =
                    plc.SetDevice(
                        device,
                        1
                    );


                Console.WriteLine(
                    $"[PLC] SetDevice({device}, 1) 결과: 0x{onResult:X8}"
                );


                if (onResult != 0)
                {
                    return false;
                }


                int readOnResult =
                    plc.GetDevice(
                        device,
                        out int onValue
                    );


                Console.WriteLine(
                    $"[PLC] {device} ON 확인: {onValue}"
                );


                // ==============================
                // Pulse 유지 후 반드시 OFF
                // ==============================

                int offResult = -1;


                try
                {
                    Thread.Sleep(
                        pulseMilliseconds
                    );
                }
                finally
                {
                    offResult =
                        plc.SetDevice(
                            device,
                            0
                        );


                    Console.WriteLine(
                        $"[PLC] SetDevice({device}, 0) 결과: 0x{offResult:X8}"
                    );
                }


                if (offResult != 0)
                {
                    return false;
                }


                // ==============================
                // OFF 확인
                // ==============================

                int readOffResult =
                    plc.GetDevice(
                        device,
                        out int offValue
                    );


                Console.WriteLine(
                    $"[PLC] {device} OFF 확인: {offValue}"
                );


                bool valid =
                    readOnResult == 0
                    && onValue == 1
                    && readOffResult == 0
                    && offValue == 0;


                if (valid)
                {
                    Console.WriteLine(
                        $"[PLC] Pulse 완료 - {device} / {pulseMilliseconds}ms"
                    );
                }
                else
                {
                    Console.WriteLine(
                        $"[PLC] Pulse 검증 실패 - {device}"
                    );
                }


                return valid;
            },

            cancellationToken
        );
    }


    // ======================================
    // COM 명령은 무조건 PLC STA Thread에서 실행
    // ======================================

    private Task<T> InvokeAsync<T>(
        Func<ActUtlType64Class, T> operation,
        CancellationToken cancellationToken)
    {
        if (!_isConnected ||
            _plc == null ||
            _commandQueue == null)
        {
            throw new InvalidOperationException(
                "PLC가 연결되어 있지 않습니다."
            );
        }


        if (cancellationToken
            .IsCancellationRequested)
        {
            return Task.FromCanceled<T>(
                cancellationToken
            );
        }


        var source =
            new TaskCompletionSource<T>(
                TaskCreationOptions
                    .RunContinuationsAsynchronously
            );


        try
        {
            _commandQueue.Add(
                () =>
                {
                    if (source.Task.IsCompleted)
                    {
                        return;
                    }


                    try
                    {
                        T result =
                            operation(
                                _plc
                            );


                        source.TrySetResult(
                            result
                        );
                    }
                    catch (Exception ex)
                    {
                        source.TrySetException(
                            ex
                        );
                    }
                }
            );
        }
        catch (Exception ex)
        {
            source.TrySetException(
                ex
            );
        }


        return source.Task;
    }


    // ======================================
    // PLC Disconnect
    // ======================================

    public async Task DisconnectAsync(
        CancellationToken cancellationToken = default)
    {
        if (_commandQueue != null &&
            !_commandQueue.IsAddingCompleted)
        {
            _commandQueue
                .CompleteAdding();
        }


        if (_plcThread != null &&
            _plcThread.IsAlive)
        {
            await Task.Run(
                () =>
                {
                    _plcThread.Join();
                },
                cancellationToken
            );
        }


        _plcThread = null;

        _commandQueue?.Dispose();

        _commandQueue = null;
    }


    // ======================================
    // Close
    //
    // 반드시 PLC STA Thread에서 호출
    // ======================================

    private void ClosePlcOnStaThread()
    {
        if (_plc == null)
        {
            _isConnected = false;

            return;
        }


        try
        {
            if (_isConnected)
            {
                int result =
                    _plc.Close();


                Console.WriteLine(
                    $"[PLC] Close 결과: 0x{result:X8}"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"[PLC] Close 오류: {ex.Message}"
            );
        }
        finally
        {
            _isConnected = false;


            try
            {
                if (Marshal.IsComObject(
                    _plc))
                {
                    Marshal
                        .FinalReleaseComObject(
                            _plc
                        );
                }
            }
            catch
            {
            }


            _plc = null;
        }
    }
}