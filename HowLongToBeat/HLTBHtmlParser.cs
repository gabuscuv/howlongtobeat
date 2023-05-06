using AngleSharp;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HowLongToBeat
{
    public interface IHLTBHtmlParser
    {
        Task<List<Game>> GetGameDetailsAsync(string html);
    }

    public class HLTBHtmlParser : IHLTBHtmlParser
    {
        public async Task<List<Game>> GetGameDetailsAsync(string html)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(html));

            var gameTitles = GetGameTitles(document);
            var gameImgs = GetGameImgs(document);
            var gameDetails = GetGameDetails(document);

            var result = new List<Game>();
            for (var k = 0; k < gameTitles.Length; k++)
            {
                var details = MapToDictionary(gameDetails[k]);
                var gd = new Game(
                    title: gameTitles[k],
                    imgURL: gameImgs[k],
                    main: details["main"],
                    mainandextras: details["mainAndExtras"],
                    completionist: details["completionist"]
                );
                result.Add(gd);
            }

            return await Task.FromResult(result);
        }

        private static string[] GetGameImgs(AngleSharp.Dom.IDocument document)
        {
            var gameImgNodes = document.All.Where(x => x.LocalName == "img");
            var gameImgs = new string[gameImgNodes.Count()];
            var i = -1;
            foreach (var x in gameImgNodes)
            {
                i++;
                var imgNode = x.Attributes.First(x => x.LocalName == "src");
                var imgURI = $"https://howlongtobeat.com{imgNode.Value}";
                gameImgs[i] = imgURI;
            }

            return gameImgs;
        }

        private static string[] GetGameTitles(AngleSharp.Dom.IDocument document)
        {
            var gameTitlesNodes = document.All.Where(x => x.LocalName == "a" 
                && (x.ClassList.Contains("text_white") || x.ClassList.Contains("text_green")) );
            var gameTitles = new string[gameTitlesNodes.Count()];

            var i = -1;
            foreach (var x in gameTitlesNodes)
            {
                i++;
                var contentWithoutTabsOrNewLine = Regex.Replace(x.TextContent, @"\t|\n", " ").Trim();
                var title = Regex.Replace(contentWithoutTabsOrNewLine, @"\s+", " ");
                gameTitles[i] = title;
            }

            return gameTitles;
        }

        private static string[] GetGameDetails(AngleSharp.Dom.IDocument document)
        {
            var gameDetailsNodes = document.All.Where(x => x.LocalName == "div" && x.ClassList.Contains("search_list_details_block"));
            var gameDetails = new string[gameDetailsNodes.Count()];

            int j = -1;
            foreach (var x in gameDetailsNodes)
            {
                j++;
                var contentWithouTabs = Regex.Replace(x.TextContent, @"\t", " ").Trim();
                var content = Regex.Replace(contentWithouTabs, @"\n", ",");
                gameDetails[j] = content.Trim();
            }

            return gameDetails;
        }

        private static Dictionary<string, string> MapToDictionary(string text)
        {
            var result = new Dictionary<string, string>();
            var collection = text.Split(",");

            // Some data returns differently so we'll ignore those for now
            if (collection.Length != 6)
            {
                result.Add("main", "");
                result.Add("mainAndExtras", "");
                result.Add("completionist", "");
            }
            else
            {
                result.Add("main", collection[1].Trim());
                result.Add("mainAndExtras", collection[3].Trim());
                result.Add("completionist", collection[5].Trim());
            }
            return result;
        }
    }
}