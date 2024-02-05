Imports HtmlAgilityPack

Imports System.IO
Imports System.Text

Imports System.Threading

Module GamefaqsUtil

    ''' <summary>
    ''' Fake method of <see cref="ScrapLastPageNumber"/>.
    ''' <para></para>
    ''' Returns the provided value in <paramref name="returnValue"/> parameter.
    ''' <para></para>
    ''' This is for testing purposes.
    ''' </summary>
    ''' <param name="returnValue">The value to return.</param>
    ''' <returns>The value provided in <paramref name="returnValue"/> parameter.</returns>
    Public Function FakeScrapLastPageNumber(returnValue As Integer) As Integer
        Return returnValue
    End Function

    ''' <summary>
    ''' Scraps the source game list url to retrieve the last page number 
    ''' from the paginate class (&lt;ul class="paginate"&gt;).
    ''' </summary>
    ''' <param name="uri">The source url where to retrieve the last page number.</param>
    ''' <returns>The last page number</returns>
    Public Function ScrapLastPageNumber(uri As Uri) As Integer

        Dim htmlSource As String = Module1.WebClient.DownloadString(uri)
        Dim htmlDoc As New HtmlDocument()
        htmlDoc.LoadHtml(htmlSource)

        Dim paginateXpath As String = "//ul[@class='paginate']/li"
        Dim paginateNode As HtmlNode = htmlDoc.DocumentNode.SelectSingleNode(paginateXpath)
        Dim paginateText As String = paginateNode?.GetDirectInnerText()

        If (paginateNode Is Nothing) OrElse String.IsNullOrEmpty(paginateText) Then
            Module1.WriteErrorAndExit($"Can't locate paginate element (XPath: ""{paginateXpath}"") in html source-code of uri: {uri}")
        End If

        Dim lastPageNumber As Integer = CInt(paginateText.Split({" "c}, StringSplitOptions.RemoveEmptyEntries).Last())
        Return lastPageNumber

    End Function

    ''' <summary>
    ''' Scraps the platform exclusive games from the input list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' </summary>
    ''' <param name="description">The description name (e.g. Playstation Store PS3) for the games in the provided list uri.</param>
    ''' <param name="platformInfo">The source <see cref="PlatformInfo"/> for which the games in the provided list uri belongs to.</param>
    ''' <param name="uri">The input game list uri where to scrap the platform exclusive games.</param>
    ''' <returns>A <see cref="List(Of GameProfileInfo)"/> representing each scraped game</returns>
    Public Function ScrapExclusiveGames(description As String, platformInfo As PlatformInfo, uri As Uri) As List(Of GameInfo)

        Dim exclusiveGamesList As New List(Of GameInfo)
        Dim lastPageNumber As Integer = GamefaqsUtil.FakeScrapLastPageNumber(0) ' GamefaqsUtil.ScrapLastPageNumber(uri)

        Dim entryCount As Integer

        For pageIndex As Integer = 0 To lastPageNumber - 1

            Dim currentUri As New Uri($"{uri}&page={pageIndex}")

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            ' Console.WriteLine($"Url: '{currentUri} ...")
            ' Console.WriteLine("")

            Dim htmlSource As String = Module1.WebClient.DownloadString(currentUri)
            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(htmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If (titleNodes Is Nothing) OrElse Not titleNodes.Any Then
                Module1.WriteErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {currentUri}")
            End If

            For Each titleNode As HtmlNode In titleNodes
                Dim nodeInnerHtml As String = titleNode.InnerHtml

                If nodeInnerHtml.Contains("""cancel""") Then ' Cancelled game.
                    Continue For

                ElseIf nodeInnerHtml.Contains("""unrel""") Then ' Not yet released game.
                    Continue For

                Else ' Released game.
                    Dim gametitle As String = titleNode.InnerText.Trim()
                    Dim gameProfileBaseName As String = titleNode.SelectSingleNode("a").Attributes("href").Value
                    Dim gameProfileUrl As New Uri($"https://gamefaqs.gamespot.com{gameProfileBaseName}")

                    Console.WriteLine($"Scraping... | {description} | Page {pageIndex + 1} of {lastPageNumber} | Entry: {Interlocked.Increment(entryCount)} | Game: '{gametitle}' ...")
                    ' Console.WriteLine($"Url: {gameProfileUrl} ...")
                    ' Console.WriteLine("")

                    Dim gameProfileHtmlsource As String = Module1.WebClient.DownloadString(gameProfileUrl)
                    Dim gameProfileHtmlDoc As New HtmlDocument()
                    gameProfileHtmlDoc.LoadHtml(gameProfileHtmlsource)

                    ' 1. Determine whether this entry belongs to a expansion or DLC content:

                    Const expansionXpath As String = "//div[@class='content']"
                    Dim expansionNode As HtmlNode =
                        (From node As HtmlNode In gameProfileHtmlDoc.DocumentNode.SelectNodes(expansionXpath)
                         Where node.InnerHtml.ToLower().Contains("<b>expansion for:</b> ")
                        ).FirstOrDefault()

                    Dim isExpansionOrDLC As Boolean = expansionNode IsNot Nothing
                    If isExpansionOrDLC Then
                        Continue For
                    End If

                    ' 2. Retrieve the genre:

                    Const contentXpath As String = "//div[@class='content']"

                    Dim genreNode As HtmlNode =
                        (From node As HtmlNode In gameProfileHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.Trim.StartsWith("<b>Genre:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim genre As String =
                        genreNode?.InnerHtml.Replace("<b>Genre:</b> ", "").
                                             Replace($"<a href=""/{platformInfo.PlatformUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platformInfo.PlatformUrl}").
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

                    ' 3. Retrieve the release date:

                    Dim releaseDateNode As HtmlNode =
                        (From node As HtmlNode In gameProfileHtmlDoc.DocumentNode.SelectNodes(contentXpath)
                         Where node.InnerHtml.Trim.StartsWith("<b>Release:</b> ", StringComparison.OrdinalIgnoreCase)
                        ).FirstOrDefault()

                    Dim releaseDate As String =
                        releaseDateNode?.InnerHtml.Replace("<b>Release:</b> ", "").
                                                   Replace($"<a href=""/{platformInfo.PlatformUrl.ToString().TrimEnd("/"c).Split("/"c).Last}", $"<a href=""{platformInfo.PlatformUrl}").
                                                   Replace(""">", """ target=""_blank"" rel=""noopener noreferrer"">").Trim()

                    If String.IsNullOrEmpty(releaseDate) Then
                        releaseDate = "N/A"
                    End If

                    ' 4. At this point the game entry has passed all checks and it is considered a valid exclusive game.

                    Dim gameInfo As New GameInfo() With {
                        .PlatformName = description,
                        .GameTitle = gametitle,
                        .EntryUrl = gameProfileUrl,
                        .Genre = genre,
                        .ReleaseDate = releaseDate
                    }

                    exclusiveGamesList.Add(gameInfo)
                End If

                GamefaqsUtil.SleepRandom(3000, 5000)
            Next titleNode

            GamefaqsUtil.SleepRandom(3000, 5000)
        Next pageIndex

        Return exclusiveGamesList

    End Function

    ''' <summary>
    ''' Scraps the only the game titles and their entry urls from the input list url,
    ''' and returns a <see cref="List(Of GameInfo)"/> representing each scraped game.
    ''' </summary>
    ''' <param name="description">The description name (e.g. Playstation Store PS3) for the games in the provided list uri.</param>
    ''' <param name="uri">The input game list uri where to scrap the platform exclusive games.</param>
    ''' <returns>A <see cref="List(Of GameProfileInfo)"/> representing each scraped game</returns>
    Public Function ScrapOnlyGameTitlesAndEntryurls(description As String, uri As Uri) As List(Of GameInfo)

        Dim gamesList As New List(Of GameInfo)
        Dim lastPageNumber As Integer = GamefaqsUtil.ScrapLastPageNumber(uri)

        For pageIndex As Integer = 0 To lastPageNumber - 1

            Dim currentUri As New Uri($"{uri}&page={pageIndex}")

            Console.WriteLine($"Parsing '{description}' page {pageIndex + 1} of {lastPageNumber} ...")
            ' Console.WriteLine($"Url: '{currentUri} ...")
            ' Console.WriteLine("")

            Dim htmlSource As String = Module1.WebClient.DownloadString(currentUri)
            Dim htmlDoc As New HtmlDocument
            htmlDoc.LoadHtml(htmlSource)

            Const titleXpath As String = "//td[@class='rtitle']"
            Dim titleNodes As HtmlNodeCollection = htmlDoc.DocumentNode.SelectNodes(titleXpath)
            If (titleNodes Is Nothing) OrElse Not titleNodes.Any Then
                Module1.WriteErrorAndExit($"Can't locate game title elements (XPath: ""{titleXpath}"") in html source-code of uri: {currentUri}")
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
                        .GameTitle = gametitle,
                        .EntryUrl = gameEntryUrl,
                        .Genre = Nothing,
                        .ReleaseDate = Nothing
                    }

                    gamesList.Add(gameInfo)
                End If

            Next titleNode

            GamefaqsUtil.SleepRandom(3000, 5000)
        Next pageIndex

        Return gamesList

    End Function

    ''' <summary>
    ''' Converts the game title to a valid file name that can be used in Windows OS,
    ''' by replacing any unsupported characters in the game title.
    ''' </summary>
    ''' <param name="gameTitle">The game title.</param>
    ''' <returns>The resulting file name that can be used in Windows OS.</returns>
    Public Function ConvertGameTitleToWindowsFileName(gameTitle As String) As String

        Return gameTitle.Replace("<", "˂").Replace(">", "˃").
                         Replace("\", "⧹").Replace("/", "⧸").Replace("|", "ǀ").
                         Replace(":", "∶").Replace("?", "ʔ").Replace("*", "✲").
                         Replace(ControlChars.Quote, Char.ConvertFromUtf32(&H201D))

    End Function

    ''' <summary>
    ''' Suspends the current thread for the a random number of milliseconds between the provided minimum and maximum values.
    ''' </summary>
    ''' <param name="minimum">The minimum milliseconds to wait.</param>
    ''' <param name="maximum">The maximum milliseconds to wait.</param>
    Public Sub SleepRandom(minimum As Integer, maximum As Integer)
        Dim ms As Integer = GamefaqsUtil.SleepRandomGenerator.Next(minimum, maximum)
        Thread.Sleep(ms)
    End Sub
    Private ReadOnly SleepRandomGenerator As New Random(Seed:=Environment.TickCount)

    Public Function BuildMarkdownTable(headerTitle As String, games As List(Of GameInfo)) As String
        Dim sb As New StringBuilder()
        sb.AppendLine($"# {headerTitle}")
        sb.AppendLine("|Index|Title|Release Date|Genre|")
        sb.AppendLine("|:--:|--|--|--|")

        Dim entryCount As Integer = 0
        For Each gameProfile As GameInfo In games
            sb.AppendLine($"|{Interlocked.Increment(entryCount)}|<a href=""{gameProfile.EntryUrl}"" target=""_blank"" rel=""noopener noreferrer"">{gameProfile.GameTitle}</a>|{gameProfile.ReleaseDate}|{gameProfile.Genre}|")
        Next

        Return sb.ToString()
    End Function

    Public Sub WriteMarkdownFile(platformInfo As PlatformInfo, ParamArray strings As String())

        Dim outputDir As New DirectoryInfo($".\{platformInfo.PlatformName}")
        If Not outputDir.Exists Then
            outputDir.Create()
        End If

        Dim fullBody As String = String.Join(Environment.NewLine, strings)

        Dim outputFilePath As String = $"{outputDir.FullName}\{platformInfo.PlatformName}.md"
        File.WriteAllText(outputFilePath, fullBody, Encoding.UTF8)

    End Sub

    Public Sub WriteUrlFiles(platformInfo As PlatformInfo, games As List(Of GameInfo))

        Dim outputDir As New DirectoryInfo($".\{platformInfo.PlatformName}\Urls")
        If Not outputDir.Exists Then
            outputDir.Create()
        End If

        For Each game As GameInfo In games
            Dim fileName As String = GamefaqsUtil.ConvertGameTitleToWindowsFileName(game.GameTitle)
            Dim outputFilePath As String = $"{outputDir.FullName}\{fileName}.url"
            File.WriteAllText(outputFilePath,
$"[InternetShortcut]
URL={game.EntryUrl}", encoding:=Encoding.UTF8)
        Next

    End Sub

End Module
