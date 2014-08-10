namespace Yahoo.Yui.Compressor.Build.Nant
{
    using System.Collections.Generic;

    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Types;

    using Yahoo.Yui.Compressor.Build;

    public class CompressorTask : Task
    {
        private readonly NantLogAdapter logger;
        protected readonly CompressorTaskEngine TaskEngine;

        [TaskAttribute("loggingType")]
        public string LoggingType { get; set; }

        [BuildElement("sourceFiles", Required = true)]
        public FileSet SourceFiles { get; set; }

        [TaskAttribute("outputFile", Required = true)]
        [StringValidator(AllowEmpty = false)]
        public string OutputFile { get; set; }

        [TaskAttribute("compressionType")]
        public string CompressionType { get; set; }

        [TaskAttribute("encodingType")]
        public string EncodingType { get; set; }

        [TaskAttribute("deleteSourceFiles")]
        public bool DeleteSourceFiles { get; set; }

        [TaskAttribute("lineBreakPosition")]
        public int LineBreakPosition { get; set; }

        protected CompressorTask(ICompressor compressor)
        {
            this.logger = new NantLogAdapter();
            this.TaskEngine = new CompressorTaskEngine(logger, compressor) { SetTaskEngineParameters = this.SetTaskEngineParameters };
            this.SourceFiles = new FileSet();
            this.DeleteSourceFiles = false;
            this.LineBreakPosition = -1;
        }

        protected override void ExecuteTask()
        {
            logger.Project = this.Project;
            TaskEngine.Execute();
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
            if (this.SourceFiles == null)
            {
                return;
            }
            foreach (var sourceFile in this.SourceFiles.Includes)
            {
                fileSpecs.Add(new FileSpec(sourceFile, string.Empty));
            }
            this.TaskEngine.SourceFiles = fileSpecs.ToArray();
        }
    }
}