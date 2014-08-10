namespace Yahoo.Yui.Compressor
{
    public interface ICompressor
    {
        CompressionType CompressionType { get; set; }
        int LineBreakPosition { get; set; }
        string ContentType { get; }

        string Compress(string source);
    }
}