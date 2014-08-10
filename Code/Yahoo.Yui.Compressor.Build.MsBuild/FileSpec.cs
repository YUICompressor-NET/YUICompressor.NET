namespace Yahoo.Yui.Compressor.Build
{
    public class FileSpec
    {
        private string fileName;

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value.Trim();
            }
        }

        public string CompressionType { get; set; }

        public FileSpec(string fileName, string compressionType)
        {
            FileName = fileName;
            CompressionType = compressionType;
        }
    }
}