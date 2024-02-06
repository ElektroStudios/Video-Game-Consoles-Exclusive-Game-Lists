#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics
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
    ''' <param name="minimum">The minimum milliseconds to wait.</param>
    ''' <param name="maximum">The maximum milliseconds to wait.</param>
    <DebuggerStepThrough>
    Friend Sub SleepRandom(minimum As Integer, maximum As Integer)
        Dim ms As Integer = MiscUtil.RandomGenerator.Next(minimum, maximum + 1)
        Thread.Sleep(ms)
    End Sub

    ''' <summary>
    ''' Prints the input error message in the attached console, 
    ''' and closes the running application with the provided exit code.
    ''' </summary>
    ''' <param name="errorMessage">The input error message to print in the attached console.</param>
    ''' <param name="exitcode">The exit code to send when closing the running application.</param>
    <DebuggerStepThrough>
    Friend Sub PrintErrorAndExit(errorMessage As String, exitcode As Integer)

        Dim sb As New StringBuilder()
        sb.AppendLine(errorMessage)
        sb.AppendLine()
        sb.AppendLine("This program will close now. Press any key to continue...")

        Console.WriteLine(sb.ToString())
        Console.ReadKey(intercept:=False)
        Environment.Exit(exitcode)

    End Sub

    ''' <summary>
    ''' Converts the input string value to a valid file name that can be used in Windows OS,
    ''' by replacing any unsupported characters in the string.
    ''' </summary>
    ''' <param name="value">The input string value.</param>
    ''' <returns>The resulting file name that can be used in Windows OS.</returns>
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
