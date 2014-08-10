namespace Yahoo.Yui.Compressor
{
    public interface ICssCompressor : ICompressor
    {
        bool RemoveComments { get; set; }
    }
}