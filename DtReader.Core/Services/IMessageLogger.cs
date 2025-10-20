using System;

namespace DtReader.Core.Services;

public interface IMessageLogger
{
    public void LogInfo(string message);
    public void LogError(string message, Exception? ex = null);
}