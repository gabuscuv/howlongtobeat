﻿using RandomUserAgent;
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
        private readonly HLTBHtmlParser hLTBHtmlParser;

        public HLTBWebScraper(HttpClient client, HLTBHtmlParser hLTBHtmlParser)
        {
            this.client = client;
            this.hLTBHtmlParser = hLTBHtmlParser;
        }

        public async Task<List<Game>> Search(string query)
        {
            string html = await GetGameHTMLResultsAsync(query);
            var result = await hLTBHtmlParser.GetGameDetailsAsync(html);

            return result;
        }

        private async Task<string> GetGameHTMLResultsAsync(string query)
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            var userAgent = RandomUa.RandomUserAgent;
            client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

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
                {"detail", " 0"},
            };

            string url = "https://howlongtobeat.com/search_results?page=1";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data).ConfigureAwait(false);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
    }
}