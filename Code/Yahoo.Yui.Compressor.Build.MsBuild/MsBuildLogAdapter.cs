namespace Yahoo.Yui.Compressor.Build.MsBuild
{
    using System;

    using Microsoft.Build.Utilities;

    public class MsBuildLogAdapter : ILog
    {
        private readonly TaskLoggingHelper logger;

        public void LogMessage(string message)
        {
            logger.LogMessage(message);
        }

        public void LogBoolean(string name, bool value)
        {
            LogMessage(name + ": " + (value ? "Yes" : "No"));
        }

        public void LogError(string message, params object[] messageArgs)
        {
            logger.LogError(message, messageArgs);
        }

        public void LogErrorFromException(Exception exception)
        {
            this.logger.LogErrorFromException(exception);
        }

        public void LogErrorFromException(Exception exception, bool showStackTrace)
        {
            this.logger.LogErrorFromException(exception, showStackTrace);
        }

        public MsBuildLogAdapter(TaskLoggingHelper logger)
        {
            this.logger = logger;
        }
    }
}