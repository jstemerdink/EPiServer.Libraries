# A custom localization provider for EPiServer 7. 

By Jeroen Stemerdink

## About

I wanted Editors to be able to change translations on their own, with no fear of loadbalancers / file replication making a mess of things. So it needed to be done within EPiServer itself, not with a plugin in admin mode, but in the content tree.

I created three PageTypes. 

* ```TranslationContainer``` for normal translations
* ```CategoryTranslationContainer``` for translations of Categories (as the xml for those translations are slightly different from a normal translation).
* ```Translation Item``` a translation

To be able to use the translations created within the website, I created a ```TranslationProvider```, which is based on the XmlLocalizationProvider from EPiServer 7.

The provider gets initialiazed through an IInitializableModule,```TranslationProviderInitialization ```, so no configuration is necessary and gets its data from the “TranslationFactory”.

## Get started

In a multi site setup you will need to create the ```TranslationContainer``` underneath the Root.

Otherwise you start with creating a ```TranslationContainer``` underneath the ```StartPage```. 

If you want to put the container somewhere else, you will need to set a property of type ```PageReference``` on the ```StartPage```, and add an atrribute ``` [TranslationContainer]```.
For backwards compatibility you can also still use a ```PageReference``` with the fixed name ```TranslationContainer```.

Create containers or translation items beneath the main translation container. 

A container for category translations is best created directly underneath the main translation container.

## Quirks

A few things you will need to keep in mind.

When you create a translation the ```OriginalText``` is the key, and can only be set on the master language. To keep it simple for the editors just name them normally. 
To reference this, use the name without spaces and special characters, all lower case, this to be in line with the langfiles.

Alternatively you can set the ```ContainerName``` property, which has the ```PageName``` as fallback.

So if the (Container)Name of the container is e.g. ```Jeroen Stemerdink```, the key in the xml will be ```jeroenstemerdink```. The same applies to the translation items.

See ```Bonus``` for a tip on how to get the key in an easy way.

For translations for Categories it is slightly different. You create a ```CategoryTranslationContainer``` beneath the main container, this will trigger a different rendering of the xml.  
You can organize the translations with subcontainers, but in the xml those subcontainers will not be used. 
The value of the ```OriginalText``` property needs to be exactly the same as the name of the ```Category``` you want to translate.

## Bonus

On the “TranslationItem” I have added three properties you can use on a template, if you create one for a translation that is. You will not use it in the site, but it can display a few things using those properties.
* ```MissingValues```:  the languages that have no translation yet.
* ```LookupKey```: the key to use in e.g. the Translate WebControl.
* ```TranslatedValues```:  a dictionary containing the language and the translation.


## Requirements

* EPiServer >= 7.7.1
* log4net
* .Net 4.0

## Deploy

* Compile the project. 
* Drop the dll in the bin.
