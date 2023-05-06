using RandomUserAgent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HowLongToBeat
{
    public interface IHLTBWebScraper
    {
        Task<List<Game>> Search(string query);
    }

    public class HLTBWebScraper : IHLTBWebScraper
    {
        private readonly HttpClient client;

        public HLTBWebScraper(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<Game>> Search(string query)
        {
            string html = await GetGameHTMLResultsAsync(query);
            var parser = new HLTBHtmlParser();
            var result = await parser.GetGameDetailsAsync(html);

            return result;
        }

        private async Task<string> GetGameHTMLResultsAsync(string query)
        {
            // HOTFIX: Make sure is cleaned for reusing httpclient requests
            // TODO: Possible Fix: Set the Headers only once, maybe with the constructor.
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var userAgent = RandomUa.RandomUserAgent;
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
            client.DefaultRequestHeaders.Add("Origin", "https://howlongtobeat.com");
            client.DefaultRequestHeaders.Add("Referer", "https://howlongtobeat.com");

            var values = new Dictionary<string, string>
            {
                {"queryString", query},
                {"t", "games"},
                {"sorthead", "popular"},
                {"sortd", "Normal Order"},
                {"plat", ""},
                {"length_type", " main"},
                {"length_min", ""},
                {"length_max", ""},
                {"detail", "0"},
                {"v", ""},
                {"f", ""},
                {"g", ""},
                {"randomize", "0"},
            };

            string url = "https://howlongtobeat.com/search_results?page=1";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}