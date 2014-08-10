using System;
using System.IO;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class CssCompressorTest
    {
        private CssCompressor target;

        [SetUp]
        public void SetUp()
        {
            target = new CssCompressor();
        }

        [Test]
        public void CompressCssWithNoColumnWidthSucessfullyCompressesText()
        {
            // Arrange.
            var source = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void An_Exception_Is_Thrown_If_The_Incoming_Css_Is_Null()
        {
            target.Compress(null);
        }

        [Test]
        public void CompressCssWithASpecificColumnWidthSucessfullyCompressesText()
        {
            // Arrange.
            var source = File.ReadAllText(@"Cascading Style Sheet Files\SampleStylesheet1.css");
            target.LineBreakPosition = 73;

            // Act.
            var actual = target.Compress(source);
            
            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void A_Stylesheet_With_Empty_Content_Only_Returns_An_Empty_Result()
        {
            // Arrange.
            const string source = @"body
                                      {
                                      }";

            // Act & Assert
            CompressAndCompare(source, string.Empty);
        }

        [Test]
        public void Background_Position_Should_Have_Spurious_0s_Removed()
        {
            // Arrange
            const string source = @"a {background-position: 0 0 0 0;}
                                    b {BACKGROUND-POSITION: 0 0;}";
            const string expected = @"a{background-position:0 0}b{background-position:0 0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Background_None_Will_Be_Replaced_With_Background_0()
        {
            // Arrange
            const string source = @"a {
                                        border: none;
                                    }
                                    s {border-top: none;}";
            const string expected = @"a{border:0}s{border-top:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Border_None_Will_Be_Replaced_With_Border_0()
        {
            // Arrange
            const string source = @"a {
                                        BACKGROUND: none;
                                    }
                                    b {BACKGROUND:none}";
            const string expected = @"a{background:0}b{background:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Box_Model_Hack_Css_Is_Compressed_Correctly()
        {
            // Box Model Hack: http://tantek.com/CSS/Examples/boxmodelhack.html

            // Arrange
            const string source = @"#elem { 
                                         width: 100px; 
                                         voice-family: ""\""}\""""; 
                                         voice-family:inherit;
                                         width: 200px;
                                        }
                                        html>body #elem {
                                         width: 200px;
                                        }";
            const string expected = @"#elem{width:100px;voice-family:""\""}\"""";voice-family:inherit;width:200px}html>body #elem{width:200px}";

            // Act & Assert
            CompressAndCompare(source, expected); 
        }

        [Test]
        [Description("https://github.com/yui/yuicompressor/blob/d36d4470ff786aadc2e70a36e689882d0bce4cc0/tests/bug2527998.css")]
        public void An_Empty_Body_Should_Be_Removed_But_A_Preserved_Comment_Should_Remain()
        {
            // Arrange
            const string source = @"/*! special */
                                    body {

                                    }
                                    ";
            const string expected = @"/*! special */";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("https://github.com/yui/yuicompressor/blob/d36d4470ff786aadc2e70a36e689882d0bce4cc0/tests/bug2528034.css")]
        public void An_Empty_First_Child_Should_Be_Removed()
        {
            // Arrange
            const string source = @"a[href$=""/test/""] span:first-child { b:1; }
                                    a[href$=""/test/""] span:first-child { }";
            const string expected = @"a[href$=""/test/""] span:first-child{b:1}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("https://github.com/yui/yuicompressor/blob/98310d3cd799ab6f243689c7611503ac234ad2db/tests/charset-media.css")]
        public void Yahoo_YUICompressor_Bug_2495387_Should_Be_Fixed()
        {
            // Arrange
            const string source = @"/* re: 2495387 */
                                    @charset 'utf-8';
                                    @media all {
                                    body {
                                    }
                                    body {
                                    background-color: gold;
                                    }
                                    }";
            const string expected = @"@charset 'utf-8';@media all{body{background-color:gold}}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Color_Styles_Have_Rgb_Values_Replaced_With_Hex_Values()
        {
            // Arrange
            const string source = @".color {
                                      me: rgb(123, 123, 123);
                                      background: none repeat scroll 0 0 rgb(255, 0,0);
                                      alpha: rgba(1, 2, 3, 4);
                                    }";
            const string expected = @".color{me:#7b7b7b;background:none repeat scroll 0 0 #f00;alpha:rgba(1,2,3,4)}";
            
            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Color_Styles_Have_Unquoted_Hex_Values_Compressed_To_Shorter_Equivalents()
        {
            // Arrange
            const string source = @".color {
                                      impressed: #ffeedd;
                                      filter: chroma(color=""#FFFFFF"");
                                    }";
            const string expected = @".color{impressed:#fed;filter:chroma(color=""#FFFFFF"")}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://en.wikipedia.org/wiki/CSS_filter#Child_selector_hack")]
        public void Empty_Comments_After_A_Child_Selector_Are_Preserved_As_Hack_For_IE7()
        {
            // Arrange
            const string source = @"html >/**/ body p {
                                        color: blue; 
                                    }
                                    ";
            const string expected = @"html>/**/body p{color:blue}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void The_Output_Should_Retain_Only_The_First_Charset_If_The_Source_Contains_Multiple_Charsets()
        {
            // Arrange
            const string source = @"/* This is invalid CSS, but frequently happens as a result of concatenation. */
                                    @charset ""utf-8"";
                                    #foo {
                                        border-width:1px;
                                    }
                                    /*
                                    Note that this is erroneous!
                                    The actual CSS file can only have a single charset.
                                    However, this is the job of the author/application.
                                    The compressor should not get involved.
                                    */
                                    @charset ""another one"";
                                    #bar {
                                        border-width:10px;
                                    }";
            const string expected = @"@charset ""utf-8"";#foo{border-width:1px}#bar{border-width:10px}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Decimal_Values_Are_Preserved_With_Leading_Zeroes_Removed()
        {
            // Arrange
            const string source = @"::selection { 
                                      margin: 0.6px 0.333pt 1.2em 8.8cm;
                                   }";
            const string expected = @"::selection{margin:.6px .333pt 1.2em 8.8cm}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("PK to Look at")]
        public void A_Comment_With_Dollar_Header_Is_Preserved_But_Only_It_Seemes_Because_The_Preserve_Comment_Exclaimation_Exists()
        {
            // Originally called DollarHeaderCssTest, using dollar-header.css and dollar-header.css.min
            // What is the significant of the $Header bit?
            // Removing the '!' makes the comment not be preserved anymore - so what are we trying to achieve?

            // Arrange
            const string source = @"/*!
                                    $Header: /temp/dirname/filename.css 3 2/02/08 3:37p JSmith $
                                    */

                                    foo {
                                        bar: baz
                                    }";
            const string expected = @"/*!
                                    $Header: /temp/dirname/filename.css 3 2/02/08 3:37p JSmith $
                                    */foo{bar:baz}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void FontFace_Elements_Are_Compressed_As_Expected()
        {
            // Arrange
            const string source = @"@font-face {
                                      font-family: 'gzipper';
                                      src: url(yanone.eot);
                                      src: local('gzipper'), 
                                              url(yanone.ttf) format('truetype');
                                    }
                                    ";
            const string expected = @"@font-face{font-family:'gzipper';src:url(yanone.eot);src:local('gzipper'),url(yanone.ttf) format('truetype')}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://www.sam-i-am.com/work/sandbox/css/mac_ie5_hack.html")]
        public void Commented_Backslash_Should_Be_Compressed_As_Expected_As_Ie5_Mac_Hack()
        {
            // Arrange
            const string source = @"/* Ignore the next rule in IE mac \*/
                                    .selector {
                                       color: khaki;
                                    }
                                    /* Stop ignoring in IE mac */";
            // ie, the bachslah -------\ here is retained, and the closing comment braces to stop ignoring
            const string expected = @"/*\*/.selector{color:khaki}/**/"; 

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Empty_Elements_Should_Be_Removed()
        {
            // Arrange
            const string source = @".empty {}
                                    alsoEmpty { ;}
                                    .notEmpty { 
                                        color: purple;
                                    }";
            const string expected = @".notEmpty{color:purple}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }


        [Test]
        public void Empty_Media_Elements_Should_Be_Removed()
        {
            // Arrange
            const string source = @"@media print {
                                        .noprint { display: none; }
                                    }

                                    @media screen {
                                        /* this empty rule should be removed, not simply minified.*/
                                        .emptymedia {}
                                        .printonly { display: none; }
                                    }";
            const string expected = @"@media print{.noprint{display:none}}@media screen{.printonly{display:none}}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Media_Elements_Retain_The_Space_After_An_And()
        {
            // Arrange
            // This space --------------------------\ should be retained
            const string source = @"@media screen and (-webkit-min-device-pixel-ratio:0) {
                                      some-css : here
                                    }";
            const string expected = @"@media screen and (-webkit-min-device-pixel-ratio:0){some-css:here}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Multiple_Media_Elements_Retain_Spaces_After_An_And()
        {
            // Arrange
            // These spaces ---------------------------\ ----------------------------\ -------------------------------------\ should be retained
            const string source = @"@media only all and (max-width:50em), only all and (max-device-width:800px), only all and (max-width:780px) {
                                      some-css : here
                                    }
                                    ";
            const string expected = @"@media only all and (max-width:50em),only all and (max-device-width:800px),only all and (max-width:780px){some-css:here}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Opacity_Filters_Are_Compressed_As_Expected()
        {
            // Arrange
            const string source = @"/*  example from https://developer.mozilla.org/en/CSS/opacity */
                                    pre {                               /* make the box translucent (80% opaque) */
                                       border: solid red;
                                       opacity: 0.8;                    /* Firefox, Safari(WebKit), Opera */
                                       -ms-filter: ""progid:DXImageTransform.Microsoft.Alpha(Opacity=80)""; /* IE 8 */
                                       filter: PROGID:DXImageTransform.Microsoft.Alpha(Opacity=80);       /* IE 4-7 */
                                       zoom: 1;       /* set ""zoom"", ""width"" or ""height"" to trigger ""hasLayout"" in IE 7 and lower */ 
                                    }

                                    /** and again */
                                    code {
                                       -ms-filter: ""PROGID:DXImageTransform.Microsoft.Alpha(Opacity=80)""; /* IE 8 */
                                       filter: progid:DXImageTransform.Microsoft.Alpha(Opacity=80);       /* IE 4-7 */
                                    }";
            const string expected = @"pre{border:solid red;opacity:.8;-ms-filter:""alpha(opacity=80)"";filter:alpha(opacity=80);zoom:1}code{-ms-filter:""alpha(opacity=80)"";filter:alpha(opacity=80)}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void String_Values_Should_Retain_New_Line_Characters()
        {
            // Arrange
            const string source = @"#sel-o {
                                      content: ""on\""ce upon \
                                    a time"";
                                      content: 'once upon \
                                    a ti\'me';
                                    }";
            const string expected = @"#sel-o{content:""on\""ce upon \
                                    a time"";content:'once upon \
                                    a ti\'me'}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void String_Values_Should_Be_Preserved_Exactly()
        {
            // Arrange
            const string source = @"/* preserving strings */
                                    .sele {
                                        content: ""\""keep  \""    me"";
                                        something: '\\\' .     . ';
                                        else: 'empty{}';
                                        content: ""/* test */""; /* <---- this is not a comment, should be be kept */
                                    }";
            const string expected = @".sele{content:""\""keep  \""    me"";something:'\\\' .     . ';else:'empty{}';content:""/* test */""}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Pseudo_First_Letter_Element_Should_Be_Compressed_As_Expected()
        {
            // Arrange
            const string source = @"/* 
                                    because of IE6 first-letter and first-line
                                    must be followed by a space
                                    http://reference.sitepoint.com/css/pseudoelement-firstletter
                                    Thanks: P.Sorokin comment at http://www.phpied.com/cssmin-js/ 
                                    */
                                    p:first-letter{
                                        buh: hum;
                                    }
                                    p:first-line{
                                        baa: 1;
                                    }

                                    p:first-line,a,p:first-letter,b{
                                        color: red;
                                    }";
            const string expected = @"p:first-letter {buh:hum}p:first-line {baa:1}p:first-line ,a,p:first-letter ,b{color:red}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Pseudo_Css_Elements_Should_Be_Compressed_And_Have_Suprious_Semi_Colons_Removed()
        {
            // Arrange
            const string source = @"p :link { 
                                      ba:zinga;;;
                                      foo: bar;;;
                                    }";
            const string expected = @"p :link{ba:zinga;foo:bar}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Star_And_Underscore_Hacks_Are_Preserved()
        {
            // Arrange
            const string source = @"#elementarr {
                                      width: 1px;
                                      *width: 3pt;
                                      _width: 2em;
                                    }";
            const string expected = @"#elementarr{width:1px;*width:3pt;_width:2em}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Comments_Should_Be_Removed()
        {
            // Arrange.
            const string source = @".moreactions_applyfilter_reset
                                    {
                                        /* First Comment */
                                        text-align: right;
                                    }
                                    /* Second Comment */";
            const string expected = @".moreactions_applyfilter_reset{text-align:right}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/3723")]
        public void Commments_With_No_Closing_Symbol_Are_Removed()
        {
            // Arrange.
            const string source = @".moreactions_applyfilter_reset
                                    {
                                    text-align: right;
                                    }

                                    /* end of moreactions_filter
                                    All of this and the below will be removed because it is deemed part of the unclosed comment
                                    .OtherClass {           
                                        color: purple
                                    }";
            const string expected = @".moreactions_applyfilter_reset{text-align:right}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("PK to Look at")]
        public void Some_Commments_Are_Preserved_Empty_But_Im_Not_Sure_If_This_Is_Correct_Or_Not()
        {
            // Was called StringInCommentCssTest, using string-in-comment.css and string-in-comment.css.min
            // Arrange
            const string source = @"/* te "" st */
                                    a{a:1}
                                    /* quite "" quote ' \' \"" */
                                    /* ie mac \*/
                                    c {c : 3}
                                    /* end hiding */";
            const string expected = @"a{a:1}/*\*/c{c:3}/**/";

            // Act & Assert
            CompressAndCompare(source, expected);            
        }

        [Test]
        [Description("PK to look at")]
        public void Comments_Marked_To_Be_Preserved_Are_Retained_In_The_Output()
        {
            // Also was called StringInCommentCssTest, using string-in-comment.css and string-in-comment.css.min
            // Seemed like it was testing two things - see the comment about the next test also - I think we only need one of these
            // and Comments_Marked_To_Be_Preserved_Are_Retained_In_The_Output2 woulg be my bet...BUT
            // not sure wy there is all the extra stuff ie mac, end hiding etc & why the end hiding /**/
            // are preserved but not the words "end hiding"?  Wouldn't expect either to be preserved....  Do we need another test?

            // Arrange
            const string source = @"/* te "" st */
                                    a{a:1}
                                    /*!""preserve"" me*/
                                    /* quite "" quote ' \' \"" */
                                    /* ie mac \*/
                                    c {c : 3}
                                    /* end hiding */";
            const string expected = @"a{a:1}/*!""preserve"" me*//*\*/c{c:3}/**/";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("PK to look at")]
        public void Comments_Marked_To_Be_Preserved_Are_Retained_In_The_Output2()
        {
            // From what was originally called SpecialCommentsCssTest, using "special-comments.css" and "special-comments.css.min"
            // Does this test add anything that the test above (which also checks preserved comments) doesn't do?
            // It is clearer re: !preserved comments, so I would keep this for preserved comments and ditch the otherone, or have
            // the other one test something else (see my comment re: special chars

            // Arrange
            const string source = @"/*!************88****
                                     Preserving comments
                                        as they are
                                     ********************
                                     Keep the initial !
                                     *******************/
                                    #yo {
                                        ma: ""ma"";
                                    }
                                    /*!
                                    I said
                                    pre-
                                    serve! */";

            const string expected = @"/*!************88****
                                     Preserving comments
                                        as they are
                                     ********************
                                     Keep the initial !
                                     *******************/#yo{ma:""ma""}/*!
                                    I said
                                    pre-
                                    serve! */";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Comments_In_A_Content_Value_Are_Retained_In_The_Output()
        {
            // Arrange
            const string source = @"a{content: ""/* comment in content*/""}";
            const string expected = @"a{content:""/* comment in content*/""}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Webkit_And_Moz_Transform_Origins_Have_Single_0_Replaced_With_Two_0s()
        {
            // Arrange
            const string source = @"a {-webkit-transform-origin: 0;}
                                    b {-webkit-transform-origin: 0 0;}
                                    c {-MOZ-TRANSFORM-ORIGIN: 0 }
                                    d {-MOZ-TRANSFORM-ORIGIN: 0 0;}";
            const string expected = @"a{-webkit-transform-origin:0 0}b{-webkit-transform-origin:0 0}c{-moz-transform-origin:0 0}d{-moz-transform-origin:0 0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void Zeroes_Have_The_Measurement_Type_Removed()
        {
            // Arrange
            const string source = @"a { 
                                      margin: 0px 0pt 0em 0%;
                                      _padding-top: 0ex;
                                      background-position: 0 0;
                                      padding: 0in 0cm 0mm 0pc
                                    }";
            const string expected = @"a{margin:0;_padding-top:0;background-position:0 0;padding:0}";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void When_The_CompressionType_Is_None_The_Input_Is_Returned_Unchanged()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "body      {  color : blue    }  table {   border    :   2 px;    }  /*  Some Comment */";
            target.CompressionType = CompressionType.None;
           
            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(source));
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9529")]
        public void A_Background_Retains_The_Space_Between_The_Colour_And_The_Data_Ur()
        {
            const string source = @"
                ui-widget-shadow { 
                    margin: -5px 0 0 -5px; 
                    padding: 5px; 
                    background: #000000 url(""data:image/png;charset=utf-8;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAABkCAYAAAD0ZHJ6AAAAeUlEQVRoge3OMQHAIBAAsQf/nlsJDDfAkCjImplvHrZvB04EK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEqx92LQHHRpDUNwAAAABJRU5ErkJggg=="") 50% 50% repeat-x; 
                    opacity: .20;
                    filter:Alpha(Opacity=20);
                    -moz-border-radius: 5px; 
                    -khtml-border-radius: 5px;
                    -webkit-border-radius: 5px; 
                    border-radius: 5px; 
                 }";
            const string expected = @"ui-widget-shadow{margin:-5px 0 0 -5px;padding:5px;background:#000 url(""data:image/png;charset=utf-8;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAABkCAYAAAD0ZHJ6AAAAeUlEQVRoge3OMQHAIBAAsQf/nlsJDDfAkCjImplvHrZvB04EK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEK8FKsBKsBCvBSrASrAQrwUqwEqwEqx92LQHHRpDUNwAAAABJRU5ErkJggg=="") 50% 50% repeat-x;opacity:.20;filter:Alpha(Opacity=20);-moz-border-radius:5px;-khtml-border-radius:5px;-webkit-border-radius:5px;border-radius:5px}";
            
            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [Description("https://yuicompressor.codeplex.com/workitem/11445")]
        public void Calc_Has_Spaces_Preserved_Between_Brackets()
        {
            const string source = @"
                .test {
                    width: calc(100% - (40em + 10px));
                    height: 11px;
                }";
            const string expected = @".test{width:calc(100% - (40em + 10px));height:11px}";

            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        private void CompressAndCompare(string source, string expected)
        {
            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    // ReSharper restore InconsistentNaming
}