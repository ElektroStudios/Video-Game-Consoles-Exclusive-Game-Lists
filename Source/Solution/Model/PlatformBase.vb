#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices

#End Region

#Region " PlatformBase"

''' <summary>
''' Base class used to implement platforms.
''' </summary>
Friend MustInherit Class PlatformBase : Implements IPlatform, IDisposable

#Region " Fields "

    ''' <summary>
    ''' Flag to determine whether method <see cref="PlatformBase.DoScrap"/> was called.
    ''' </summary>
    Protected scrapCompleted As Boolean

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the Markdown table of filtered (included, excluded) kind of titles.
    ''' </summary>
    Protected MustOverride ReadOnly Property MarkdownFiltersTable As String

#End Region

#Region " IPlatform Properties "

    ''' <summary>
    ''' Gets the <see cref="Global.PlatformInfo"/> object representing this platform.
    ''' </summary>
    Friend MustOverride ReadOnly Property PlatformInfo As PlatformInfo Implements IPlatform.PlatformInfo

    ''' <summary>
    ''' Gets the Markdown header to use for this platform when building the MD file of exclusive game titles.
    ''' </summary>
    Friend ReadOnly Property MarkdownHeaderForExclusiveTitles As String
        <DebuggerStepThrough>
        Get
            Return $"# List of exclusive {Me.PlatformInfo?.Name} titles.

> *Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))}*

_Platform exclusivity refers to the status of a video game being developed for and released only on the specified platform._

-----------------------------

 - The following table of platform exclusive games was generated programmatically by scraping content from Gamefaqs website: 

    - {Me.PlatformInfo?.AllGamesUrl}

    - We use a filter to arbitrarily include and exclude platform exclusive content:

      {Me.MarkdownFiltersTable}

 - The items in the table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The cell entries are made up of useful hyperlinks pointing to the game entry, release date information and genre categories from Gamefaqs website.

-----------------------------"
        End Get
    End Property

    ''' <summary>
    ''' Gets the Markdown header to use for this platform when building the MD file of multi-platform game titles.
    ''' </summary>
    Friend ReadOnly Property MarkdownHeaderForMultiPlatformTitles As String
        <DebuggerStepThrough>
        Get
            Return $"# List of multi-platform {Me.PlatformInfo?.Name} titles.

> *Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))}*

_Multi-platform refers to the status of a video game being developed and released on multiple platforms._

-----------------------------

 - The following table of multi-platform games was generated programmatically by scraping content from Gamefaqs website: 

    - {Me.PlatformInfo?.AllGamesUrl}

 - The items in the table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The cell entries are made up of useful hyperlinks pointing to the game entry, release date information and genre categories from Gamefaqs website.

-----------------------------"
        End Get
    End Property

#End Region

