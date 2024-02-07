#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " IPlatform"

''' <summary>
''' Interface used to define platforms.
''' </summary>
Friend Interface IPlatform

#Region " Properties "

    ''' <summary>
    ''' Gets the <see cref="Global.PlatformInfo"/> object representing this platform.
    ''' </summary>
    ReadOnly Property PlatformInfo As PlatformInfo

#End Region

#Region " Methods "

    ''' <summary>
    ''' Does the scraping of platform exclusive games for this platform.
    ''' </summary>
    Sub DoScrap()

    ''' <summary>
    ''' Creates the markdown file.
    ''' </summary>
    Sub CreateMarkdownFile()

    ''' <summary>
    ''' Creates the URL files.
    ''' </summary>
    Sub CreateUrlFiles()

#End Region

End Interface

#End Region
