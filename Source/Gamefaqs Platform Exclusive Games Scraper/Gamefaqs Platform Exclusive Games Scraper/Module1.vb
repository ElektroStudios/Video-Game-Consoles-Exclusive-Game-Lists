#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Linq
Imports System.IO
Imports System.Text
Imports System.Threading

Imports HtmlAgilityPack
Imports System.Net

Imports HtmlDocument = HtmlAgilityPack.HtmlDocument
Imports Microsoft.SqlServer.Server
Imports System.Globalization
' Imports DevCase.Core.Networking.Common

#End Region

Module Module1

#Region " ReadOnly Fields "

    Public Const MyScraperUserAgent As String =
            "Find_PlatformExclusive_Games_Bot/1.0 (Windows; .NET Framework 4.8; non-harmful scraper; bot; scraper; en-US)"

    Public ReadOnly WebClient As New WebClient()

    ' Platform Infos

    Public ReadOnly ps3PlatformInfo As New PlatformInfo With {
        .PlatformName = "PlayStation 3",
        .PlatformUrl = New Uri("https://gamefaqs.gamespot.com/ps3")
    }

#End Region

#Region " Main Entry Point "

    Sub Main()
        Module1.WebClient.Headers.Add("User-Agent", Module1.MyScraperUserAgent)

        ' Playstation 3 Games

        Dim ps3Games As List(Of GameInfo) =
                GamefaqsUtil.ScrapExclusiveGames("Playstation 3 Games", ps3PlatformInfo,
                                                 ps3PlatformInfo.AllGamesUrl)

        ' Playstation Store PS3 Games

        Dim ps3StoreGames As List(Of GameInfo) =
            GamefaqsUtil.ScrapExclusiveGames("Playstation Store PS3 Games", ps3PlatformInfo,
                                             New Uri($"{ps3PlatformInfo.AllGamesUrl}?dist=17"))

        Dim ps3StoreGamesUrls As String() =
            (From gameinfo As GameInfo In ps3StoreGames
             Select (gameinfo.EntryUrl.ToString())
            ).ToArray()

        ' Filter out Playstation Store PS3 games from PS3 games list.
        ps3Games = (From gameinfo As GameInfo In ps3Games
                    Where Not ps3StoreGamesUrls.Contains(gameinfo.EntryUrl.ToString())
                   ).ToList()

        ' Playstation 3 Compilations

        Dim ps3Compilations As List(Of GameInfo) =
            (From gameinfo As GameInfo In ps3Games
             Where gameinfo.Genre.Contains("Compilation")
            ).ToList()

        Dim ps3CompilationsUrls As String() =
            (From gameinfo As GameInfo In ps3Compilations
             Select (gameinfo.EntryUrl.ToString())
            ).ToArray()

        ' Filter out Compilations from PS3 games list.
        ps3Games = (From gameinfo As GameInfo In ps3Games
                    Where Not ps3CompilationsUrls.Contains(gameinfo.EntryUrl.ToString())
                   ).ToList()

        ' Build Markdown Files
        Console.WriteLine($"Creating markdown file for: {ps3PlatformInfo.PlatformName}")
        Dim ps3GamesTable As String = GamefaqsUtil.BuildMarkdownTable("Exclusive Playstation 3 Games", ps3Games)
        Dim ps3StoreGamesTable As String = GamefaqsUtil.BuildMarkdownTable("Exclusive Playstation Store Games", ps3StoreGames)
        Dim ps3CompilationsTable As String = GamefaqsUtil.BuildMarkdownTable("Exclusive Compilations", ps3Compilations)
        GamefaqsUtil.WriteMarkdownFile(ps3PlatformInfo, MarkdownStrings.ps3Header, ps3GamesTable, ps3StoreGamesTable, ps3CompilationsTable)

        ' Build URL Files
        Console.WriteLine($"Creating URL files for: {NameOf(ps3Games)}")
        GamefaqsUtil.WriteUrlFiles(ps3PlatformInfo, ps3Games)

    End Sub


#End Region

#Region " Public Methods "

    Public Sub WriteErrorAndExit(errorMessage As String)

        Console.WriteLine(errorMessage)
        Console.WriteLine("This program will close now. Press any key to continue...")
        Console.ReadKey(intercept:=False)
        Environment.Exit(1)

    End Sub

#End Region

End Module
