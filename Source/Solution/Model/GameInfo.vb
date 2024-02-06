#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Diagnostics

#End Region

#Region " GameInfo "

''' <summary>
''' Represents basic information of a scraped game from Gamefaqs website.
''' </summary>
Friend NotInheritable Class GameInfo

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the name of the platform that this game belongs to.
    ''' </summary>
    Friend Property PlatformName As String

    ''' <summary>
    ''' Gets or sets the game title.
    ''' </summary>
    Friend Property Title As String

    ''' <summary>
    ''' Gets or sets the Gamefaqs entry url for this game.
    ''' </summary>
    Friend Property EntryUrl As Uri

    ''' <summary>
    ''' Gets or sets the release date for this game.
    ''' </summary>
    Friend Property ReleaseDate As String

    ''' <summary>
    ''' Gets or sets the genre for this game.
    ''' </summary>
    Friend Property Genre As String

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="GameInfo"/> class.
    ''' </summary>
    <DebuggerNonUserCode>
    Friend Sub New()
    End Sub

#End Region

End Class

#End Region