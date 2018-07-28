## Project Description
This is a .NET port of the Yahoo! UI Library's YUI Compressor Java project. The objective of this project is to compress any Javascript and Cascading Style Sheets to an efficient level that works exactly as the original source, before it was minified.

![CI Status via AppVeyor](https://ci.appveyor.com/api/projects/status/ttirf8q8kpm89a2v) ![NuGet](http://img.shields.io/nuget/dt/yuicompressor.net.svg?syle=flat-square)

## Available via NuGet
[![NuGet Command](http://i.imgur.com/Bkfqq.png)](http://nuget.org/packages/YUICompressor.NET)

Package Name: `YUICompressor.NET`  
CLI: `install-package YUICompressor.NET`

[![NuGet Command with MSBuild Task](http://i.imgur.com/aqKIj.png)](http://nuget.org/packages/YUICompressor.NET.MSBuild)

Package Name: `YUICompressor.NET.MSBuild`  
CLI: `install-package YUICompressor.NET.MSBuild`

## Main Features

- Compress and/or Bundle Javascript and/or Cascading StyleSheets.
- 1 source file -> 1 destination file (that's just compression)
- Multiple source files -> 1 destination file (compression and bundling)
- Multiple source files -> multiple destination files (so you're pro at this now?!)
- Optional MSBuild Tasks.

## Previous releases
Prior to release 3.0, this library _also_ consisted of
- NAnt task
- MVC4 BundleTransform.

These have all retired and are now available via referencing older commits/tags in the source code.  
The most recent Tag taken before these libraries were retired is tag [Original-NET40-version](https://github.com/YUICompressor-NET/YUICompressor.NET/tree/Original-NET40-version).

## Referenced Version
Based on YUI Compressor version: 2.4.4 (last checked at 2011-01-17).

## How close to the Java Port?
Pretty old. This library hasn't been checked/updated since about early 2011.

## Video Tutorials

[![Using YUI Compressor .NET (Core)](http://i.imgur.com/9KBgp.png)](http://www.youtube.com/watch?v=LzoYUsKikx0)

[![Using YUI Compressor .NET (MSBuild](http://i.imgur.com/m34Hx.png)](http://www.youtube.com/watch?v=sFFZ0nQog8U)

[![Using YUI Compressor .NET (Web Optimization))](http://i.imgur.com/MQR0h.png)](http://www.youtube.com/watch?v=NSHGSbViMm8)

[![(Older video) Using YUI Compressor .NET 1.7 with MSBuild](http://i.imgur.com/T4ULh.png)](http://www.youtube.com/watch?v=Cj8MHPCubuM)


## References
YUI Compressor home page: http://developer.yahoo.com/yui/compressor/

---
