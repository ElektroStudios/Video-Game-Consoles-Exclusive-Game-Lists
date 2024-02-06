#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Globalization

#End Region

#Region " GameBoy "

Namespace Platforms

    ''' <summary>
    ''' Nintendo Game Boy platform.
    ''' </summary>
    Friend NotInheritable Class GameBoy : Inherits PlatformBase ': Implements IPlatform

#Region " IPlatform Properties "

        Friend Overrides ReadOnly Property PlatformInfo As New PlatformInfo With {
            .Name = "Game Boy",
            .BaseUrl = New Uri("https://gamefaqs.gamespot.com/gameboy")
        }

        Friend Overrides ReadOnly Property MarkdownHeader As String =
$"# List of exclusive {Me.PlatformInfo.Name} titles.

> *Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))}*

_Platform exclusivity refers to the status of a video _game_ being developed for and released only on the specified platform._

-----------------------------

 - The following table of platform exclusive games was generated programmatically by scraping content from Gamefaqs website: 

    - {Me.PlatformInfo.AllGamesUrl}

 - We use a filter to arbitrarily include and exclude platform exclusive content:

    |Included:|Excluded:|
    |:--|:--|
    |Games|Cancelled or Not yet released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)

 - The items in the following table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The items in the following table are made up of useful hyperlinks pointing to the game entry, release date information, and genre categories from Gamefaqs website.

-----------------------------"

#End Region

    End Class

End Namespace

#End Region
