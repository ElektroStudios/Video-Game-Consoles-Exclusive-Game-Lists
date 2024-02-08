#Region " PlayStation 5 "

Namespace Platforms

    ''' <summary>
    ''' PlayStation 5 platform.
    ''' </summary>
    Friend NotInheritable Class PlayStation5 : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PlayStation 5", "ps5")

        Protected Overrides ReadOnly Property OnlineStoreDistributionId As Integer = 42

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not yet released Games
    |[PlayStation Store Games]({Me.PlatformInfo.BaseUrl}/category/49-miscellaneous?dist={Me.OnlineStoreDistributionId})|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
