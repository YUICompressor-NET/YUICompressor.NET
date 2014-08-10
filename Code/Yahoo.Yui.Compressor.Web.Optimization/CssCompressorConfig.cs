namespace Yahoo.Yui.Compressor.Web.Optimization
{
    public class CssCompressorConfig : CompressorConfig
    {
        public CssCompressorConfig()
        {
            RemoveComments = true;
        }

        public bool RemoveComments { get; set; }
    }
}