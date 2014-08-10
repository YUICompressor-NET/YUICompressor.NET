namespace Yahoo.Yui.Compressor
{
    public interface ICompressorTask
    {
        string CssCompressionType { get; set; }
        string DeleteCssFiles { get; set; }
        string CssOutputFile { get; set; }
        string ObfuscateJavaScript { get; set; }
        string PreserveAllSemicolons { get; set; }
        string DisableOptimizations { get; set; }
        string LineBreakPosition { get; set; }
        string EncodingType { get; set; }
        string DeleteJavaScriptFiles { get; set; }
        string JavaScriptOutputFile { get; set; }
        string LoggingType { get; set; }
        string ThreadCulture { get; set; }
        string IsEvalIgnored { get; set; }
        string DoNotErrorWhenNoFilesAreProvided { get; set; }
        string PreserveCssComments { get; set; }
    }
}