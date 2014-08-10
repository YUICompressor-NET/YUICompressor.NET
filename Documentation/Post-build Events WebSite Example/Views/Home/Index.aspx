<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="SampleWebSite.Extensions" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<style>
    pre 
    {
        font-size:10pt
    }
</style>

    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
        Build and run this example in Debug mode and, if you examine the source to this page, you should see entries (near the bottom of the page) for:
<pre>
    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">script</span> <span style="color:Red;">type</span><span style="color:Blue;">=</span><span style="color:Blue;">"text/javascript"</span> <span style="color:Red;">src</span><span style="color:Blue;">=</span><span style="color:Blue;">"../../Scripts/jquery-1.3.2.js"</span> <span style="color:Blue;">/&gt;</span>
    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">script</span> <span style="color:Red;">type</span><span style="color:Blue;">=</span><span style="color:Blue;">"text/javascript"</span> <span style="color:Red;">src</span><span style="color:Blue;">=</span><span style="color:Blue;">"../../Scripts/MicrosoftAjax.debug.js"</span> <span style="color:Blue;">/&gt;</span>
    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">script</span> <span style="color:Red;">type</span><span style="color:Blue;">=</span><span style="color:Blue;">"text/javascript"</span> <span style="color:Red;">src</span><span style="color:Blue;">=</span><span style="color:Blue;">"../../Scripts/MicrosoftMvcAjax.debug.js"</span> <span style="color:Blue;">/&gt;</span>
</pre>
        but build and run it in Release mode and you should see:
<pre>
    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">script</span> <span style="color:Red;">type</span><span style="color:Blue;">=</span><span style="color:Blue;">"text/javascript"</span> <span style="color:Red;">src</span><span style="color:Blue;">=</span><span style="color:Blue;">"../../Scripts/JavaScript.minified.js"</span> <span style="color:Blue;">/&gt;</span>
</pre>
<p>
The post build event, which you can see in project properties, looks like this:
<pre>
    $(MSBuildBinPath)\msbuild.exe "$(ProjectDir)YUICompress.msbuild" /target:Minimize /p:ConfigurationName=$(ConfigurationName)
</pre>
and if you look at the YUICompress.msbuild file itself, you will see something like this:
<pre>
    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">UsingTask</span> <span style="color:Red;">TaskName</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">JavaScriptCompressorTask</span><span style="color:Black;">"</span> <span style="color:Red;">AssemblyFile</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">..\..\..\MainAssemblies\Yahoo.Yui.Compressor.MsBuildTask.dll</span><span style="color:Black;">"</span> <span style="color:Blue;">/&gt;</span>

    <span style="color:Blue;">&lt;</span><span style="color:#A31515;">Target</span> <span style="color:Red;">Name</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">Minimize</span><span style="color:Black;">"</span> <span style="color:Red;">Condition</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">'$(ConfigurationName)' == 'Release'</span><span style="color:Black;">"</span><span style="color:Blue;">&gt;</span>
        <span style="color:Blue;">&lt;</span><span style="color:#A31515;">ItemGroup</span><span style="color:Blue;">&gt;</span>
          <span style="color:Blue;">&lt;</span><span style="color:#A31515;">JavaScriptFiles</span> <span style="color:Red;">Include</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">Scripts\jquery-1.3.2.min.js</span><span style="color:Black;">"</span><span style="color:Blue;">&gt;</span>
            <span style="color:Blue;">&lt;</span><span style="color:#A31515;">CompressionType</span><span style="color:Blue;">&gt;</span>None<span style="color:Blue;">&lt;/</span><span style="color:#A31515;">CompressionType</span><span style="color:Blue;">&gt;</span>
          <span style="color:Blue;">&lt;/</span><span style="color:#A31515;">JavaScriptFiles</span><span style="color:Blue;">&gt;</span>
            <span style="color:Blue;">&lt;</span><span style="color:#A31515;">JavaScriptFiles</span> <span style="color:Red;">Include</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">Scripts\MicrosoftAjax.js</span><span style="color:Black;">"</span><span style="color:Blue;">/&gt;</span>
            <span style="color:Blue;">&lt;</span><span style="color:#A31515;">JavaScriptFiles</span> <span style="color:Red;">Include</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">Scripts\MicrosoftMvcAjax.js</span><span style="color:Black;">"</span><span style="color:Blue;">/&gt;</span>
        <span style="color:Blue;">&lt;/</span><span style="color:#A31515;">ItemGroup</span><span style="color:Blue;">&gt;</span>

      <span style="color:Blue;">&lt;</span><span style="color:#A31515;">JavaScriptCompressorTask</span>
          <span style="color:Red;">SourceFiles</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">@(JavaScriptFiles)</span><span style="color:Black;">"</span>
          <span style="color:Red;">DeleteSourceFiles</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">false</span><span style="color:Black;">"</span>
          <span style="color:Red;">OutputFile</span><span style="color:Blue;">=</span><span style="color:Black;">"</span><span style="color:Blue;">Scripts\JavaScript.minified.js</span><span style="color:Black;">"</span>
       <span style="color:Blue;">/&gt;</span>
    <span style="color:Blue;">&lt;/</span><span style="color:#A31515;">Target</span><span style="color:Blue;">&gt;</span>
</pre>
A HtmlHelper extension is used on the aspx page to test if you are in debug mode or not, and the condition on the build target above is there to ensure it is only called when you build in Release mode - you can remove this if you wish to use the minified files in Debug mode also.
</p>
You can create multiple item groups and make multiple calls to the compressor if you wish to have different groups of scripts and different includes for individual web pages.
<p>
The css compressor works in a similar way - see the YUICompress.msbuild file and Site.Master for an example of this in action.
</p>
<% if (Html.IsInDebugMode())
       {%>
       <script type="text/javascript" src="../../Scripts/jquery-1.3.2.js" />
       <script type="text/javascript" src="../../Scripts/MicrosoftAjax.debug.js" />
       <script type="text/javascript" src="../../Scripts/MicrosoftMvcAjax.debug.js" />
    <% } else {%>
      <script type="text/javascript" src="../../Scripts/JavaScript.minified.js" />
    <% } %>
</asp:Content>
