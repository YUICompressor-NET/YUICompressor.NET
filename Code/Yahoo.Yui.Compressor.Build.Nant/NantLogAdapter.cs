using System;

using NAnt.Core;

namespace Yahoo.Yui.Compressor.Build.Nant
{
    public class NantLogAdapter : ILog
    {
        public Project Project { get; set; }

        public void LogMessage(string message)
        {
            this.Project.Log(Level.Info, message);
        }

        public void LogBoolean(string name, bool value)
        {
            LogMessage(name + ": " + (value ? "Yes" : "No"));
        }

        public void LogError(string message, params object[] messageArgs)
        {
            this.Project.Log(Level.Error, message, messageArgs);
        }

        public void LogErrorFromException(Exception exception)
        {
            this.Project.Log(Level.Error, exception.Message);
        }

        public void LogErrorFromException(Exception exception, bool showStackTrace)
        {
            this.Project.Log(Level.Error, exception.Message + (showStackTrace ? Environment.NewLine + exception.StackTrace : string.Empty));
        }
    }
}
