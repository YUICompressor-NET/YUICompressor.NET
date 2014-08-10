using System;

namespace Yahoo.Yui.Compressor
{
    public abstract class Compressor : ICompressor
    {
        protected Compressor()
        {
            CompressionType = CompressionType.Standard;
            LineBreakPosition = -1;
        }

        #region ICompressor Members

        public CompressionType CompressionType { get; set; }
        public int LineBreakPosition { get; set; }
        public abstract string ContentType { get; }

        public string Compress(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }

            if (CompressionType == CompressionType.None)
            {
                return source;
            }

            return DoCompress(source);
        }

        #endregion

        protected abstract string DoCompress(string source);
    }
}