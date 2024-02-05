Imports System.Globalization

Public Module MarkdownStrings

    Public ReadOnly ps3Header As String =
$"# List of exclusive Playstation 3 games

> *Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.CreateSpecificCulture("en-US"))}*

_Platform exclusivity refers to the status of a video _game_ being developed for and released only on the specified platform._

-----------------------------

 - The following table of platform exclusive games was generated programmatically by scraping content from input website: 

    - https://gamefaqs.gamespot.com/ps3/category/999-all

 - We use a filter to arbitrarily include and exclude platform exclusive content:

    |Includes|Not Includes|
    |:--|:--|
    |General Games|Cancelled or Not yet released Games
    |[PlayStation Store Games](https://gamefaqs.gamespot.com/ps3/category/49-miscellaneous?dist=17)|[Expansions / DLC Distribution](https://gamefaqs.gamespot.com/ps3/category/999-all?dist=6)
    |[Compilations](https://gamefaqs.gamespot.com/ps3/category/233-miscellaneous-compilation)|[Demo Discs](https://gamefaqs.gamespot.com/ps3/category/280-miscellaneous-demo-disc)
    |[Party / Minigames](https://gamefaqs.gamespot.com/ps3/category/181-miscellaneous-party-minigame)|[Software / Applications](https://gamefaqs.gamespot.com/ps3/category/277-miscellaneous-application)

 - The items in the following table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The items in the following table are made up of useful hyperlinks pointing to the game entry, release date information, and genre categories from input website.

-----------------------------"

End Module
