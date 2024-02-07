#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " ExitCodes "

''' <summary>
''' Application Exit Codes.
''' </summary>
Friend Module ExitCodes

#Region " Fields "

    ''' <summary>
    ''' Exit code to use for general errors.
    ''' </summary>
    Friend Const ExitCodeDefault As Integer = 1

    ''' <summary>
    ''' Exit code to use when the creation of a file fails.
    ''' </summary>
    Friend Const ExitCodeCreatefileError As Integer = 2

    ''' <summary>
    ''' Exit code to use when an HTTP request fails.
    ''' </summary>
    Friend Const ExitCodeHttpError As Integer = 3

    ''' <summary>
    ''' Exit code to use when can't find an HTML element by the specified XPath.
    ''' </summary>
    Friend Const ExitCodeXPathNotFound As Integer = 4

#End Region

End Module

#End Region
