﻿#Region " PlayStation 3 "

Namespace Platforms

    ''' <summary>
    ''' PlayStation 3 platform.
    ''' </summary>
    Friend NotInheritable Class PlayStation3 : Inherits PlatformBaseWithOnlineStore

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PlayStation 3", "ps3")

        Protected Overrides ReadOnly Property StoreGamesDistributionId As Integer = 17

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |General Games|Cancelled or Not yet released Games
    |[PlayStation Store Games]({Me.PlatformInfo.BaseUrl}/category/49-miscellaneous?dist={Me.StoreGamesDistributionId})|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
