namespace Yahoo.Yui.Compressor.Build.MsBuild
{
    public class CssCompressorTask : CompressorTask
    {
        private readonly ICssCompressor _compressor;

        public bool PreserveComments { get; set; }

        public CssCompressorTask() : this(new CssCompressor())
        {
        }

        public CssCompressorTask(ICssCompressor compressor) : base(compressor)
        {
            _compressor = compressor;
        }

        public override bool Execute()
        {
            _compressor.RemoveComments = !PreserveComments;
            return base.Execute();
        }
    }
}