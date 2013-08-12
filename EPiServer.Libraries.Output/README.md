# Custom output for EPiServer 7. 

By Jeroen Stemerdink

## About

I wanted to be able to render pages in different ways, just by adding the format to the url.

So, I created a few output types

* ```JsonOutputFormat``` handles the output for JSON.
* ```XmlOutputFormat``` handles the output for XML.
* ```TxtOutputFormat``` handles the output for TXT.
* ```PdfOutputFormat``` handles the output for PDF.

To be able to render the different outputs, I created a ```OutputPartialRouter```, which routes the url segment after the page url.

The router gets initialiazed through an IInitializableModule,```OutputInitialization ```, so no configuration is necessary.

The outputs can be enabled in the ```OutputSettings```.

## Get started

For each property that you want to be rendered in e.g. the xml, add the ```[UseInOutput(Order = 10)]``` attribute to it. You can set the order in which you want the property to be rendered on the attribute.

If you add it to a ```ContentArea``` it will take the properties from the content added to the area that have the ```[UseInOutput]``` attribute.

Make sure you set the order value on the ```[Display]``` attribute, the properties get rendered in that order.

You can add two properties ```PdfHeader``` and ```PdfFooter``` to your startpage if you want a header and footer to be rendered in the pdf.

Enable the outputs you want in the settings, add the location of a print stylesheet for pdf output, if you have one.

Add ```/pdf```, ```/xml```, ```/txt``` or ```/json``` to the url and it gets rendered in the desired format.

## Code specifics

For the basics of partial routing read the [SDK](http://sdkbeta.episerver.com/SDK-html-Container/?path=/SdkDocuments/CMS/7/Knowledge%20Base/Developer%20Guide/Partial%20Routing/Partial%20Routing.htm&vppRoot=/SdkDocuments//CMS/7/Knowledge%20Base/Developer%20Guide/).

The router checks if there is an output segment in the url, checks if the requested output is enabled, renders the output and returns the url without the format.

## Requirements

* EPiServer 7
* log4net
* .Net 4.0
* HtmlAgilityPack
* iTextSharp
* ExCSS
* Newtonsoft.Json

## Deploy

* Compile the project. 
* Drop the dll in the bin.
