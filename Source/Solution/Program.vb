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
Imports System.Text
Imports System.Threading

Imports Win32
Imports Win32.SafeHandles

#End Region

#Region " Program "

''' <summary>
''' Main Program Module.
''' </summary>
Friend Module Program

#Region " Main Entry Point "

    ''' <summary>
    ''' The platforms.
    ''' </summary>
    Private ReadOnly platforms As New List(Of IPlatform) From {
        New Platforms.GameBoy(),
        New Platforms.GameBoyColor(),
        New Platforms.PlayStation1(),
        New Platforms.PlayStation2(),
        New Platforms.PlayStation3(),
        New Platforms.PlayStation4(),
        New Platforms.PlayStation5(),
        New Platforms.WonderSwan(),
        New Platforms.WonderSwanColor()
    }

    ''' <summary>
    ''' Defines the main entry point of the application.
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Sub Main()
        Program.InitializeConsoleContext()

        Dim allPlatformNames As String() =
            (From item As IPlatform In platforms
             Select item.PlatformInfo.Name
            ).ToArray()

        Dim sb As New StringBuilder()
        sb.AppendLine("Defined platforms:")
        sb.AppendLine("")
        sb.Append(" - ")
        sb.AppendLine(String.Join(Environment.NewLine & " - ", allPlatformNames))
        sb.AppendLine()
        sb.AppendLine("Press any key to start scraping for the defined platforms above...")
        sb.AppendLine()
        sb.AppendLine("Press CTRL+C at any moment to abort the scraping work and terminate this program...")
        sb.AppendLine()
        Console.WriteLine(sb.ToString())
        Console.ReadKey(intercept:=True)

        For i As Integer = 0 To platforms.Count - 1

            Console.WriteLine($"{i + 1} of {platforms.Count} platforms | Running scraping work for platform: {platforms(i).PlatformInfo.Name}...")

            If TypeOf platforms(i) Is PlatformBaseWithOnlineStore Then
                Using platform As PlatformBaseWithOnlineStore = DirectCast(platforms(i), PlatformBaseWithOnlineStore)
                    platform.DoScrap()
                    platform.CreateMarkdownFile()
                    platform.CreateUrlFiles()
                End Using

            ElseIf TypeOf platforms(i) Is PlatformBase Then
                Using platform As PlatformBase = DirectCast(platforms(i), PlatformBase)
                    platform.DoScrap()
                    platform.CreateMarkdownFile()
                    platform.CreateUrlFiles()
                End Using

            Else
                ' Prevents an improbable unexpected type cast issue.
                Throw New NotImplementedException()

            End If

        Next i

        Console.WriteLine("All the scraping work has been completed!. Press any key to close this program...")
        Console.ReadKey(intercept:=True)
        Environment.Exit(0)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Initializes the console context.
    ''' </summary>
    <DebuggerStepperBoundary>
    Private Sub InitializeConsoleContext()
        Console.Title = $"{My.Application.Info.Title}" &
                        $" v{My.Application.Info.Version.Major}.{My.Application.Info.Version.Minor}" &
                        $" | {My.Application.Info.Copyright}"

        Console.CursorVisible = False
        Console.OutputEncoding = Encoding.UTF8

        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US")

        ' Set console window transparency.
        Dim transparency As Single = 0.9F
        Using hWnd As SafeWindowHandle = NativeMethods.GetConsoleWindow()
            NativeMethods.SetLayeredWindowAttributes(hWnd, 0, CByte(255 * transparency), &H2)
        End Using
    End Sub

#End Region

End Module

#End Region
