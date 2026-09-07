using DtReader.Core.Models;

namespace DtReader.Tests;

/// <summary>Records log calls so tests can assert what the code under test logged.</summary>
public sealed class RecordingLogger : DtReader.Core.Services.IMessageLogger
{
    public List<string> Infos { get; } = [];
    public List<(string Message, Exception? Exception)> Errors { get; } = [];

    public void LogInfo(string message) => Infos.Add(message);

    public void LogError(string message, Exception? ex = null) => Errors.Add((message, ex));
}
