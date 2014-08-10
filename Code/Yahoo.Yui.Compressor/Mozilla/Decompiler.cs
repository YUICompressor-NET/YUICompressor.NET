using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Yahoo.Yui.Compressor.Mozilla
{
    public class Decompiler
    {
        #region Fields

        // Flag to indicate that the decompilation should omit the function header and trailing brace.
        public static int ONLY_BODY_FLAG = 1 << 0;

        // Flag to indicate that the decompilation generates toSource result.
        public static int TO_SOURCE_FLAG = 1 << 1;

        //Decompilation property to specify initial ident value.
        public static int INITIAL_INDENT_PROP = 1;

        // Decompilation property to specify default identation offset.
        public static int INDENT_GAP_PROP = 2;

        // Decompilation property to specify identation offset for case labels.
        public static int CASE_GAP_PROP = 3;

        // Marker to denote the last RC of function so it can be distinguished from
        // the last RC of object literals in case of function expressions
        private static int FUNCTION_END = Convert.ToInt32(Token.LAST_TOKEN) + 1;

        private char[] _sourceBuffer = new char[128];

        // Per script/function source buffer top: parent source does not include a
        // nested functions source and uses function index as a reference instead.
        private int _sourceTop;

        // Whether to do a debug print of the source information, when decompiling.
        private static bool _printSource = false;

        #endregion

        #region Properties

        public String EncodedSource
        {
            get { return this.SourceToString(0); }
        }

        public int CurrentOffset
        {
            get { return this._sourceTop; }
        }

        #endregion

        #region Methods

        #region Private Methods

        private void AppendString(String value)
        {
            int valueLength;
            int lengthEncodingSize;
            int nextTop;


            valueLength = value.Length;

            lengthEncodingSize = valueLength >= 0x8000 ? 2 : 1;
            
            nextTop = this._sourceTop + lengthEncodingSize + valueLength;
            
            if (nextTop > this._sourceBuffer.Length)
            {
                this.IncreaseSourceCapacity(nextTop);
            }

            if (valueLength >= 0x8000)
            {
                // Use 2 chars to encode strings exceeding 32K, were the highest
                // bit in the first char indicates presence of the next byte
                // NOTE: There is no '>>>' in C#. Handle it via unsigned, shift, signed.
                //       Info found here: http://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=1690218&SiteID=1
                uint x = Convert.ToUInt32(valueLength);
                this._sourceBuffer[this._sourceTop] = Convert.ToChar(0x8000 | Convert.ToInt32(x >> 16));
                this._sourceTop++;
            }

            this._sourceBuffer[this._sourceTop] = Convert.ToChar(valueLength);
            this._sourceTop++;
            value.CopyTo(0,
                this._sourceBuffer,
                this._sourceTop,
                valueLength);
            this._sourceTop = nextTop;
        }

        private void Append(char c)
        {
            if (this._sourceTop == this._sourceBuffer.Length)
            {
                this.IncreaseSourceCapacity(this._sourceTop + 1);
            }
            this._sourceBuffer[this._sourceTop] = c;
            this._sourceTop++;
        }

        private void IncreaseSourceCapacity(int minimalCapacity)
        {
            // Call this only when capacity increase is must
            //if (minimalCapacity <= this._sourceBuffer.Length) Kit.codeBug();

            int newCapacity = this._sourceBuffer.Length * 2;
            if (newCapacity < minimalCapacity)
            {
                newCapacity = minimalCapacity;
            }
            char[] tmp = new char[newCapacity];
            this._sourceBuffer.CopyTo(tmp,
                0);
            //System.arraycopy(sourceBuffer, 0, tmp, 0, sourceTop);
            this._sourceBuffer = tmp;
        }

        private string SourceToString(int offset)
        {
            //if (offset < 0 || sourceTop < offset) Kit.codeBug();
            return new string(this._sourceBuffer, 
                offset, 
                this._sourceTop - offset);
        }

        #endregion

        #region Public Methods

        public int MarkFunctionStart(int functionType)
        {
            int savedOffset;


            savedOffset = this.CurrentOffset;
            this.AddToken(Token.FUNCTION);
            this.Append((char)functionType);

            return savedOffset;
        }

        public int MarkFunctionEnd(int functionStart)
        {
            int offset;

            offset = CurrentOffset;
            this.Append((char)Decompiler.FUNCTION_END);

            return offset;
        }

        public void AddToken(Token token)
        {
            this.AddToken(token.ToInt());
        }

        public void AddToken(int token)
        {
            if (!(0 <= token && 
                token <= Token.LAST_TOKEN.ToInt()))
            {
                throw new ArgumentException();
            }

            this.Append((char)token);
        }

        public void AddEOL(int token)
        {
            if (!(0 <= token 
                && token <= Token.LAST_TOKEN.ToInt()))
            {
                throw new ArgumentException();
            }

            this.Append((char)token);
            this.Append(Token.EOL.ToChar());
        }

        public void AddName(string name)
        {
            this.AddToken(Token.NAME);
            this.AppendString(name);
        }

        public void AddRegexp(string regexp, 
            string flags)
        {
            this.AddToken(Token.REGEXP);
            this.AppendString('/' + regexp + '/' + flags);
        }

        public void AddJScriptConditionalComment(string jScriptConditionalComment)
        {
            this.AddToken(Token.SPECIALCOMMENT);
            this.AppendString(jScriptConditionalComment);
        }

        public void AddNumber(double number)
        {
            long lbits;


            this.AddToken(Token.NUMBER);

            /* encode the number in the source stream.
             * Save as NUMBER type (char | char char char char)
             * where type is
             * 'D' - double, 'S' - short, 'J' - long.

             * We need to retain float vs. integer type info to keep the
             * behavior of liveconnect type-guessing the same after
             * decompilation.  (Liveconnect tries to present 1.0 to Java
             * as a float/double)
             * OPT: This is no longer true. We could compress the format.

             * This may not be the most space-efficient encoding;
             * the chars created below may take up to 3 bytes in
             * constant pool UTF-8 encoding, so a Double could take
             * up to 12 bytes.
             */

            lbits = Convert.ToInt64(number);
            if (lbits != number)
            {
                // if it's floating point, save as a Double bit pattern.
                // (12/15/97 our scanner only returns Double for f.p.)
                lbits = Convert.ToInt64(number);
                this.Append('D');
                this.Append((char)(lbits >> 48));
                this.Append((char)(lbits >> 32));
                this.Append((char)(lbits >> 16));
                this.Append((char)lbits);
            }
            else
            {
                // we can ignore negative values, bc they're already prefixed
                // by NEG
                // TODO: Wtf is Kit.codeBug()?
                //if (lbits < 0) Kit.codeBug();

                // will it fit in a char?
                // this gives a short encoding for integer values up to 2^16.
                if (lbits <= char.MaxValue)
                {
                    this.Append('S');
                    this.Append((char)lbits);
                }
                else
                { // Integral, but won't fit in a char. Store as a long.
                    this.Append('J');
                    this.Append((char)(lbits >> 48));
                    this.Append((char)(lbits >> 32));
                    this.Append((char)(lbits >> 16));
                    this.Append((char)lbits);
                }
            }
        }

        #endregion

        #endregion
    }
}