# Custom output channels for EPiServer 7. 

By Jeroen Stemerdink

## About

I wanted to be able to render pages in different ways, based on the request header.

So, I created a few ```DisplayChannels```

* ```JsonChannel``` handles the output for JSON.
* ```XmlChannel``` handles the output for XML.
* ```TxtChannel``` handles the output for TXT.
* ```PdfChannel``` handles the output for PDF.

To be able to render the different outputs, I created an  ```OutputTemplatePage```, based on the ```TemplatePage<T>```.

If you already have a custom template page, you can also use the ```UseActiveChannel``` method in the ```OutputHelper``` class.

The channels can be enabled in the ```ChannelSettings```.

## Get started

This module depends on [Enable different outputs for PageData](../EPiServer.Libraries.Output/README.md).

Enable the output channels you want in the settings.

## Code specifics

For the basics of display channels read the [SDK](http://sdkbeta.episerver.com/SDK-html-Container/?path=/SdkDocuments/CMS/7/Knowledge%20Base/Developer%20Guide/Content/DisplayChannels.htm&vppRoot=/SdkDocuments//CMS/7/Knowledge%20Base/Developer%20Guide/).

The channel definition checks if the requested output is enabled and whether the ContentType in the request header containes e.g. ```application/json```. If so, it is marked as active.

The ```UseActiveChannel``` checks if the channel for the ContentType is active. If so, the content gets rendered as requested.

## Requirements

* EPiServer 7
* log4net
* .Net 4.0
* HtmlAgilityPack
* iTextSharp
* ExCSS
* Newtonsoft.Json
* EPiServer.Libraries.Output

## Deploy

* Compile the project. 
* Drop the dll in the bin.