#Region " Game Lists Properties "

    ''' <summary>
    ''' Gets the games that were released exclusively on this platform.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBase.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveGames As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The games that were released exclusively on this platform.
    ''' </summary>
    Protected exclusiveGames_ As List(Of GameInfo)

    ''' <summary>
    ''' Gets the multi-platform games that were released on this platform.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBase.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property MultiplatformGames As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.multiPlatformGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The multi-platform games that were released on this platform.
    ''' </summary>
    Protected multiPlatformGames_ As List(Of GameInfo)

    ''' <summary>
    ''' Gets the game compilations that were released exclusively on this platform.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBase.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveCompilations As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveCompilations_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The game compilations that were released exclusively on this platform.
    ''' </summary>
    Protected exclusiveCompilations_ As List(Of GameInfo)

    ''' <summary>
    ''' Gets the multi-platform game compilations that were released on this platform.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBase.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property MultiPlatformCompilations As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.multiPlatformCompilations_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The multi-platform game compilations that were released on this platform.
    ''' </summary>
    Protected multiPlatformCompilations_ As List(Of GameInfo)

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of this platform class.
    ''' </summary>
    <DebuggerNonUserCode>
    Friend Sub New()
    End Sub

#End Region

#Region " IPlatform Methods "

    ''' <summary>
    ''' Performs the web-scraping of game entries for this platform.
    ''' <para></para>
    ''' Calling this method will initialize the following properties with the scraped items: 
    ''' <para></para> - <see cref="ExclusiveGames"/> 
    ''' <para></para> - <see cref="ExclusiveCompilations"/>
    ''' <para></para> - <see cref="MultiplatformGames"/> 
    ''' <para></para> - <see cref="MultiplatformCompilations"/>
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Overridable Sub DoScrap() Implements IPlatform.DoScrap
        Me.scrapCompleted = False

        ' Scrap exclusive and multi-platform games.
        GamefaqsUtil.ScrapGames($"{Me.PlatformInfo.Name} Games", Me.PlatformInfo,
Me.PlatformInfo.AllGamesUrl,
                                Me.exclusiveGames_, Me.multiPlatformGames_)

        ' Build Exclusive Compilations list.
        Me.exclusiveCompilations_ =
            (From game As GameInfo In Me.exclusiveGames_
             Where game.Genre.Contains("Compilation")
             Order By game.Title
            )?.ToList()

        ' Filter out Exclusive Compilations from Exclusive Games list.
        If exclusiveCompilations_?.Any() Then
            Dim exclusiveCompilationUrls As Uri() =
                (From game As GameInfo In Me.exclusiveCompilations_
                 Select game.EntryUrl
                ).ToArray()

            Me.exclusiveGames_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where Not exclusiveCompilationUrls.Contains(game.EntryUrl)
                 Order By game.Title
                ).ToList()
        End If

        ' Build Multi-platform Compilations list.
        Me.multiPlatformCompilations_ =
            (From game As GameInfo In Me.multiPlatformGames_
             Where game.Genre.Contains("Compilation")
             Order By game.Title
            )?.ToList()

        ' Filter out Multi-platform Compilations from Multi-platform Games list.
        If multiPlatformCompilations_?.Any() Then
            Dim multiPlatformCompilationUrls As Uri() =
                (From game As GameInfo In Me.multiPlatformCompilations_
                 Select game.EntryUrl
                ).ToArray()

            Me.multiPlatformGames_ =
                (From game As GameInfo In Me.multiPlatformGames_
                 Where Not multiPlatformCompilationUrls.Contains(game.EntryUrl)
                 Order By game.Title
                ).ToList()
        End If

        Console.WriteLine("")
        Console.WriteLine($"Scraping completed for {Me.PlatformInfo.Name} platform.")
        Console.WriteLine("")

        Me.scrapCompleted = True
    End Sub

    ''' <summary>
    ''' Creates the markdown file and writes the tables with the scraped games.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overridable Sub CreateMarkdownFile() Implements IPlatform.CreateMarkdownFile
        Me.FailIfScrapNotCompleted()

        Dim platformName As String = Me.PlatformInfo.Name

        ' Write exclusive titles.

        Dim exclusiveGamesTable As String =
            If(Me.exclusiveGames_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Exclusive Games",
                                               Me.exclusiveGames_), "")

        Dim exclusiveCompilationsTable As String =
            If(Me.exclusiveCompilations_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Exclusive Compilations",
                                               Me.exclusiveCompilations_), "")

        GamefaqsUtil.CreateMarkdownFile(platformName, $"{platformName} (Exclusives)", Me.MarkdownHeaderForExclusiveTitles,
                                        exclusiveGamesTable, exclusiveCompilationsTable)

        ' Write multi-platform titles.

        Dim multiPlatformGamesTable As String =
            If(Me.multiPlatformGames_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Multi-platform Games",
                                               Me.multiPlatformGames_), "")

        Dim multiPlatformCompilationsTable As String =
            If(Me.multiPlatformCompilations_.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{platformName}∶ Multi-platform Compilations",
                                               Me.multiPlatformCompilations_), "")

        GamefaqsUtil.CreateMarkdownFile(platformName, $"{platformName} (Multi-platform)", Me.MarkdownHeaderForMultiPlatformTitles,
                                        multiPlatformGamesTable, multiPlatformCompilationsTable)
    End Sub

    ''' <summary>
    ''' Creates the URL files for each scraped game.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overridable Sub CreateUrlFiles() Implements IPlatform.CreateUrlFiles
        Me.FailIfScrapNotCompleted()

        Dim platformName As String = Me.PlatformInfo.Name

        ' Create Url files from exclusive titles.
        If Me.exclusiveGames_.Any() Then
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Exclusive Games", Me.exclusiveGames_)
        End If
        If Me.exclusiveCompilations_.Any() Then
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Exclusive Compilations", Me.exclusiveCompilations_)
        End If

        ' Create Url files from multi-platform titles.
        If Me.multiPlatformGames_.Any() Then
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Multi-platform Games", Me.multiPlatformGames_)
        End If
        If Me.multiPlatformCompilations_.Any() Then
            GamefaqsUtil.CreateUrlFiles(platformName, $"{platformName}∶ Multi-platform Compilations", Me.multiPlatformCompilations_)
        End If
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Throws a exception of type <see cref="InvalidOperationException"/> 
    ''' if method <see cref="PlatformBase.DoScrap"/> was not called previously.
    ''' </summary>
    ''' <param name="callerName">
    ''' Optional. The method or property name of the caller to this method.
    ''' </param>
    <DebuggerStepThrough>
    Protected Sub FailIfScrapNotCompleted(<CallerMemberName> Optional callerName As String = Nothing)

        If Not Me.scrapCompleted Then

            Dim callerProperty As PropertyInfo =
                Me.GetType().GetProperty(callerName, BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)

            Dim errorMsg As String =
                If(callerProperty IsNot Nothing,
                   $"You must call method '{NameOf(DoScrap)}' to initialize the value of property: {callerProperty.Name}",
                   $"You must call method '{NameOf(DoScrap)}' before calling method: {callerName}")

            Throw New InvalidOperationException(errorMsg)

        End If

    End Sub

#End Region

#Region " IDisposable Implementation "

    ''' <summary>
    ''' Flag used by method <see cref="PlatformBase.Dispose"/> to avoid redundant calls.
    ''' </summary>
    Protected disposedValue As Boolean

    ''' <summary>
    ''' Releases all resources used by this instance.
    ''' </summary>
    ''' <param name="disposing">
    ''' <see langword="True"/> to release both managed and unmanaged resources;
    ''' <see langword="False"/> to release only unmanaged resources.
    ''' </param>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.exclusiveGames_?.Clear()
                Me.exclusiveCompilations_?.Clear()
                Me.multiPlatformGames_?.Clear()
                Me.multiPlatformCompilations_?.Clear()

                Me.exclusiveGames_ = Nothing
                Me.exclusiveCompilations_ = Nothing
                Me.multiPlatformGames_ = Nothing
                Me.multiPlatformCompilations_ = Nothing
            End If
            Me.disposedValue = True
        End If
    End Sub

    ''' <summary>
    ''' Releases all resources used by this instance.
    ''' </summary>
    Friend Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class

#End Region
