namespace Yahoo.Yui.Compressor.Build
{
    using System;

    public interface ILog
    {
        void LogMessage(string message);

        void LogBoolean(string name, bool value);

        void LogError(string message, params object[] messageArgs);

        void LogErrorFromException(Exception exception);

        void LogErrorFromException(Exception exception, bool showStackTrace);
    }
}
