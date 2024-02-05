Public Class PlatformInfo

    Public Property PlatformName As String
    Public Property PlatformUrl As Uri
    Public ReadOnly Property AllGamesUrl As Uri
        Get
            Return New Uri($"{Me.PlatformUrl}/category/999-all")
        End Get
    End Property

    Public Sub New()
    End Sub

End Class
