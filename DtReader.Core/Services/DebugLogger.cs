using System;
using System.Diagnostics;

namespace DtReader.Core.Services;

public class DebugLogger : IMessageLogger
{
    public void LogInfo(string message)
    {
        Debug.WriteLine(message);
    }

    public void LogError(string message, Exception? ex = null)
    {
        Debug.WriteLine(message);
        Debug.WriteLine(ex);
    }
}