#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Globalization

#End Region

#Region " Playstation 3 "

Namespace Platforms

    ''' <summary>
    ''' Playstation 3 platform.
    ''' </summary>
    Friend NotInheritable Class Playstation3 : Inherits PlatformBaseWithOnlineStore

#Region " IPlatform Properties "

        ''' <summary>
        ''' Gets the <see cref="Global.PlatformInfo"/> object representing this platform.
        ''' </summary>
        Friend Overrides ReadOnly Property PlatformInfo As New PlatformInfo With {
            .Name = "PlayStation 3",
            .BaseUrl = New Uri("https://gamefaqs.gamespot.com/ps3")
        }

        ''' <summary>
        ''' Gets the Markdown header to use for this platform when building the MD file.
        ''' </summary>
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
    |General Games|Cancelled or Not yet released Games
    |[PlayStation Store Games]({Me.PlatformInfo.BaseUrl}/category/49-miscellaneous?dist=17)|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    |[Party / Minigames]({Me.PlatformInfo.BaseUrl}/category/181-miscellaneous-party-minigame)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)

 - The items in the following table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The items in the following table are made up of useful hyperlinks pointing to the game entry, release date information, and genre categories from Gamefaqs website.

-----------------------------"

#End Region

        '#Region " Additional Games List Properties "

        '        ''' <summary>
        '        ''' Gets the platform exclusive store games.
        '        ''' <para></para>
        '        ''' Note: You must call method <see cref="DoScrap"/> to initialize the value of this property.
        '        ''' </summary>
        '        Friend ReadOnly Property ExclusiveStoreGames As List(Of GameInfo)
        '            <DebuggerStepThrough>
        '            Get
        '                Me.FailIfScrapNotCompleted()
        '                Return Me.exclusiveStoreGames_
        '            End Get
        '        End Property
        '        ''' <summary>
        '        ''' ( Backing Field )
        '        ''' <para></para>
        '        ''' The platform exclusive store games.
        '        ''' </summary>
        '        Private exclusiveStoreGames_ As List(Of GameInfo)

        '#End Region

        '#Region " IPlatform Methods "

        '        ''' <summary>
        '        ''' Does the scraping of platform exclusive games for this platform.
        '        ''' <para></para>
        '        ''' Calling this method will initialize the following properties with the scraped items: 
        '        ''' <para></para> - <see cref="ExclusiveGames"/> 
        '        ''' <para></para> - <see cref="ExclusiveStoreGames"/> 
        '        ''' <para></para> - <see cref="ExclusiveCompilations"/>
        '        ''' </summary>
        '        <DebuggerStepperBoundary>
        '        Friend Sub DoScrap() Implements IPlatform.DoScrap
        '            Me.scrapCompleted = False

        '            ' Scrap Exclusive Games list.
        '            Me.exclusiveGames_ =
        '                GamefaqsUtil.ScrapExclusiveGames("Playstation 3 Games", Me.PlatformInfo,
        '                                                 Me.PlatformInfo.AllGamesUrl)

        '            ' Scrap Exclusive Store Games list.
        '            Me.exclusiveStoreGames_ =
        '                GamefaqsUtil.ScrapExclusiveGames("Playstation Store PS3 Games", Me.PlatformInfo,
        '                                                 New Uri($"{Me.PlatformInfo.AllGamesUrl}?dist=17"))

        '            Dim storeGamesUrls As Uri() =
        '                (From gameinfo As GameInfo In Me.exclusiveStoreGames_
        '                 Select gameinfo.EntryUrl).ToArray()

        '            ' Filter out Exclusive Store Games from Exclusive Games list.

        '            Me.exclusiveGames_ =
        '                (From gameinfo As GameInfo In Me.exclusiveGames_
        '                 Where Not storeGamesUrls.Contains(gameinfo.EntryUrl)).ToList()

        '            ' Build Exclusive Compilations list.

        '            Me.exclusiveCompilations_ =
        '                (From gameinfo As GameInfo In (Me.exclusiveGames_.Concat(Me.exclusiveStoreGames_))
        '                 Where gameinfo.Genre.Contains("Compilation")).ToList()

        '            Dim compilationUrls As Uri() =
        '                (From gameinfo As GameInfo In Me.exclusiveCompilations_
        '                 Select gameinfo.EntryUrl).ToArray()

        '            ' Filter out Exclusive Compilations from Exclusive Games list.

        '            Me.exclusiveGames_ =
        '                (From gameinfo As GameInfo In Me.exclusiveGames_
        '                 Where Not compilationUrls.Contains(gameinfo.EntryUrl)).ToList()

        '            ' Filter out Exclusive Compilations from Exclusive Store Games list.

        '            Me.exclusiveStoreGames_ =
        '                (From gameinfo As GameInfo In Me.exclusiveGames_
        '                 Where Not compilationUrls.Contains(gameinfo.EntryUrl)).ToList()

        '            Console.WriteLine("")
        '            Console.WriteLine($"Scraping completed for {Me.PlatformInfo.Name} platform.")
        '            Console.WriteLine("")

        '            Me.scrapCompleted = True
        '        End Sub

        '        ''' <summary>
        '        ''' Creates the markdown file.
        '        ''' </summary>
        '        <DebuggerStepperBoundary>
        '        Friend Sub CreateMarkdownFile() Implements IPlatform.CreateMarkdownFile
        '            Me.FailIfScrapNotCompleted()
        '            Dim gamesTable As String = GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_)
        '            Dim storeGamesTable As String = GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Playstation Store Games", Me.exclusiveStoreGames_)
        '            Dim compilationsTable As String = GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_)
        '            GamefaqsUtil.WriteMarkdownFile(Me.PlatformInfo.Name, Me.MarkdownHeader, gamesTable, storeGamesTable, compilationsTable)
        '        End Sub

        '        ''' <summary>
        '        ''' Creates the URL files.
        '        ''' </summary>
        '        <DebuggerStepperBoundary>
        '        Friend Sub CreateUrlFiles() Implements IPlatform.CreateUrlFiles
        '            Me.FailIfScrapNotCompleted()
        '            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_)
        '            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Playstation Store Games", Me.exclusiveStoreGames_)
        '            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_)
        '        End Sub

        '#End Region

    End Class

End Namespace

#End Region
