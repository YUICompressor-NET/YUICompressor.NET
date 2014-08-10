using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Optimization;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using NUnit.Framework;
using Shouldly;
using Yahoo.Yui.Compressor.Web.Optimization;

namespace Yahoo.Yui.Compressor.Tests
{
    [TestFixture]
    public class YuiCompressorTransformTests
    {
        [Test]
        public void GivenAJavascriptFile_Process_ReturnsABundledResponse()
        {
            // Arrange.
            var compressorConfig = new JavaScriptCompressorConfig();
            var transform = new YuiCompressorTransform(compressorConfig);
            var contextBase = A.Fake<HttpContextBase>();
            var bundles = new BundleCollection();
            var javascriptContent = File.ReadAllText("Javascript Files\\jquery-1.10.2.js");
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(javascriptContent));
            var fakeStream = A.Fake<Stream>(x => x.Wrapping(memoryStream));
            var fakeVirtualFile = A.Fake<VirtualFile>(x => x.WithArgumentsForConstructor(new[] { "/Scripts/jquery-1.10.2.js" }));
            fakeVirtualFile.CallsTo(x => x.Open()).Returns(fakeStream);

            
            var bundleFiles = new List<BundleFile>
            {
                new BundleFile("/Scripts/jquery-1.10.2.js", fakeVirtualFile)
            };
            var bundleContext = new BundleContext(contextBase, bundles, "~/bundles/jquery");
            var bundleResponse = new BundleResponse(null, bundleFiles);

            // Act.
            transform.Process(bundleContext, bundleResponse);

            // Assert.
            bundleResponse.ShouldNotBe(null);
            bundleResponse.Content.Substring(0, 300).ShouldBe("/*\n * jQuery JavaScript Library v1.10.2\n * http://jquery.com/\n *\n * Includes Sizzle.js\n * http://sizzlejs.com/\n *\n * Copyright 2005, 2013 jQuery Foundation, Inc. and other contributors\n * Released under the MIT license\n * http://jquery.org/license\n *\n * Date: 2013-07-03T13:48Z\n */\n(function(bW,bU){v");
            bundleResponse.Content.Length.ShouldBe(105397);
        }
    }
}