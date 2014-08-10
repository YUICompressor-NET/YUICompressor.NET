using NAnt.Core.Attributes;

namespace Yahoo.Yui.Compressor.Build.Nant
{
    [TaskName("cssCompressor")]
    public class CssCompressorTask : CompressorTask
    {
        private readonly ICssCompressor compressor;

        [TaskAttribute("preserveComments")]
        public bool PreserveComments { get; set; }

        public CssCompressorTask() : this(new CssCompressor())
        {
        }

        public CssCompressorTask(ICssCompressor compressor) : base(compressor)
        {
            this.compressor = compressor;
        }

        protected override void ExecuteTask()
        {
            compressor.RemoveComments = !PreserveComments;
            base.ExecuteTask();
        }
    }
}