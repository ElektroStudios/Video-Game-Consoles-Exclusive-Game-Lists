#Region " WonderSwanColor "

Namespace Platforms

    ''' <summary>
    ''' Bandai WonderSwan Color platform.
    ''' </summary>
    Friend NotInheritable Class WonderSwanColor : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("WonderSwan Color", "wsc")

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not yet released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|
"

#End Region

    End Class

End Namespace

#End Region
