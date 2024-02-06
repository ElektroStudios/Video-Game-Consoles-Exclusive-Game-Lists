#Region "Option Statements"

Option Explicit On
Option Strict On
Option Infer Off

#End Region

#Region " Imports"

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Text
Imports System.Threading

Imports HtmlAgilityPack

#End Region

#Region " GamefaqsUtil "

''' <summary>
''' Gamefaqs Scraping and File Creation Utilities.
''' </summary>
<HideModuleName>
Friend Module GamefaqsUtil

#Region " Scrap Methods "

    ''' <summary>
    ''' Fake method of <see cref="ScrapLastPageNumber"/>.
    ''' <para></para>
    ''' Returns the provided value in <paramref name="returnValue"/> parameter.
    ''' <para></para>
    ''' This is for testing purposes.
    ''' </summary>
    ''' <param name="returnValue">The value to return.</param>
    ''' <returns>The value provided in <paramref name="returnValue"/> parameter.</returns>
    <DebuggerStepThrough>
    Friend Function FakeScrapLastPageNumber(returnValue As Integer) As Integer
        Return returnValue
    End Function

    ''' <summary>
    ''' Scraps the source game list url to retrieve the last page number 
    ''' from the paginate class (&lt;ul class="paginate"&gt;).
    ''' </summary>
    ''' <param name="uri">The source url where to retrieve the last page number.</param>
    ''' <returns>The last page number</returns>
    <DebuggerStepperBoundary>
    Friend Function ScrapLastPageNumber(uri As Uri) As Integer

        Dim htmlSource As String
        Using wc As New WebClient
            wc.Headers.Add("User-Agent", Program.MyScraperUserAgent)
            htmlSource = wc.DownloadString(uri)
        End Using
        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(htmlSource)

        Dim paginateXpath As String = "//ul[@class='paginate']/li"
        Dim paginateNode As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode(paginateXpath)
        Dim paginateText As String = paginateNode?.GetDirectInnerText()

        If (paginateNode Is Nothing) OrElse String.IsNullOrEmpty(paginateText) Then
            MiscUtil.PrintErrorAndExit($"Can't locate paginate element (XPath: ""{paginateXpath}"") in html source-code of uri: {uri}", exitcode:=1)
        End If

        Dim lastPageNumber As Integer = CInt(paginateText.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).Last())
        Return lastPageNumber

    End Function

    ''' <summary>
    ''' Scraps only the platform exclusive games from the input list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' </summary>
    ''' <param name="description">The description name (e.g. Playstation Store PS3) for the games in the provided list uri.</param>
    ''' <param name="platform">The source <see cref="PlatformInfo"/> for which the games in the provided list uri belongs to.</param>
    ''' <param name="uri">The input game list uri where to scrap the platform exclusive games.</param>
    ''' <returns>A <see cref="List(Of GameInfo)"/> representing each scraped game</returns>
    <DebuggerStepperBoundary>
    Friend Function ScrapExclusiveGames(description As String, platform As PlatformInfo, uri As Uri) As List(Of GameInfo)

        Dim exclusiveGamesList As New List(Of GameInfo)
        Dim lastPageNumber As Integer = GamefaqsUtil.ScrapLastPageNumber(uri)
        ' Dim lastPageNumber As Integer = GamefaqsUtil.FakeScrapLastPageNumber(1)

        Dim entryCount As Integer

        For pageIndex As Integer = 0 To lastPageNumber - 1
            Dim currentPageUri As New Uri($"{uri}&page={pageIndex}")

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            Console.WriteLine($"Url: '{currentPageUri} ...")
            Console.WriteLine("")

            Dim htmlSource As String
            Using wc As New WebClient
                wc.Headers.Add("User-Agent", Program.MyScraperUserAgent)
                htmlSource = wc.DownloadString(currentPageUri)
            End Using
            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(htmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If (titleNodes Is Nothing) OrElse Not titleNodes.Any Then
                MiscUtil.PrintErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {currentPageUri}", exitcode:=1)
            End If

            ' Iterate game title entries.

            For Each titleNode As HtmlNode In titleNodes
                Dim nodeInnerHtml As String = titleNode.InnerHtml

                If nodeInnerHtml.Contains("""cancel""") Then ' Cancelled game.
                    Continue For

                ElseIf nodeInnerHtml.Contains("""unrel""") Then ' Not yet released game.
                    Continue For

                Else ' Released game.
                    Thread.Sleep(2000)
                    ' MiscUtil.SleepRandom(2000, 3000)

                    ' Note that the "titleNode.InnerText" value can return a game title in Japanese or other language,
                    ' so this value can't be considered as the proper game title name to use.
                    Dim entryTitle As String = titleNode.InnerText.Trim()
                    Dim entryurlBase As String = titleNode.SelectSingleNode("a").Attributes("href").Value
                    Dim entryUrl As New Uri($"https://gamefaqs.gamespot.com{entryurlBase}")

                    Console.WriteLine($"Scraping {description}... | Page {pageIndex + 1} of {lastPageNumber} | Entry Count: {Interlocked.Increment(entryCount)} | Title: {entryTitle}")
                    Console.WriteLine($"Url: {entryUrl}")
                    Console.WriteLine("")

                    Dim gameEntryHtmlsource As String = Nothing
                    Using wc As New WebClient
                        wc.Headers.Add("User-Agent", Program.MyScraperUserAgent)
                        gameEntryHtmlsource = wc.DownloadString(entryUrl)
                    End Using
                    Dim gameEntryHtmlDoc As New HtmlDocument()
                    gameEntryHtmlDoc.LoadHtml(gameEntryHtmlsource)

                    ' 1. Determine whether this game is listed for other platforms.

                    If gameEntryHtmlsource.Contains("""also_name"">") Then
                        Continue For
                    End If

                    Const contentXpath As String = "//div[@class='content']"

                    ' 2. Determine whether this entry belongs to an expansion or DLC content:

                    Dim expansionNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.ToLower().Contains("<b>expansion for:</b> ")
                        ).FirstOrDefault()

                    Dim isExpansionOrDLC As Boolean = expansionNode IsNot Nothing
                    If isExpansionOrDLC Then
                        Continue For
                    End If

                    ' 3. Retrieve the genre:

                    Dim genreNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.Trim.StartsWith("<b>Genre:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim genre As String =
                        genreNode?.InnerHtml.Replace("<b>Genre:</b> ", "").
                                             Replace($"<a href=""/{platform.BaseUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platform.BaseUrl}").
                                             Replace(""">", """ target=""_blank"" rel=""noopener noreferrer"">").Trim()

                    If String.IsNullOrWhiteSpace(genre) Then
                        genre = "N/A"
                    End If

                    ' This is Software / Application, not a game.
                    If genre.Contains(">Application<") Then
                        Continue For
                    End If

                    ' This is Hardware, not a game.
                    If genre.Contains(">Hardware<") Then
                        Continue For
                    End If

                    ' This is a Demonstration, not a full game.
                    If genre.Contains(">Demo Disc<") OrElse genre.Contains(">Demo<") Then
                        Continue For
                    End If

                    ' 4. Retrieve the release date:

                    Dim releaseDateNode As HtmlNode =
                        (From node As HtmlNode In gameEntryHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.Trim.StartsWith("<b>Release:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim releaseDate As String =
                        releaseDateNode?.InnerHtml.Replace("<b>Release:</b> ", "").
                                                   Replace($"<a href=""/{platform.BaseUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platform.BaseUrl}").
                                                   Replace(""">", """ target=""_blank"" rel=""noopener noreferrer"">").Trim()

                    If String.IsNullOrEmpty(releaseDate) Then
                        releaseDate = "N/A"
                    End If

                    ' 5. Retrieve the game title:

                    Const pageTitleXpath As String = "//h1[@class='page-title']"

                    Dim pageTitleNode As HtmlNode = gameEntryHtmlDoc.DocumentNode.SelectSingleNode(pageTitleXpath)
                    Dim pageTitle As String = pageTitleNode?.InnerText
                    If String.IsNullOrWhiteSpace(pageTitle) Then
                        MiscUtil.PrintErrorAndExit($"Can't locate game title / page title element (XPath: ""{pageTitleNode}"") in html source-code of uri: {entryUrl}", exitcode:=1)
                    End If

                    ' 6. At this point the game entry has passed all checks and it is considered a valid exclusive game.

                    Dim gameInfo As New GameInfo() With {
                        .PlatformName = platform.Name,
                        .Title = pageTitle,
                        .EntryUrl = entryUrl,
                        .Genre = genre,
                        .ReleaseDate = releaseDate
                    }

                    ' Some entries are duplicated in the Gamefaqs list,
                    ' such as one entry with Japanese name and the other in English,
                    ' so we ensure the entry does not already exists in the list.
                    Dim entryExists As Boolean = (From game As GameInfo In exclusiveGamesList Where game.EntryUrl.Equals(gameInfo.EntryUrl)).Any()
                    If Not entryExists Then
                        exclusiveGamesList.Add(gameInfo)
                    End If
                End If

            Next titleNode

            MiscUtil.SleepRandom(2000, 3000)
        Next pageIndex

        Return exclusiveGamesList

    End Function

    ''' <summary>
    ''' Scraps only the game titles and their entry urls from the input list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' <para></para>
    ''' Note: this function returns all titles, including non-exclusive, demo discs, compilations, etc.
    ''' </summary>
    ''' <param name="description">The description name (e.g. Playstation Store PS3) for the games in the provided list uri.</param>
    ''' <param name="uri">The input game list uri where to scrap the platform exclusive games.</param>
    ''' <returns>A <see cref="List(Of GameInfo)"/> representing each scraped game</returns>
    <DebuggerStepperBoundary>
    Friend Function ScrapOnlyTitlesAndEntryUrls(description As String, uri As Uri) As List(Of GameInfo)

        Dim gamesList As New List(Of GameInfo)
        Dim lastPageNumber As Integer = GamefaqsUtil.ScrapLastPageNumber(uri)

        For pageIndex As Integer = 0 To lastPageNumber - 1

            Dim currentUri As New Uri($"{uri}&page={pageIndex}")

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            ' Console.WriteLine($"Url: '{currentUri} ...")
            ' Console.WriteLine("")

            Dim htmlSource As String
            Using wc As New WebClient
                wc.Headers.Add("User-Agent", Program.MyScraperUserAgent)
                htmlSource = wc.DownloadString(currentUri)
            End Using
            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(htmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If (titleNodes Is Nothing) OrElse Not titleNodes.Any Then
                MiscUtil.PrintErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {currentUri}", exitcode:=1)
            End If

            For Each titleNode As HtmlNode In titleNodes
                Dim nodeInnerHtml As String = titleNode.InnerHtml

                If nodeInnerHtml.Contains("""cancel""") Then ' Cancelled game.
                    Continue For

                ElseIf nodeInnerHtml.Contains("""unrel""") Then ' Not yet released game.
                    Continue For

                Else ' Released game.
                    Dim gametitle As String = titleNode.InnerText.Trim()
                    Dim gameEntryBaseName As String = titleNode.SelectSingleNode("a").Attributes("href").Value
                    Dim gameEntryUrl As New Uri($"https://gamefaqs.gamespot.com{gameEntryBaseName}")

                    ' Console.WriteLine($"Scraping... | {description} | Page {pageIndex + 1} of {lastPageNumber} | Entry: {Interlocked.Increment(entryCount)} | Game: '{gametitle}' ...")

                    Dim gameInfo As New GameInfo() With {
                        .PlatformName = description,
                        .Title = gametitle,
                        .EntryUrl = gameEntryUrl,
                        .Genre = Nothing,
                        .ReleaseDate = Nothing
                    }

                    gamesList.Add(gameInfo)
                End If

            Next titleNode

            MiscUtil.SleepRandom(2000, 3000)
        Next pageIndex

        Return gamesList

    End Function

#End Region

#Region " Markdown Methods "

    ''' <summary>
    ''' Builds a markdown table from the provided <see cref="List(Of GameInfo)"/> object.
    ''' </summary>
    ''' <param name="headerTitle">The table header title.</param>
    ''' <param name="games">The <see cref="List(Of GameInfo)"/> containing the games to add in the table.</param>
    ''' <returns>The resulting table in Markdown format.</returns>
    <DebuggerStepThrough>
    Friend Function BuildMarkdownTable(headerTitle As String, games As List(Of GameInfo)) As String
        Dim sb As New StringBuilder()
        sb.AppendLine($"# {headerTitle}")
        sb.AppendLine("|Index|Title|Release Date|Genre|")
        sb.AppendLine("|:--:|--|--|--|")

        Dim entryCount As Integer = 0
        For Each game As GameInfo In games
            sb.AppendLine($"|{Interlocked.Increment(entryCount)}|<a href=""{game.EntryUrl}"" target=""_blank"" rel=""noopener noreferrer"">{game.Title}</a>|{game.ReleaseDate}|{game.Genre}|")
        Next

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Creates a Markdown's MD file using the provided string values as its file content.
    ''' </summary>
    ''' <param name="platformName">The name of the platform, used as the output directory name and MD file name.</param>
    ''' <param name="values">The values to write in the MD file.</param>
    <DebuggerStepperBoundary>
    Friend Sub WriteMarkdownFile(platformName As String, ParamArray values As String())

        Dim outputDir As New DirectoryInfo($".\{platformName}")
        If Not outputDir.Exists Then
            outputDir.Create()
        End If

        Dim fullMarkdown As String = String.Join(Environment.NewLine, values)

        Dim outputFilePath As String = $"\\?\{outputDir.FullName}\{platformName}.md"

        Console.WriteLine($"Creating markdown file: {outputFilePath.Replace("\\?\", "")}...")
        File.WriteAllText(outputFilePath, fullMarkdown, Encoding.UTF8)

    End Sub

#End Region

#Region " URL (files) Methods "

    ''' <summary>
    ''' Creates an URL file for each item in the provided <see cref="List(Of GameInfo)"/> object.
    ''' </summary>
    ''' <param name="platformName">The name of the platform, used as the output base directory name.</param>
    ''' <param name="subDirName">The subdirectory name to append to output directory path.</param>
    ''' <param name="games">The <see cref="List(Of GameInfo)"/> containing the games from which to create URL files.</param>
    <DebuggerStepperBoundary>
    Friend Sub CreateUrlFiles(platformName As String, subDirName As String, games As List(Of GameInfo))

        Dim outputDir As New DirectoryInfo($".\{platformName}\{subDirName}\Urls")
        If Not outputDir.Exists Then
            outputDir.Create()
        End If

        For Each game As GameInfo In games
            Dim sb As New StringBuilder()
            sb.AppendLine("[InternetShortcut]")
            sb.AppendLine($"URL={game.EntryUrl}")

            Dim fileName As String = MiscUtil.ConvertStringToWindowsFileName(game.Title)
            Dim outputFilePath As String = $"\\?\{outputDir.FullName}\{fileName}.url"

            Console.WriteLine($"Creating URL file: {outputFilePath.Replace("\\?\", "")}...")
            File.WriteAllText(outputFilePath, sb.ToString(), Encoding.UTF8)
        Next game

    End Sub

#End Region

End Module

#End Region
