// Copyright 2023 Gabriel Bustillo del Cuvillo
// MIT License

using System.Collections.Generic;
using Newtonsoft.Json;

namespace HowLongToBeat.DTO
{
    public class HLTBResponse
    {
        public HLTBResponse(List<Game> gameList)
        {
            GameList = gameList;
        }
        [JsonProperty("data")]
        public List<Game>? GameList {get;}
    }
}