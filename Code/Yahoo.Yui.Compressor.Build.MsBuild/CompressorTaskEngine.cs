using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

// ReSharper disable CheckNamespace
namespace Yahoo.Yui.Compressor.Build
// ReSharper restore CheckNamespace
{
    public class CompressorTaskEngine
    {
        private CompressionType _compressionType;

        protected internal LoggingType LogType;

        protected internal Encoding Encoding;

        private readonly ICompressor _compressor;

        public string LoggingType { get; set; }

        public FileSpec[] SourceFiles { get; set; }

        public string OutputFile { get; set; }

        public string CompressionType { get; set; }

        public string EncodingType { get; set; }

        public bool DeleteSourceFiles { get; set; }

        public int LineBreakPosition { get; set; }

        public ILog Log { get; private set; }

        public Action SetTaskEngineParameters;

        public Action ParseAdditionalTaskParameters;

        public Action LogAdditionalTaskParameters;

        public Action SetCompressorParameters;

        public delegate void Action();

        public CompressorTaskEngine(ILog log, ICompressor compressor)
        {
            Log = log;
            _compressor = compressor;
            Encoding = Encoding.Default;
            DeleteSourceFiles = false;
            LineBreakPosition = -1;
        }

        public bool Execute()
        {
            try
            {
                if (SetTaskEngineParameters != null)
                {
                    SetTaskEngineParameters();
                }
                ParseTaskParameters();
                if (SetCompressorParameters != null)
                {
                    SetCompressorParameters();
                }
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }

            // Check to make sure we have the bare minimum arguments supplied to the task.
            if (SourceFiles == null || SourceFiles.Length == 0)
            {
                Log.LogError("At least one file is required to be compressed / minified.");
                return false;
            }

            if (string.IsNullOrEmpty(OutputFile))
            {
                Log.LogError("The outfile is required if one or more css input files have been defined.");
                return false;
            }

            foreach (var sourceFile in SourceFiles)
            {
                if (string.Compare(sourceFile.FileName, OutputFile, StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    Log.LogError("Output file cannot be the same as source file(s).");
                    return false;
                }
            }

            if (LogType == Yui.Compressor.LoggingType.Debug)
            {
                LogTaskParameters();
                if (LogAdditionalTaskParameters != null)
                {
                    LogAdditionalTaskParameters();
                }
            }

            Log.LogMessage("Starting Compression...");

            // Determine and log the Assembly version.
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            var assemblyFileVersion = fileVersionAttributes.Length > 0
                                             ? ((AssemblyFileVersionAttribute)fileVersionAttributes[0]).Version
                                             : "Unknown File Version";

            var assemblyTitleAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            var assemblyTitle = assemblyTitleAttributes.Length > 0
                                       ? ((AssemblyTitleAttribute)assemblyTitleAttributes[0]).Title
                                       : "Unknown Title";

            Log.LogMessage(string.Format("Using version {0} of {1}.", assemblyFileVersion, assemblyTitle));

            // What is the current thread culture?
            Log.LogMessage(
                string.Format(
                    "Current thread culture / UI culture (before modifying, if requested): {0}/{1}",
                    Thread.CurrentThread.CurrentCulture.EnglishName,
                    Thread.CurrentThread.CurrentUICulture.EnglishName));

            Log.LogMessage(string.Empty); // This, in effect, is a new line.

            var startTime = DateTime.Now;
            var errorFound = false;
            var compressedText = CompressFiles(out errorFound);

            if (errorFound)
            {
                Log.LogMessage("Error found during compression - see log.");
                return false;
            }

            // Save this css to the output file, if we have some result text.
            if (!SaveCompressedText(compressedText))
            {
                Log.LogMessage("Failed to finish compression - terminating prematurely.");
                return false;
            }

            Log.LogMessage("Finished compression.");
            Log.LogMessage(
                string.Format(
                    CultureInfo.InvariantCulture, "Total time to execute task: {0}", (DateTime.Now - startTime)));
            Log.LogMessage("8< ---------------------------------  ( o Y o )  --------------------------------- >8");
            Log.LogMessage(string.Empty); // This, in effect, is a new line.

            return true;
        }

        public void ParseTaskParameters()
        {
            ParseLoggingType();
            if (string.IsNullOrEmpty(CompressionType))
            {
                LogMessage("No Compression type defined. Defaulting to 'Standard'.");
                _compressionType = ParseCompressionType("Standard");
            }
            else
            {
                _compressionType = ParseCompressionType(CompressionType);
            }
            ParseEncoding();
            if (ParseAdditionalTaskParameters != null)
            {
                ParseAdditionalTaskParameters();
            }
        }

        protected void LogMessage(string message, bool isIndented = false)
        {
            if (LogType != Yui.Compressor.LoggingType.None)
            {
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture, "{0}{1}", isIndented ? "    " : string.Empty, message));
            }
        }

        private void ParseLoggingType()
        {
            if (string.IsNullOrEmpty(LoggingType))
            {
                LogType = Yui.Compressor.LoggingType.Info;
                LogMessage("No logging argument defined. Defaulting to 'Info'.");
                return;
            }

            switch (LoggingType.ToLowerInvariant())
            {
                case "none":
                    LogType = Yui.Compressor.LoggingType.None;
                    break;
                case "debug":
                    LogType = Yui.Compressor.LoggingType.Debug;
                    break;
                case "info":
                    LogType = Yui.Compressor.LoggingType.Info;
                    break;
                default:
                    throw new ArgumentException("Logging Type: " + LoggingType + " is invalid.", "LoggingType");
            }
        }

        private void ParseEncoding()
        {
            if (string.IsNullOrEmpty(EncodingType))
            {
                Encoding = Encoding.Default;
                return;
            }

            switch (EncodingType.ToLowerInvariant())
            {
                case "ascii":
                    Encoding = Encoding.ASCII;
                    break;
                case "bigendianunicode":
                    Encoding = Encoding.BigEndianUnicode;
                    break;
                case "unicode":
                    Encoding = Encoding.Unicode;
                    break;
                case "utf32":
                case "utf-32":
                    Encoding = Encoding.UTF32;
                    break;
                case "utf7":
                case "utf-7":
                    Encoding = Encoding.UTF7;
                    break;
                case "utf8":
                case "utf-8":
                    Encoding = Encoding.UTF8;
                    break;
                case "default":
                    Encoding = Encoding.Default;
                    break;
                default:
                    throw new ArgumentException("Encoding: " + EncodingType + " is invalid.", "EncodingType");
            }
        }

        protected virtual void LogTaskParameters()
        {
            LogMessage("CompressionType: " + CompressionType);
            LogMessage("DeleteSourceFiles: " + DeleteSourceFiles);
            LogMessage("EncodingType: " + EncodingType);
            LogMessage("LoggingType: " + LoggingType);
        }

        private CompressionType ParseCompressionType(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "none":
                    return Yui.Compressor.CompressionType.None;
                case "standard":
                    return Yui.Compressor.CompressionType.Standard;
                default:
                    throw new ArgumentException("Compression Type: " + type + " is invalid.", "type");
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private StringBuilder CompressFiles(out bool errorFound)
        {
            errorFound = false;
            int totalOriginalContentLength = 0;
            StringBuilder finalContent = null;

            if (SourceFiles != null)
            {
                LogMessage(string.Format(CultureInfo.InvariantCulture, "# {0} file{1} requested.", SourceFiles.Length, Extensions.ToPluralString(SourceFiles.Length)));

                // Now compress each file.
                foreach (var file in SourceFiles)
                {
                    var message = "=> " + file.FileName;

                    // Load up the file.
                    try
                    {
                        var originalContent = File.ReadAllText(file.FileName, Encoding);
                        totalOriginalContentLength += originalContent.Length;

                        if (string.IsNullOrEmpty(originalContent))
                        {
                            errorFound = true;
                            LogMessage(message, true);
                            Log.LogError(string.Format(CultureInfo.InvariantCulture, "There is no data in the file [{0}]. Please check that this is the file you want to compress.", file.FileName));
                        }

                        var compressedContent = Compress(file, originalContent);

                        if (!string.IsNullOrEmpty(compressedContent))
                        {
                            if (finalContent == null)
                            {
                                finalContent = new StringBuilder();
                            }
                            finalContent.Append(compressedContent);
                        }

                        // Try and remove this file, if the user requests to do 
                        try
                        {
                            if (DeleteSourceFiles)
                            {
                                if (LogType == Yui.Compressor.LoggingType.Debug)
                                {
                                    Log.LogMessage("Deleting source file: " + file.FileName);
                                }
                                File.Delete(file.FileName);
                            }
                        }
                        catch (Exception exception)
                        {
                            errorFound = true;
                            Log.LogError(
                                string.Format(CultureInfo.InvariantCulture,
                                    "Failed to delete the path/file [{0}]. It's possible the file is locked?",
                                    file.FileName));
                            Log.LogErrorFromException(exception, false);
                        }
                    }
                    catch (Exception exception)
                    {
                        errorFound = true;
                        if (exception is FileNotFoundException)
                        {
                            Log.LogError(string.Format(CultureInfo.InvariantCulture, "ERROR reading file or path [{0}].", file.FileName));
                        }
                        else
                        {
                            // FFS :( Something bad happened.
                            Log.LogError(string.Format(CultureInfo.InvariantCulture, "Failed to read/parse data in file [{0}].", file.FileName));
                        }
                        Log.LogErrorFromException(exception, false);
                    }
                }

                LogMessage(
                    string.Format(CultureInfo.InvariantCulture,
                        "Finished compressing all {0} file{1}.",
                        SourceFiles.Length,
                        Extensions.ToPluralString(SourceFiles.Length)),
                    true);

                int finalContentLength = finalContent == null ? 0 : finalContent.ToString().Length;

                LogMessage(
                    string.Format(CultureInfo.InvariantCulture,
                        "Total original file size: {0}. After compression: {1}. Compressed down to {2}% of original size.",
                        totalOriginalContentLength,
                        finalContentLength,
                        100 - (totalOriginalContentLength - (float)finalContentLength) / totalOriginalContentLength * 100));

                LogMessage(string.Format(CultureInfo.InvariantCulture, "Compression Type: {0}.", _compressionType));
            }

            return finalContent;
        }

        protected internal virtual string Compress(FileSpec file, string originalContent)
        {
            _compressor.CompressionType = GetCompressionTypeFor(file);
            _compressor.LineBreakPosition = LineBreakPosition;
            return _compressor.Compress(originalContent);
        }

        private CompressionType GetCompressionTypeFor(FileSpec file)
        {
            var message = "=> " + file.FileName;
            var actualCompressionType = _compressionType;
            var overrideType = file.CompressionType;
            if (!string.IsNullOrEmpty(overrideType))
            {
                actualCompressionType = ParseCompressionType(overrideType);
                if (actualCompressionType != _compressionType)
                {
                    message += string.Format(" (CompressionType: {0})", actualCompressionType.ToString());
                }
            }
            LogMessage(message, true);
            return actualCompressionType;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private bool SaveCompressedText(StringBuilder compressedText)
        {
            // Note: compressedText CAN be null or empty, so no check.

            var outputDirectory = Path.GetDirectoryName(Path.GetFullPath(OutputFile));
            if (!Directory.Exists(outputDirectory))
            {
                Log.LogMessage(string.Format("Creating output directory {0}", outputDirectory));
                Directory.CreateDirectory(outputDirectory);
            }

            try
            {
                File.WriteAllText(OutputFile, compressedText == null ? string.Empty : compressedText.ToString(), Encoding);
                Log.LogMessage(string.Format(CultureInfo.InvariantCulture, "Compressed content saved to file [{0}].{1}",
                                             OutputFile, Environment.NewLine));
            }
            catch (Exception exception)
            {
                // Most likely cause of this exception would be that the user failed to provide the correct path/file
                // or the file is read only, unable to be written, etc.. 
                Log.LogError(string.Format(CultureInfo.InvariantCulture,
                                           "Failed to save the compressed text into the output file [{0}]. Please check the path/file name and make sure the file isn't magically locked, read-only, etc..",
                                           OutputFile));
                Log.LogErrorFromException(exception, false);

                return false;
            }
            return true;
        }
    }
}