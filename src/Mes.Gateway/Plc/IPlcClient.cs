namespace Mes.Gateway.Plc;

public interface IPlcClient
{
    bool IsConnected { get; }


    Task<bool> ConnectAsync(
        CancellationToken cancellationToken = default
    );


    Task<int?> ReadDeviceAsync(
        string device,
        CancellationToken cancellationToken = default
    );


    Task<bool> WriteDeviceAsync(
        string device,
        int value,
        CancellationToken cancellationToken = default
    );


    Task DisconnectAsync(
        CancellationToken cancellationToken = default
    );

    Task<bool> PulseDeviceAsync(
    string device,
    int pulseMilliseconds = 300,
    CancellationToken cancellationToken = default
);
}