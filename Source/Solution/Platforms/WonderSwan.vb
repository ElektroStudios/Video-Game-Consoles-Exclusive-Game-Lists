#Region " WonderSwan "

Namespace Platforms

    ''' <summary>
    ''' Bandai WonderSwan platform.
    ''' </summary>
    Friend NotInheritable Class WonderSwan : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("WonderSwan", "wonderswan")

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not yet released Games
"

#End Region

    End Class

End Namespace

#End Region
