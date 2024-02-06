#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics

#End Region

#Region " Program "

''' <summary>
''' Main Program Module.
''' </summary>
Module Program

#Region "  Fields "

    ''' <summary>
    ''' User-Agent used to identify the scraper to Gamefaqs server.
    ''' </summary>
    Friend Const MyScraperUserAgent As String =
            "Find_PlatformExclusive_Games_Bot/1.0 (Windows; .NET Framework 4.8; non-harmful scraper; bot; scraper; en-US)"

#End Region

#Region " Main Entry Point "

    ''' <summary>
    ''' Defines the main entry point of the application.
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Sub Main()

        Dim ps3 As New Platforms.Playstation3()

        ' MsgBox(ps3.ExclusiveGames.FirstOrDefault)

        ps3.DoScrap()
        ' ps3.DoScrap()
        'ps3.CreateMarkdownFile()
        ' ps3.CreateUrlFiles()


        Console.WriteLine("")
        Console.WriteLine("All work has been completed. Press any key to close this program...")
        Console.ReadKey(intercept:=False)
        Environment.Exit(0)
    End Sub

#End Region

End Module

#End Region
