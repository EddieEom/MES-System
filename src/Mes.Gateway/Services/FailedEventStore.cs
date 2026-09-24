using System.Text;
using System.Text.Json;

namespace Mes.Gateway.Services;

public class FailedEventStore
{
    private readonly SemaphoreSlim _lock =
        new(
            1,
            1
        );


    private readonly string _directory;


    public FailedEventStore()
    {
        _directory =
            Path.Combine(
                AppContext.BaseDirectory,
                "failed-events"
            );

        Directory.CreateDirectory(
            _directory
        );
    }


    public async Task SaveAsync(
        string eventType,
        object payload,
        string reason)
    {
        var record =
            new
            {
                failedAt =
                    DateTimeOffset.UtcNow,

                eventType,

                reason,

                payload
            };


        var json =
            JsonSerializer.Serialize(
                record
            )
            + Environment.NewLine;


        var filePath =
            Path.Combine(
                _directory,
                $"failed-{DateTime.UtcNow:yyyyMMdd}.jsonl"
            );


        await _lock.WaitAsync();

        try
        {
            await File.AppendAllTextAsync(
                filePath,
                json,
                Encoding.UTF8
            );
        }
        finally
        {
            _lock.Release();
        }


        Console.WriteLine(
            $"[FAILED EVENT] 저장 완료 - {eventType}"
        );
    }
}