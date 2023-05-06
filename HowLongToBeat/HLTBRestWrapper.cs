// Copyright 2023 Gabriel Bustillo del Cuvillo
// MIT License

// #define WITH_PREFIX

using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using HowLongToBeat.DTO;

using Newtonsoft.Json.Linq;

namespace HowLongToBeat
{
    public class HLTBRestWrapper
    {
        private static string BasePath = "https://howlongtobeat.com/api/search";
        private HttpClient client;

        public HLTBRestWrapper(HttpClient client)
        {
            this.client = client;
        }

        public async Task<List<Game>> Search(string query)
        {
            HttpResponseMessage response = await client.PostAsync(
                BasePath,
                new StringContent(getBody(query), System.Text.Encoding.UTF8, "application/json")
                ).ConfigureAwait(false);
            
//            if (response.StatusCode != System.Net.HttpStatusCode.OK){return new List<Game>();} ??

            HLTBResponse? list = JObject.Parse(await response.Content.ReadAsStringAsync()).ToObject<HLTBResponse>();

            if (list == null || list.GameList == null) { return new List<Game>(); }

            return list.GameList;
        }


        private string getBody(string query)
        {
#if WITH_PREFIX
            return "{" + prefix + "\"searchType\":\"games\",\"searchTerms\":[" +
#else
            return "{\"searchType\":\"games\",\"searchTerms\":["+
#endif
            String.Join(",", query.Split(" ").Select(p => "\"" + p.ToString() + "\"").ToArray())
            + "],\"searchPage\":1,\"size\":20}";
        }

#if WITH_PREFIX
        private string prefix = "\"searchOptions\": {\"filter\": \"\",\"games\": {\"gameplay\": {\"flow\": \"\",\"genre\": \"\",\"perspective\": \"\"},\"modifier\": \"\",\"platform\": \"\",\"rangeCategory\": \"main\",\"rangeTime\": {\"max\": null,\"min\": null},\"rangeYear\": {\"max\": \"\",\"min\": \"\"},\"sortCategory\": \"popular\",\"userId\": 0},\"lists\": {\"sortCategory\": \"follows\"},\"randomizer\": 0,\"sort\": 0,\"users\": {\"sortCategory\": \"postcount\"}},";
#endif
    }
}