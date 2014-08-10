using System;
using System.Globalization;

using NAnt.Core.Attributes;

using EcmaScript.NET;

namespace Yahoo.Yui.Compressor.Build.Nant
{
    [TaskName("javaScriptCompressor")]
    public class JavaScriptCompressorTask : CompressorTask
    {
        private readonly IJavaScriptCompressor compressor;

        private CultureInfo threadCulture;

        [TaskAttribute("obfuscateJavaScript")]
        public bool ObfuscateJavaScript { get; set; }

        [TaskAttribute("preserveAllSemicolons")]
        public bool PreserveAllSemicolons { get; set; }

        [TaskAttribute("disableOptimizations")]
        public bool DisableOptimizations { get; set; }

        [TaskAttribute("threadCulture")]
        public string ThreadCulture { get; set; }

        [TaskAttribute("isEvalIgnored")]
        public bool IsEvalIgnored { get; set; }

        public JavaScriptCompressorTask() : this(new JavaScriptCompressor())
        {
        }

        public JavaScriptCompressorTask(IJavaScriptCompressor compressor) : base(compressor)
        {
            this.compressor = compressor;
            ObfuscateJavaScript = true;
            TaskEngine.ParseAdditionalTaskParameters = this.ParseAdditionalTaskParameters;
            TaskEngine.LogAdditionalTaskParameters = this.LogAdditionalTaskParameters;
            TaskEngine.SetCompressorParameters = this.SetCompressorParameters;
        }

        protected override void ExecuteTask()
        {
            try
            {
                base.ExecuteTask();
            }
            catch (EcmaScriptException ecmaScriptException)
            {
                TaskEngine.Log.LogError("An error occurred in parsing the Javascript file.");
                if (ecmaScriptException.LineNumber == -1)
                {
                    TaskEngine.Log.LogError("[ERROR] {0} ********", ecmaScriptException.Message);
                }
                else
                {
                    TaskEngine.Log.LogError(
                        "[ERROR] {0} ******** Line: {2}. LineOffset: {3}. LineSource: \"{4}\"",
                        ecmaScriptException.Message,
                        string.IsNullOrEmpty(ecmaScriptException.SourceName)
                            ? string.Empty
                            : "Source: {1}. " + ecmaScriptException.SourceName,
                        ecmaScriptException.LineNumber,
                        ecmaScriptException.ColumnNumber,
                        ecmaScriptException.LineSource);
                }
            }
        }

        private void ParseAdditionalTaskParameters()
        {
            ParseThreadCulture();
        }

        private void SetCompressorParameters()
        {
            compressor.DisableOptimizations = DisableOptimizations;
            compressor.IgnoreEval = IsEvalIgnored;
            compressor.ObfuscateJavascript = ObfuscateJavaScript;
            compressor.PreserveAllSemicolons = PreserveAllSemicolons;
            compressor.ThreadCulture = threadCulture;
            compressor.Encoding = TaskEngine.Encoding;
            compressor.ErrorReporter = new CustomErrorReporter(TaskEngine.LogType);
        }

        private void LogAdditionalTaskParameters()
        {
            TaskEngine.Log.LogBoolean("Obfuscate Javascript", ObfuscateJavaScript);
            TaskEngine.Log.LogBoolean("Preserve semi colons", PreserveAllSemicolons);
            TaskEngine.Log.LogBoolean("Disable optimizations", DisableOptimizations);
            TaskEngine.Log.LogBoolean("Is Eval Ignored", IsEvalIgnored);
            TaskEngine.Log.LogMessage(
                "Line break position: "
                + (LineBreakPosition <= -1 ? "None" : LineBreakPosition.ToString(CultureInfo.InvariantCulture)));
            TaskEngine.Log.LogMessage("Thread Culture: " + threadCulture.DisplayName);
        }

        private void ParseThreadCulture()
        {
            if (string.IsNullOrEmpty(ThreadCulture))
            {
                threadCulture = CultureInfo.InvariantCulture;
                return;
            }

            try
            {
                switch (ThreadCulture.ToLowerInvariant())
                {
                    case "iv":
                    case "ivl":
                    case "invariantculture":
                    case "invariant culture":
                    case "invariant language":
                    case "invariant language (invariant country)":
                        {
                            threadCulture = CultureInfo.InvariantCulture;
                            break;
                        }
                    default:
                        {
                            threadCulture = CultureInfo.CreateSpecificCulture(ThreadCulture);
                            break;
                        }
                }
            }
            catch
            {
                throw new ArgumentException("Thread Culture: " + ThreadCulture + " is invalid.", "ThreadCulture");
            }
        }
    }
}