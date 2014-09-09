##Project Description
This is a .NET port of the Yahoo! UI Library's YUI Compressor Java project. The objective of this project is to compress any Javascript and Cascading Style Sheets to an efficient level that works exactly as the original source, before it was minified.

![CI Status via AppVeyor](https://ci.appveyor.com/api/projects/status/ttirf8q8kpm89a2v) ![NuGet](http://img.shields.io/nuget/dt/yuicompressor.net.svg?syle=flat-square)

##Available via NuGet
[![NuGet Command](http://i.imgur.com/Bkfqq.png)](http://nuget.org/packages/YUICompressor.NET)

[![NuGet Command with MSBuild Task](http://i.imgur.com/aqKIj.png)](http://nuget.org/packages/YUICompressor.NET.MSBuild)

[![NuGet Command with NAnt Task](http://i.imgur.com/xtRJg.png)](http://nuget.org/packages/YUICompressor.NET.NAnt)

[![NuGet Command with Web Optimization](http://i.imgur.com/kKyzt.png)](http://nuget.org/packages/YUICompressor.NET.Web.Optimization)

 
##Main Features

- Compress and/or Bundle Javascript and/or Cascading StyleSheets.
- 1 source file -> 1 destination file (that's just compression)
- Multiple source files -> 1 destination file (compression and bundling)
- Multiple source files -> multiple destination files (so you're pro at this now?!)
- Optional MSBuild Tasks, NAnt Tasks and MVC4 BundleTransform.

##Referenced Version
Based on YUI Compressor version: 2.4.4 (last checked at 2011-01-17).

##How close to the Java Port?
Pretty damn close, now :) *Same unit tests (which pass)*. 
The only thing I haven't been able to do is get the exact same obfuscation because the java code is using a hashtable instead of a sorted hashtable/dictionary. It's identical besides that (and works the same). Just visually look different but is exactly the same file size, etc.

##Video Tutorials

[![Using YUI Compressor .NET (Core)](http://i.imgur.com/9KBgp.png)](http://www.youtube.com/watch?v=LzoYUsKikx0)

[![Using YUI Compressor .NET (MSBuild](http://i.imgur.com/m34Hx.png)](http://www.youtube.com/watch?v=sFFZ0nQog8U)

[![Using YUI Compressor .NET (Web Optimization))](http://i.imgur.com/MQR0h.png)](http://www.youtube.com/watch?v=NSHGSbViMm8)

[![(Older video) Using YUI Compressor .NET 1.7 with MSBuild](http://i.imgur.com/T4ULh.png)](http://www.youtube.com/watch?v=Cj8MHPCubuM)



##Who's utilising this code/library?
If you use this code in some project, please drop us a message so we can include it in this list :)

- [Nancy.BundleIt](https://github.com/donnyv/Nancy.BundleIt) by [DonnyV](https://github.com/donnyv) 
- SquishIt by Justin Etheredge
- .Less (dot-less) by Christopher Owen, Erik van Brakel, Daniel Hoelbling and James Foster
- Shinkansen: compress, crunch, combine, and cache JavaScript and CSS by Milan Negovan
- gStyleManager
- Phil Haack and his T4CSS : A T4 Template for .Less CSS With Compression
- Karl Seguin (from CodeBetter fame)
- StreetAdvisor.com
- FUser.com
- Include-Combiner
- HippoValidator
- Bundle Transformer
- WebMarkupMin


##References
YUI Compressor home page: http://developer.yahoo.com/yui/compressor/

##Thank You's!
We couldn't have made this project without the support of the following software:

[![The best C# refactoring plugin for Visual Studio](http://i.imgur.com/FyAhs.png)](http://www.jetbrains.com/resharper/features/code_refactoring.html)

Please support them buy trying their software. If you like it (like we do!) then purchase it. Thank you :)
