using System;
using System.IO;
using System.Text;
using System.Web.Optimization;

namespace Yahoo.Yui.Compressor.Web.Optimization
{
    public class YuiCompressorTransform : IBundleTransform
    {
        private readonly CompressorConfig _compressorConfig;

        public YuiCompressorTransform(CompressorConfig compressorConfig)
        {
            if (compressorConfig == null)
            {
                throw new ArgumentNullException("compressorConfig");
            }

            _compressorConfig = compressorConfig;
        }

        #region Implementation of IBundleTransform

        public void Process(BundleContext context, BundleResponse response)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            // Grab all of the content.
            var rawContent = new StringBuilder();
            foreach (var fileInfo in response.Files)
            {
                using (var stream = fileInfo.VirtualFile.Open())
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        rawContent.Append(streamReader.ReadToEnd());
                    }
                }
            }

            // Now lets compress.
            var compressor = DetermineCompressor(_compressorConfig);
            var output = compressor.Compress(rawContent.ToString());
            context.HttpContext.Response.Cache.SetLastModifiedFromFileDependencies();
            response.Content = output;
            response.ContentType = compressor.ContentType;
        }

        #endregion

        private ICompressor DetermineCompressor(CompressorConfig compressorConfig)
        {
            if (compressorConfig == null)
            {
                throw new ArgumentNullException("compressorConfig");
            }

            // Define the compressor we wish to use.
            ICompressor compressor;
            var config = _compressorConfig as CssCompressorConfig;
            if (config != null)
            {
                var cssCompressorConfig = config;
                compressor = new CssCompressor
                {
                    CompressionType = cssCompressorConfig.CompressionType,
                    LineBreakPosition = cssCompressorConfig.LineBreakPosition,
                    RemoveComments = cssCompressorConfig.RemoveComments
                };
            }
            else if (_compressorConfig is JavaScriptCompressorConfig)
            {
                var jsCompressorConfig = (JavaScriptCompressorConfig)_compressorConfig;
                compressor = new JavaScriptCompressor
                {
                    CompressionType = jsCompressorConfig.CompressionType,
                    DisableOptimizations = jsCompressorConfig.DisableOptimizations,
                    Encoding = jsCompressorConfig.Encoding,
                    ErrorReporter = jsCompressorConfig.ErrorReporter,
                    IgnoreEval = jsCompressorConfig.IgnoreEval,
                    LineBreakPosition = jsCompressorConfig.LineBreakPosition,
                    LoggingType = jsCompressorConfig.LoggingType,
                    ObfuscateJavascript = jsCompressorConfig.ObfuscateJavascript,
                    PreserveAllSemicolons = jsCompressorConfig.PreserveAllSemicolons,
                    ThreadCulture = jsCompressorConfig.ThreadCulture
                };
            }
            else
            {
                throw new InvalidOperationException("Unhandled CompressorConfig instance while trying to initialize internal compressor properties.");
            }

            return compressor;
        }
    }
}