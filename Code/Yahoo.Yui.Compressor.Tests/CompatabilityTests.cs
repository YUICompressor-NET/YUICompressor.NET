using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Yahoo.Yui.Compressor.Tests
{
    /// <summary>
    /// All the Java YUICompressor tests should run successfully through our compressors
    /// https://github.com/yui/yuicompressor/tree/master/tests
    /// </summary>
    public abstract class CompatabilityTests
    {
        [TestFixture, Category("Compatability")]
        public class When_Verifying_JavascriptCompressor_Compatability : CompatabilityTests
        {
            [Test]
            public void All_Javascript_Tests_Should_Pass_Succesfully()
            {
                var exclusions = new List<string> {"_syntax_error"};
                ExecuteFileTests(new JavaScriptCompressor(), ".js", exclusions);
            }

            [Test]
            public void The_SyntaxError_Test_File_Should_Be_Close_To_The_Java_Version()
            {
                // Arrange
                var source = File.ReadAllText(@"Compatability Test Files\_syntax_error.js", Encoding.UTF8);
                var expected = File.ReadAllText(@"Compatability Test Files\_syntax_error.js.min");

                // Act
                var actual = new JavaScriptCompressor().Compress(source);

                // Assert
                Assert.That(actual, Is.Not.Null.Or.Empty, "Null Or Empty");
                // Because the Java code uses a Hashtable to determine what variables names can be obfuscated, we can't do an exact file compare. But we can
                // do a file LENGTH compare .. which might be a bit closer to fair Assert test.
                Assert.That(actual.Length, Is.EqualTo(expected.Length), "Length mismatch");
            }
        }

        [TestFixture, Category("Compatability")]
        public class When_Verifying_CssCompressor_Compatabiliity : CompatabilityTests
        {
            [Test]
            public void All_Css_Tests_Should_Pass_Succesfully()
            {
                var exclusions = new List<string>
                                     {
                                         "color-simple",
                                         "color",
                                         "dataurl-base64-linebreakindata",
                                         /*"dataurl-dbquote-font",
                                         "dataurl-nonbase64-doublequotes",
                                         "dataurl-nonbase64-noquotes",
                                         "dataurl-nonbase64-singlequotes",
                                         "dataurl-noquote-multiline-font",
                                         "dataurl-realdata-doublequotes",
                                         "dataurl-realdata-yuiapp",
                                         "dataurl-singlequote-font" */
                                     };
                ExecuteFileTests(new CssCompressor(), ".css", exclusions);
            }
        }

        private void ExecuteFileTests(ICompressor compressor, string extension, List<string> exclusions)
        {
            // Bung the folder name and extension onto each element - makes it easier in testing membership
            // Since we don't have linq in .net 2 land :}
            for (var i = 0; i < exclusions.Count; i++)
            {
                exclusions[i] = "Compatability Test Files\\" + exclusions[i] + extension;
            }

            var sourceFiles = Directory.GetFiles("Compatability Test Files", "*" + extension);
            Assume.That(sourceFiles.Length, Is.GreaterThan(0), "No matching source files found, nothing to test");

            foreach (var file in Directory.GetFiles("Compatability Test Files", "*" + extension))
            {
                Debug.WriteLine("File: " + file);
                if (exclusions.Contains(file))
                {
                    continue;
                }
                var source = File.ReadAllText(file);
                var expected = File.ReadAllText(file + ".min");

                // Some of the test files have a load of carriage returns at the end, so we should strip those out
                while (expected.EndsWith("\n"))
                {
                    expected = expected.Substring(0, expected.Length - 1);
                }

                // Act
                var actual = compressor.Compress(source);

                // Assert
                Assert.That(actual, Is.EqualTo(expected), file);
            }
        }
    }
}
