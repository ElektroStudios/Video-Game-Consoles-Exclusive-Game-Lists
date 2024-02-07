#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics
Imports System.Text

#End Region

#Region " Program "

''' <summary>
''' Main Program Module.
''' </summary>
Friend Module Program

#Region " Main Entry Point "

    ''' <summary>
    ''' Defines the main entry point of the application.
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Sub Main()
        Console.CursorVisible = False
        Console.OutputEncoding = Encoding.UTF8
        Console.Title = $"{My.Application.Info.Title} v{My.Application.Info.Version} | {My.Application.Info.Copyright}"

        ' WonderSwan
        Dim ws As New Platforms.WonderSwan()
        ws.DoScrap()
        ws.CreateMarkdownFile()
        ws.CreateUrlFiles()

        ' WonderSwan Color
        Dim wsc As New Platforms.WonderSwanColor()
        wsc.DoScrap()
        wsc.CreateMarkdownFile()
        wsc.CreateUrlFiles()

        ' Nintendo Game Boy
        Dim gb As New Platforms.GameBoy()
        gb.DoScrap()
        gb.CreateMarkdownFile()
        gb.CreateUrlFiles()

        ' PlayStation 3
        Dim ps3 As New Platforms.PlayStation3()
        ps3.DoScrap()
        ps3.CreateMarkdownFile()
        ps3.CreateUrlFiles()

        Console.WriteLine("All the scraping work has been completed. Press any key to close this program...")
        Console.ReadKey(intercept:=False)
        Environment.Exit(0)
    End Sub

#End Region

End Module

#End Region
