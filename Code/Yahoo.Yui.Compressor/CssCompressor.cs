using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahoo.Yui.Compressor
{
    public class CssCompressor : Compressor, ICssCompressor
    {
        public bool RemoveComments { get; set; }
        public override string ContentType { get { return "text/css"; } }

        public CssCompressor()
        {
            RemoveComments = true;
        }

        protected override string DoCompress(string source)
        {
            int totalLen = source.Length;
            int startIndex = 0;
            var comments = new ArrayList();
            var preservedTokens = new ArrayList();
            int max;

            if (RemoveComments)
            {
                while ((startIndex = source.IndexOf(@"/*", startIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    int endIndex = source.IndexOf(@"*/", startIndex + 2, StringComparison.OrdinalIgnoreCase);
                    if (endIndex < 0)
                    {
                        endIndex = totalLen;
                    }

                    // Note: java substring-length param = end index - 2 (which is end index - (startindex + 2))
                    string token = source.Substring(startIndex + 2, endIndex - (startIndex + 2));

                    comments.Add(token);

                    string newResult = Extensions.Replace(source, startIndex + 2, endIndex,
                                                   Tokens.PreserveCandidateComment + (comments.Count - 1) +
                                                   "___");

                    startIndex += 2;
                    source = newResult;
                }
            }

            // Preserve strings so their content doesn't get accidently minified
            var stringBuilder = new StringBuilder();
            var pattern = new Regex("(\"([^\\\\\"]|\\\\.|\\\\)*\")|(\'([^\\\\\']|\\\\.|\\\\)*\')");
            Match match = pattern.Match(source);
            int index = 0;
            while (match.Success)
            {
                var text = match.Groups[0].Value;
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                var token = match.Value;
                var quote = token[0];

                // Java code: token.substring(1, token.length() -1) .. but that's ...
                //            token.substring(start index, end index) .. while .NET it's length for the 2nd arg.
                token = token.Substring(1, token.Length - 2);

                // Maybe the string contains a comment-like substring?
                // one, maybe more? put'em back then.
                if (token.IndexOf(Tokens.PreserveCandidateComment, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    max = comments.Count;
                    for (int i = 0; i < max; i += 1)
                    {
                        token = token.Replace(Tokens.PreserveCandidateComment + i + "___",
                            comments[i].ToString());
                    }
                }

                // Minify alpha opacity in filter strings.
                token = Extensions.RegexReplace(token, "(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=",
                    "alpha(opacity=");

                preservedTokens.Add(token);
                string preserver = quote + Tokens.PreservedToken + (preservedTokens.Count - 1) + "___" +
                                   quote;

                index = Extensions.AppendReplacement(match, stringBuilder, source, preserver, index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Strings are safe, now wrestle the comments.
            max = comments.Count;
            for (int i = 0; i < max; i += 1)
            {
                var token = comments[i].ToString();
                var placeholder = Tokens.PreserveCandidateComment + i + "___";

                // ! in the first position of the comment means preserve
                // so push to the preserved tokens while stripping the !
                if (token.StartsWith("!"))
                {
                    preservedTokens.Add(token);
                    source = source.Replace(placeholder, Tokens.PreservedToken + (preservedTokens.Count - 1) + "___");
                    continue;
                }

                // \ in the last position looks like hack for Mac/IE5
                // shorten that to /*\*/ and the next one to /**/
                if (token.EndsWith("\\"))
                {
                    preservedTokens.Add("\\");
                    source = source.Replace(placeholder, Tokens.PreservedToken + (preservedTokens.Count - 1) + "___");
                    i = i + 1; // attn: advancing the loop.
                    preservedTokens.Add(string.Empty);
                    source = source.Replace(Tokens.PreserveCandidateComment + i + "___",
                                      Tokens.PreservedToken + (preservedTokens.Count - 1) + "___");
                    continue;
                }

                // keep empty comments after child selectors (IE7 hack)
                // e.g. html >/**/ body
                if (token.Length == 0)
                {
                    startIndex = source.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase);
                    if (startIndex > 2)
                    {
                        if (source[startIndex - 3] == '>')
                        {
                            preservedTokens.Add(string.Empty);
                            source = source.Replace(placeholder,
                                              Tokens.PreservedToken + (preservedTokens.Count - 1) + "___");
                        }
                    }
                }

                // In all other cases kill the comment.
                // Is this a closed comment?
                if (source.Contains("/*" + placeholder + "*/"))
                {
                    source = source.Replace("/*" + placeholder + "*/", string.Empty);
                }
                else
                {
                    // Nope - is it an unclosed comment?
                    if (source.Contains("/*" + placeholder))
                    {
                        // TODO: Add a Warning to the log (once we have a log!)
                        source = source.Replace("/*" + placeholder, string.Empty);
                    }
                }
            }

            // Preserve any calc(...) css - https://yuicompressor.codeplex.com/workitem/11445
            stringBuilder = new StringBuilder();
            pattern = new Regex("(calc\\s*\\(.*\\))");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                preservedTokens.Add(match.Value);
                string preserver = Tokens.PreservedToken + (preservedTokens.Count - 1) + "___";

                index = Extensions.AppendReplacement(match, stringBuilder, source, preserver, index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Normalize all whitespace strings to single spaces. Easier to work with that way.
            source = Extensions.RegexReplace(source, "\\s+", " ");

            // Remove the spaces before the things that should not have spaces before them.
            // But, be careful not to turn "p :link {...}" into "p:link{...}"
            // Swap out any pseudo-class colons with the token, and then swap back.
            stringBuilder = new StringBuilder();
            pattern = new Regex("(^|\\})(([^\\{:])+:)+([^\\{]*\\{)");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                string text = match.Value;
                text = text.Replace(":", Tokens.PseudoClassColon);
                text = text.Replace("\\\\", "\\\\\\\\");
                text = text.Replace("\\$", "\\\\\\$");
                index = Extensions.AppendReplacement(match, stringBuilder, source, text, index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Remove spaces before the things that should not have spaces before them.
            source = Extensions.RegexReplace(source, "\\s+([!{};:>+\\(\\)\\],])", "$1");

            // Bring back the colon.
            source = Extensions.RegexReplace(source, Tokens.PseudoClassColon, ":");

            // Retain space for special IE6 cases.
            source = Extensions.RegexReplace(source, ":first\\-(line|letter)(\\{|,)", ":first-$1 $2");

            // no space after the end of a preserved comment.
            source = Extensions.RegexReplace(source, "\\*/ ", "*/");

            // If there is a @charset, then only allow one, and push to the top of the file.
            source = Extensions.RegexReplace(source, "^(.*)(@charset \"[^\"]*\";)", "$2$1");
            source = Extensions.RegexReplace(source, "^(\\s*@charset [^;]+;\\s*)+", "$1");

            // Put the space back in some cases, to support stuff like
            // @media screen and (-webkit-min-device-pixel-ratio:0){
            source = Extensions.RegexReplace(source, "\\band\\(", "and (");

            // Remove the spaces after the things that should not have spaces after them.
            source = Extensions.RegexReplace(source, "([!{}:;>+\\(\\[,])\\s+", "$1");

            // remove unnecessary semicolons.
            source = Extensions.RegexReplace(source, ";+}", "}");

            // Replace 0(px,em,%) with 0.
            source = Extensions.RegexReplace(source, "([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");

            // Replace 0 0 0 0; with 0.
            source = Extensions.RegexReplace(source, ":0 0 0 0(;|})", ":0$1");
            source = Extensions.RegexReplace(source, ":0 0 0(;|})", ":0$1");
            source = Extensions.RegexReplace(source, ":0 0(;|})", ":0$1");

            // Replace background-position:0; with background-position:0 0;
            // same for transform-origin
            stringBuilder = new StringBuilder();
            pattern =
                new Regex("(?i)(background-position|transform-origin|webkit-transform-origin|moz-transform-origin|o-transform-origin|ms-transform-origin):0(;|})");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                index = Extensions.AppendReplacement(match, stringBuilder, source,
                                                match.Groups[1].Value.ToLowerInvariant() + ":0 0" + match.Groups[2],
                                                index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Replace 0.6 to .6, but only when preceded by : or a white-space.
            source = Extensions.RegexReplace(source, "(:|\\s)0+\\.(\\d+)", "$1.$2");

            // Shorten colors from rgb(51,102,153) to #336699
            // This makes it more likely that it'll get further compressed in the next step.
            stringBuilder = new StringBuilder();
            pattern = new Regex("rgb\\s*\\(\\s*([0-9,\\s]+)\\s*\\)");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                var rgbcolors = match.Groups[1].Value.Split(',');
                var hexcolor = new StringBuilder("#");
                foreach (var rgbColour in rgbcolors)
                {
                    int value;
                    if (!Int32.TryParse(rgbColour, out value))
                    {
                        value = 0;
                    }

                    if (value < 16)
                    {
                        hexcolor.Append("0");
                    }
                    hexcolor.Append(Extensions.ToHexString(value).ToLowerInvariant());
                }

                index = Extensions.AppendReplacement(match, stringBuilder, source, hexcolor.ToString(), index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Shorten colors from #AABBCC to #ABC. Note that we want to make sure
            // the color is not preceded by either ", " or =. Indeed, the property
            //     filter: chroma(color="#FFFFFF");
            // would become
            //     filter: chroma(color="#FFF");
            // which makes the filter break in IE.
            stringBuilder = new StringBuilder();
            pattern = new Regex("([^\"'=\\s])(\\s*)#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                // Test for AABBCC pattern.
                if (Extensions.EqualsIgnoreCase(match.Groups[3].Value, match.Groups[4].Value) &&
                    Extensions.EqualsIgnoreCase(match.Groups[5].Value, match.Groups[6].Value) &&
                    Extensions.EqualsIgnoreCase(match.Groups[7].Value, match.Groups[8].Value))
                {
                    string replacement = String.Concat(match.Groups[1].Value, match.Groups[2].Value, "#",
                                                       match.Groups[3].Value, match.Groups[5].Value,
                                                       match.Groups[7].Value);
                    index = Extensions.AppendReplacement(match, stringBuilder, source, replacement, index);
                }
                else
                {
                    index = Extensions.AppendReplacement(match, stringBuilder, source, match.Value, index);
                }

                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // border: none -> border:0
            stringBuilder = new StringBuilder();
            pattern =
                new Regex("(?i)(border|border-top|border-right|border-bottom|border-right|outline|background):none(;|})");
            match = pattern.Match(source);
            index = 0;
            while (match.Success)
            {
                string replacement = match.Groups[1].Value.ToLowerInvariant() + ":0" + match.Groups[2].Value;
                index = Extensions.AppendReplacement(match, stringBuilder, source, replacement, index);
                match = match.NextMatch();
            }
            Extensions.AppendTail(stringBuilder, source, index);
            source = stringBuilder.ToString();

            // Shorter opacity IE filter.
            source = Extensions.RegexReplace(source, "(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=", "alpha(opacity=");

            // Remove empty rules.
            source = Extensions.RegexReplace(source, "[^\\}\\{/;]+\\{\\}", string.Empty);

            if (LineBreakPosition >= 0)
            {
                // Some source control tools don't like it when files containing lines longer
                // than, say 8000 characters, are checked in. The linebreak option is used in
                // that case to split long lines after a specific column.
                int i = 0;
                int linestartpos = 0;
                stringBuilder = new StringBuilder(source);
                while (i < stringBuilder.Length)
                {
                    char c = stringBuilder[i++];
                    if (c == '}' && i - linestartpos > LineBreakPosition)
                    {
                        stringBuilder.Insert(i, '\n');
                        linestartpos = i;
                    }
                }

                source = stringBuilder.ToString();
            }

            // Replace multiple semi-colons in a row by a single one.
            // See SF bug #1980989.
            source = Extensions.RegexReplace(source, ";;+", ";");

            // Restore preserved comments and strings.
            max = preservedTokens.Count;
            for (int i = 0; i < max; i++)
            {
                source = source.Replace(Tokens.PreservedToken + i + "___", preservedTokens[i].ToString());
            }

            // Trim the final string (for any leading or trailing white spaces).
            source = source.Trim();

            // Write the output...
            return source;
        }
    }
}