# Add block content to the index.

By Jeroen Stemerdink

## About

This week I needed to add Blocks used on a page to EPiServer Search Index as content of the page they were used on.
This is supported in Find, but as we are not using Find in this project, I needed a different solution.

First I added an extra property to my base class to hold the content of my Blocks. 
Note the ScaffoldColumn(false) attribute, which hides it in edit mode.

        [CultureSpecific]
        [ScaffoldColumn(false)]
        [Searchable]
        [AdditionalSearchContent]
        public virtual string SearchText { get; set; }

Next I created an InitializableModule to attach some functionality to the publishing event.

When publishing the page, I loop through all Blocks in the ContentAreas defined on the page.

I get all values from string properties marked ```Searchable``` in the Blocks and add them to the SearchText property on my PageType. 
Note: If for some reason you don't want a property on a Block indexed, just add ```[Seachable(false)]``` to it.
The aggregated content from the blocks is added to the property marked with ```[AdditionalSearchContent]```.

The content of the property marked with ```[AdditionalSearchContent]`` is added to the index and you get results also if the term you are looking for is only used in a Block on the page.

## Requirements

* EPiServer >= 7.7.1
* log4net
* .Net 4.0

## Deploy

* Compile the project.
* Drop the dll in the bin.
