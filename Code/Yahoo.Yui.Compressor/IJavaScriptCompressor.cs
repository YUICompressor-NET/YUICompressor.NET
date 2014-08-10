namespace Yahoo.Yui.Compressor
{
    using System.Globalization;
    using System.Text;

    using EcmaScript.NET;

    public interface IJavaScriptCompressor : ICompressor
    {
        Encoding Encoding { get; set; }
        ErrorReporter ErrorReporter { get; set; }
        bool DisableOptimizations { get; set; }
        bool IgnoreEval { get; set; }
        bool ObfuscateJavascript { get; set; }
        bool PreserveAllSemicolons { get; set; }
        CultureInfo ThreadCulture { get; set; }
    }
}