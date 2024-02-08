#Region " PlayStation "

Namespace Platforms

    ''' <summary>
    ''' PlayStation (PSX) platform.
    ''' </summary>
    Friend NotInheritable Class PlayStation1 : Inherits PlatformBase

#Region " Properties "

        Friend Overrides ReadOnly Property PlatformInfo As _
            New PlatformInfo("PlayStation", "ps")

        Protected Overrides ReadOnly Property MarkdownFiltersTable As String = $"
    |Included:|Excluded:|
    |:--|:--|
    |Released Games|Cancelled or Not yet released Games
    |[Compilations]({Me.PlatformInfo.BaseUrl}/category/233-miscellaneous-compilation)|[Expansions / DLC Distribution]({Me.PlatformInfo.BaseUrl}/category/999-all?dist=6)
    ||[Demo Discs]({Me.PlatformInfo.BaseUrl}/category/280-miscellaneous-demo-disc)
    ||[Software / Applications]({Me.PlatformInfo.BaseUrl}/category/277-miscellaneous-application)
"

#End Region

    End Class

End Namespace

#End Region
