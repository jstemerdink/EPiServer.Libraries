# Attaching ContentProviders programmaticly in EPiServer 7. 

By Jeroen Stemerdink

## About

I needed Editors/Admins to be able to add a ContentProvider whenever and wherever they wanted. Imagine a big container with pages that need to be reused on several sites e.g.
I also needed the provider not to be configured in the episerver.config, because of load balancing considerations.

I decided to use the ```Dynamic Data Store``` to store the settings.
I created a ```ContentProviderAdministration``` plugin to add and remove the providers. 
It uses the ```ClonedContentProvider``` from the ``Alloy``` sample.

The module gets initialiazed through an IInitializableModule,```ClonedContentProviderInitialization```, so no configuration is necessary.

## Requirements

* EPiServer 7
* log4net
* .Net 4.0

## Deploy

* Compile the project. 
* Drop the dll in the bin.
