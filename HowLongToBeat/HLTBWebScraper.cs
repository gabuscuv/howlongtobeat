using RandomUserAgent;
using HowLongToBeat.Enums;
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
        private readonly HLTBRestWrapper restWrapper;
        private readonly EMethodBehaviour behaviour;

        public HLTBWebScraper(HttpClient client, EMethodBehaviour behaviour = EMethodBehaviour.RestCalls)
        {
            this.behaviour = behaviour;
            this.client = client;
            client.DefaultRequestHeaders.Add("Origin", "https://howlongtobeat.com");
            client.DefaultRequestHeaders.Add("Referer", "https://howlongtobeat.com");
            restWrapper = new HLTBRestWrapper(client);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            do{
            try{
            client.DefaultRequestHeaders.UserAgent.ParseAdd(RandomUa.RandomUserAgent);
            break;
            }catch(System.FormatException)
            {
                // Quick Hack
            }
            }while(true);
        }

        public async Task<List<Game>> Search(string query)
        {

            if ( behaviour == EMethodBehaviour.WebScrapper)
            {
                string html = await GetGameHTMLResultsAsync(query);
                var parser = new HLTBHtmlParser();
                var result = await parser.GetGameDetailsAsync(html);
                return result;
            }

            return await restWrapper.Search(query);
            
        }

        private async Task<string> GetGameHTMLResultsAsync(string query)
        {

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