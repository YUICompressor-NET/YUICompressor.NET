using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Yahoo.Yui.Compressor.Build.MsBuild
{
    using System.Collections.Generic;

    public abstract class CompressorTask : Task
    {
        protected readonly CompressorTaskEngine TaskEngine;

        public string LoggingType { get; set; }

        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public string CompressionType { get; set; }

        public string EncodingType { get; set; }

        public bool DeleteSourceFiles { get; set; }

        public int LineBreakPosition { get; set; }

        protected CompressorTask(ICompressor compressor)
        {
            TaskEngine = new CompressorTaskEngine(new MsBuildLogAdapter(Log), compressor) { SetTaskEngineParameters = this.SetTaskEngineParameters };
            DeleteSourceFiles = false;
            LineBreakPosition = -1;
        }

        public override bool Execute()
        {
            return this.TaskEngine.Execute();
        }

        protected virtual void SetTaskEngineParameters()
        {
            this.TaskEngine.CompressionType = this.CompressionType;
            this.TaskEngine.DeleteSourceFiles = this.DeleteSourceFiles;
            this.TaskEngine.EncodingType = this.EncodingType;
            this.TaskEngine.LineBreakPosition = this.LineBreakPosition;
            this.TaskEngine.LoggingType = this.LoggingType;
            this.TaskEngine.OutputFile = this.OutputFile;
            var fileSpecs = new List<FileSpec>();
            if (SourceFiles == null)
            {
                return;
            }
            foreach (var sourceFile in SourceFiles)
            {
                fileSpecs.Add(new FileSpec(sourceFile.ItemSpec, sourceFile.GetMetadata("CompressionType")));
            }
            this.TaskEngine.SourceFiles = fileSpecs.ToArray();
        }
    }
}