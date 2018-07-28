using System;
using System.Text;
using System.Text.RegularExpressions;

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

            var cleanedSource = RemoveByteOrderMark(source);
            
            return DoCompress(cleanedSource);
        }

        #endregion

        protected abstract string DoCompress(string source);

        /// <summary>
        /// This removes the BOM from -anywhere- in some source text.
        /// </summary>
        /// <remarks>We've notived that files concatenated together leave BOM's -within- the source text, not just at the start. As such, these cause compression errors.</remarks>
        private static string RemoveByteOrderMark(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }

            var bomPreamble = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            return source.IndexOf(bomPreamble, StringComparison.OrdinalIgnoreCase) >= 0
                    ? source.Replace(bomPreamble, string.Empty)
                    : source;
        }
    }
}