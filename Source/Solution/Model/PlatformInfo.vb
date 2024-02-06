#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics

#End Region

#Region " PlatformInfo "

''' <summary>
''' Represents basic information of a platform from Gamefaqs website.
''' </summary>
Friend NotInheritable Class PlatformInfo

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the platform name.
    ''' </summary>
    Friend Property Name As String

    ''' <summary>
    ''' Gets or sets the Gamefaqs url for this platform.
    ''' </summary>
    Friend Property BaseUrl As Uri

    ''' <summary>
    ''' Gets the Gamefaqs url pointing to the "all games list" of this platform.
    ''' </summary>
    Friend ReadOnly Property AllGamesUrl As Uri
        <DebuggerStepThrough>
        Get
            If Me.BaseUrl Is Nothing Then
                Throw New ArgumentNullException(paramName:=NameOf(Me.BaseUrl))
            End If
            Return New Uri($"{Me.BaseUrl}/category/999-all")
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="PlatformInfo"/> class.
    ''' </summary>
    <DebuggerNonUserCode>
    Friend Sub New()
    End Sub

#End Region

End Class

#End Region
