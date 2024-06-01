using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysProgSpotifyApp
{
    internal class AlbumSearch
    {
        private static readonly string _apiKey = "";
        private static readonly string _baseUrl = "https://api.spotify.com/v1/search";
        public static List<AlbumInfo> FetchAlbumInfo(string query)
        {
            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = client.GetAsync($"?q=Taylor&type=album&limit=1&access_token=BQDVowFBs_852Oi4cKZWg_4RTX6XmB6xw79nXHbHlz-HDD8660O31tBek0J9JhlLCRxHAD9e7dhm_oKEAiH5IhlZV3ormDYH6Lys-drLfSnoU1iRmvo").Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Greska prilikom rada sa API-em");
                }
                var content = response.Content.ReadAsStringAsync().Result;
                var jsonObject = JsonConvert.DeserializeObject<JObject>(content);

                List<AlbumInfo> albums = new List<AlbumInfo>();
                foreach (var item in jsonObject["albums"]["items"])
                {
                    AlbumInfo w = new AlbumInfo();
                    w.AlbumName = (string)item["name"];
                    w.ArtistName = (string)item["artists"][0]["name"];
                    w.ReleaseDate = (string)item["release_date"];
                    w.TotalTracks = (int)item["total_tracks"];



                    albums.Add(w);
                }
                return albums;
            }
            catch (Exception e)
            {
                throw new HttpRequestException($"Greska prilikom izvrsenja: {e.Message}");
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
