//using System.IO;
//using System.Text;
//using Microsoft.Build.Framework;
//using Microsoft.Build.Utilities;
//using NUnit.Framework;
//using Yahoo.Yui.Compressor.MsBuildTask;
//using Yahoo.Yui.Compressor.Tests.TestHelpers;

//namespace Yahoo.Yui.Compressor.Tests
//{
//    // ReSharper disable InconsistentNaming
//    public abstract class CssCompressorTaskTests
//    {
//        private CssCompressorTask target;

//        public class When_PreserveComments_Is_True : CssCompressorTaskTests
//        {
//            [Test]
//            [Description("http://yuicompressor.codeplex.com/workitem/9527")]
//            public void Comments_Are_Preserved()
//            {
//                // Arrange
//                target = GetCompressorFor("bug9527");
//                target.PreserveComments = true;

//                // Act
//                var worked = target.Execute();
//                var compressedCss = File.ReadAllText(target.OutputFile);

//                // Assert
//                Assert.That(worked, Is.True, "Task Didn't work");
//                Assert.That(compressedCss, Is.StringContaining("/* comment */"), compressedCss);
//            }
//        }

//        public class When_PreserveComments_Is_False : CssCompressorTaskTests
//        {
//            [SetUp]
//            public void Setup()
//            {
//                // Arrange
//                target = GetCompressorFor("bug9527");
//                target.PreserveComments = false;
//            }

//            [Test]
//            [Description("http://yuicompressor.codeplex.com/workitem/9527")]
//            public void Comments_Are_Removed()
//            {
//                // Act
//                var worked = target.Execute();
//                var compressedCss = File.ReadAllText(target.OutputFile);

//                // Assert
//                Assert.That(worked, Is.True, "Task Didn't work");
//                Assert.That(compressedCss, Is.Not.StringContaining("/* comment */"), compressedCss);
//            }

//            /// <summary>
//            /// There is a specific IE7 hack to preserve these, so just test it works ;)
//            /// </summary>
//            [Test]
//            public void Empty_Comments_After_Child_Selectors_Are_Not_Removed()
//            {
//                // Act.
//                var worked = target.Execute();
//                var compressedCss = File.ReadAllText(target.OutputFile);

//                // Assert.
//                Assert.That(worked, Is.True, "Task Didn't work");
//                Assert.That(compressedCss, Is.StringContaining("/**/"), compressedCss);
//            }

//            [Test]
//            public void Comments_Marked_To_Be_Preserved_Are_Not_Removed()
//            {
//                // Act.
//                var worked = target.Execute();
//                var compressedCss = File.ReadAllText(target.OutputFile);

//                // Assert.
//                Assert.That(worked, Is.True, "Task Didn't work");
//                Assert.That(compressedCss, Is.StringContaining("/*! preserved comment */"), compressedCss);
//            }
//        }

//        [TestFixture]
//        public class In_Testing_The_Effects_Of_The_Compression_Type_Value : CssCompressorTaskTests
//        {
//            [SetUp]
//            public void Setup()
//            {
//                // Arrange
//                target = CreateCompressorTask();
//                target.SourceFiles = new ITaskItem[]
//                    {
//                        new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css"),
//                        new TaskItem(@"Cascading Style Sheet Files\SampleStylesheet1.css")
//                    };
//            }

//            [Test]
//            public void When_The_CompressionType_Is_None_The_Input_Files_Are_Concatenated_Unchanged()
//            {
//                // Arrange
//                target.CompressionType = CompressionType.None.ToString();
//                target.OutputFile = "noCompression.css";

//                // Act
//                target.Execute();

//                // Assert
//                var actual = File.ReadAllText("noCompression.css");
//                var sb = new StringBuilder();
//                foreach (var file in target.SourceFiles)
//                {
//                    sb.Append(File.ReadAllText(file.ItemSpec));
//                }
//                Assert.That(actual, Is.EqualTo(sb.ToString()));
//            }

//            [Test]
//            public void When_The_CompressionType_Is_Not_Specified_The_Input_Files_Are_Compressed()
//            {
//                // Arrange
//                target.OutputFile = "compressed.css";

//                // Act
//                target.Execute();

//                // assert
//                var actual = File.ReadAllText("compressed.css");
//                var sb = new StringBuilder();
//                foreach (var file in target.SourceFiles)
//                {
//                    sb.AppendLine(File.ReadAllText(file.ItemSpec));
//                }
//                Assert.That(actual.Length, Is.LessThan(sb.Length));
//            }

//            [Test]
//            public void When_Compressions_Type_Is_Overridden_On_An_Individual_Item_It_Takes_Precedence_Over_The_Task_Compression_Type()
//            {
//                // Arrange
//                target.CompressionType = CompressionType.None.ToString();
//                target.SourceFiles[0].SetMetadata("CompressionType", CompressionType.Standard.ToString());
//                target.OutputFile = "semicompressed.css";

//                // Act
//                target.Execute();

//                // Assert
//                var actual = File.ReadAllText("semicompressed.css");
//                var sb = new StringBuilder();
//                foreach (var file in target.SourceFiles)
//                {
//                    sb.Append(File.ReadAllText(file.ItemSpec));
//                }
//                Assert.That(actual.Length, Is.LessThan(sb.Length));
//            }
//        }

//        private static CssCompressorTask GetCompressorFor(string fileName)
//        {
//            var compressorTask = CreateCompressorTask();
//            if (!string.IsNullOrEmpty(fileName))
//            {
//                compressorTask.SourceFiles = new ITaskItem[]
//                    { new TaskItem(@"Cascading Style Sheet Files\" + fileName + ".css") };
//                compressorTask.OutputFile = fileName + "min.css";
//            }
//            return compressorTask;
//        }

//        private static CssCompressorTask CreateCompressorTask()
//        {
//            return new CssCompressorTask
//                {
//                    BuildEngine = new BuildEngineStub(),
//                    DeleteSourceFiles = false,
//                    CompressionType = "Standard",
//                    EncodingType = "Default",
//                    LoggingType = "None",
//                    PreserveComments = false
//                };
//        }
//    }
//    // ReSharper restore InconsistentNaming
//}