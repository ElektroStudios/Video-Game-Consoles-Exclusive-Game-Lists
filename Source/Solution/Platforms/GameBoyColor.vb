#Region " GameBoy "

Namespace Platforms

    ''' <summary>
    ''' Nintendo Game Boy Color platform.
    ''' </summary>
    Friend NotInheritable Class GameBoyColor : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("Game Boy Color", "gbc")

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not yet released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
