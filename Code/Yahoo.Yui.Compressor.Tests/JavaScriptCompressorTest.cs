using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using EcmaScript.NET;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests
{
    // ReSharper disable InconsistentNaming

    [TestFixture]
    public class JavaScriptCompressorTest
    {
        private JavaScriptCompressor target;

        [SetUp]
        public void SetUp()
        {
            target = new JavaScriptCompressor();
        }

        [Test]
        public void CompressSampleJavaScript1ReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript1.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }
        
        [Test]
        public void CompressSampleJavaScript2ReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript2.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void A_New_Line_Appended_In_The_Source_Is_Retained_In_The_Output()
        {
            // Arrange.
            const string source = @"fred += '\n'; ";
            const string expected = @"fred+=""\n"";";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void CompressJQuery126VSDocReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.2.6-vsdoc.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
            Assert.That(actual.Length, Is.EqualTo(61591), "Exact Length");
        }

        [Test]
        public void CompressJQuery131JavascriptReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void CompressWithObfuscationTest()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript4.js");

            // Act.
            target.ObfuscateJavascript = false;
            var actualCompressedNotObfuscatedJavascript = target.Compress(source);

            target.ObfuscateJavascript = true;
            var actualCompressedObfuscatedJavascript = target.Compress(source);

            // Assert.
            Assert.That(actualCompressedNotObfuscatedJavascript, Is.Not.Null.Or.Empty, "Not Obfuscated Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actualCompressedNotObfuscatedJavascript.Length), "Not Obfuscated Not Greater");
            Assert.That(actualCompressedObfuscatedJavascript, Is.Not.Null.Or.Empty, "Obfuscated Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actualCompressedObfuscatedJavascript.Length), "Obfuscated Not Greater");

            // Is the obfuscated smaller?
            Assert.That(actualCompressedObfuscatedJavascript.Length, Is.LessThan(actualCompressedNotObfuscatedJavascript.Length), "Obfuscated not smaller than Not Obfuscated");
        }

        [Test]
        public void CompressNestedIdentifiersTest()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript5.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
        }

        [Test]
        public void CompressRegExWithUnicodeTest()
        {
            // Arrange.
            const string source = @"// Helper function used by the dimensions and offset modules
                                    function num(elem, prop) {
                                       return elem[0] && parseInt(jQuery.curCSS(elem[0], prop, true), 10) || 0;
                                   } 
                            
                                   var chars = jQuery.browser.safari && parseInt(jQuery.browser.version) < 417 ? 
                                               ""(?:[\\w*_-]|\\\\.)"" : ""(?:[\\w\u0128-\uFFFF*_-]|\\\\.)"",
                                   quickChild = new RegExp(""^>\\s*("" + chars + ""+)""),
                                   quickID = new RegExp(""^("" + chars + ""+)(#)("" + chars + ""+)""),
                                   quickClass = new RegExp(""^([#.]?)("" + chars + ""*)"");";
            
            // Act.
            target.ObfuscateJavascript = true;
            var compressedJavascript = target.Compress(source); 
            target.ObfuscateJavascript = false;
            var compressedJavascriptNoObfuscation = target.Compress(source);

            // Assert.
            Assert.That(compressedJavascript, Is.Not.StringContaining(@"}get var"));
            Assert.That(compressedJavascript, Is.StringContaining(@"\w\u0128"));
            Assert.That(compressedJavascriptNoObfuscation, Is.StringContaining(@"\w\u0128"));
        }

        [Test]
        public void CompressJQuery131WithNoMungeReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery-1.3.1.js");
            target.ObfuscateJavascript = false;

            // Act.
            var actual = target.Compress(source);
         
            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(source.Length, Is.GreaterThan(actual.Length), "Not Greater");
            Assert.That(actual.Length, Is.EqualTo(71146), "Exact Length");
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/discussions/46679")]
        public void Compressing_An_ExtJs_Definition_Works_As_Expected()
        {
            // Arrange
            const string source = @"controls.SearchCombo = Ext.extend(Ext.form.ComboBox, {
                                        forceSelection: true,
                                        loadingText: 'Searching...',
                                        minChars: 3,
                                        mode: 'remote',
                                        msgTarget: 'side',
                                        queryDelay: 300,
                                        queryParam: 'q',
                                        selectOnFocus: true,
                                        typeAhead: false
                                    }); ";

            const string expected = @"controls.SearchCombo=Ext.extend(Ext.form.ComboBox,{forceSelection:true,loadingText:""Searching..."",minChars:3,mode:""remote"",msgTarget:""side"",queryDelay:300,queryParam:""q"",selectOnFocus:true,typeAhead:false});";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        public void CompressJavascriptIgnoreEvalReturnsCompressedJavascript()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");
            target.IgnoreEval = true;

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(actual, Is.Not.StringContaining("number"), "Turning on ignoreEval should compress functions that call eval");
        }

        [Test]
        public void CompressJavascriptRespectEval()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\SampleJavaScript-ignoreEval.js");
            target.IgnoreEval = false;

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
            Assert.That(actual, Is.StringContaining("number"), "Functions that call eval should not be compressed when ignoreEval is false");
        }

        [Test]
        public void If_CultureInfo_Is_Supplied_Then_The_Original_Thread_Culture_Is_Restored_After_Compression()
        {
            // Arrange
            // Save existing culture
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;

            var expectedCulture = CultureInfo.CreateSpecificCulture("fr-FR");
            try
            {
                // Change the culture to something specific
                Thread.CurrentThread.CurrentCulture = expectedCulture;
                Thread.CurrentThread.CurrentUICulture = expectedCulture;

                // Act
                // Pass in some other culture
                target.ThreadCulture = CultureInfo.CreateSpecificCulture("it-IT");
                target.Compress("var stuff = {foo:0.9, faa:3};");

                // Assert
                // Check the culture is thee sam
                Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(expectedCulture), "Test CurrentCulture");
                Assert.That(Thread.CurrentThread.CurrentUICulture, Is.EqualTo(expectedCulture), "Test CurrentUICulture");
            }
            finally
            {
                // Restore original culture coming into the tests
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
                
                // heck the culture is now restored
                Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(originalThreadCulture), "Original CurrentCulture");
                Assert.That(Thread.CurrentThread.CurrentUICulture, Is.EqualTo(originalThreadUICulture), "Original CurrentUICulture");
            }
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/discussions/243522")]
        public void If_CultureInfo_Is_Supplied_Then_The_Output_Respects_It_Irrespective_Of_The_Current_Thread_Culture()
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string expected = "var stuff={foo:0.9,faa:3};";
            target.ThreadCulture = CultureInfo.InvariantCulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = target.Compress(source); 

                // Assert.
                Assert.That(actual, Is.EqualTo(expected));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9876")]
        public void If_CultureInfo_Is_Not_Supplied_Then_The_Invariant_Culture_Is_Used_And_The_Output_Is_Not_Converted_To_The_Current_Thread_Culture()
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            const string source = "var stuff = {foo:0.9, faa:3};";
            const string expected = "var stuff={foo:0.9,faa:3};";   // This would be 0,9 if we respected the culture

            try
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("it-IT");
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("it-IT");

                // Act
                var actual = target.Compress(source); 

                // Assert.
                Assert.That(actual, Is.EqualTo(expected));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/10107")]
        public void The_Original_Thread_Culture_Is_Restored_If_An_Exception_Is_Thrown()
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            var expectedThreadCulture = CultureInfo.CreateSpecificCulture("it-IT");

            target.ThreadCulture = CultureInfo.CreateSpecificCulture("zh-CN");

            // The following script should throw an exception
            const string source = @"<script type=""text/javascript>alert('hello world');</script>";

            try
            {
                Thread.CurrentThread.CurrentCulture = expectedThreadCulture;
                Thread.CurrentThread.CurrentUICulture = expectedThreadCulture;

                // Act
                target.Compress(source);
            }
            catch (Exception)
            {
                // Assert.
                Assert.That(Thread.CurrentThread.CurrentCulture.LCID, Is.EqualTo(expectedThreadCulture.LCID), "CurrentCulture");
                Assert.That(Thread.CurrentThread.CurrentUICulture.LCID, Is.EqualTo(expectedThreadCulture.LCID), "CurrentUICulture");
                return;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
            Assert.Fail("Exception not thrown");
        }

        [TestCase("sv-SE", "sv-SE")]
        [TestCase("en-US", "sv-SE")]
        [TestCase("sv-SE", "en-US")]
        [TestCase("en-US", "en-US")]
        [TestCase(null, null)]
        [TestCase(null, "sv-SE")]
        [TestCase("sv-SE", null)]
        [TestCase(null, "en-US")]
        [TestCase("en-US", null)]
        public void Accents_Should_Be_Retained_In_All_Cases(string threadCulture, string compressorCulture)
        {
            // Arrange
            var originalThreadCulture = Thread.CurrentThread.CurrentCulture;
            var originalThreadUICulture = Thread.CurrentThread.CurrentUICulture;
            const string source = @"Strings = {
                                        IncorrectLogin: 'Felaktigt användarnamn eller lösenord. Försök igen.'
                                    }";
            const string expected = @"Strings={IncorrectLogin:""Felaktigt användarnamn eller lösenord. Försök igen.""};";

            try
            {
                if (threadCulture != null)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(threadCulture);
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(threadCulture);
                }
                var compressor = new JavaScriptCompressor();
                compressor.Encoding = Encoding.UTF8;
                if (compressorCulture != null)
                {
                    compressor.ThreadCulture = CultureInfo.CreateSpecificCulture(compressorCulture);
                }

                // Act
                var actual = compressor.Compress(source);

                // Assert.
                Assert.That(actual, Is.EqualTo(expected));
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalThreadCulture;
                Thread.CurrentThread.CurrentUICulture = originalThreadUICulture;
            }
        }

        [Test]
        public void The_Output_Is_Obfuscated_When_IsObfuscateJavascript_Is_True()
        {
            // Arrange
            const string source =
                @"(function() {
                    var w = window;
                    w.hello = function(a, abc) {
                    ""a:nomunge"";
                    w.alert(""Hello, "" + a);
                };
            })();";
            const string expected = @"(function(){var a=window;a.hello=function(a,b){a.alert(""Hello, ""+a)}})();";
            target.ObfuscateJavascript = true;

            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void The_Output_Is_Not_Obfuscated_When_IsObfuscateJavascript_Is_False()
        {
            // Arrange
            const string source =
                @"(function() {
                    var w = window;
                    w.hello = function(a, abc) {
                    ""a:nomunge"";
                    w.alert(""Hello, "" + a);
                };
            })();";
            const string expected = @"(function(){var w=window;w.hello=function(a,abc){w.alert(""Hello, ""+a)}})();";
            target.ObfuscateJavascript = false;

            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Concatenated_Strings_Are_Combined()
        {
            // Arrange
            const string source = @"function test(){
                                        var a = ""a"" +
                                        ""b"" +
                                        ""c"";
                                    }";
            const string expected = @"function test(){var a=""abc""};";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/8092")]
        public void Bug8092_Should_Be_Fixed()
        {
            // Arrange
            var source = string.Format("var anObject = {{{0}property: \"value\",{0}propertyTwo: \"value2\"{0}}};{0}{0}alert('single quoted string ' + anObject.property + ' end string');{0}// Outputs: single quoted string value end string", Environment.NewLine);
            const string expected = "var anObject={property:\"value\",propertyTwo:\"value2\"};alert(\"single quoted string \"+anObject.property+\" end string\");";

            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void When_The_CompressionType_Is_None_The_Input_Is_Returned_Unchanged()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            target.CompressionType = CompressionType.None;

            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.EqualTo(source));
        }

        [Test]
        public void The_Input_Will_Be_Compressed_By_Default()
        {
            // Arrange
            // Deliberately include loads of spaces and comments
            const string source = "function   foo() {   return 'bar';   }  /*  Some Comment */";
            const string expected = @"function foo(){return""bar""};";

            // Act & Assert
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9856")]
        public void Decimals_Will_Be_Reasonably_Accurate()
        {
            // Also see http://yuicompressor.codeplex.com/discussions/279118
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just confirming some of the more accurate values

            // Arrange
            const string source = @"var serverResolutions = [ 
                                        156543.03390625,
                                        9783.939619140625, 
                                        611.4962261962891,
                                        0.07464553542435169
                                   ];";
            const string expected = @"var serverResolutions=[156543.03390625,9783.939619140625,611.4962261962891,0.07464553542435169];";

            // Act
            CompressAndCompare(source, expected);
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9856")]
        public void Decimals_Will_Not_Be_Entirely_Accurate_Until_We_Implement_A_Proper_Solution()
        {
            // Also see http://yuicompressor.codeplex.com/discussions/279118
            // There is a problem with ScriptConvert in the EcmaScript library, where doubles are losing accuracy
            // Decimal would be better, but requires major re-engineering.
            // As an interim measure, the accuracy has been improved a little.
            // This test is just checking that some inaccuracies still exist & can be removed once we have a proper solution
            // If this test fails, it means accuracy is now sorted!

            // Arrange
            const string source = @"var serverResolutions = [ 
                                      152.87405654907226,
                                      0.14929107084870338
                                  ];";
            
            // Act
            var actual = target.Compress(source);

            // Assert
            Assert.That(actual, Is.Not.EqualTo("var serverResolutions=[152.87405654907226,0.14929107084870338];"));
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/9856")]
        [Ignore("We have not yet fixed the problem")]
        public void Decimals_Will_Be_Entirely_Accurate_Once_We_Implement_A_Proper_Solution()
        {
            var values = new[] {         
                                    "156543.03390625", 
                                    "78271.516953125",  
                                    "39135.7584765625",         
                                    "19567.87923828125", 
                                    "9783.939619140625",  
                                    "4891.9698095703125",         
                                    "2445.9849047851562", 
                                    "1222.9924523925781",  
                                    "611.4962261962891",         
                                    "305.74811309814453", 
                                    "152.87405654907226",  
                                    "76.43702827453613",         
                                    "38.218514137268066", 
                                    "19.109257068634033",  
                                    "9.554628534317017",         
                                    "4.777314267158508", 
                                    "2.388657133579254",  
                                    "1.194328566789627",         
                                    "0.5971642833948135",  
                                    "0.29858214169740677",  
                                    "0.14929107084870338",         
                                    "0.07464553542435169"    
            };

            var output = string.Format("{0,22} {1,22} {2,22} {3,22}{4}", "Original", "G", "G16", "G17", Environment.NewLine);
            output += string.Format("{0,22} {1,22} {2,22} {3,22}{4}", "===================", "===================", "===================", "===================", Environment.NewLine);
            foreach (var value in values)
            {
                var decVal = decimal.Parse(value);
                string G = decVal.ToString("G");
                string G16 = decVal.ToString("G16");
                string G17 = decVal.ToString("G99");
                output += string.Format("{0,22} {1,22}{4}{2,22}{5}{3,22}{6}{7}", value, G, G16, G17, value == G ? string.Empty : "*", value == G16 ? string.Empty : "*", value == G17 ? string.Empty : "*", Environment.NewLine);
            }

            Console.WriteLine(output);
            const string source = @"var serverResolutions = [         
                                        156543.03390625, 
                                        78271.516953125,  
                                        39135.7584765625,         
                                        19567.87923828125, 
                                        9783.939619140625,  
                                        4891.9698095703125,         
                                        2445.9849047851562, 
                                        1222.9924523925781,  
                                        611.4962261962891,         
                                        305.74811309814453, 
                                        152.87405654907226,  
                                        76.43702827453613,         
                                        38.218514137268066, 
                                        19.109257068634033,  
                                        9.554628534317017,         
                                        4.777314267158508, 
                                        2.388657133579254,  
                                        1.194328566789627,         
                                        0.5971642833948135,  
                                        0.29858214169740677,  
                                        0.14929107084870338,         
                                        0.07464553542435169     
                                        ];";

            var result = target.Compress(source);
            Assert.AreEqual("var serverResolutions=[156543.03390625,78271.516953125,39135.7584765625,19567.87923828125,9783.939619140625,4891.9698095703125,2445.9849047851562,1222.9924523925781,611.4962261962891,305.74811309814453,152.87405654907226,76.43702827453613,38.218514137268066,19.109257068634033,9.554628534317017,4.777314267158508,2.388657133579254,1.194328566789627,0.5971642833948135,0.29858214169740677,0.14929107084870338,0.07464553542435169];", result);
        }

        [Test]
        public void Errors_Will_Include_Line_Numbers()
        {
            // Arrange
            const string source = @"var terminated = 'some string';
                                    var unterminated = 'some other;";

            // Act
            try
            {
                target.Compress(source);
                Assert.Fail("Succeeded");
            }
            catch (EcmaScriptRuntimeException ex)
            {
                // Assert
                Assert.That(ex.LineNumber, Is.EqualTo(2));
            }
        }

        [Test]
        public void Warnings_Will_Include_Line_Numbers_Where_Available()
        {
            // Arrange
            const string source = @"function foo(bar, bar) {}";
            target.LoggingType = LoggingType.Debug;

            // Act
            target.Compress(source);

            // Assert
            var reporter = (CustomErrorReporter) target.ErrorReporter;
            Assert.That(reporter.ErrorMessages.Count, Is.Not.EqualTo(0), "No Messages");

            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("[WARNING] Duplicate parameter name \"bar\""))
                {
                    Assert.That(errorMessage, Is.StringContaining("Line: 1"), "\"Line: 1\" not found in: " + errorMessage);
                    return;
                }
            }
            Assert.Fail("Message not found");
        }

        [Test]
        public void Warnings_Will_Not_Include_Line_Numbers_Where_Not_Available()
        {
            // Arrange
            const string source = @"var foo = 'bar';
                                    var foo = 'bar';";
            target.LoggingType = LoggingType.Debug;

            // Act
            target.Compress(source);

            // Assert
            var reporter = (CustomErrorReporter) target.ErrorReporter;
            Assert.That(reporter.ErrorMessages.Count, Is.Not.EqualTo(0), "No Messages");
            
            foreach (var errorMessage in reporter.ErrorMessages)
            {
                if (errorMessage.Contains("The variable foo has already been declared in the same scope"))
                {
                    Assert.That(errorMessage, Is.Not.StringContaining("Line:"), "\"Line:\" found in: "+ errorMessage);
                    return;
                }
            }
            Assert.Fail("Message not found");
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/10742")]
        [Ignore("Ignored until the ECMAScript library is fixed")]
        public void Compressing_A_File_With_A_Unicode_BOM_Character_At_Start_Of_File_Should_Succeed()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery.caret.1.02_bom_at_start.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
        }

        [Test]
        [Description("http://yuicompressor.codeplex.com/workitem/10742")]
        public void Compressing_A_File_With_A_Unicode_BOM_Character_Not_At_Start_Of_File_Should_Succeed()
        {
            // Arrange.
            var source = File.ReadAllText(@"Javascript Files\jquery.caret.1.02.js");

            // Act.
            var actual = target.Compress(source);

            // Assert.
            Assert.That(actual, Is.Not.Null.Or.Empty, "Null or Empty");
        }

        private void CompressAndCompare(string source, string expected)
        {
            // Act
            target.ObfuscateJavascript = false;
            var actual = target.Compress(source); 

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }

    // ReSharper restore InconsistentNaming
}