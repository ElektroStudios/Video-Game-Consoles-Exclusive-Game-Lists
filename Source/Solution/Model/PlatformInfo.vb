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
    ''' Gets the name of this platform.
    ''' </summary>
    Friend ReadOnly Property Name As String

    ''' <summary>
    ''' Gets the name of this platform typed as it appears in the Gamefaqs url.
    ''' </summary>
    Friend ReadOnly Property HttpName As String

    ''' <summary>
    ''' Gets the Gamefaqs url that points to this platform.
    ''' </summary>
    Friend ReadOnly Property BaseUrl As Uri
        Get
            Return New Uri($"https://gamefaqs.gamespot.com/{Me.HttpName}")
        End Get
    End Property

    ''' <summary>
    ''' Gets the Gamefaqs url that points to the "all games" list for this platform.
    ''' </summary>
    Friend ReadOnly Property AllGamesUrl As Uri
        <DebuggerStepThrough>
        Get
            Return New Uri($"{Me.BaseUrl}/category/999-all")
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="PlatformInfo"/> class from being created.
    ''' </summary>
    Private Sub New()
    End Sub

    ''' <summary>
    ''' Initializes a new instance of the <see cref="PlatformInfo"/> class.
    ''' </summary>
    ''' <param name="name">The name for this platform. (e.g. PlayStation 3)</param>
    ''' <param name="httpName">The name of this platform typed as it appears in the Gamefaqs url (e.g. ps3).</param>
    <DebuggerNonUserCode>
    Friend Sub New(name As String, httpName As String)
        If String.IsNullOrWhiteSpace(name) Then
            Throw New ArgumentNullException(paramName:=NameOf(name))
        End If
        If String.IsNullOrWhiteSpace(httpName) Then
            Throw New ArgumentNullException(paramName:=NameOf(httpName))
        End If

        Me.Name = name
        Me.HttpName = httpName
    End Sub

#End Region

End Class

#End Region
