#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices

#End Region

#Region " PlatformBase"

''' <summary>
''' Base class used to implement platforms.
''' </summary>
Friend MustInherit Class PlatformBase : Implements IPlatform

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
    ''' Gets the Markdown header to use for this platform when building the MD file.
    ''' </summary>
    Friend ReadOnly Property MarkdownHeader As String
        <DebuggerStepThrough>
        Get
            Return $"# List of exclusive {Me.PlatformInfo?.Name} titles.

> *Last updated on {Date.Now.ToString("MMMM d, yyyy", CultureInfo.GetCultureInfo("en-US"))}*

_Platform exclusivity refers to the status of a video _game_ being developed for and released only on the specified platform._

-----------------------------

 - The following table of platform exclusive games was generated programmatically by scraping content from Gamefaqs website: 

    - {Me.PlatformInfo?.AllGamesUrl}

    - We use a filter to arbitrarily include and exclude platform exclusive content:

      {Me.MarkdownFiltersTable}

 - The items in the following table are ordered alphabetically by Title column. If you need to sort items in another column, you may search for custom Markdown plugins / user-scripts or convert this table to a more suitable format, such as CSV or HTML with JavaScript.

 - The items in the following table are made up of useful hyperlinks pointing to the game entry, release date information, and genre categories from Gamefaqs website.

-----------------------------"
        End Get
    End Property

#End Region

#Region " Game Lists Properties "

    ''' <summary>
    ''' Gets the platform exclusive games.
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
    ''' The platform exclusive games.
    ''' </summary>
    Protected exclusiveGames_ As List(Of GameInfo)

    ''' <summary>
    ''' Gets the platform exclusive compilations.
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
    ''' The platform exclusive compilations.
    ''' </summary>
    Protected exclusiveCompilations_ As List(Of GameInfo)

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
    ''' Does the scraping of platform exclusive games for this platform.
    ''' <para></para>
    ''' Calling this method will initialize the following properties with the scraped items: 
    ''' <para></para> - <see cref="ExclusiveGames"/> 
    ''' <para></para> - <see cref="ExclusiveCompilations"/>
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Overridable Sub DoScrap() Implements IPlatform.DoScrap
        Me.scrapCompleted = False

        ' Scrap Exclusive Games list.

        Me.exclusiveGames_ =
                GamefaqsUtil.ScrapExclusiveGames(
                    $"{Me.PlatformInfo.Name} Games", Me.PlatformInfo,
                    Me.PlatformInfo.AllGamesUrl)

        ' Build Exclusive Compilations list.

        Me.exclusiveCompilations_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where game.Genre.Contains("Compilation")
                 Order By game.Title
                )?.ToList()

        If exclusiveCompilations_?.Any() Then
            Dim compilationUrls As Uri() =
                    (From game As GameInfo In Me.exclusiveCompilations_
                     Select game.EntryUrl
                    ).ToArray()

            ' Filter out Exclusive Compilations from Exclusive Games list.

            Me.exclusiveGames_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where Not compilationUrls.Contains(game.EntryUrl)
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

        Dim gamesTable As String =
            If(Me.exclusiveGames_?.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_), "")

        Dim compilationsTable As String =
            If(Me.exclusiveCompilations_?.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_), "")

        GamefaqsUtil.WriteMarkdownFile(Me.PlatformInfo.Name, Me.MarkdownHeader, gamesTable, compilationsTable)
    End Sub

    ''' <summary>
    ''' Creates the URL files for each scraped game.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overridable Sub CreateUrlFiles() Implements IPlatform.CreateUrlFiles
        Me.FailIfScrapNotCompleted()

        If Me.exclusiveGames_?.Any() Then
            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_)
        End If

        If Me.exclusiveCompilations_?.Any() Then
            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_)
        End If
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Throws a exception of type <see cref="InvalidOperationException"/> 
    ''' if method <see cref="PlatformBase.DoScrap"/> was not called previously.
    ''' </summary>
    ''' <param name="callerName">Optional. The method or property name of the caller to this method.</param>
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

End Class

#End Region
