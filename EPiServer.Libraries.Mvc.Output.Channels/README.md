# Custom output channels with MVC for EPiServer 7. 

By Jeroen Stemerdink

## About

Using the Output channels within MVC requires a slightly different approach than with WebForms.

So, I created a few ```Controllers```

* ```JsonController``` handles the output for JSON.
* ```XmlController``` handles the output for XML.
* ```TxtController``` handles the output for TXT.
* ```PdfController``` handles the output for PDF.

The Channels get initialized within an ```InitializationModule```.

## Get started

This module depends on [Enable different outputs for PageData](../EPiServer.Libraries.Output/README.md) and This module depends on [Enable different outputs for PageData](../EPiServer.Libraries.Output.Channels/README.md).

## Code specifics

For the basics of display channels read the [SDK](http://sdkbeta.episerver.com/SDK-html-Container/?path=/SdkDocuments/CMS/7/Knowledge%20Base/Developer%20Guide/Content/DisplayChannels.htm&vppRoot=/SdkDocuments//CMS/7/Knowledge%20Base/Developer%20Guide/).

The controllers are in fact a "catch-all", it works for all PageData for the specified Channel.

## Requirements

* EPiServer 7
* log4net
* .Net 4.0
* HtmlAgilityPack
* iTextSharp
* ExCSS
* Newtonsoft.Json
* EPiServer.Libraries.Output
* EPiServer.Libraries.Output,Channels

## Deploy

* Compile the project. 
* Drop the dll in the bin.
