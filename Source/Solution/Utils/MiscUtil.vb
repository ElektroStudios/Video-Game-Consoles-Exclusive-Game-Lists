#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics
Imports System.Net
Imports System.Text
Imports System.Threading

#End Region

#Region " MiscUtil "

''' <summary>
''' Miscellaneous Utilities.
''' </summary>
<HideModuleName>
Friend Module MiscUtil

#Region " Private Fields "

    ''' <summary>
    ''' Represents a pseudo-random number generator, which is a device that produces 
    ''' a sequence of numbers that meet certain statistical requirements for randomness.
    ''' </summary>
    Private ReadOnly RandomGenerator As New Random(Seed:=Environment.TickCount)

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Suspends the current thread for the a random number of milliseconds 
    ''' between the provided minimum and maximum values.
    ''' </summary>
    ''' <param name="minimum">
    ''' The minimum milliseconds to wait.
    ''' </param>
    ''' <param name="maximum">
    ''' The maximum milliseconds to wait.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub SleepRandom(minimum As Integer, maximum As Integer)
        Dim ms As Integer = MiscUtil.RandomGenerator.Next(minimum, maximum + 1)
        Thread.Sleep(ms)
    End Sub

    ''' <summary>
    ''' Prints the input error message in the attached console, 
    ''' and closes the running application with the provided exit code.
    ''' </summary>
    ''' <param name="errorMessage">
    ''' The input error message to print in the attached console.
    ''' </param>
    ''' <param name="exitcode">
    ''' The exit code to send when closing the running application.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub PrintErrorAndExit(errorMessage As String, exitcode As Integer)

        Dim sb As New StringBuilder()
        sb.AppendLine(errorMessage)
        sb.AppendLine()
        sb.AppendLine("This program will close now. Press any key to continue...")

        Console.WriteLine(sb.ToString())
        Console.ReadKey(intercept:=True)
        Environment.Exit(exitcode)

    End Sub

    ''' <summary>
    ''' Tries to download the HTML page source-code from the specified <see cref="Uri"/>. 
    ''' <para></para>
    ''' Whenever it fails to download, it prints the HTTP error code and waits the specified interval to retry again.
    ''' </summary>
    ''' <param name="uri">
    ''' The input <see cref="Uri"/> from which to download the html page source-code.
    ''' </param>
    ''' <param name="refHtmlPage">
    ''' A <see langword="ByRef"/> value that contains the resulting HTML page source-code when this method returns.
    ''' </param>
    ''' <param name="retryIntervalSeconds">
    ''' The interval, in seconds, to wait for retry the download. Default value is: 10 seconds.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub DownloadHtmlPageWithRetry(uri As Uri, ByRef refHtmlPage As String,
                                         Optional retryIntervalSeconds As Integer = 10)

        refHtmlPage = Nothing

        Using wc As New WebClient

            Do While String.IsNullOrEmpty(refHtmlPage)
                wc.Headers.Remove(HttpRequestHeader.UserAgent)
                wc.Headers.Add("User-Agent", GamefaqsUtil.ScraperUserAgent)
                Try
                    refHtmlPage = wc.DownloadString(uri)

                Catch ex As WebException
                    Dim response As HttpWebResponse = TryCast(ex.Response, HttpWebResponse)
                    If response IsNot Nothing Then

                        Dim statusCode As HttpStatusCode = response.StatusCode
                        Console.WriteLine($"Remote server error: ({CInt(statusCode)}) {statusCode}.")
                        Console.WriteLine($"Url: {uri}")

                        Select Case statusCode

                            Case HttpStatusCode.NotFound
                                MiscUtil.PrintErrorAndExit("This error indicates that the webpage or resource linked to by the URL does not exist." & Environment.NewLine &
                                                           "Check Gamefaqs website or contact their support to explain this problem.",
                                                           exitcode:=ExitCodes.ExitCodeHttpError)

                            Case HttpStatusCode.Forbidden
                                MiscUtil.PrintErrorAndExit("This error may indicate that your IP address have been banned." & Environment.NewLine &
                                                           "Check Gamefaqs website or contact their support to explain this problem.",
                                                           exitcode:=ExitCodes.ExitCodeHttpError)

                            Case Else
                                Console.WriteLine($"Waiting {retryIntervalSeconds} seconds to retry again...")
                                Console.WriteLine()
                                Thread.Sleep(TimeSpan.FromSeconds(retryIntervalSeconds))

                        End Select

                    Else
                        Throw

                    End If

                Catch ex As Exception
                    Throw

                End Try
            Loop

        End Using

    End Sub

    ''' <summary>
    ''' Converts the input string value to a valid file name that can be used in Windows OS,
    ''' by replacing any unsupported characters in the string.
    ''' </summary>
    ''' <param name="value">
    ''' The input string value.
    ''' </param>
    ''' <returns>
    ''' The resulting file name that can be used in Windows OS.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function ConvertStringToWindowsFileName(value As String) As String

        Return value.Replace("<", "˂").Replace(">", "˃").
                     Replace("\", "⧹").Replace("/", "⧸").Replace("|", "ǀ").
                     Replace(":", "∶").Replace("?", "ʔ").Replace("*", "✲").
                     Replace(ControlChars.Quote, Char.ConvertFromUtf32(&H201D))

    End Function

#End Region

End Module

#End Region
