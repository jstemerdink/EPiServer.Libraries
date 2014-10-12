# Add keywords to your page through a provider.

By Jeroen Stemerdink

## About

Analyze content on your page (marked Searchable), including used blocks, and add them to your keywords property,
which you need to mark with the ```[KeywordsMetaTag]``` attribute.

## NOTE
The service you want to use needs to be injected.
You can use either the Alchemy provider I created in ```EPiServer.Libraries.SEO.Alchemy```. 
Or write your own for the service you would like to use. In that case you will need to implement  ```IExtractionService``` and add the following attribute to your class ```[ServiceConfiguration(typeof(IExtractionService))]``` 


## Requirements

* EPiServer >= 7.7.1
* log4net
* .Net 4.0

## Deploy

* Compile the project.
* Drop the dll in the bin.
