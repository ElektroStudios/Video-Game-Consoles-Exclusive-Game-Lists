#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices

#End Region

#Region " PlatformBase"

''' <summary>
''' Base class used to implement platforms.
''' </summary>
Friend MustInherit Class PlatformBase : Implements IPlatform

#Region " Private Fields "

    ''' <summary>
    ''' Flag to determine whether method <see cref="DoScrap"/> was called.
    ''' </summary>
    Protected scrapCompleted As Boolean

#End Region

#Region " IPlatform Properties "

    ''' <summary>
    ''' Gets the <see cref="Global.PlatformInfo"/> object representing this platform.
    ''' </summary>
    Friend MustOverride ReadOnly Property PlatformInfo As PlatformInfo Implements IPlatform.PlatformInfo

    ''' <summary>
    ''' Gets the Markdown header to use for this platform when building the MD file.
    ''' </summary>
    Friend MustOverride ReadOnly Property MarkdownHeader As String Implements IPlatform.MarkdownHeader

#End Region

#Region " Game Lists Properties "

    ''' <summary>
    ''' Gets the platform exclusive games.
    ''' <para></para>
    ''' Note: You must call method <see cref="DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveGames As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveGames_
        End Get
    End Property
    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The platform exclusive games.
    ''' </summary>
    Protected exclusiveGames_ As List(Of GameInfo)

    ''' <summary>
    ''' Gets the platform exclusive compilations.
    ''' <para></para>
    ''' Note: You must call method <see cref="DoScrap"/> to initialize the value of this property.
    ''' </summary>
    Friend ReadOnly Property ExclusiveCompilations As List(Of GameInfo)
        <DebuggerStepThrough>
        Get
            Me.FailIfScrapNotCompleted()
            Return Me.exclusiveCompilations_
        End Get
    End Property

    ''' <summary>
    ''' ( Backing Field )
    ''' <para></para>
    ''' The platform exclusive compilations.
    ''' </summary>
    Protected exclusiveCompilations_ As List(Of GameInfo)

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of this platform class.
    ''' </summary>
    <DebuggerNonUserCode>
    Friend Sub New()
    End Sub

#End Region

#Region " IPlatform Methods "

    ''' <summary>
    ''' Does the scraping of platform exclusive games for this platform.
    ''' <para></para>
    ''' Calling this method will initialize the following properties with the scraped items: 
    ''' <para></para> - <see cref="ExclusiveGames"/> 
    ''' <para></para> - <see cref="ExclusiveCompilations"/>
    ''' </summary>
    <DebuggerStepperBoundary>
    Friend Overridable Sub DoScrap() Implements IPlatform.DoScrap
        Me.scrapCompleted = False

        ' Scrap Exclusive Games list.

        Me.exclusiveGames_ =
                GamefaqsUtil.ScrapExclusiveGames($"{Me.PlatformInfo.Name} Games", Me.PlatformInfo,
                                                 Me.PlatformInfo.AllGamesUrl)

        ' Build Exclusive Compilations list.

        Me.exclusiveCompilations_ =
                (From gameinfo As GameInfo In Me.exclusiveGames_
                 Where gameinfo.Genre.Contains("Compilation")
                ).ToList()

        Dim compilationUrls As Uri() =
                (From gameinfo As GameInfo In Me.exclusiveCompilations_
                 Select gameinfo.EntryUrl).ToArray()

        ' Filter out Exclusive Compilations from Exclusive Games list.

        Me.exclusiveGames_ =
                (From gameinfo As GameInfo In Me.exclusiveGames_
                 Where Not compilationUrls.Contains(gameinfo.EntryUrl)
                ).ToList()

        Console.WriteLine("")
        Console.WriteLine($"Scraping completed for {Me.PlatformInfo.Name} platform.")
        Console.WriteLine("")

        Me.scrapCompleted = True
    End Sub

    ''' <summary>
    ''' Creates the markdown file and writes the tables with the scraped games.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overridable Sub CreateMarkdownFile() Implements IPlatform.CreateMarkdownFile
        Me.FailIfScrapNotCompleted()
        Dim gamesTable As String = GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_)
        Dim compilationsTable As String = GamefaqsUtil.BuildMarkdownTable($"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_)
        GamefaqsUtil.WriteMarkdownFile(Me.PlatformInfo.Name, Me.MarkdownHeader, gamesTable, compilationsTable)
    End Sub

    ''' <summary>
    ''' Creates the URL files for each scraped game.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Overridable Sub CreateUrlFiles() Implements IPlatform.CreateUrlFiles
        Me.FailIfScrapNotCompleted()
        GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Games", Me.exclusiveGames_)
        GamefaqsUtil.CreateUrlFiles(Me.PlatformInfo.Name, $"{Me.PlatformInfo.Name}∶ Exclusive Compilations", Me.exclusiveCompilations_)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Throws a exception of type <see cref="InvalidOperationException"/> 
    ''' if method <see cref="IPlatform.DoScrap"/> was not called previously.
    ''' </summary>
    ''' <param name="callerName">Optional. The method or property name of the caller to this method.</param>
    <DebuggerStepThrough>
    Protected Sub FailIfScrapNotCompleted(<CallerMemberName> Optional callerName As String = Nothing)

        If Not Me.scrapCompleted Then
            Dim bindingFlags As BindingFlags = BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic

            Dim callerProperty As PropertyInfo = Me.GetType().GetProperty(callerName, bindingFlags)
            If callerProperty IsNot Nothing Then
                Throw New InvalidOperationException($"You must call method '{NameOf(IPlatform.DoScrap)}' to initialize the value of property '{callerProperty.Name}'.")
            End If

            Dim callerMethod As MethodInfo = Me.GetType().GetMethod(callerName, bindingFlags)
            If callerMethod IsNot Nothing Then
                Throw New InvalidOperationException($"You must call method '{NameOf(IPlatform.DoScrap)}' before calling method '{callerName}'.")
            End If

            Throw New InvalidOperationException($"You must call method '{NameOf(IPlatform.DoScrap)}' before calling member '{callerName}'.")
        End If

    End Sub

#End Region

End Class

#End Region
