#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

#End Region

#Region " PlatformBaseWithOnlineStore"

''' <summary>
''' Base class used to implement platforms having an online game store.
''' </summary>
Friend MustInherit Class PlatformBaseWithOnlineStore : Inherits PlatformBase

#Region " Properties "

    ''' <summary>
    ''' Gets the id. of the "dist" parameter that points to the online store games list for this paltform.
    ''' <para></para>
    ''' (e.g. the number '17' at the end of this url: 
    ''' <para></para>
    ''' https://gamefaqs.gamespot.com/ps3/category/49-miscellaneous?dist=17,
    ''' <para></para>
    ''' or the number '26' at the end of this url:
    ''' <para></para>
    ''' https://gamefaqs.gamespot.com/ps4/category/49-miscellaneous?dist=26)
    ''' </summary>
    Protected MustOverride ReadOnly Property StoreGamesDistributionId As Integer

    ''' <summary>
    ''' Gets the platform exclusive online store games.
    ''' <para></para>
    ''' Note: You must call method <see cref="PlatformBaseWithOnlineStore.DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveStoreGames As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveStoreGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The platform exclusive store games.
    ''' </summary>
    Protected exclusiveStoreGames_ As List(Of GameInfo)

#End Region

#Region " IPlatform Methods "

    ''' <summary>
    ''' Does the scraping of platform exclusive games for this platform.
    ''' <para></para>
    ''' Calling this method will initialize the following properties with the scraped items: 
    ''' <para></para> - <see cref="ExclusiveGames"/> 
    ''' <para></para> - <see cref="ExclusiveStoreGames"/> 
    ''' <para></para> - <see cref="ExclusiveCompilations"/>
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Overrides Sub DoScrap()

        Me.scrapCompleted = False

        ' Scrap Exclusive Games list.

        Me.exclusiveGames_ =
                GamefaqsUtil.ScrapExclusiveGames(
                    $"{Me.PlatformInfo.Name} Games", Me.PlatformInfo,
                    Me.PlatformInfo.AllGamesUrl)

        ' Scrap Exclusive Store Games list.

        Me.exclusiveStoreGames_ =
                GamefaqsUtil.ScrapExclusiveGames(
                    $"{Me.PlatformInfo.Name} Store Games", Me.PlatformInfo,
                    New Uri($"{Me.PlatformInfo.AllGamesUrl}?dist={Me.StoreGamesDistributionId}"))

        Dim storeGamesUrls As Uri() =
                (From game As GameInfo In Me.exclusiveStoreGames_
                 Select game.EntryUrl
                )?.ToArray()

        ' Filter out Exclusive Store Games from Exclusive Games list.

        Me.exclusiveGames_ =
                (From game As GameInfo In Me.exclusiveGames_
                 Where Not storeGamesUrls.Contains(game.EntryUrl)
                ).ToList()

        ' Build Exclusive Compilations list.

        Me.exclusiveCompilations_ =
                (From game As GameInfo In (Me.exclusiveGames_.Concat(Me.exclusiveStoreGames_))
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

            ' Filter out Exclusive Compilations from Exclusive Store Games list.

            Me.exclusiveStoreGames_ =
                (From game As GameInfo In Me.exclusiveStoreGames_
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
    Friend Overrides Sub CreateMarkdownFile()
        Me.FailIfScrapNotCompleted()

        Dim gamesTable As String =
            If(Me.exclusiveGames_?.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_), "")

        Dim storeGamesTable As String =
            If(Me.exclusiveStoreGames_?.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Games (Online Store)", Me.exclusiveStoreGames_), "")

        Dim compilationsTable As String =
            If(Me.exclusiveCompilations_?.Any(),
               GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_), "")

        GamefaqsUtil.WriteMarkdownFile(Me.PlatformInfo.Name, Me.MarkdownHeader, gamesTable, storeGamesTable, compilationsTable)
    End Sub

    ''' <summary>
    ''' Creates the URL files for each scraped game.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overrides Sub CreateUrlFiles()
        MyBase.CreateUrlFiles()

        If Me.exclusiveStoreGames_?.Any() Then
            GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Games (Online Store)", Me.exclusiveStoreGames_)
        End If
    End Sub

#End Region

End Class

#End Region
