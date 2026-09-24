using System.Collections.Concurrent;
using Mes.Gateway.Models;

namespace Mes.Gateway.Services;

public class MachineStatusService
{
    private readonly ConcurrentDictionary<string, MachineStatusMessage>
        _latestStatus = new();

    public void Update(MachineStatusMessage status)
    {
        _latestStatus[status.Machine] = status;

        Console.WriteLine(
            $"[MACHINE SERVICE] {status.Machine} 최신 상태 저장"
        );
    }

    public MachineStatusMessage? Get(string machine)
    {
        _latestStatus.TryGetValue(
            machine,
            out var status
        );

        return status;
    }

    public IReadOnlyCollection<MachineStatusMessage> GetAll()
    {
        return _latestStatus.Values.ToList();
    }
}