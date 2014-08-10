using System.Globalization;
using System.Text;
using EcmaScript.NET;

namespace Yahoo.Yui.Compressor.Web.Optimization
{
    public class JavaScriptCompressorConfig : CompressorConfig
    {
        public JavaScriptCompressorConfig()
        {
            Encoding = Encoding.Default;
            DisableOptimizations = false;
            ObfuscateJavascript = true;
            PreserveAllSemicolons = false;
            IgnoreEval = false;
            ThreadCulture = CultureInfo.InvariantCulture;
        }

        public Encoding Encoding { get; set; }
        public ErrorReporter ErrorReporter { get; set; }
        public bool DisableOptimizations { get; set; }
        public bool ObfuscateJavascript { get; set; }
        public bool PreserveAllSemicolons { get; set; }
        public bool IgnoreEval { get; set; }
        public CultureInfo ThreadCulture { get; set; }
        public LoggingType LoggingType { get; set; }
    }
}