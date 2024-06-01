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
        public static async Task<List<AlbumInfo>> FetchAlbumInfo(string query)
        {
            HttpClient client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri(_baseUrl);
                var response = await client.GetAsync($"?q={query}&type=album&limit=50&access_token=BQDljUK-X9GlPfueixyQa3wifXXEfEZm5nh0XQqlfE7HWtO_M5UG42UVKm-lg4nBW88itDcI2DuY0cihhxPySRiNZCu5QGYlmxGtODamnIPdNA6DFZ8");
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Greska prilikom rada sa API-em");
                }
                var content = await response.Content.ReadAsStringAsync();
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
