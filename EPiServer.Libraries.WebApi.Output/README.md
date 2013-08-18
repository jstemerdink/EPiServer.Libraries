# Custom output WebApi for EPiServer 7. 

By Jeroen Stemerdink

## About

I wanted to be able to return page content in a specified format through a WebApi.

So, I created a ```ContentController```

It takes the PageID and an optional language parameter.

## Get started

This module depends on [Enable different outputs for PageData](../EPiServer.Libraries.Output/README.md).


## Code specifics

There are no specifics, just a regular WebApi implementation.

## Requirements

* EPiServer 7
* log4net
* .Net 4.0
* EPiServer.Libraries.Output
* EPiServer.Libraries.Output.Channels

## Deploy

* Compile the project. 
* Drop the dll in the bin.
